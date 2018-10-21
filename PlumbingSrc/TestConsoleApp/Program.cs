using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;
using Plisky.Plumbing;
using System;
using System.Diagnostics;
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

            b.Info.Log("Go!");
            HttpHelper hh = new HttpHelper("http://myos.azurewebsites.net/");
            HttpHelper h2 = new HttpHelper("http://www.justsoballoons.co.uk/");
            hh.Stem = "home/carmen";
            hh.Verb = HttpMethod.Get;
            var wcr = await hh.Execute("");
            var jsb = await h2.Execute("");

            Console.WriteLine("Done");
            Console.WriteLine(wcr.ResponseText);
            Console.WriteLine(jsb.ResponseText);
            //Console.ReadLine();
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