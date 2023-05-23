using System.IO;
using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;
using Plisky.Plumbing;
using Xunit;

namespace Plisky.Test {

    public class FeatureProviderTests {
        protected Bilge b = new Bilge();
        protected UnitTestHelper uth = new UnitTestHelper();

        private const string FEATURENAME = "MyFeatureName";

        public FeatureProviderTests() {
            b.AddHandler(new TCPHandler("127.0.0.1", 9060));
        }

        [Fact(DisplayName = nameof(HardcodedFP_AddFeatureCanGet))]
        public void HardcodedFP_AddFeatureCanGet() {
            b.Info.Flow();

            var f = new Feature(FEATURENAME, true);
            FeatureHardCodedProvider sut = new FeatureHardCodedProvider();
            sut.AddFeature(f);
            var result = sut.GetFeature(FEATURENAME);

            Assert.Same(f, result);
        }

        [Fact(DisplayName = nameof(HardcodedFP_NoFeatureIsNull))]
        public void HardcodedFP_NoFeatureIsNull() {
            b.Info.Flow();

            var sut = new FeatureHardCodedProvider();
            var result = sut.GetFeature(FEATURENAME);

            Assert.Null(result);
        }

        [Fact(DisplayName = nameof(JsonProvider_ReadsFileOk))]
        public void JsonProvider_ReadsFileOk() {
            b.Info.Flow();
            var fn = uth.NewTemporaryFileName();
            try {
                Feature f = new Feature(FEATURENAME, true);
                string serialised = FeatureSerializer.GetFeatureAsString(f);
                File.WriteAllText(fn, serialised);
            } finally {
                uth.ClearUpTestFiles();
            }
        }
    }
}