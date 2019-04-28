using System;
using System.Collections.Generic;
using System.Text;

namespace Plisky.Plumbing {
    public class Feature {
        public static void Reset() {
            resolver = null;
        }

        // Feature Management
        private static IResolveFeatures resolver;

        public static void AddProvider(IResolveFeatures prov) {
            resolver = prov;
        }

        public static Feature GetFeatureByName(string featureName) {
            if (resolver!=null) {
                return resolver.GetFeature(featureName);
            }
            return null;
        }


        // Feature instance.

        private bool? featureBool;
        private int? featureLevel;

        /// <summary>
        /// Determines if this feature is currently active.  Features that are active are designed to be running in code.
        /// </summary>
        public bool Active { get; private set; } = false;

        /// <summary>
        /// The Name of the feature, this must be unique and is case sensitive.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Used to take all of the feature data and work out whether or not the boolean for "active" should be set.  Any feature value makes the
        /// feature active.  To get more detail other methods need to be called.
        /// </summary>
        private void CalculateFeatureActive() {

            if (featureBool.HasValue) {
                Active = featureBool.Value;
            } else if (featureLevel.HasValue) {
                Active = (featureLevel.Value > 0);
            } else {
                Active = false;
            }
            
        }


        /// <summary>
        /// When a feature has a level it will be set here.  Feature levels return integer numbers, if the level is zero then the feature is off.
        /// Any other value for the feature level means that the feature is on.
        /// </summary>
        public int Level {
            get {
                if (featureLevel.HasValue) {
                    return featureLevel.Value;
                }
                return 0;
            }
        }
        

        /// <summary>
        /// Creates a new instance of the Feature class, giving it a name and a boolean Active value.
        /// </summary>
        /// <param name="featureName">The Feature Name</param>
        /// <param name="featureValue">A value for whether the feature is Active.</param>
        public Feature(string featureName, bool featureValue) {
            Name = featureName;
            this.featureBool = featureValue;
            CalculateFeatureActive();
        }

        /// <summary>
        /// Creates a new instance of the Feature class, giving it a level.
        /// </summary>
        /// <param name="featureName">The Feature Name</param>
        /// <param name="featureValue">A value for the level, when set to zero the feature is not active.</param>
        public Feature(string fn, int level) {
            Name = fn;
            featureLevel = level;
            CalculateFeatureActive();
        }


       

        /// <summary>
        /// IsActive() will refresh the instance of the feature using the underlying feature provider - and then return whether the feature is active 
        /// or not.  This will cause a hit on the underlying provider of the feature implementation, which may itself call other services or cache
        /// the result.  Its down to the underlying provider what actually happens.  
        /// </summary>
        /// <returns>The value of Active for the refreshed feature</returns>
        public bool IsActive() {
            if (resolver!=null) {
                var ft= resolver.GetFeature(this.Name);
                if (ft!=null) {
                    this.featureBool = ft.Active;
                    this.featureLevel = ft.Level;
                }
            }
            return Active;
        }

        public void SetDateRange(DateTime? startDate, DateTime? endDate) {
            throw new NotImplementedException();
        }
    }
}
