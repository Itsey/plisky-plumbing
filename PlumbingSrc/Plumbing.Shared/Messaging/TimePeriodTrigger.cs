using System;

namespace Plisky.Plumbing {

    public class TimePeriodTrigger {
        public DateTime TimeToOccur { get; set; }
        public TimeSpan EveryInterval { get; set; }
        public object State { get; set; }

        public DateTime LastTimeExecuted { get; set; }

        public TimePeriodTrigger() {
            LastTimeExecuted = DateTime.MinValue;
            EveryInterval = TimeSpan.MinValue;
        }
    }
}