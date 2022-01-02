﻿using Plisky.Plumbing;
using System;
using System.Collections.Generic;

namespace Plisky.Test {
    internal class MockFeatureProvider : IResolveFeatures {
        private Dictionary<string, bool> boolFeatures = new Dictionary<string, bool>();
        private Dictionary<string, int> callCount = new Dictionary<string, int>();

        public void MockAddBoolFeature(string name, bool val) {
            boolFeatures.Add(name, val);
        }

        public void InjectHub(ConfigHub h) {
            throw new System.NotImplementedException();
        }

        public Feature GetFeature(string byName) {
            if (!callCount.ContainsKey(byName)) {
                callCount.Add(byName, 0);
            }

            callCount[byName] += 1;

            if (boolFeatures.ContainsKey(byName)) {
                return new Feature(byName, boolFeatures[byName]);
            }

            return null;
        }

        public void Initialise(BaseFeatureOptions bfo) {
            throw new System.NotImplementedException();
        }

        public MockFeatureProvider() {
        }

        internal int HowManyCallsForThisFeature(string nameToCheck) {
            if (callCount.ContainsKey(nameToCheck)) {
                return callCount[nameToCheck];
            }
            return 0;
        }
    }
}