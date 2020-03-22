using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Plisky.Plumbing {

    [StructLayout(LayoutKind.Explicit)]
    public struct PasswordSalt {
        // Crapola.  REally need a byte array here.

        [FieldOffset(0)]
        internal byte Byte1;

        [FieldOffset(1)]
        internal byte Byte2;

        [FieldOffset(2)]
        internal byte Byte3;

        [FieldOffset(3)]
        internal byte Byte4;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "integer")]
        [FieldOffset(0)]
        public int IntegerSalt;

        public void FillWithSalt() {
            // Grr unions completely dont work properly in c# this is completely lame but does
            // what i want in the end.  Could of course use << << but why!
            byte[] byteArraySalt = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(byteArraySalt);

            this.Byte1 = byteArraySalt[0];
            this.Byte2 = byteArraySalt[1];
            this.Byte3 = byteArraySalt[2];
            this.Byte4 = byteArraySalt[3];
        }

        public override bool Equals(object obj) {
            if (!(obj is PasswordSalt)) { return false; }

            return this.IntegerSalt == ((PasswordSalt)obj).IntegerSalt;
        }
    }
}