namespace Plisky.Test {

    using System;
    using System.Threading.Tasks;
    using Plisky.Plumbing;

    internal class Program {

        private static async Task<int> Main(string[] args) {
            Console.WriteLine("online");
            ConfigHubTests();
            // Test
#if false
            var f = new FeatureHardCodedProvider();
            f.AddFeature(new Feature("TEST", true));

            if (Feature.GetFeatureByName("TEST").Active) {
            }
#endif
            var cla = new CommandLineArgs();
            var clas = new CommandArgumentSupport();
            clas.ProcessArguments(cla, args);
            clas.AddExample("TestConsoleApp.exe -x", "Runs it with an x parameter");
            clas.AddExample("MonkeyFishBannana -x -y", "Does something cool");
            string sa = clas.GenerateShortHelp(cla, "TestConsoleApp");

            Console.WriteLine("sec");
            Console.WriteLine(sa);

            Console.ReadLine();

            return 0;
        }

        private static void ConfigHubTests() {
            LiveFaultOnJSB();

            //            ConfigHub.Current.AddDirectoryFallbackProvider
            ConfigHub.Current.AddDirectoryFallbackProvider("%PLISKYAPPROOT%\\config\\");
            string chromeDriverPath = ConfigHub.Current.GetSetting<string>("chromedriverpath", false);
            string edgeDriverPath = ConfigHub.Current.GetSetting<string>("EdgeDriverPath".ToLowerInvariant(), false);
            string remoteDriver = ConfigHub.Current.GetSetting<string>("RemoteDriver", false);
            string remoteDriverAccessKey = ConfigHub.Current.GetSetting<string>("RemoteDriverAccessKey", false);

            ConfigHub.Current.AddDirectoryFallbackProvider("%PLISKYAPPROOT%\\config\\", "tests");
            string s = ConfigHub.Current.GetSetting<string>("testvalue");

            if (s != "CONSTR-Value") {
                throw new InvalidOperationException();
            }
        }

        private static void LiveFaultOnJSB() {
            ConfigHub.Current.AddDirectoryFallbackProvider("%PLISKYAPPROOT%\\Config\\", "jsb_1000.donotcommit");
            var f = new Feature("JSB_FEATURED_HALLOWEEN", true);
            f.SetDateRange(new DateTime(2019, 10, 1), new DateTime(2019, 10, 31), true);
        }

        private static void NewOne() {
#if false
            NewBilge nb = new NewBilge("Context");

            nb.AddHandler(new TCPHandler("127.0.0.1", 9060));
            //nb.AddHandler(new ODSHandler());
           // nb.Error.Log("Hello");
            nb.Info.Log("ONLINE (NEW) .......");
            Console.WriteLine("This is written to the console NEW");
            Console.Error.WriteLine("this is written to the error NEW ");
           // nb.Info.Log("DONE....");
            for (int i = 0; i < 10; i++) {
                Thread.Sleep(200);
            }

            Emergency.Diags.Shutdown();
            Console.WriteLine("Hit a key");
            Console.ReadLine();
            //nb.Info.Log("REALLY ? .....");
            CommandArgumentSupport cas = new CommandArgumentSupport();
        }

      /*  private static void Original() {
            OldBilge.QueueMessages = false;
            OldBilge.Log("ONLINE.....");
            Console.WriteLine("This is written to the console");
            Console.Error.WriteLine("this is written to the error");
            OldBilge.Log("DONE....");
            Thread.Sleep(2000);
            OldBilge.Log("REAlly");
        }*/
#endif
        }
    }
}