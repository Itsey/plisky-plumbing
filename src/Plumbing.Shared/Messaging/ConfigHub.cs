namespace Plisky.Plumbing {

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;
    using Plisky.Diagnostics;

    /// <summary>
    /// A class responsible for managing access to configuration information that may be required by an application.  Providers can place information into
    /// the hub and recievers can call GetSetting to retrieve single pieces of information.
    /// </summary>
    public partial class ConfigHub {
        protected List<string> fallbackRegistrationWarnings = new List<string>();
        private Bilge b = new Bilge("Plisky-ConfigHub");
        private const string CONFIGHUB_EXTENSION = ".chConfig";

        public const string DATETIMESETTINGNAME = "defaultdatetimevalue";
        public const string DEFAULTMACHINENAME = "defaultmachinename";
        public IDecryptStuff CryptoProvider { get; set; }
        private static bool dontBotherCheckingMachineNameAgain = false;
        private static string thisMachineName= DEFAULTMACHINENAME;
        private Dictionary<string, Func<string, string>> fallbackByMachineList = new Dictionary<string, Func<string, string>>();

        private Dictionary<string, Delegate> fl = new Dictionary<string, Delegate>();
        private Dictionary<string, Func<string>> functionList = new Dictionary<string, Func<string>>();

        private List<Func<string, string?>> fallbackList1 = new();

        #region private bits

        private string PerformDecryptOperation(string s) {
            if (CryptoProvider == null) {
                throw new ConfigHubConfigurationFailureException("The encryption plugin has not been loaded, decrption is not possible");
            }
            return CryptoProvider.DecryptValue(s);
        }

        #endregion private bits

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

        #endregion static instance stuff

        public ConfigHub() {
            b.Info.Log("ConfigHub Instance Online.");
        }

        /// <summary>
        /// Adds a fallback handler to look up the settings from App.Config based on the settings name.  The default app config behaviour is that
        /// the search is not case sensitive and that nested settings delimited by $$SETTINGNAME$$ can be used.
        /// </summary>
        public void AddDefaultAppConfigFallback() {
#if PLISKYNETFRAMEWORK
            b.Verbose.Log("AppConfigFallback Registration Request Made");

            RegisterFallbackProvider((pName) => {

                #region marker constants

                const string MARKERIDENTIFIER = "$$";
                const int MARKERIDLEN = 2;  // MARKERIDENTIFIER.Length
                const int MARKERIDLENDOUBLED = 4;  // MARKERIDENTIFIER.Length ;

                #endregion marker constants

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
#else
            b.Warning.Log("AppConfigFallback not suportted under net core");
#endif
        }

        /// <summary>
        /// Adds a fall back handler to look up settings in xml files based on the machine name in a specific driectory. The behaviour is to find files
        /// that are machinename.chcfg to resolve the settings names.  If an app setting is set to Environment name then environmentname.chcfg will
        /// be looked for instead.
        /// </summary>
        /// <param name="directory">The directory to search [APP] can be used as the app directory and [APP]\subdir works too. Null uses current directory.  Environment variables are expanded</param>
        /// <param name="fileName">Optional: Filename that is added in the directory, defaults to machinename.chConfig, appends chConfig to the string you supply if not already there.</param>
        public void AddDirectoryFallbackProvider(string directory, string fileName = null) {
            b.Info.Flow($"Dir[{directory}], filename [{fileName ?? "null"}]");
            string actualDir = GetDirectoryName(directory);

            if (fileName == null) {
                SetupMachineName();
                b.Verbose.Log("Filename not specified, using machinename", thisMachineName);
                fileName = Path.ChangeExtension(thisMachineName, CONFIGHUB_EXTENSION);
            }

            string machineBasedFilename = Path.Combine(actualDir, fileName);

            if (string.IsNullOrWhiteSpace(Path.GetExtension(machineBasedFilename))) {
                machineBasedFilename = Path.ChangeExtension(machineBasedFilename, CONFIGHUB_EXTENSION);
            }

            b.Verbose.Log($"Filename {machineBasedFilename}");

            if (File.Exists(machineBasedFilename)) {
                RegisterFallbackProvider((setName) => {
                    b.Verbose.Log($"Directory Fallback - Setting:{setName}", $"fallback provider file: {machineBasedFilename}");
                    var x = XDocument.Load(machineBasedFilename);
                    return GetSettingsFromCustomXmlFile(x, setName);
                });
            } else {
                string warning = $"Fallback Directory Searcher Failed To Register - File Not Found. {fileName}";
                fallbackRegistrationWarnings.Add(warning);
                b.Warning.Log(warning);
            }
        }

        protected string GetDirectoryName(string directory) {
            b.Info.Flow($"{directory}");

            string result = directory;
            if (string.IsNullOrEmpty(directory)) {
                result = Environment.CurrentDirectory;
                b.Verbose.Log("Using CurrentDirectory");
            }

            if (directory.StartsWith("[APP]")) {
                string path = ActualGetEntryPointPath();

                result = directory.Replace("[APP]", path);
                b.Verbose.Log($"Replacing [APP] With application routing [{result}]");
            }

            result = ActualExpandForEnvironmentVariables(result);

            b.Verbose.Log($"Result [{result}]");
            return result;
        }

        /// <summary>
        /// OS Abstraction, Environment Variable Expansion
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        protected virtual string ActualExpandForEnvironmentVariables(string result) {
            if (result.Contains("%")) {
                // Environment variable tokenisation
                result = Environment.ExpandEnvironmentVariables(result);
                b.Verbose.Log($"Expanded [{result}]");
            }
            return result;
        }

        /// <summary>
        /// OS Abstraction, get the entry point for this app.
        /// </summary>
        /// <returns></returns>
        protected virtual string ActualGetEntryPointPath() {
            string result;
            var asm = Assembly.GetEntryAssembly();
            if (asm != null) {
                result = Path.GetDirectoryName(asm.Location);
            } else {
                asm = Assembly.GetExecutingAssembly();
                if (asm != null) {
                    result = Path.GetDirectoryName(asm.Location);
                } else {
                    result = Directory.GetCurrentDirectory();
                }
            }
            return result;
        }

        private string GetSettingsFromCustomXmlFile(XDocument configFile, string settingName) {
            settingName = settingName.ToLowerInvariant();
            try {
                var set = configFile.Element("chub_settings").Element("settings");

                if (set != null) {
                    foreach (var el in set.Elements()) {
                        b.Verbose.Log($"Trying {el.Name}");
                        if (el.Name.ToString().ToLowerInvariant() == settingName) {
                            return el.Value;
                        }
                    }
                    b.Verbose.Log("No setting found, no match");
                    return null;
                } else {
                    b.Verbose.Log($"Invalid configuration file, chub_settings and settings elements not present.", configFile.ToString());
                }
            } catch (Exception x) {
                b.Warning.Log("XML Parsing Failed, corrupt settings file, setting NOT Matched");
                b.Info.Dump(x, "XML Exception Reading Config");
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
                if (fallbackByMachineList.Count == 0) {
                    SetupMachineName();
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
                        } else if (fallbackByMachineList.ContainsKey(DEFAULTMACHINENAME)) {
                            vmatch = fallbackByMachineList[DEFAULTMACHINENAME](settingName);
                        }
                    }
                    // If the machine wasnt found then this returns null.
                    return vmatch;
                });
            }
        }

        private void SetupMachineName() {
            if ((thisMachineName == DEFAULTMACHINENAME)&&(dontBotherCheckingMachineNameAgain==false)) {
                lock (lockme) {
                    try {
                        b.Verbose.Log("MachineName Cache Refreshed.");
                        thisMachineName = Environment.MachineName.ToLower();
                    } catch (InvalidOperationException) {
                        // Probably access denied
                        b.Verbose.Log("Access Denied to Machinename, falling back to default", DEFAULTMACHINENAME);
                        thisMachineName = DEFAULTMACHINENAME;
                        dontBotherCheckingMachineNameAgain = true;
                    }
                }
            }
        }

        /// <summary>
        /// Register your own, custom, fallback provider.  Will be called when a specific provider for a value can not be found.
        /// </summary>
        /// <remarks>Fallback providers are called after no setting value has been found.</remarks>
        /// <param name="getAllSettings"></param>
        public void RegisterFallbackProvider(Func<string, string?> getAllSettings) {
            if (getAllSettings != null) {
                fallbackList1.Add(getAllSettings);
            }
        }

        /// <summary>
        /// Register a provider for a specific type of configuration request.
        /// </summary>
        /// <typeparam name="T">The type to register the provider for</typeparam>
        /// <param name="name">The setting name that this specific provider returns a value for</param>
        /// <param name="getSetting">The delegate to return the correct setting value when executed.</param>
        public void RegisterProvider<T>(string name, Func<T> getSetting) {
            name = name.ToLowerInvariant();
            fl.Add(name, getSetting);
        }

        /// <summary>
        /// Registers a simple provider for a string based configuration request.
        /// </summary>
        /// <param name="forValue">The setting name that this specific provider returns a value for</param>
        /// <param name="provider">The delegate used to return the correct setting when executed.</param>
        public void RegisterProvider(string forValue, Func<string> provider) {
            forValue = forValue.ToLowerInvariant();
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
            settingName = settingName.ToLowerInvariant();

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

            #region validation

            if (settingName == null) {
                throw new ArgumentNullException(nameof(settingName), "You must provide a setting name to get a setting");
            }
            if (string.IsNullOrWhiteSpace(settingName)) {
                throw new ArgumentOutOfRangeException(nameof(settingName), "You must provide a setting name to get a settting");
            }

            #endregion validation

            settingName = settingName.ToLowerInvariant();
            b.Info.Flow($"{settingName},{mustBePresent}");
            string val = null;

            if (functionList.ContainsKey(settingName)) {
                b.Verbose.Log($"Direct Retrieval Funcation Call Made");
                val = functionList[settingName]();
            }

            if (val == null) {
                b.Verbose.Log($"Setting Value Is Null, Trying Fallback List {fallbackList1.Count} fallbacks registered.");

                foreach (var v in fallbackList1) {
                    val = v(settingName);
                    if (val != null) {
                        b.Verbose.Log($"Fallback -> Value Match-]{val}");
                        break;
                    }
                }
            }

            if ((val != null) && (isEncrypted)) {
                b.Verbose.Log("Encrypted - Running through registered decryptor");
                val = CryptoProvider.DecryptValue(val);
            }

            if ((val == null) && (mustBePresent)) {
                b.Warning.Log("Value not matched, and must be present set - throwing exception");
                var ex = new ConfigHubMissingConfigException($"The setting {settingName ?? "null"} must be present and have a value.");
                PopulateDiagnostics(ex);
                throw ex;
            }

            b.Verbose.Log($"{settingName}={val}");
            return val;
        }

        private void PopulateDiagnostics(ConfigHubMissingConfigException ex) {
            string functionListRetrievers = "";
            foreach (string fl in functionList.Keys) {
                functionListRetrievers += fl + ",";
            }
            if (functionListRetrievers.Length == 0) {
                functionListRetrievers = "Non Registered";
            }
            ex.Data.Add("Direct Fuction List", functionListRetrievers);

            string fbr = "";
            foreach (string f in fallbackRegistrationWarnings) {
                fbr += f + "," + Environment.NewLine;
            }
            if (fbr.Length == 0) {
                fbr = "No fallback Warnings";
            }
            ex.Data.Add("Fallback Warnings", fbr);

            string handlers = $"FL: {functionList.Keys.Count}, FB: {fallbackList1.Count}, MFB: {fallbackByMachineList.Count}";
            ex.Data.Add("Total Handlers", handlers);
        }

        public DateTime GetNow() {
            try {
                var dt = GetSetting<DateTime>(DATETIMESETTINGNAME);
                if (dt == default(DateTime)) {
                    dt = DateTime.Now;
                }
                return dt;
            } catch (ConfigHubMissingConfigException) {
                return DateTime.Now;
            }
        }
    }
}