using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Plisky.Infrastructure {

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
        public System.Int32 IntegerSalt;

        public void FillWithSalt() {
            // Grr unions completely dont work properly in c# this is completely lame but does
            // what i want in the end.  Could of course use << << but why!
            byte[] ByteArraySalt = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetNonZeroBytes(ByteArraySalt);

            this.Byte1 = ByteArraySalt[0];
            this.Byte2 = ByteArraySalt[1];
            this.Byte3 = ByteArraySalt[2];
            this.Byte4 = ByteArraySalt[3];
        }

        public override bool Equals(object obj) {
            if (!(obj is PasswordSalt)) { return false; }

            return this.IntegerSalt == ((PasswordSalt)obj).IntegerSalt;
        }
    }
}