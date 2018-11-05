﻿namespace Plisky.Plumbing {

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;

    /// <summary>
    /// A class responsible for managing access to configuration information that may be required by an application.  Providers can place information into
    /// the hub and recievers can call GetSetting to retrieve single pieces of information.
    /// </summary>
    public class ConfigHub {
        public const string DateTimeSettingName = "defaultdatetimevalue";
        public const string DefaultMachineName = "defaultmachinename";
        public IDecryptStuff CryptoProvider { get; set; }

        private static string thisMachineName;
        private Dictionary<string, Func<string, string>> fallbackByMachineList;

        private Dictionary<string, Delegate> fl = new Dictionary<string, Delegate>();
        private Dictionary<string, Func<string>> functionList = new Dictionary<string, Func<string>>();

        private List<Func<string, string>> fallbackList1 = new List<Func<string, string>>();

        #region private bits

        private string PerformDecryptOperation(string s) {
            if (CryptoProvider == null) {
                throw new ConfigHubConfigurationFailureException("The encryption plugin has not been loaded, decrption is not possible");
            }
            return CryptoProvider.DecryptValue(s);
        }

        #endregion

        #region static instance stuff

        private static ConfigHub instance;
        private static object lockme = new object();

        /// <summary>
        /// Returns a single static, instance of the hub.  Not thread safe.
        /// </summary>
        public static ConfigHub Current {
            get {
                if (instance == null) {
                    lock (lockme) {
                        instance = new ConfigHub();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Resets all of the hub for the ConfigHub.Current instance.  Not normally required but useful for unit testing where components
        /// use the Current config hub rather than a passed instance.
        /// </summary>
        public static void ResetCurrentConfigHub() {
            lock (lockme) {
                instance = null;
            }
        }

        #endregion

#if NET452
        /// <summary>
        /// Adds a fallback handler to look up the settings from App.Config based on the settings name.  The default app config behaviour is that
        /// the search is not case sensitive and that nested settings delimited by $$SETTINGNAME$$ can be used.
        /// </summary>
        public void AddDefaultAppConfigFallback() {
            RegisterFallbackProvider((pName) => {

#region marker constants

                const string MARKERIDENTIFIER = "$$";
                const int MARKERIDLEN = 2;  // MARKERIDENTIFIER.Length
                const int MARKERIDLENDOUBLED = 4;  // MARKERIDENTIFIER.Length ;

#endregion

                string vmatch;
                vmatch = ConfigurationManager.AppSettings[pName];
                if ((vmatch != null) && (vmatch.Length > MARKERIDLENDOUBLED)) {
                    int markerIdx = vmatch.IndexOf(MARKERIDENTIFIER);
                    if (markerIdx >= 0) {
                        markerIdx += MARKERIDLEN;
                        int endMarker = vmatch.IndexOf(MARKERIDENTIFIER, markerIdx);
                        if (endMarker > 0) {
                            string replacement = vmatch.Substring(markerIdx, endMarker - markerIdx);
                            vmatch = vmatch.Substring(0, markerIdx - MARKERIDLEN) + (GetSetting(replacement) ?? "") + vmatch.Substring(endMarker + MARKERIDLEN);
                        }
                    }
                }
                return vmatch;
            });
        }
#endif

        /// <summary>
        /// Adds a fall back handler to look up settings in xml files based on the machine name in a specific driectory. The behaviour is to find files
        /// that are machinename.chcfg to resolve the settings names.
        /// </summary>
        /// <param name="directory"></param>
        public void AddDirectoryFallbackProvider(string directory) {
            string actualDir = GetDirectoryName(directory);

            lock (lockme) {
                SetupMachineName();
            }
            string machineBasedFilename = Path.Combine(directory, thisMachineName + ".chcfg");
            if (File.Exists(machineBasedFilename)) {
                RegisterFallbackProvider((setName) => {
                    XDocument x = XDocument.Load(machineBasedFilename);
                    return GetSettingsFromCustomXmlFile(x, setName);
                });
            }
        }

        private string GetDirectoryName(string directory) {
            string result = directory;
            if (string.IsNullOrEmpty(directory)) {
                result = Environment.CurrentDirectory;
            }
            if (directory.StartsWith("[APP]")) {
                result = directory.Replace("[APP]", Assembly.GetEntryAssembly().Location);
            }
            return result;
        }

        private string GetSettingsFromCustomXmlFile(XDocument configFile, string settingName) {
            var set = configFile.Element("chub_settings").Element("settings").Element(settingName.ToLower());
            if (set != null) {
                return set.Value;
            }
            return null;
        }

        /// <summary>
        /// Adds a fallback provider which is called only when the machine that the configuration hub runs on is matched to the machine name specified in the registration
        /// call.  You can therefore add fallback providers for different machines and have the right one called by machine name.
        /// </summary>
        /// <remarks>If the hub is unable to retrieve the machine name (due to access rights etc) then it will try and call a fallback with the name of "default", if you
        /// add a machine name with the word "default" then this will be called if no other machine name matches.</remarks>
        /// <param name="machineName">the machine name to execute the fallback provider for</param>
        /// <param name="getAllSettings">the fallback provider that will be executed</param>
        public void AddMachineFallbackProvider(string machineName, Func<string, string> getAllSettings) {
            if (string.IsNullOrEmpty(machineName)) {
                throw new ArgumentOutOfRangeException("machineName", "The machine name can not be empty or null");
            }
            lock (lockme) {
                if (fallbackByMachineList == null) {
                    SetupMachineName();
                    fallbackByMachineList = new Dictionary<string, Func<string, string>>();
                }

                machineName = machineName.ToLower();
                if (fallbackByMachineList.ContainsKey(machineName)) {
                    throw new InvalidOperationException("The machine name [" + machineName + "] has already been added to the fallback provider list.");
                }
                fallbackByMachineList.Add(machineName, getAllSettings);

                RegisterFallbackProvider((settingName) => {
                    string vmatch = null;
                    lock (lockme) {
                        if (fallbackByMachineList.ContainsKey(thisMachineName)) {
                            vmatch = fallbackByMachineList[thisMachineName](settingName);
                        } else if (fallbackByMachineList.ContainsKey(DefaultMachineName)) {
                            vmatch = fallbackByMachineList[DefaultMachineName](settingName);
                        }
                    }
                    // If the machine wasnt found then this returns null.
                    return vmatch;
                });
            }
        }

        private static void SetupMachineName() {
            if (thisMachineName == null) {
                try {
                    thisMachineName = Environment.MachineName.ToLower();
                } catch (InvalidOperationException) {
                    // Probably access denied
                    thisMachineName = DefaultMachineName;
                }
            }
        }

        /// <summary>
        /// Register your own, custom, fallback provider.  Will be called when a specific provider for a value can not be found.
        /// </summary>
        /// <remarks>Fallback providers are called after no setting value has been found.</remarks>
        /// <param name="getAllSettings"></param>
        public void RegisterFallbackProvider(Func<string, string> getAllSettings) {
            fallbackList1.Add(getAllSettings);
        }

        /// <summary>
        /// Register a provider for a specific type of configuration request.
        /// </summary>
        /// <typeparam name="T">The type to register the provider for</typeparam>
        /// <param name="name">The setting name that this specific provider returns a value for</param>
        /// <param name="getSetting">The delegate to return the correct setting value when executed.</param>
        public void RegisterProvider<T>(string name, Func<T> getSetting) {
            fl.Add(name, getSetting);
        }

        /// <summary>
        /// Registers a simple provider for a string based configuration request.
        /// </summary>
        /// <param name="forValue">The setting name that this specific provider returns a value for</param>
        /// <param name="provider">The delegate used to return the correct setting when executed.</param>
        public void RegisterProvider(string forValue, Func<string> provider) {
            functionList.Add(forValue, provider);
        }

        /// <summary>
        /// Retrieves a setting for a specific type, checking the registered providers to see which one is able to return a setting
        /// of the correct type.
        /// </summary>
        /// <typeparam name="T">The type of the setting to retrieve</typeparam>
        /// <param name="settingName">The name of the setting required</param>
        /// <returns>A setting value of type T</returns>
        public T GetSetting<T>(string settingName) {
            return GetSetting<T>(settingName, false, false);
        }

        public T GetSetting<T>(string settingName, bool mustBePresent = false, bool requiresDecryption = false) {
            if (fl.ContainsKey(settingName)) {
                var v = (Func<T>)fl[settingName];
                return v();
            } else {
                string s = GetSetting(settingName, mustBePresent);

                if (s == null) {
                    return default(T);
                }

                if (requiresDecryption) {
                    s = PerformDecryptOperation(s);
                }

                // This code is not nice generic code, however it allows us to use the same interface as the typed generic get methods, while still
                // supporting things like the machine fallback. It is not, however, either maintanable nor is it efficient.
                if (typeof(T) == typeof(bool)) {
                    return (T)Convert.ChangeType(Convert.ToBoolean(s), typeof(T));
                }
                if (typeof(T) == typeof(int)) {
                    return (T)Convert.ChangeType(Convert.ToInt32(s), typeof(T));
                }
                if (typeof(T) == typeof(double)) {
                    return (T)Convert.ChangeType(Convert.ToDouble(s), typeof(T));
                }
                if (typeof(T) == typeof(DateTime)) {
                    return (T)Convert.ChangeType(Convert.ToDateTime(s), typeof(T));
                }
                if (typeof(T) == typeof(string)) {
                    return (T)Convert.ChangeType(s, typeof(string));
                }
            }
            throw new NotImplementedException("That type has not been implemented, use a string");
        }

        /// <summary>
        /// Retrieves a string setting value using a string key name from the configuration hub.  If there is a fallback provider registered
        /// for strings then this fallback provider will be called.  If the default appconfig provider is registerd this will also be called.
        /// </summary>
        /// <param name="settingName">The key identifying the setting</param>
        /// <param name="mustBePresent">Boolean, defaults to false.  Set to true if an exception should be thrown if no value can be found.</param>
        /// <returns>A string based value indicating the setting</returns>
        public string GetSetting(string settingName, bool mustBePresent = false, bool isEncrypted = false) {
            string val = null;

            if (functionList.ContainsKey(settingName)) {
                val = functionList[settingName]();
            }

            if (val == null) {
                foreach (var v in fallbackList1) {
                    val = v(settingName);
                    if (val != null) {
                        break;
                    }
                }
            }

            if ((val != null) && (isEncrypted)) {
                val = CryptoProvider.DecryptValue(val);
            }

            if ((val == null) && (mustBePresent)) {
                throw new ConfigHubMissingConfigException($"The setting {settingName ?? "null"} must be present and have a value.");
            }
            return val;
        }

        public DateTime GetNow() {
            try {
                DateTime dt = GetSetting<DateTime>(DateTimeSettingName);
                return dt;
            } catch (ConfigHubMissingConfigException) {
                return DateTime.Now;
            }
        }
    }
}