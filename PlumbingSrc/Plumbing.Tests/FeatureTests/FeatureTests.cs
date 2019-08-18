using Plisky.Diagnostics;
using Plisky.Plumbing;
using Plisky.Test;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Plisky.Test {
    public class FeatureTests {
        protected Bilge b = new Bilge();
        const string FEATURENAME = "MyFeatureName";


        

        [Fact(DisplayName = nameof(Feature_CreateConstructorOk))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_CreateConstructorOk() {
            b.Info.Flow();

            var f = new Feature(FEATURENAME, true);

            Assert.True(f.Active);
            Assert.Equal(FEATURENAME, f.Name);
        }

        [Fact(DisplayName = nameof(Feature_CreateConstructorOk2))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_CreateConstructorOk2() {
            b.Info.Flow();

            var f = new Feature(FEATURENAME, false);

            Assert.False(f.Active);
            Assert.Equal(FEATURENAME, f.Name);
        }

        [Fact(DisplayName = nameof(Provider_ReturnsOnlyOnesItProvides))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Provider_ReturnsOnlyOnesItProvides() {
            b.Info.Flow();
            try {
                string featureName1 = FEATURENAME + "1";
                string featureName2 = FEATURENAME + "2";

                var mfp = new MockFeatureProvider();
                mfp.MockAddBoolFeature(featureName1, true);
                Feature.AddProvider(mfp);

                var f1 = Feature.GetFeatureByName(featureName1);
                var f2 = Feature.GetFeatureByName(featureName2);

                Assert.NotNull(f1);
                Assert.Null(f2);
            } finally {
                Feature.Reset();
            }
        }



        [Fact(DisplayName = nameof(MissingFeature_ReturnsNull))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void MissingFeature_ReturnsNull() {
            b.Info.Flow();

            var ft = Feature.GetFeatureByName(FEATURENAME);
            Assert.Null(ft);

        }


        [Fact(DisplayName = nameof(Provider_ReturnsNamedFeature))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Provider_ReturnsNamedFeature() {
            b.Info.Flow();
            try {
                var mfp = new MockFeatureProvider();
                mfp.MockAddBoolFeature(FEATURENAME, true);
                Feature.AddProvider(mfp);

                var ft = Feature.GetFeatureByName(FEATURENAME);
                Assert.NotNull(ft);
                Assert.True(ft.Active);
            } finally {
                Feature.Reset();
            }
        }



        [Fact(DisplayName = nameof(Feature_WithLevelGtZeroIsActive))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_WithLevelGtZeroIsActive() {
            b.Info.Flow();
            const int LEVEL = 1;

            Feature f = new Feature(FEATURENAME, LEVEL);
            Assert.True(f.Active);
            Assert.Equal(LEVEL, f.Level);
        }


        [Fact(DisplayName = nameof(Feature_WithLevelZeroNotActive))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_WithLevelZeroNotActive() {
            b.Info.Flow();

            const int LEVEL = 0;

            Feature f = new Feature(FEATURENAME, LEVEL);
            Assert.False(f.Active);
            Assert.Equal(LEVEL, f.Level);
        }


        [Fact(DisplayName = nameof(Feature_CacheResult))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_CacheResult() {
            b.Info.Flow();

            const int CALLSTOMAKE = 4;

            try {
                var mfp = new MockFeatureProvider();
                mfp.MockAddBoolFeature(FEATURENAME, true);
                Feature.AddProvider(mfp);

                var ft = Feature.GetFeatureByName(FEATURENAME);

                for (int i = 0; i < CALLSTOMAKE; i++) {
                    // Not really part of the test, we just want to call active but might as well make it an assertion.
                    Assert.True(ft.Active);
                }

                var ct = mfp.HowManyCallsForThisFeature(FEATURENAME);

                Assert.Equal(1, ct);
            } finally {
                Feature.Reset();
            }
        }


        [Fact(DisplayName = nameof(Feature_CacheResult_UnlessRefresh))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_CacheResult_UnlessRefresh() {
            b.Info.Flow();
            const int CALLSTOMAKE = 4;

            try {
                var mfp = new MockFeatureProvider();
                mfp.MockAddBoolFeature(FEATURENAME, true);
                Feature.AddProvider(mfp);

                var ft = Feature.GetFeatureByName(FEATURENAME);

                for (int i = 0; i < CALLSTOMAKE; i++) {
                    // Not really part of the test, we just want to call active but might as well make it an assertion.                    
                    Assert.True(ft.IsActive());
                }

                var ct = mfp.HowManyCallsForThisFeature(FEATURENAME);
                // +1 because of the first GetFeatureByName, then the additonal CALLSTOMAKE in the loop.
                Assert.Equal(CALLSTOMAKE+1, ct);
                
                
            } finally {
                Feature.Reset();
            }
        }


        [Fact(DisplayName = nameof(WhenNotSet_FeatureLevel_IsZero))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void WhenNotSet_FeatureLevel_IsZero() {
            b.Info.Flow();

            Feature f = new Feature(FEATURENAME, true);

            Assert.Equal(0, f.Level);
        }


        [Fact(DisplayName = nameof(Feature_StartDateWorks))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_StartDateWorks() {
            b.Info.Flow();

            Feature sut = new Feature(FEATURENAME, true);
            var when = ConfigHub.Current.GetNow();
            var tomorrow = when.AddDays(1);

            sut.SetDateRange(tomorrow,null);

            Assert.False(sut.Active);
        }


        [Fact(DisplayName = nameof(Feature_EndDateWorks))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_EndDateWorks() {
            b.Info.Flow();
            Feature sut = new Feature(FEATURENAME, true);
            var when = ConfigHub.Current.GetNow();
            var yesterday = when.AddDays(-1);

            sut.SetDateRange(null, yesterday);

            Assert.False(sut.Active);

        }



        [Theory(DisplayName = nameof(Feature_DateRangeWorks))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        [InlineData("2019,01,01", "2019,31,12","2019,06,06",true)]
        [InlineData("2019,01,01", "2019,31,12", "2020,06,06", false)]
        public void Feature_DateRangeWorks(string start, string end, string now, bool inRange) {
            b.Info.Flow();

            ConfigHub c = new ConfigHub();
            Feature.UseHub(c);

            DateTime startDate = DateTime.ParseExact(start, "yyyy,dd,MM", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(end, "yyyy,dd,MM", CultureInfo.InvariantCulture);
            DateTime currentDate = DateTime.ParseExact(now, "yyyy,dd,MM", CultureInfo.InvariantCulture);

           c.RegisterProvider<DateTime>(ConfigHub.DateTimeSettingName, () => {
                return currentDate;
            });

            Feature sut = new Feature(FEATURENAME, true);
            sut.SetDateRange(startDate, endDate);

            Assert.Equal(inRange, sut.IsActive());
        }

        [Theory]
        [InlineData("02/10/2017", "04/10/2017", "03/10/2017", true)]    // in range
        [InlineData("02/10/2017", "04/10/2017", "05/10/2017", false)]  // after max date by 1
        [InlineData("01/12/2012", "01/01/2013", "31/12/2012", true)]   // in date range across year
        [InlineData("01/12/2012", "01/01/2013", "31/12/2017", false)]  // After max date
        [InlineData("01/12/2012", "01/01/2013", "31/12/2010", false)]  // Before min date
        [InlineData("01/12/2012", "01/01/2013", "01/12/2012", true)]  // edge case in start
        [InlineData("01/12/2012", "01/01/2013", "01/01/2013", true)]  // edge case in end
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_DateRange_Expected(string featureActiveDate, string featureEndDate, string todaysDate, bool shouldPass) {
            b.Info.Flow($"{featureActiveDate} >> {featureEndDate} >> {todaysDate} >> {shouldPass}");

            ConfigHub c = new ConfigHub();
            Feature.UseHub(c);

            string fname = nameof(Feature_DateRange_Expected);
            CultureInfo ci = new CultureInfo("en-GB");

            Feature f = new Feature(fname, true);
            f.SetDateRange(DateTime.Parse(featureActiveDate, ci), DateTime.Parse(featureEndDate, ci));

            c.RegisterProvider<DateTime>(ConfigHub.DateTimeSettingName, () => {
                return DateTime.Parse(todaysDate, ci);
            });


            Assert.Equal(shouldPass, f.IsActive());

        }

        [Theory]
        [InlineData("02/10/2017", "04/10/2017", "03/10/2018", true)]    // after is valid
        [InlineData("02/10/2017", "04/10/2017", "05/10/2017", false)]  // out not valid
        [InlineData("01/12/2012", "01/01/2013", "31/12/2010", true)]   // before is valid
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void GetFeature_AnualAgnostic_IsValid(string featureActiveDate, string featureEndDate, string todaysDate, bool shouldPass) {
            b.Info.Flow();

            ConfigHub c = new ConfigHub();
            Feature.UseHub(c);

            string fname = nameof(GetFeature_AnualAgnostic_IsValid);
            CultureInfo ci = new CultureInfo("en-GB");

            Feature f = new Feature(fname, true);
            f.SetDateRange(DateTime.Parse(featureActiveDate, ci), DateTime.Parse(featureEndDate, ci), true);

            c.RegisterProvider<DateTime>(ConfigHub.DateTimeSettingName, () => {
                return DateTime.Parse(todaysDate, ci);
            });

            Assert.Equal(shouldPass, f.IsActive());

        }


    }
}
