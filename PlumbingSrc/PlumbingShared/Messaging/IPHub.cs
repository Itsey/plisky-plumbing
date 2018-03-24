namespace Plisky.Messaging {
#if false
    using Plisky.Plumbing;
    using System.IO.MemoryMappedFiles;
    using System.Threading;
    using System.Text;

    /// <summary>
    /// Interprocess Hub, sends messages between processes on the same machine.  Uses a specific message format to send messages
    /// between processes and optionally get responses back.
    /// </summary>
    public class IPHub : Hub {
        private string ipcEventSignalName;
        private string ipcEventRecieveName;
        private string ipcMMFName;

        private byte[] message = new byte[100];
        private EventWaitHandle messageWait;
        private EventWaitHandle messageHandled;
        private MemoryMappedFile mmf;
        private MemoryMappedViewStream view;

        private void UpdateNames(string newName) {
            CloseDownIPC();

            ipcEventRecieveName = "ipchub_rcv_" + newName;
            ipcEventSignalName = "ipchub_sig_" + newName;
            ipcMMFName = "ipchub_mmf_" + newName;
        }

        private void OpenUpIPC() {
            messageWait = new EventWaitHandle(false, EventResetMode.AutoReset, ipcEventRecieveName);
            messageHandled = new EventWaitHandle(false, EventResetMode.AutoReset, ipcEventSignalName);
            mmf = MemoryMappedFile.CreateOrOpen("mmf", 100);

            view = mmf.CreateViewStream();
        }

        private void CloseDownIPC() {
            throw new System.NotImplementedException();
        }

        private bool quit;

        private void ThreadLoop() {
            while (!quit) {
                messageWait.WaitOne();
                b.Info.Log("trigger");
                if (quit) {
                    break;
                }
                view.Position = 0;
                view.Read(message, 0, 100);
                //Application.DoEvents();
                Thread.Sleep(100);
                messageHandled.Set();
                //DoStuff(message);
            }

            //var messageWait = new EventWaitHandle(false, EventResetMode.AutoReset, "wait");
            //var messageHandled = new EventWaitHandle(false, EventResetMode.AutoReset, "handled");
            //var mmf = MemoryMappedFile.CreateOrOpen("mmf", 100);
            //var view = mmf.CreateViewStream();
            byte[] buffer = new byte[100];
            byte[] temp = ASCIIEncoding.UTF8.GetBytes("testmessage");
            for (int i = 0; i < 100; i++) {
                buffer[i] = 0;
                if (i < temp.Length) {
                    buffer[i] = temp[i];
                }
            }
            view.Write(buffer, 0, 100);
            messageWait.Set();
        }

        /// <summary>
        /// Magic string to help two applications join up.  Not required if all applications on the box recieve all messages,  only
        /// if you are running mutiple IPHubs and dont want them to interact.  If this is the case give the IPHubs that you do want
        /// to interact the same IPCKeyValue.
        /// </summary>
        public string IPCKeyValue { get; set; }

        public IPHub() {
            UpdateNames("dflt");
        }
    }
#endif
}