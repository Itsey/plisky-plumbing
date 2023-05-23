using System.Collections.Generic;

namespace Plisky.Plumbing {

    public class FeatureHardCodedProvider : IResolveFeatures {
        private Dictionary<string, Feature> allFeatures = new Dictionary<string, Feature>();
        private ConfigHub injectedHub = ConfigHub.Current;
        private BaseFeatureOptions opts;

        public void AddFeature(Feature f) {
            allFeatures.Add(f.Name, f);
        }

        public Feature GetFeature(string byName) {
            if (allFeatures.ContainsKey(byName)) {
                return allFeatures[byName];
            }
            return null;
        }

        public void Initialise(BaseFeatureOptions bfo) {
            opts = bfo;
        }

        public void InjectHub(ConfigHub h) {
            injectedHub = h;
        }
    }
}