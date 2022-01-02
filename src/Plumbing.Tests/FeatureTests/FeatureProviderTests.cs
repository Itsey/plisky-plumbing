using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;
using Plisky.Plumbing;
using Plisky.Test;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Plisky.Test {
    public class FeatureProviderTests {
        protected Bilge b = new Bilge();
        protected UnitTestHelper uth = new UnitTestHelper();

        const string FEATURENAME = "MyFeatureName";

        public FeatureProviderTests() {
            b.AddHandler(new TCPHandler("127.0.0.1", 9060));
        }

        [Fact(DisplayName = nameof(HardcodedFP_AddFeatureCanGet))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void HardcodedFP_AddFeatureCanGet() {
            b.Info.Flow();

            var f = new Feature(FEATURENAME, true);
            FeatureHardCodedProvider sut = new FeatureHardCodedProvider();
            sut.AddFeature(f);
            var result = sut.GetFeature(FEATURENAME);

            
            Assert.Same(f,result);
        }

        [Fact(DisplayName = nameof(HardcodedFP_NoFeatureIsNull))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void HardcodedFP_NoFeatureIsNull() {
            b.Info.Flow();

            var sut = new FeatureHardCodedProvider();
            var result = sut.GetFeature(FEATURENAME);


            Assert.Null( result);
        }


        [Fact(DisplayName = nameof(JsonProvider_ReadsFileOk))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
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
