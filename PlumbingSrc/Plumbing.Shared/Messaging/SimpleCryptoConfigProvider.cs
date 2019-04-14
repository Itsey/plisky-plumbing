namespace Plisky.Plumbing {

    public class SimpleCryptoConfigProvider : IDecryptStuff {
        private byte[] keyStore;

        public SimpleCryptoConfigProvider(int keyVal) {
            keyStore = CryptoHelper.GenerateKeyFromIdentity(keyVal);
        }

        public string DecryptValue(string input) {
            return CryptoHelper.DecryptValue(keyStore, input);
        }

        public string Encrypt(string input) {
            return CryptoHelper.EncryptValue(keyStore, input);
        }
    }
}