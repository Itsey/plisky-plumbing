#if false
using Plisky.Plumbing;

namespace Plisky.Test.Mocks {

    public class MockSimpleCrypto : IDecryptStuff {
        private string retval = "yyy";

        public string DecryptValue(string input) {
            return retval;
        }

        public string AlwaysThisValue {
            get {
                return retval;
            }
        }
    }
}
#endif