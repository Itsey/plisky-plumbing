
using Plisky.Plumbing;
using System.Collections.Generic;

namespace Plisky.Test {

    public class MockSimpleCrypto : IDecryptStuff {
        Dictionary<string, string> supportedDecrypts = new Dictionary<string, string>();

        private string retval = "yyy";

        public string DecryptValue(string input) {
            if (supportedDecrypts.ContainsKey(input)) {
                return supportedDecrypts[input];
            }
            return retval;
        }

        public string AlwaysThisValue {
            get {
                return retval;
            }
        }

        public void AddDecryption(string crypto, string plaintext) {
            supportedDecrypts.Add(crypto, plaintext);
        }

        /// <summary>
        /// Simply returns a preset string when asked to decrpyt a key string.
        /// </summary>
        public MockSimpleCrypto() {

        }
    }
}
