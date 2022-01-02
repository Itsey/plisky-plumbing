using Plisky.Plumbing;
using System;

namespace Plisky.Test {
    internal class MockFeature : Feature {
        public MockFeature(string featureName, bool featureValue) : base(featureName, featureValue) {
        }

        public DateTime? GetStartDate() {
            return this.StartActive;
        }

        public DateTime? GetEndDate() {
            return this.EndActive;
        }

    }
}