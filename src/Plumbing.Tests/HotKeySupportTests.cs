#if false
namespace Plisky.Test {
    using Plisky.Native;
    using System;
    using Xunit;


    public class HotKeySupportTests {


        [Fact]
        [Trait("xunit", "regression")]
        public void ParseHK_SampleControlX_Works() {
            string hkString = "Control_X";
            HotkeySupport sut = new HotkeySupport();
            MK parsedModifier;
            VK_Keys parsedKey;

            sut.ParseKeyStringToModAndKey(hkString, out parsedModifier, out parsedKey);
            Assert.Equal(MK.Control, parsedModifier);
            Assert.Equal(VK_Keys.X, parsedKey);
        }

        [Fact]
        [Trait("xunit", "regression")]
        public void ParseHK_WinF_IsFound() {
            string hkString = "Win_F";
            HotkeySupport sut = new HotkeySupport();
            MK parsedModifier;
            VK_Keys parsedKey;

            sut.ParseKeyStringToModAndKey(hkString, out parsedModifier, out parsedKey);
            Assert.Equal(MK.Win, parsedModifier);
            Assert.Equal(VK_Keys.F, parsedKey);
        }

        [Fact]
        [Trait("xunit", "regression")]
        public void ParseHK_InvalidModifierThrowsException() {
            string hkString = "CTL_X";
            HotkeySupport sut = new HotkeySupport();
            MK parsedModifier;
            VK_Keys parsedKey;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
              sut.ParseKeyStringToModAndKey(hkString, out parsedModifier, out parsedKey)
            );
        }

        [Fact]
        [Trait("xunit", "regression")]
        public void ParseHK_InvalidKeyThrowsException() {
            string hkString = "Control_FFS";
            HotkeySupport sut = new HotkeySupport();
            MK parsedModifier;
            VK_Keys parsedKey;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            sut.ParseKeyStringToModAndKey(hkString, out parsedModifier, out parsedKey)
            );
        }

        [Fact]
        [Trait("xunit", "regression")]
        public void ParseHK_ControlAlt_IsFound() {
            string hkString = "CONTROLALT_X";
            HotkeySupport sut = new HotkeySupport();
            MK parsedModifier;
            VK_Keys parsedKey;

            sut.ParseKeyStringToModAndKey(hkString, out parsedModifier, out parsedKey);
            MK ctrlAlt = MK.Control | MK.Alt;
            Assert.Equal(ctrlAlt, parsedModifier);
            Assert.Equal(VK_Keys.X, parsedKey);
        }

    }
}
#endif