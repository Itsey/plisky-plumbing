﻿namespace Plisky.Test {

    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using Plisky.Diagnostics;
    using Plisky.Diagnostics.Listeners;
    using Plisky.Plumbing;
    using TestData;
    using Xunit;

    public class ConfigHubTests {
        private UnitTestHelper uth = new UnitTestHelper();
        protected Bilge b = new Bilge();

        public ConfigHubTests() {
            var h = new TCPHandler("192.168.1.15", 9060);
            b.AddHandler(h);
        }

        ~ConfigHubTests() {
            uth.ClearUpTestFiles();
        }

        [Fact]
        public void GetConfig_NoProvider_ThrowsIfRequired() {
            var sut = new ConfigHub();
            Assert.Throws<ConfigHubMissingConfigException>(() => sut.GetSetting("arflebarfle", true));
        }

        [Fact]
        public void GetConfig_NoProvider_NoThrowNotRequired() {
            var sut = new ConfigHub();
            sut.GetSetting("arflebarfle", false);
        }

        [Fact(DisplayName = nameof(MissingConfig_ExceptionData_Present))]
        public void MissingConfig_ExceptionData_Present() {
            b.Info.Flow();

            var sut = new ConfigHub();

            ConfigHubMissingConfigException theException = null;
            try {
                sut.GetSetting("arflebarfle", true);
            } catch (ConfigHubMissingConfigException aex) {
                theException = aex;
            }

            Assert.NotNull(theException);
            Assert.True(theException.Data.Keys.Count > 0);
        }

        [Fact(DisplayName = nameof(FailedDirectoryFallback_DetailsInException))]
        public void FailedDirectoryFallback_DetailsInException() {
            b.Info.Flow();

            var sut = new ConfigHub();
            sut.AddDirectoryFallbackProvider("c:\\dontexistatll_veryunlikely", "testoleo");
            ConfigHubMissingConfigException theException = null;
            try {
                sut.GetSetting("arflebarfle", true);
            } catch (ConfigHubMissingConfigException aex) {
                theException = aex;
            }

            Assert.NotNull(theException);
            Assert.True(theException.Data.Keys.Count > 0);
            bool matchedFilename = false;
            foreach (var f in theException.Data.Keys) {
                string str = theException.Data[f].ToString();
                if (str.Contains("testoleo")) {
                    matchedFilename = true;
                }
            }
            Assert.True(matchedFilename);
        }

        [Fact(DisplayName = nameof(DateTime_Setting_Works))]
        public void DateTime_Setting_Works() {
            b.Info.Flow();
            DateTime expected = new DateTime(2019, 1, 1);

            ConfigHub sut = new ConfigHub();
            sut.RegisterProvider<DateTime>(ConfigHub.DATETIMESETTINGNAME, () => {
                return expected;
            });

            Assert.Equal<DateTime>(expected, sut.GetNow());
        }

        [Fact]
        public void GetConfigCurrentInstance_SingleProvider_ReturnsCorrectly() {
            string returnString = SampleTestData.GENERIC_STRING1;

            ConfigHub.Current.RegisterProvider("test", () => {
                return returnString;
            });

            string actual = ConfigHub.Current.GetSetting("test");

            Assert.Equal(returnString, actual);
        }

        [Fact(DisplayName = nameof(GetSetting_ThrowsInfNullSetting))]
        public void GetSetting_ThrowsInfNullSetting() {
            b.Info.Flow();

            var sut = new ConfigHub();
            Assert.Throws<ArgumentNullException>(() => {
                sut.GetSetting(null);
            });
        }

        [Fact]
        public void GetConfig_SingleProvider_ReturnsCorrectly() {
            string returnString = "arflebarflegloop";

            ConfigHub sut = new ConfigHub();
            sut.RegisterProvider("test", () => {
                return returnString;
            });

            string actual = sut.GetSetting("test");

            Assert.Equal(returnString, actual);
        }

        [Fact]
        public void GetConfig_FallbackProvider_ReturnsCorrectly() {
            string returnString = "arflebarflegloop";

            ConfigHub sut = new ConfigHub();
            sut.RegisterFallbackProvider((pname) => {
                if (pname == "test1") {
                    return returnString;
                }
                return null;
            });

            string actual = sut.GetSetting("test1");
            Assert.Equal(returnString, actual);
        }

        [Fact]
        public void GetConfig_NoProvider_ReturnsNullIfNotRequired() {
            ConfigHub sut = new ConfigHub();
            string res = sut.GetSetting("arflebarfle");
            Assert.Null(res);
        }

        [Fact]
        public void GetConfig_FallbackProviderLoaded_DoesNotReturnForNotSpecified() {
            string returnString = "arflebarflegloop";

            ConfigHub sut = new ConfigHub();
            sut.RegisterFallbackProvider((pname) => {
                if (pname == "test1") {
                    return returnString;
                }
                return null;
            });

            string actual = sut.GetSetting("test2");
            Assert.Null(actual);
        }

        [Fact(DisplayName = nameof(EmptyDirectory_DefaultsToCurrent))]
        public void EmptyDirectory_DefaultsToCurrent() {
            b.Info.Flow();
            var sut = new MockConfigHub();

            string cd = sut.Mock.GetDirectoryName(string.Empty);

            Assert.False(string.IsNullOrWhiteSpace(cd));
        }

        [Fact(DisplayName = nameof(Directory_ResolvesEnvironmentVariable))]
        public void Directory_ResolvesEnvironmentVariable() {
            b.Info.Flow();

            var sut = new MockConfigHub();
            sut.Mock.AddEnvironmentVariable("MOCKENVVAR", "C:\\EnvVarDir");

            var dn = sut.Mock.GetDirectoryName("%MOCKENVVAR%\\MyDir");

            Assert.Equal("C:\\EnvVarDir\\MyDir", dn);
        }

        [Fact(DisplayName = nameof(Bug_AppTagMarkerIncludesExeName))]
        [Trait(Traits.Style, Traits.LiveBug)]
        public void Bug_AppTagMarkerIncludesExeName() {
            b.Info.Flow();

            // This bug was because "ToLower" was used in the settings therefore the connectionString could not be found
            // as it wasnt lowercase in the file.  This is currently failing as have fixed the file.

            // TODO - Implement case insensitivity option.
            var sut = new MockConfigHub();

            string output = sut.Mock.GetDirectoryName("[APP]");

            Assert.NotNull(output);
            Assert.DoesNotContain("[APP]", output);
        }

        [Fact(DisplayName = nameof(Bug_CanNotFindConnectionString))]
        [Trait(Traits.Style, Traits.LiveBug)]
        public void Bug_CanNotFindConnectionString() {
            b.Info.Flow();

            // This bug was because "ToLower" was used in the settings therefore the connectionString could not be found
            // as it wasnt lowercase in the file.  This is currently failing as have fixed the file.

            var ch = GetConfigHubWithSampleDataFallbackFile(TestResourcesReferences.XMLUseCaseFile);

            string cs = ch.GetSetting("connectionstring", true, false);
            Assert.NotNull(cs);

            cs = ch.GetSetting("connectionString", true, false);
            Assert.NotNull(cs);

            cs = ch.GetSetting("coNNectionString", true, false);
            Assert.NotNull(cs);

            cs = ch.GetSetting("CONNECTIONSTRING", true, false);
            Assert.NotNull(cs);
        }

        public ConfigHub GetConfigHubWithSampleDataFallbackFile(TestResourcesReferences whichFile) {
            var ch = new ConfigHub();
            string df = uth.GetTestDataFile(TestResources.GetIdentifiers(whichFile));
            string dirForFile = Path.GetDirectoryName(df);
            string filename = Path.GetFileName(df);

            ch.AddDirectoryFallbackProvider("C:\\DoesNotExistAtAll", "notfoundfile.xml");
            ch.AddDirectoryFallbackProvider(dirForFile, filename);
            return ch;
        }

        [Fact(DisplayName = nameof(Directory_DoubleFallbackWorks))]
        public void Directory_DoubleFallbackWorks() {
            b.Info.Flow();

            var sut = new ConfigHub();

            string df = uth.GetTestDataFile(TestResources.GetIdentifiers(TestResourcesReferences.ConfigHubTestData));
            string dirForFile = Path.GetDirectoryName(df);
            string filename = Path.GetFileName(df);

            sut.AddDirectoryFallbackProvider("C:\\DoesNotExistAtAll", "notfoundfile.xml");
            sut.AddDirectoryFallbackProvider(dirForFile, filename);

            string str = sut.GetSetting("setting1", true);

            Assert.Equal("setting1value", str);
        }

        [Fact(DisplayName = nameof(Directory_FullConfigStringRetrieval))]
        public void Directory_FullConfigStringRetrieval() {
            b.Info.Flow();

            var sut = GetConfigHubWithSampleDataFallbackFile(TestResourcesReferences.ConfigHubTestData);
            string str = sut.GetSetting("setting1", true);

            Assert.Equal("setting1value", str);
        }

        [Fact(DisplayName = nameof(Directory_FullConfigStringRetrievalCrypto))]
        public void Directory_FullConfigStringRetrievalCrypto() {
            b.Info.Flow();
            const string PLAINTEXT = "ItAllWorked";

            var sut = GetConfigHubWithSampleDataFallbackFile(TestResourcesReferences.ConfigHubTestData);

            var mcr = new MockSimpleCrypto();
            mcr.AddDecryption("setting1value", PLAINTEXT);
            sut.CryptoProvider = mcr;

            string str = sut.GetSetting("setting1", true, true);

            Assert.Equal(PLAINTEXT, str);
        }

        [Fact]
        public void GetConfig_DirectoryFallbackProvider_Works() {
            const string TESTDATA = "bannana";
            string fn = uth.NewTemporaryFileName();

            var x = new XDocument();
            x.Add(new XElement("chub_settings", new XElement("settings", new XElement("monkeyfish", TESTDATA))));
            x.Save(fn);

            ConfigHub sut = new ConfigHub();

            sut.AddDirectoryFallbackProvider(Path.GetDirectoryName(fn), Path.GetFileName(fn));
            string res = sut.GetSetting("monkeyfish", true);

            Assert.Equal(TESTDATA, res);
        }

        [Fact]
        public void GetConfig_FallbackProvider_CalledForAllRequests() {
            Dictionary<string, bool> testSettings = new Dictionary<string, bool>();

            // Used to avoid issue where changing collection below causes fault.
            var settingNames = new List<string>();

            for (int i = 0; i < 10; i++) {
                string next = "testsetting" + i.ToString();
                testSettings.Add(next, false);
                settingNames.Add(next);
            }

            var sut = new ConfigHub();
            sut.RegisterFallbackProvider((pName) => {
                if (testSettings.ContainsKey(pName)) {
                    testSettings[pName] = true;
                }
                return null;
            });

            foreach (string v in settingNames) {
                sut.GetSetting(v);
            }

            foreach (string test in testSettings.Keys) {
                Assert.True(testSettings[test], "The setting " + test + " was not hit");
            }
        }

        [Fact]
        public void GetConfig_FallbackProvider_CalledForAllStrings() {
            var td = new SampleTestData();
            var sut = new ConfigHub();
            int hits = 0;

            sut.RegisterFallbackProvider((pname) => {
                hits++;
                return "test";
            });

            int totalHits = td.RandomStore.Next(100) + 1;

            int i;
            for (i = 0; i < totalHits; i++) {
                sut.GetSetting("asetting" + i.ToString());
            }

            Assert.Equal<int>(i, hits);
        }

        [Fact]
        public void GetConfig_TwoProviders_SpecificOverridesFallback() {
            ConfigHub sut = new ConfigHub();
            sut.RegisterFallbackProvider((pName) => {
                return "result1";
            });

            sut.RegisterProvider("value1", () => {
                return "result2";
            });

            string actual = sut.GetSetting("value1");

            Assert.Equal("result2", actual);
        }

        [Fact]
        public void GetConfig_FallbackAppConfigProvider_Returnsconfiguration() {
            ConfigHub sut = new ConfigHub();
            sut.AddDefaultAppConfigFallback();
            string val = sut.GetSetting("testSettingValue");
            Assert.Equal("arfleBARFLEgloop", val);
        }

        [Fact]
        public void GetConfig_FallbackAppConfigProvider_NotInstalledByDefault() {
            ConfigHub sut = new ConfigHub();
            string val = sut.GetSetting("testSettingValue");
            Assert.Null(val);
        }

        [Fact]
        public void GetConfig_FallbackAppConfigProvider_IsCaseInsensitive() {
            ConfigHub sut = new ConfigHub();
            sut.AddDefaultAppConfigFallback();
            string val = sut.GetSetting("TESTsEttInGvAlUe");
            Assert.Equal("arfleBARFLEgloop", val);
        }

        [Fact]
        public void GetConfig_CustomType_ReturnsCorrectly() {
            ConfigHub sut = new ConfigHub();
            sut.RegisterProvider<TestMessage>("settingVal", () => {
                return new TestMessage("HelloWorld");
            });
            TestMessage result = sut.GetSetting<TestMessage>("settingVal");

            Assert.Equal("HelloWorld", result.Data);
        }

        [Fact]
        public void GetConfig_FallbackAppConfig_NestedIsEmptyWhenNotPresent() {
            string combinedString = "ARFLEUSBarfleyGloopify";
            var sut = new ConfigHub();
            sut.AddDefaultAppConfigFallback();

            string val = sut.GetSetting("testSettingSubValue2");
            Assert.Equal(combinedString, val);
        }

        [Fact]
        public void GetConfig_FallbackAppConfig_SupportsNestedReplacements() {
            string combinedString = "contactinatedIntoARFLEUSBarfleyGloopify";
            var sut = new ConfigHub();
            sut.AddDefaultAppConfigFallback();

            string val = sut.GetSetting("testSettingSubValue");

            Assert.Equal(combinedString, val);
        }

        [Fact]
        public void GetConfig_GetSetting_SpecificOveridesFallback() {
            var sut = new ConfigHub();
            sut.AddDefaultAppConfigFallback();
            sut.RegisterProvider("testSettingSubValue2", () => {
                return "NothingHere";
            });
            string val = sut.GetSetting("testSettingSubValue2");
            Assert.Equal("NothingHere", val);
        }

        [Fact]
        public void RegisterMachineFallback_CallBackOccursForThisMachine() {
            var sut = new ConfigHub();

            string machineName = Environment.MachineName;
            string expected = "Hello";
            bool executed = false;

            sut.AddMachineFallbackProvider(machineName, (settingName) => {
                executed = true;
                return expected;
            });

            string result = sut.GetSetting("TestSettingName");
            Assert.True(executed, "The handler should have been executed");
            Assert.Equal(expected, result);
        }

        [Fact]
        public void RegisterMachineFallback_DuplicateName_CausesException() {
            var sut = new ConfigHub();
            string machineName = "MachineName";

            sut.AddMachineFallbackProvider(machineName, (settingName) => {
                return "Hello";
            });
            Assert.Throws<InvalidOperationException>(() =>
              sut.AddMachineFallbackProvider(machineName, (settingName) => {
                  return "Hello";
              }));
        }

        [Fact]
        public void RegisterFallbacks_ExcutedInOrderOfRegistration() {
            var sut = new ConfigHub();
            string machineName = Environment.MachineName;
            bool firstExecuted = false;
            bool secondExecuted = false;

            sut.AddMachineFallbackProvider(machineName, (settingName) => {
                firstExecuted = true;
                return "Hello";
            });
            sut.RegisterFallbackProvider((settingName2) => {
                secondExecuted = true;
                return "Hello";
            });

            string result = sut.GetSetting("TestSettingName");

            Assert.True(firstExecuted, "The first one should have been executed");
            Assert.False(secondExecuted, "The second one should not have been executed");
        }

        [Fact]
        public void RegisterMachineFallback_NoMachineMatches_DefaultIsCalled() {
            var sut = new ConfigHub();

            bool firstExecuted = false;
            bool secondExecuted = false;

            sut.AddMachineFallbackProvider("notmymachinename", (settingName) => {
                firstExecuted = true;
                return "Hello";
            });
            sut.AddMachineFallbackProvider(ConfigHub.DEFAULTMACHINENAME, (settingName) => {
                secondExecuted = true;
                return "Hello";
            });

            string result = sut.GetSetting("TestSettingName");

            Assert.False(firstExecuted, "The first one should not be executed as the machine name does not match.");
            Assert.True(secondExecuted, "The second one should have been executed as a default");
        }

        [Fact]
        public void GetTypedSetting_DoesExecuteFallback() {
            var sut = new ConfigHub();
            bool fallbackExecuted = false;
            sut.AddMachineFallbackProvider(ConfigHub.DEFAULTMACHINENAME, (settingName) => {
                fallbackExecuted = true;
                return "0";
            });

            int val = sut.GetSetting<int>("Test");
            Assert.True(fallbackExecuted, "The fallback was not execued");
        }

        [Fact]
        public void GetTypedSetting_FromFallback_ReturnsBool() {
            var sut = new ConfigHub();
            sut.AddMachineFallbackProvider(ConfigHub.DEFAULTMACHINENAME, (settingName) => {
                return "true";
            });

            bool val = sut.GetSetting<bool>("SettingName");
            Assert.True(val, "The incorrect value was returned");
        }

        [Fact]
        public void GetTypedSetting_FromFallback_ReturnsInt() {
            ConfigHub sut = new ConfigHub();
            sut.AddMachineFallbackProvider(ConfigHub.DEFAULTMACHINENAME, (settingName) => {
                return "12367";
            });

            int val = sut.GetSetting<int>("SettingName");
            Assert.Equal(12367, val);
        }

        [Fact]
        public void GetTypedSetting_FromFallback_ReturnsDouble() {
            ConfigHub sut = new ConfigHub();
            sut.AddMachineFallbackProvider(ConfigHub.DEFAULTMACHINENAME, (settingName) => {
                return "1236754";
            });

            double val = sut.GetSetting<double>("SettingName");
            Assert.Equal(1236754, val);
        }

        [Fact]
        public void Crypto_DecryptorPresent_ChangesValue() {
            var sut = new ConfigHub();
            var msc = new MockSimpleCrypto();
            sut.RegisterProvider("CryptoTest", () => {
                return "xxx";
            });

            sut.CryptoProvider = msc;
            string s = sut.GetSetting<string>("CryptoTest", false, true);

            Assert.NotNull(s);
            Assert.Equal(msc.AlwaysThisValue, s);
        }

        [Fact]
        public void Crypto_DecryptorPresent_NoChangeNonEncrypted() {
            string inputVal = "xxx";
            var sut = new ConfigHub();
            var msc = new MockSimpleCrypto();
            sut.RegisterProvider("CryptoTest", () => {
                return inputVal;
            });

            sut.CryptoProvider = msc;
            string s = sut.GetSetting<string>("CryptoTest", false, false);

            Assert.Equal(s, inputVal);
        }

        [Fact]
        public void Crypto_DecryptNeeded_NoCrypto_ThrowsException() {
            var sut = new ConfigHub();

            sut.RegisterProvider("CryptoTest", () => {
                return "xxx";
            });

            Assert.Throws<ConfigHubConfigurationFailureException>(() =>
              sut.GetSetting<string>("CryptoTest", false, true)
            );
        }

        [Fact]
        public void Crypto_SimpleCryptoWorks() {
            var sut = new ConfigHub();
            var scc = new SimpleCryptoConfigProvider(12);
            sut.CryptoProvider = scc;

            string sec = "secret oh secret";
            string b64Val = scc.Encrypt(sec);

            sut.RegisterProvider("testVal", () => {
                return b64Val;
            });

            var result = sut.GetSetting("testVal", true, true);

            Assert.Equal(sec, result);
        }

        //TODO: Fix this up to run on the build server.
#if false
        [Fact(DisplayName = nameof(Bug_ConfigHubExceptionExposedAllSettings))]

        public void Bug_ConfigHubExceptionExposedAllSettings() {
            b.Info.Flow();

            // Bug introduced with the case insensitive fix, meant that in the event of no match
            // all of the contents of a settings file were returned.

            var sut = new ConfigHub();
            sut.AddDirectoryFallbackProvider("%PLISKYAPPROOT%\\Config\\", "tkd1100.settings");
            bool b2 = sut.GetSetting<bool>("doesnotexist");

            Assert.False(b2);
        }

        [Fact(DisplayName = nameof(Bug_ConfigHubExeption_ReturnedWrongOnNoMatch))]

        public void Bug_ConfigHubExeption_ReturnedWrongOnNoMatch() {
            b.Info.Flow();

            // Bug introduced with the case insensitive fix, meant that in the event of no match
            // all of the contents of a settings file were returned.

            var sut = new ConfigHub();
            ConfigHub.Current.AddDirectoryFallbackProvider("%PLISKYAPPROOT%\\Config\\", "tkdEnv_1100.donotcommit");
            ConfigHub.Current.AddDirectoryFallbackProvider("[APP]", "prod.donotcommit");

            var accountSid = ConfigHub.Current.GetSetting("twilio-accountsid", true);
            var authToken = ConfigHub.Current.GetSetting("twilio-authtoken", true);
            var sendGridApiKey = ConfigHub.Current.GetSetting("twilio-sendgridapi", true);

            string b2 = sut.GetSetting<string>("doesnotexist");

            Assert.Null(b2);
            Assert.Null(sendGridApiKey);
        }

#endif
    }
}