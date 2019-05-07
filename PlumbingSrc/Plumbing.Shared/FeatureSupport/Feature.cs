using Plisky.Diagnostics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Plisky.Plumbing {
    public class Feature {
        private static ConfigHub injectedHub = ConfigHub.Current;

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
                var ft = resolver.GetFeature(featureName);
                return ft;
            }
            return null;
        }


        // Feature instance.
        protected ConfigHub cfg;
        protected Bilge b = new Bilge();
        protected bool? featureBool;
        protected int? featureLevel;
        protected DateTime? featureStartDate;
        protected DateTime? featureEndDate;
        protected bool annualAgnostic; // Start / End kicks in every year

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

            Active &= IsFeatureCurrentlyActive();
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
            cfg = injectedHub;
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
            cfg = injectedHub;
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
                }

                
            }
           

            CalculateFeatureActive();
            return Active;
        }

        private bool IsFeatureCurrentlyActive() {
            var whenIsNow = cfg.GetNow();
            b.Verbose.Log($"Using currrent date time of {whenIsNow.ToString()}");

            int endDateYearOffset = 0;
            if (featureStartDate.HasValue) {

                b.Verbose.Log("Starting Start range check");
                DateTime featureStartsAt = featureStartDate.Value;

                if (this.annualAgnostic) {
                    b.Assert.True(this.featureEndDate.HasValue,"Invalid Situation, cant be agnostic with no end date");
                    
                        // Annoying fringe case - Annual Active where the end date is not in the same year as the start date.
                    endDateYearOffset = featureEndDate.Value.Year - featureStartDate.Value.Year;
                    
                    featureStartsAt = featureStartDate.Value.AddYears(whenIsNow.Year - featureStartDate.Value.Year);
                }
                if (whenIsNow < featureStartsAt) {
                    b.Verbose.Log("Feature not enabled - date restriction");
                    return false;
                }
            }
            if (featureEndDate.HasValue) {
                b.Verbose.Log("Starting End range check");
                DateTime featureEndsAt = featureEndDate.Value;

                if (annualAgnostic) {
                    featureEndsAt = featureEndDate.Value.AddYears(whenIsNow.Year - featureEndDate.Value.Year).AddYears(endDateYearOffset);
                    b.Verbose.Log($"End Active now {featureEndsAt}");
                }
                if (whenIsNow > featureEndsAt) {
                    b.Verbose.Log("Feature not enabled - date restriction");
                    return false;
                }
            }
            return true;

        }



        /// <summary>
        /// Establishes a date range during which this feature is active.  The start date will ensure that the feature is not active
        /// before this start date.  The end date will ensure that the feature stops being active at this date.  Year agnostic means
        /// that the date range will apply each year - so you can use 01/01/2019-10/01/2019 and it will work in 2020, 2010 etc
        /// </summary>
        /// <remarks>If no start or end date is set then only the other one applies.  It is invalid to use agnostic while one or 
        /// the other date is not set.</remarks>
        /// <param name="startDate">The date before which the feature is not active</param>
        /// <param name="endDate">The date after which the feature is not active</param>
        /// <param name="yearAgnostic">Whether this date range should apply each year</param>
        public void SetDateRange(DateTime? startDate, DateTime? endDate, bool yearAgnostic = false) {
            if (yearAgnostic) {
                if ((startDate==null)||(endDate==null)) {
                    throw new InvalidOperationException("To have a feature year agnostic both start and end date must be set");
                }
            }

            featureStartDate = startDate;
            featureEndDate = endDate;
            annualAgnostic = yearAgnostic;

            IsActive();
        }

        public static void InjectHub(ConfigHub ch) {
            injectedHub = ch;
        }
    }
}
