
namespace Plisky.Test {
    using Plisky.Diagnostics;
    using Plisky.Diagnostics.Listeners;
    using Plisky.Plumbing;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using Xunit;

    public class ConfigHubTests {
        private UnitTestHelper uth = new UnitTestHelper();
        protected Bilge b = new Bilge(tl:System.Diagnostics.TraceLevel.Verbose);
        
        public ConfigHubTests () {
            var h = new TCPHandler("127.0.0.1", 9060);            
            b.AddHandler(h);
        }

        ~ConfigHubTests() {
            uth.ClearUpTestFiles();
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfigCurrentInstance_SingleProvider_ReturnsCorrectly() {
            string returnString = TestData.GenericString1;

            ConfigHub.Current.RegisterProvider("test", () => {
                return returnString;
            });

            string actual = ConfigHub.Current.GetSetting("test");

            Assert.Equal(returnString, actual);
        }


        [Fact(DisplayName = nameof(GetSetting_ThrowsInfNullSetting))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void GetSetting_ThrowsInfNullSetting() {
            b.Info.Flow();

            ConfigHub sut = new ConfigHub();
            Assert.Throws<ArgumentNullException>(() => {
                sut.GetSetting(null);
            });            
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfig_SingleProvider_ReturnsCorrectly() {
            string returnString = "arflebarflegloop";

            ConfigHub sut = new ConfigHub();
            sut.RegisterProvider("test", () => {
                return returnString;
            });

            string actual = sut.GetSetting("test");

            Assert.Equal(returnString, actual);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
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

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfig_NoProvider_ReturnsNullIfNotRequired() {
            ConfigHub sut = new ConfigHub();
            string res = sut.GetSetting("arflebarfle");
            Assert.Null(res);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]        
        public void GetConfig_NoProvider_ThrowsIfRequired() {
            ConfigHub sut = new ConfigHub();
            Assert.Throws<ConfigHubMissingConfigException>( () => sut.GetSetting("arflebarfle", true));
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
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


#if DEBUG
        [Fact(DisplayName = nameof(EmptyDirectory_DefaultsToCurrent))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Developer)]
        public void EmptyDirectory_DefaultsToCurrent() {
            b.Info.Flow();

            ConfigHub sut = new ConfigHub();
            
            var cd = sut.Test_GetDirectoryName(string.Empty);            
            Assert.False(string.IsNullOrWhiteSpace(cd));
            
        }

        [Fact(DisplayName = nameof(Directory_ResolvesEnvironmentVariable))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Developer)]
        public void Directory_ResolvesEnvironmentVariable() {
            b.Info.Flow();

            var par = Environment.GetEnvironmentVariable("PLISKYAPPROOT");
            Assert.NotNull(par);  // Validation check that this machine is configured correct.

            ConfigHub sut = new ConfigHub();
            
            var dn = sut.Test_GetDirectoryName("%PLISKYAPPROOT%\\MyDir");
            Assert.Equal(par + "\\MyDir", dn);
        }
#endif

        [Fact(DisplayName = nameof(Directory_DoubleFallbackWorks))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Integration)]
        public void Directory_DoubleFallbackWorks() {
            b.Info.Flow();


            ConfigHub sut = new ConfigHub();
            sut.InjectBilge(b);
            sut.AddDirectoryFallbackProvider("C:\\DoesNotExistAtAll", "tests.xml");
            sut.AddDirectoryFallbackProvider("%PLISKYAPPROOT%\\Config", "tests.xml");

            var str = sut.GetSetting("testconstr", true);

            Assert.Equal("CONSTR-Value", str);
        }

        [Fact(DisplayName = nameof(Directory_FullConfigStringRetrieval))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Integration)]
        public void Directory_FullConfigStringRetrieval() {
            b.Info.Flow();

            
            ConfigHub sut = new ConfigHub();

            sut.AddDirectoryFallbackProvider("%PLISKYAPPROOT%\\Config","tests.xml");
            var str = sut.GetSetting("testconstr",true);

            Assert.Equal("CONSTR-Value", str);
        }


        [Fact(DisplayName = nameof(Directory_FullConfigStringRetrievalCrypto))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Integration)]
        public void Directory_FullConfigStringRetrievalCrypto() {
            b.Info.Flow();
            const string PLAINTEXT = "ItAllWorked";

            ConfigHub sut = new ConfigHub();
            var mcr = new MockSimpleCrypto();
            mcr.AddDecryption("CONSTR-Value", PLAINTEXT);
            sut.CryptoProvider = mcr;
            sut.AddDirectoryFallbackProvider("%PLISKYAPPROOT%\\Config", "tests.xml");
            var str = sut.GetSetting("testconstr", true,true);

            Assert.Equal(PLAINTEXT, str);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfig_DirectoryFallbackProvider_Works() {
            const string testData = "bannana";
            string tmp = Path.GetTempPath() + Environment.MachineName + ".chcfg";
            uth.RegisterTemporaryFilename(tmp);
            XDocument x = new XDocument();
            x.Add(new XElement("chub_settings", new XElement("settings", new XElement("monkeyfish", testData))));
            x.Save(tmp);
            ConfigHub sut = new ConfigHub();
            sut.AddDirectoryFallbackProvider(Path.GetTempPath());
            var res = sut.GetSetting("monkeyfish", true);

            Assert.Equal(testData, res);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfig_FallbackProvider_CalledForAllRequests() {
            Dictionary<string, bool> testSettings = new Dictionary<string, bool>();

            // Used to avoid issue where changing collection below causes fault.
            List<string> settingNames = new List<string>();

            for (int i = 0; i < 10; i++) {
                string next = "testsetting" + i.ToString();
                testSettings.Add(next, false);
                settingNames.Add(next);
            }

            ConfigHub sut = new ConfigHub();
            sut.RegisterFallbackProvider((pName) => {
                if (testSettings.ContainsKey(pName)) {
                    testSettings[pName] = true;
                }
                return null;
            });

            foreach (var v in settingNames) {
                sut.GetSetting(v);
            }

            foreach (var test in testSettings.Keys) {
                Assert.True(testSettings[test], "The setting " + test + " was not hit");
            }
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfig_FallbackProvider_CalledForAllStrings() {
            TestData td = new TestData();
            ConfigHub sut = new ConfigHub();
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

        [Fact][Trait(Traits.Age,Traits.Regression)]
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

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfig_FallbackAppConfigProvider_Returnsconfiguration() {
            ConfigHub sut = new ConfigHub();
            sut.AddDefaultAppConfigFallback();
            string val = sut.GetSetting("testSettingValue");
            Assert.Equal("arfleBARFLEgloop", val);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfig_FallbackAppConfigProvider_NotInstalledByDefault() {
            ConfigHub sut = new ConfigHub();
            string val = sut.GetSetting("testSettingValue");
            Assert.Null(val);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfig_FallbackAppConfigProvider_IsCaseInsensitive() {
            ConfigHub sut = new ConfigHub();
            sut.AddDefaultAppConfigFallback();
            string val = sut.GetSetting("TESTsEttInGvAlUe");
            Assert.Equal("arfleBARFLEgloop", val);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfig_CustomType_ReturnsCorrectly() {
            ConfigHub sut = new ConfigHub();
            sut.RegisterProvider<TestMessage>("settingVal", () => {
                return new TestMessage("HelloWorld");
            });
            TestMessage result = sut.GetSetting<TestMessage>("settingVal");

            Assert.Equal("HelloWorld", result.Data);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfig_FallbackAppConfig_NestedIsEmptyWhenNotPresent() {
            string combinedString = "ARFLEUSBarfleyGloopify";
            ConfigHub sut = new ConfigHub();
            sut.AddDefaultAppConfigFallback();

            string val = sut.GetSetting("testSettingSubValue2");
            Assert.Equal(combinedString, val);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfig_FallbackAppConfig_SupportsNestedReplacements() {
            string combinedString = "contactinatedIntoARFLEUSBarfleyGloopify";
            ConfigHub sut = new ConfigHub();
            sut.AddDefaultAppConfigFallback();

            string val = sut.GetSetting("testSettingSubValue");

            Assert.Equal(combinedString, val);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetConfig_GetSetting_SpecificOveridesFallback() {
            ConfigHub sut = new ConfigHub();
            sut.AddDefaultAppConfigFallback();
            sut.RegisterProvider("testSettingSubValue2", () => {
                return "NothingHere";
            });
            string val = sut.GetSetting("testSettingSubValue2");
            Assert.Equal("NothingHere", val);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void RegisterMachineFallback_CallBackOccursForThisMachine() {
            ConfigHub sut = new ConfigHub();
            string MachineName = Environment.MachineName;
            string expected = "Hello";
            bool executed = false;

            sut.AddMachineFallbackProvider(MachineName, (settingName) => {
                executed = true;
                return expected;
            });

            string result = sut.GetSetting("TestSettingName");
            Assert.True(executed, "The handler should have been executed");
            Assert.Equal(expected, result);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]        
        public void RegisterMachineFallback_DuplicateName_CausesException() {
            ConfigHub sut = new ConfigHub();
            string MachineName = "MachineName";

            sut.AddMachineFallbackProvider(MachineName, (settingName) => {
                return "Hello";
            });
            Assert.Throws<InvalidOperationException>(() =>
              sut.AddMachineFallbackProvider(MachineName, (settingName) => {
                  return "Hello";
              }));
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void RegisterFallbacks_ExcutedInOrderOfRegistration() {
            ConfigHub sut = new ConfigHub();
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

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void RegisterMachineFallback_NoMachineMatches_DefaultIsCalled() {
            ConfigHub sut = new ConfigHub();

            bool firstExecuted = false;
            bool secondExecuted = false;

            sut.AddMachineFallbackProvider("notmymachinename", (settingName) => {
                firstExecuted = true;
                return "Hello";
            });
            sut.AddMachineFallbackProvider(ConfigHub.DefaultMachineName, (settingName) => {
                secondExecuted = true;
                return "Hello";
            });

            string result = sut.GetSetting("TestSettingName");

            Assert.False(firstExecuted, "The first one should not be executed as the machine name does not match.");
            Assert.True(secondExecuted, "The second one should have been executed as a default");
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetTypedSetting_DoesExecuteFallback() {
            ConfigHub sut = new ConfigHub();
            bool fallbackExecuted = false;
            sut.AddMachineFallbackProvider(ConfigHub.DefaultMachineName, (settingName) => {
                fallbackExecuted = true;
                return "0";
            });

            int val = sut.GetSetting<int>("Test");
            Assert.True(fallbackExecuted, "The fallback was not execued");
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetTypedSetting_FromFallback_ReturnsBool() {
            ConfigHub sut = new ConfigHub();
            sut.AddMachineFallbackProvider(ConfigHub.DefaultMachineName, (settingName) => {
                return "true";
            });

            bool val = sut.GetSetting<bool>("SettingName");
            Assert.True(val, "The incorrect value was returned");
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetTypedSetting_FromFallback_ReturnsInt() {
            ConfigHub sut = new ConfigHub();
            sut.AddMachineFallbackProvider(ConfigHub.DefaultMachineName, (settingName) => {
                return "12367";
            });

            int val = sut.GetSetting<int>("SettingName");
            Assert.Equal(12367, val);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void GetTypedSetting_FromFallback_ReturnsDouble() {
            ConfigHub sut = new ConfigHub();
            sut.AddMachineFallbackProvider(ConfigHub.DefaultMachineName, (settingName) => {
                return "1236754";
            });

            double val = sut.GetSetting<double>("SettingName");
            Assert.Equal(1236754, val);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void Crypto_DecryptorPresent_ChangesValue() {
            ConfigHub sut = new ConfigHub();
            MockSimpleCrypto msc = new MockSimpleCrypto();
            sut.RegisterProvider("CryptoTest", () => {
                return "xxx";
            });

            sut.CryptoProvider = msc;
            var s = sut.GetSetting<string>("CryptoTest", false, true);

            Assert.NotNull(s);
            Assert.Equal(msc.AlwaysThisValue, s);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void Crypto_DecryptorPresent_NoChangeNonEncrypted() {
            string inputVal = "xxx";
            ConfigHub sut = new ConfigHub();
            MockSimpleCrypto msc = new MockSimpleCrypto();
            sut.RegisterProvider("CryptoTest", () => {
                return inputVal;
            });

            sut.CryptoProvider = msc;
            var s = sut.GetSetting<string>("CryptoTest", false, false);

            Assert.Equal(s, inputVal);
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]        
        public void Crypto_DecryptNeeded_NoCrypto_ThrowsException() {
            ConfigHub sut = new ConfigHub();

            sut.RegisterProvider("CryptoTest", () => {
                return "xxx";
            });

            Assert.Throws<ConfigHubConfigurationFailureException> ( () => 
                sut.GetSetting<string>("CryptoTest", false, true)
            );
        }

        [Fact][Trait(Traits.Age,Traits.Regression)]
        public void Crypto_SimpleCryptoWorks() {
            ConfigHub sut = new ConfigHub();
            SimpleCryptoConfigProvider scc = new SimpleCryptoConfigProvider(12);
            sut.CryptoProvider = scc;

            string sec = "secret oh secret";
            string B64Bal = scc.Encrypt(sec);

            sut.RegisterProvider("testVal", () => {
                return B64Bal;
            });

            var result = sut.GetSetting("testVal", true, true);

            Assert.Equal(sec, result);
        }
    }
}
