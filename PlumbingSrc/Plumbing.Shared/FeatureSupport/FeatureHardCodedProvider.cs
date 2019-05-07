using System;
using System.Collections.Generic;
using System.Text;

namespace Plisky.Plumbing {
    public class FeatureHardCodedProvider : IResolveFeatures {
        private BaseFeatureOptions opts;
        private ConfigHub injectedHub = ConfigHub.Current;
        private Dictionary<string, Feature> allFeatures = new Dictionary<string, Feature>();

        public void AddFeatire(Feature f) {
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
