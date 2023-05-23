using System.Collections.Generic;
using Plisky.Plumbing;

namespace Plisky.Test {

    public class MockSimpleCrypto : IDecryptStuff {
        private string retval = "yyy";
        private Dictionary<string, string> supportedDecrypts = new Dictionary<string, string>();

        /// <summary>
        /// Simply returns a preset string when asked to decrpyt a key string.
        /// </summary>
        public MockSimpleCrypto() {
        }

        public string AlwaysThisValue {
            get {
                return retval;
            }
        }

        public void AddDecryption(string crypto, string plaintext) {
            supportedDecrypts.Add(crypto, plaintext);
        }

        public string DecryptValue(string input) {
            if (supportedDecrypts.ContainsKey(input)) {
                return supportedDecrypts[input];
            }
            return retval;
        }
    }
}