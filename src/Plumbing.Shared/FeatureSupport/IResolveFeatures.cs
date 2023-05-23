namespace Plisky.Plumbing {

    public interface IResolveFeatures {

        Feature GetFeature(string byName);

        void Initialise(BaseFeatureOptions bfo);

        void InjectHub(ConfigHub h);
    }
}