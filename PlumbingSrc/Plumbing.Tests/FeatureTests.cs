using Plisky.Diagnostics;
using Plisky.Diagnostics.Listeners;
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
        protected Bilge b = new Bilge(tl:System.Diagnostics.TraceLevel.Verbose);
        const string FEATURENAME = "MyFeatureName";

        public FeatureTests() {
            b.AddHandler(new TCPHandler("127.0.0.1", 9060));
        }

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
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_WithLevelZeroNotActive() {
            b.Info.Flow();

            const int LEVEL = 0;

            Feature f = new Feature(FEATURENAME, LEVEL);
            Assert.False(f.Active);
            Assert.Equal(LEVEL, f.Level);
        }


        [Fact(DisplayName = nameof(Feature_CacheResult))]
        [Trait(Traits.Age, Traits.Fresh)]
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


        [Fact(DisplayName = nameof(Feature_SetEndWorks))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_SetEndWorks() {
            b.Info.Flow();
            DateTime when = new DateTime(2010, 1, 1);
            MockFeature sut = new MockFeature(FEATURENAME, true);
            sut.SetDateRange(when, null);

            Assert.Equal(when, sut.GetStartDate());
        }

        [Fact(DisplayName = nameof(Feature_SetStartWorks))]
        [Trait(Traits.Age, Traits.Regression)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_SetStartWorks() {
            b.Info.Flow();
            DateTime when = new DateTime(2010, 1, 1);
            MockFeature sut = new MockFeature(FEATURENAME, true);
            sut.SetDateRange(null, when);

            Assert.Equal(when, sut.GetEndDate());
        }


        [Fact(DisplayName = nameof(Feature_StartDateWorks))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_StartDateWorks() {
            b.Info.Flow();
            Feature.InjectBilge(b);
            ConfigHub ch = new ConfigHub();
            Feature.InjectHub(ch);

            Feature sut = new Feature(FEATURENAME, true);
            
            var when = ch.GetNow();
            var tomorrow = when.AddDays(1);
            
            sut.SetDateRange(tomorrow,null);

            Assert.False(sut.Active);
            Assert.False(sut.IsActive());
        }


        [Fact(DisplayName = nameof(Feature_RespectsDateConfig))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_RespectsDateConfig() {
            b.Info.Flow();

            Feature sut = new Feature(FEATURENAME, true);


        }


        [Fact(DisplayName = nameof(Feature_EndDateWorks))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_EndDateWorks() {
            b.Info.Flow();
            ConfigHub testcfg = new ConfigHub();
            Feature.InjectHub(testcfg);
            Feature sut = new Feature(FEATURENAME, true);
            var when = testcfg.GetNow();
            var yesterday = when.AddDays(-1);

            sut.SetDateRange(null, yesterday);

            Assert.False(sut.Active);

        }



        [Theory(DisplayName = nameof(Feature_DateRangeWorks))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        [InlineData("2019,01,01", "2019,31,12","2019,06,06",false,true)]
        [InlineData("2019,01,01", "2019,31,12", "2020,06,06",false, false)]
        [InlineData("2019,01,01","2019,15,02","2019,14,02",false, true)]
        public void Feature_DateRangeWorks(string start, string end, string now, bool yearAgnostic, bool inRange) {
            b.Info.Flow();

            DateTime startDate = DateTime.ParseExact(start, "yyyy,dd,MM", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(end, "yyyy,dd,MM", CultureInfo.InvariantCulture);
            DateTime currentDate = DateTime.ParseExact(now, "yyyy,dd,MM", CultureInfo.InvariantCulture);
            ConfigHub ch = new ConfigHub();
            ch.RegisterProvider<DateTime>(ConfigHub.DateTimeSettingName, () => {
                return currentDate;
            });

            Feature.InjectHub(ch);
            Feature sut = new Feature(FEATURENAME, true);
            sut.SetDateRange(startDate, endDate, yearAgnostic);

            Assert.Equal(inRange, sut.Active);
            Assert.Equal(sut.Active, sut.IsActive());
        }

        
        [Theory(DisplayName = nameof(Feature_YearAgnosticDateRangeWorks))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        [InlineData("2000,01,01", "2000,31,12", "2019,06,06", true, true)]
        [InlineData("2000,01,01", "2000,31,12", "2019,31,12", true, true)]
        [InlineData("2000,01,01", "2000,31,12", "2019,01,01", true, true)]
        [InlineData("2019,01,01", "2019,02,02", "2020,06,06", true, false)]
        [InlineData("2019,13,01", "2019,02,02", "2020,01,01", true, false)]
        public void Feature_YearAgnosticDateRangeWorks(string start, string end, string now, bool yearAgnostic, bool inRange) {
            b.Info.Flow();

            DateTime startDate = DateTime.ParseExact(start, "yyyy,dd,MM", CultureInfo.InvariantCulture);
            DateTime endDate = DateTime.ParseExact(end, "yyyy,dd,MM", CultureInfo.InvariantCulture);
            DateTime currentDate = DateTime.ParseExact(now, "yyyy,dd,MM", CultureInfo.InvariantCulture);
            ConfigHub ch = new ConfigHub();
            ch.RegisterProvider<DateTime>(ConfigHub.DateTimeSettingName, () => {
                return currentDate;
            });

            Feature.InjectHub(ch);
            Feature sut = new Feature(FEATURENAME, true);
            sut.SetDateRange(startDate, endDate, yearAgnostic);

            Assert.Equal(inRange, sut.Active);
            Assert.Equal(sut.Active, sut.IsActive());
        }

        [Fact(DisplayName = nameof(Feature_DateRange_AgnosticBlowsIfStartNotSet))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_DateRange_AgnosticBlowsIfStartNotSet() {
            b.Info.Flow();

            Assert.Throws<InvalidOperationException>(() => {
                Feature sut = new Feature(FEATURENAME, true);
                sut.SetDateRange(DateTime.Now, null, true);
            });
        }

        [Fact(DisplayName = nameof(Feature_DateRange_AgnosticBlowsIfEndNotSet))]
        [Trait(Traits.Age, Traits.Fresh)]
        [Trait(Traits.Style, Traits.Unit)]
        public void Feature_DateRange_AgnosticBlowsIfEndNotSet() {
            b.Info.Flow();

            Assert.Throws<InvalidOperationException>(() => {
                Feature sut = new Feature(FEATURENAME, true);
                sut.SetDateRange(null,DateTime.Now,  true);
            });
        }

    }
}
