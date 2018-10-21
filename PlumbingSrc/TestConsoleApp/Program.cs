using Newtonsoft.Json.Linq;
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
            var b = new Bilge("TestPRog", tl: TraceLevel.Verbose);
            b.AddHandler(new TCPHandler("127.0.0.1", 9060));
            b.Info.Log("Online");
           // Original();
            NewOne();
            /*
            var uri = @"https://www.hackerrank.com/x/api/v1/";
            var qstem = "questions/";
            string method = "GET";
            string secret = "9674c8fdc7a43b8ef9662879c20063d89a9fc507484e23afc0ac90359c027e8e";
            string body = null;
            string qparam = null;

            //

            int whatToDo = 3;

            if (whatToDo == 1) {
                // add question
                method = "GET";
                body = null;
                qparam = "232646";
            } else if (whatToDo == 2) {
                method = "POST";
                body = null;
                qparam = "232646";
                //string strversionofq = File.ReadAllText(@"C:\temp\q.json");
                //var f = JObject.Parse(strversionofq);

            } else if (whatToDo == 3) {
                method = "GET";
                qstem = "tests/291241/questions/232646/testcases/";
                qparam = "0";
                body = "";
            }
            */



            b.Info.Log("Go!");
            HttpHelper hh = new HttpHelper("https://www.hackerrank.com/x/api/v1/");
            hh.AddBearerAuthHeader("9674c8fdc7a43b8ef9662879c20063d89a9fc507484e23afc0ac90359c027e8e");
            hh.Stem = "questions/";
            var wcr = await hh.Execute("232646");
            string dir = @"D:\Temp\_DelWorking\QuestionContent";

            File.WriteAllText(@"D:\Temp\_DelWorking\QuestionContent\quest.json", wcr.ResponseText);

            //  "cobol_template_head": "",  "cobol_template_tail": "",  "cobol_template": 
            var hd = File.ReadAllText(Path.Combine(dir, "cobol_head.txt"));
            var bd = File.ReadAllText(Path.Combine(dir, "cobol_bod.txt"));
            var tl = File.ReadAllText(Path.Combine(dir, "cobol_tail.txt"));

            string s = File.ReadAllText(@"d:\temp\adam.json");
            JObject jo = JObject.Parse(s);
            /*jo["model"]["cobol_template_head"] = hd;
            jo["model"]["cobol_template_tail"] = tl;
            jo["model"]["cobol_template"] = bd;*/
            // jo["model"]["allowedLanguages"] = "csharp,cobol";

            hh.BaseUri = "https://www.hackerrank.com/x/api/v1/";

            var f = await hh.Execute("",jo.ToString(), HttpMethod.Post);

            Console.WriteLine(f.Status);
            File.WriteAllText(@"D:\Temp\_DelWorking\QuestionContent\quest2.json", jo.ToString());
            //Console.WriteLine(wcr.ResponseText);
            Console.ReadLine();
            return 0;
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