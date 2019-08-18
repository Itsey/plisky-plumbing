using Plisky.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plisky.Plumbing {
    public class Feature {
        protected Bilge b = new Bilge();
        protected static ConfigHub configResolver = ConfigHub.Current;

        public static void Reset() {
            resolver = null;
        }

        // Feature Management
        private static IResolveFeatures resolver;

        public static void AddProvider(IResolveFeatures prov) {
            resolver = prov;
        }

        public static Feature GetFeatureByName(string featureName) {
            if (resolver != null) {
                return resolver.GetFeature(featureName);
            }
            return null;
        }


        // Feature instance.

        private bool? featureBool;
        private int? featureLevel;

        /// <summary>
        /// IF not null then this indicates a start date, after which the feature will be active.
        /// </summary>
        public DateTime? StartActive { get; set; }

        /// <summary>
        /// If not null then this indicates an end date, after which the feature will not be active.  
        /// </summary>
        public DateTime? EndActive { get; set; }

        /// <summary>
        /// When a date range is Annual agnostic then it kicks in each year, this is only valid when there is both a start and an end date.  Therefore 
        /// setting start to 1/1 and end to 10/1 will mean every year the feature is active between first and 10th jan.  It does not matter which year is selected
        /// for setting the dates.
        /// </summary>
        public bool AnnualAgnostic { get; set; }

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

            if ((!Active)||((StartActive==null)&&(EndActive == null))) {
                return;
            }

            var whenIsNow = configResolver.GetNow().Date;

            int endDateYearOffset = 0;
            // Now check the date element.
            if (StartActive.HasValue) {
                b.Verbose.Log("Starting Start range check");
                DateTime start = StartActive.Value;
                if (AnnualAgnostic) {
                    if (EndActive.HasValue) {
                        // Annoying fringe case - Annual Active where the end date is not in the same year as the start date.
                        endDateYearOffset = EndActive.Value.Year - StartActive.Value.Year;
                    }
                    start = StartActive.Value.AddYears(whenIsNow.Year - StartActive.Value.Year);
                }
                if (whenIsNow < start) {
                    b.Verbose.Log("Feature not enabled - date restriction");
                    Active = false;
                }
            }

            if (EndActive.HasValue) {
                b.Verbose.Log("Starting End range check");
                DateTime end = EndActive.Value;

                if (AnnualAgnostic) {
                    end = EndActive.Value.AddYears(whenIsNow.Year - EndActive.Value.Year).AddYears(endDateYearOffset);
                    b.Verbose.Log($"End Active now {EndActive.Value.ToString()}");
                }
                if (whenIsNow > end) {
                    b.Verbose.Log("Feature not enabled - date restriction");
                    Active = false;
                }
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
            if (resolver != null) {
                var ft = resolver.GetFeature(this.Name);
                if (ft != null) {
                    this.featureBool = ft.Active;
                    this.featureLevel = ft.Level;
                    this.SetDateRange(ft.StartActive, ft.EndActive, ft.AnnualAgnostic);
                }
            }
            CalculateFeatureActive();
            return Active;
        }

        public void SetDateRange(DateTime? startDate, DateTime? endDate, bool annAgSetting = false) {
            if (startDate.HasValue) { StartActive = startDate.Value.Date; } else { startDate = null; }
            if (endDate.HasValue) { EndActive = endDate.Value.Date; } else { endDate = null; }
            AnnualAgnostic = annAgSetting;

            CalculateFeatureActive(); 
        }

        public static void UseHub(ConfigHub newHubToUse) {
            if (newHubToUse==null) {
                throw new ArgumentNullException(nameof(newHubToUse));

            }
            configResolver = newHubToUse;
        }
    }
}
