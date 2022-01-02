namespace Plisky.Plumbing {


    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class TimedHub : Hub {
        private object lockCollection = new object();
        private Timer tmr;
        private List<Tuple<TimePeriodTrigger, HubMessageBase>> timeEventsWaiting = new List<Tuple<TimePeriodTrigger, HubMessageBase>>();
        protected int executing = 0;
        protected int CallBackTime = 60;

        public virtual Action<TimePeriodTrigger> LookFor(TimePeriodTrigger tpt, Action<TimePeriodTrigger> openMessage) {
            b.Info.Log("Lookfor Timer Event Started");
            InitialiseTimerIfNeccesary();

            HubMessageBase msg;
            if (UseStrongReferences) {
                msg = new StrongMessageReader<TimePeriodTrigger>(openMessage);
            } else {
                msg = new WeakMessageReader<TimePeriodTrigger>(openMessage);
            }

            timeEventsWaiting.Add(new Tuple<TimePeriodTrigger, HubMessageBase>(tpt, msg));
            return openMessage;
        }

        private void TimerCallback(object state) {
            Interlocked.Increment(ref executing);
            if (executing > 1) {
                return;
            }
            tmr.Change(500000, 500000);

            StopTimerIfNecessary();
            lock (lockCollection) {
                foreach (var v in timeEventsWaiting) {
                    DateTime current = GetDateTime();
                    b.Info.Log("Checking at " + current.ToString());
                    if (v.Item1.EveryInterval != TimeSpan.MinValue) {
                        // Interval based trigger.

                        if ((current - v.Item1.LastTimeExecuted) > v.Item1.EveryInterval) {
                            b.Info.Log("FIRING interval based timer");
                            v.Item2.OpenNote(v.Item1);
                            v.Item1.LastTimeExecuted = current;
                        }
                    } else {
                        if ((v.Item1.TimeToOccur.TimeOfDay < current.TimeOfDay) && (v.Item1.TimeToOccur.TimeOfDay > v.Item1.LastTimeExecuted.TimeOfDay)) {
                            b.Info.Log("FIRING scheduled timer");
                            v.Item2.OpenNote(v.Item1);
                            v.Item1.LastTimeExecuted = current;
                        }
                    }
                }
            }
        }

        protected virtual DateTime GetDateTime() {
            return DateTime.Now;
        }

        private void StopTimerIfNecessary() {
            lock (lockCollection) {
                if (timeEventsWaiting.Count == 0) {
                    tmr = null;
                }
            }
        }

        private void InitialiseTimerIfNeccesary() {
            lock (lockCollection) {
                if (tmr == null) {
                    tmr = new Timer(TimerCallback, null, CallBackTime, CallBackTime);
                }
            }
        }
    }
}