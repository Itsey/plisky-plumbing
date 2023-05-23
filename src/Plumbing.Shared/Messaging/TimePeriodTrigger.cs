using System;

namespace Plisky.Plumbing {

    public class TimePeriodTrigger {

        public TimePeriodTrigger() {
            LastTimeExecuted = DateTime.MinValue;
            EveryInterval = TimeSpan.MinValue;
        }

        public TimeSpan EveryInterval { get; set; }
        public DateTime LastTimeExecuted { get; set; }
        public object State { get; set; }
        public DateTime TimeToOccur { get; set; }
    }
}