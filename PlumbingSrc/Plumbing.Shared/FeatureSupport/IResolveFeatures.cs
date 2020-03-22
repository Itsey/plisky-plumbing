using System;
using System.Collections.Generic;
using System.Text;

namespace Plisky.Plumbing {
    public interface IResolveFeatures {
        void InjectHub(ConfigHub h);

        Feature GetFeature(string byName);
        void Initialise(BaseFeatureOptions bfo);
    }
}
