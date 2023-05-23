namespace Plisky.Plumbing {

    using System;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;
    using Plisky.Diagnostics;

    public class SaltyPassword {
        public const int MINIMUM_PASSWORD_LENGTH = 5;
        public const string VALID_PASSWORD_CHARACTERS = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ1234567890_+-=[]{}\\/?!.,£$%^&*()";
        public PasswordSalt Salt;
        protected Bilge b = new Bilge("plisky-plumbing-spw");

        private static bool hashB64Enc = true;
        private SecureString m_password;

        /// <summary>
        /// Createa new salty password
        /// </summary>
        /// <param name="password">The password to use</param>
        /// <param name="pws">The salt to use</param>
        public SaltyPassword(SecureString password, PasswordSalt pws) {
            this.Password = password;
            this.Salt.IntegerSalt = pws.IntegerSalt;
        }

        /// <summary>
        /// Createa new salty password
        /// </summary>
        /// <param name="password">The password to use</param>
        /// <param name="pws">The salt to use</param>
        public SaltyPassword(SecureString password, int pws) {
            this.Password = password;
            this.Salt.IntegerSalt = pws;
        }

        /// <summary>
        /// Createa new empty salty password
        /// </summary>
        private SaltyPassword() {
            this.Password = new SecureString();
            this.Salt = new PasswordSalt();
        }

        public static bool HashesAreB64Encoded {
            get { return hashB64Enc; }
            set { hashB64Enc = value; }
        }

        public SecureString Password {
            get { return m_password; }
            set {
                if (value == null) { throw new ArgumentNullException("The password can not be null"); }
                m_password = value;
            }
        }

        /// <summary>
        /// Returns the password hadh
        /// </summary>
        public string PasswordSaltHash {
            get {
                return SaltyPassword.ComputeSaltedHash(m_password, Salt);
            }
        }

        /// <summary>
        /// This method will compute a hash of a password and salt combination.  As a result it does need to take the secure string and read
        /// through its contents to compute the hash, this is probably the only time the secure string is not secure, other than when its
        /// created in the first place.
        /// </summary>
        /// <param name="password">The password that is to be hashed.</param>
        /// <param name="salt">The salt to hash with the password so that the password hash is more secure</param>
        /// <exception cref="ArgumentException">Thrown if the password is empty or null or if the salt is not initialised</exception>
        /// <returns>A string representing that password and salt, this string can be passed around and persisted wtihout fear of revealing the password</returns>
        public static string ComputeSaltedHash(SecureString password, PasswordSalt salt) {

            #region entry checking

            // TODO b.Assert(password != null, "The password SecureString can not be null when given to SaltyPassword::ComputeSaltedHash");
            // TODO Bilge.Assert((salt.IntegerSalt != 0), "When calling SaltyPassword::ComputeSaltedHash All of the salt bytes are zero.", "This is not valid salt, call SaltyPassword.Salt.FillWithSalt()");

            if ((password == null) || (password.Length == 0)) {
                throw new ArgumentException("The password supplied was not valid.");
            }
            if (salt.IntegerSalt == 0) {
                throw new ArgumentException("The salt supplied is not valid.");
            }

            #endregion entry checking

            // Create Byte array of password string
            var encoder = new ASCIIEncoding();
            var passwordAndSaltCombo = new byte[password.Length + 4];

            passwordAndSaltCombo[0] = salt.Byte1;
            passwordAndSaltCombo[1] = salt.Byte2;
            passwordAndSaltCombo[2] = salt.Byte3;
            passwordAndSaltCombo[3] = salt.Byte4;

            // Now we take the bytes from the secure string and place them into a byte array so that we can
            // perform the hash on them.  This probably moves them out of secure encrypted memory negating the
            // whole secure string thing.
            var ptr = Marshal.SecureStringToBSTR(password);
            try {
                for (int i = 0; i < password.Length; i++) {
                    // this so must break unicode.
                    passwordAndSaltCombo[i + 4] = Marshal.ReadByte(ptr, i * sizeof(char));
                }
            } finally {
                // Free and Zero the temp we have been using.
                Marshal.ZeroFreeBSTR(ptr);
            }

            //return the calculated hash.
            string resultingHash;
            if (HashesAreB64Encoded) {
                resultingHash = Convert.ToBase64String(SHA1.Create().ComputeHash(passwordAndSaltCombo));
            } else {
                resultingHash = encoder.GetString(SHA1.Create().ComputeHash(passwordAndSaltCombo));
            }
            return resultingHash;
        }

        public static SaltyPassword CreateFromCleartext(string thisOne) {
            SaltyPassword result = new SaltyPassword();

            foreach (char c in thisOne) {
                result.Password.AppendChar(c);
            }
            result.Salt.FillWithSalt();
            return result;
        }

        /// <summary>
        /// Creates a new SaltyPassword with a random pasword
        /// </summary>
        /// <returns></returns>
        public static SaltyPassword CreateNew() {
            var result = new SaltyPassword();
            result.Password = SaltyPassword.CreateRandomPassword(15);
            result.Salt.FillWithSalt();
            return result;
        }

        /// <summary>
        /// Creates a random password up to a specified maximum length.  The password is a series of randomly chosen characters from a set
        /// of predefined OK characters for use.  This method returns a different password each time it is called.
        /// </summary>
        /// <remarks>Password length must be at least 5 characters long</remarks>
        /// <param name="length">The length in characters of the password that is to be created</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown if the length passed is less than the minimum allowed by MinPassLength</exception>
        /// <returns>The newly created password stored within a secure string</returns>
        public static SecureString CreateRandomPassword(int length) {

            #region entry checking

            if (length < MINIMUM_PASSWORD_LENGTH) {
                throw new ArgumentOutOfRangeException("length", "The length specfied must be at least the same as the MinPassLength of " + MINIMUM_PASSWORD_LENGTH.ToString());
            }

            #endregion entry checking

            SecureString result = new SecureString();
            byte[] tempRandomNumbers = new byte[length];

            // Use a secure random number generator to generate even more random numbers than normal.
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(tempRandomNumbers);

            // Run through the array of cryptographically strong numbers (!!) and use each one of them as a modded offset into
            // the list of OK characters for the password (which probably negates the cryptographic strongness) then using these
            // random chars append them into a secure string

            int offset = 0;  // Get use of unnasigned local if i dont do this but im sure this is a false positive
            for (int i = 0; i < length; offset = ((int)tempRandomNumbers[i++] % VALID_PASSWORD_CHARACTERS.Length)) {
                result.AppendChar(VALID_PASSWORD_CHARACTERS[offset]);
            }

            // We now have a secure string that contains random characters from the list of OK characters.  Dont loose it!
            return result;
        } // End SaltyPassword::CreateRandomPassword

        public string DangerouslyCopySecureStringTextToANetString() {
            return Marshal.PtrToStringAuto(Marshal.SecureStringToGlobalAllocUnicode(this.Password));
        }
    }
}