using Plisky.Win32;
using System;
using System.Runtime.InteropServices;

namespace Plisky.Native {

    public delegate void PerformHotkeyAction(string parameter);

    public class HotkeyStore {
        public string ActionParameter { get; set; }
        public IntPtr HWnd { get; set; }
        public MK Modifier { get; set; }
        public VK_Keys Controller { get; set; }
        public int UniqueIdentifier { get; set; }

        public PerformHotkeyAction Action { get; set; }

        public bool Register() {
            int testerr;
            bool res = NativeMethods.RegisterHotKey(HWnd, UniqueIdentifier, (uint)Modifier, (uint)Controller);
            testerr = Marshal.GetLastWin32Error();
            return res;
        }

        public bool Unregister() {
            return NativeMethods.UnregisterHotKey(HWnd, UniqueIdentifier);
        }
    }
}