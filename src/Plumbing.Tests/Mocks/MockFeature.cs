using System;
using Plisky.Plumbing;

namespace Plisky.Test {

    internal class MockFeature : Feature {

        public MockFeature(string featureName, bool featureValue) : base(featureName, featureValue) {
        }

        public DateTime? GetEndDate() {
            return this.EndActive;
        }

        public DateTime? GetStartDate() {
            return this.StartActive;
        }
    }
}