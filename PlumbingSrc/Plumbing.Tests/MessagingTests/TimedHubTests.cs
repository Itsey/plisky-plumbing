

namespace Plisky.Test {

    
    public class TimedHubTests {


#if false

        [Fact][Trait("xunit","regression")]
        public void TimedHubCanBeCreated() {
            object test = new object();

            MockTimedHub sut = new MockTimedHub();
            sut.ReturnDate = DateTime.Now + new TimeSpan(1, 10, 0);

            TimePeriodTrigger tpt = new TimePeriodTrigger();
            tpt.EveryInterval = new TimeSpan(1,0,0);
            tpt.State = test;

            int HitCount =0;
            var res = sut.LookFor(tpt, p => {
                Assert.AreSame(test, p.State, "the wrong state object was passed");
                HitCount++;
            });

            Thread.Sleep(100);
            Assert.Equal(1,HitCount, "the hit count was not the right amount");
        }

        [Fact][Trait("xunit","regression")]
        public void TimedHub_PassesState_Correctly() {
            object test = new object();

            MockTimedHub sut = new MockTimedHub();
            sut.ReturnDate = DateTime.Now + new TimeSpan(1, 10, 0);
            TimePeriodTrigger tpt = new TimePeriodTrigger();
            tpt.EveryInterval = new TimeSpan(1, 0, 0);
            tpt.State = test;

            bool wasHit = false;
            var val = sut.LookFor <TimePeriodTrigger>(tpt, p => {
                wasHit = true;
                Assert.AreSame(test, p.State, "the wrong state object was passed");
            });

            Thread.Sleep(100);
            Assert.True(wasHit, "The method wasnt executed therefore the test is invalid");
        }

        [Fact][Trait("xunit","regression")]
        public void TimeHub_SpecificTime_NotFiredWhenTimeNotHit() {
            MockTimedHub sut = new MockTimedHub();
            sut.ReturnDate = new DateTime(2013,1,1,20,23,0);
            TimePeriodTrigger tpt = new TimePeriodTrigger();
            tpt.TimeToOccur = new DateTime(2013, 1, 1, 20, 25,0);

            bool wasHit = false;
            var res = sut.LookFor(tpt, p => {
                wasHit = true;
            });

            Thread.Sleep(100);
            Assert.False(wasHit, "The method should not have been executed.");
        }

        [Fact][Trait("xunit","regression")]
        public void TimeHub_SpecificTime_FiredWhenTimePassed() {
            MockTimedHub sut = new MockTimedHub();
            sut.ReturnDate = new DateTime(2013, 1, 1, 20, 27, 0);
            TimePeriodTrigger tpt = new TimePeriodTrigger();
            tpt.TimeToOccur = new DateTime(2013, 1, 1, 20, 25, 0);

            int hitCount = 0;
            var res = sut.LookFor(tpt, p => {
                hitCount++;
            });

            Thread.Sleep(100);
            Assert.Equal(1,hitCount, "The method should have been hit once only.");
        }
#endif
    }
}