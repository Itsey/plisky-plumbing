#if NET452
using System;

namespace Plisky.Win32 {
#pragma warning disable CS3001
#pragma warning disable CS3002
#pragma warning disable CS3003
#pragma warning disable CS3009

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1714:FlagsEnumsShouldHavePluralNames"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1028:EnumStorageShouldBeInt32"), Flags]
    public enum MouseActivity : uint {
        None = 0x0000,

        XButton1 = 0x0001,
        XButton2 = 0x0002,
        Move = 0x0001,
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        XDown = 0x0080,
        XUp = 0x0100,
        Wheel = 0x0800,
        VirtualDesk = 0x4000,
        Absolute = 0x8000
    }

#pragma warning restore CS3001
#pragma warning restore CS3002
#pragma warning restore CS3003
#pragma warning restore CS3009
}
#endif 