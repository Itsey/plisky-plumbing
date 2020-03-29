﻿using Newtonsoft.Json.Linq;
using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;
using Plisky.Plumbing;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Plisky.Test {

    internal class Program {

        static async Task<int> Main(string[] args) {
            Console.WriteLine("online");
            ConfigHubTests();
            // Test
            var f = new FeatureHardCodedProvider();
            f.AddFeature(new Feature("TEST", true));



            if (Feature.GetFeatureByName("TEST").Active) {

            }

            CommandLineArgs cla = new CommandLineArgs();
            CommandArgumentSupport clas = new CommandArgumentSupport();
            clas.ProcessArguments(cla, args);
            clas.AddExample("TestConsoleApp.exe -x", "Runs it with an x parameter");
            string sa = clas.GenerateShortHelp(cla, "TestConsoleApp");
            Console.WriteLine("sec");
            Console.WriteLine(sa);

            await HHTestExecute();

            Console.ReadLine();
                
            return 0;

        }

        private static async Task HHTestExecute() {
            HttpHelper hh = new HttpHelper("https://52.142.237.56/api/battlerequest/");
            hh.Verb = HttpMethod.Post;
            hh.Host = "";
            hh.AcceptAllCerts = true;
            var res = await hh.Execute("", "\"value\":\"asdf\"");

            Console.WriteLine($"Worked : {res.Success} {res.Status}");
            if (!res.Success) {
                Console.WriteLine($"Res: {res.ErrorText} {res.Exception.Message}" );
            }

        }

        private static void ConfigHubTests() {
            //            ConfigHub.Current.AddDirectoryFallbackProvider
            ConfigHub.Current.AddDirectoryFallbackProvider("%PLISKYAPPROOT%\\config\\");
            string chromeDriverPath = ConfigHub.Current.GetSetting<string>("chromedriverpath", false);
            string edgeDriverPath = ConfigHub.Current.GetSetting<string>("EdgeDriverPath".ToLowerInvariant(), false);
            string remoteDriver = ConfigHub.Current.GetSetting<string>("RemoteDriver", false);
            string remoteDriverAccessKey = ConfigHub.Current.GetSetting<string>("RemoteDriverAccessKey", false);

            ConfigHub.Current.AddDirectoryFallbackProvider("%PLISKYAPPROOT%\\config\\", "tests");
            var s = ConfigHub.Current.GetSetting<string>("testvalue");

            if (s != "CONSTR-Value") {
                throw new InvalidOperationException();
            }

        }

        private static async Task<string> SaveQuestion(string s) {
            HttpHelper hh = new HttpHelper("https://www.hackerrank.com/x/api/v1/");
            hh.AddBearerAuthHeader("9674c8fdc7a43b8ef9662879c20063d89a9fc507484e23afc0ac90359c027e8e");
            hh.Stem = "questions/";
            var f = await hh.Execute("", s, HttpMethod.Post);
            return f.ResponseText;
            throw new NotImplementedException();
        }

        private static async Task<string> GetQuestionById(int v) {
            HttpHelper hh = new HttpHelper("https://www.hackerrank.com/x/api/v1/");
            hh.AddBearerAuthHeader("9674c8fdc7a43b8ef9662879c20063d89a9fc507484e23afc0ac90359c027e8e");
            hh.Stem = "questions/";
            var wcr = await hh.Execute("232646");


            return wcr.ResponseText;

            throw new NotImplementedException();
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