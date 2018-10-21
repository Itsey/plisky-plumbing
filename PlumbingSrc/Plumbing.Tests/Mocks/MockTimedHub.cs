#if false
using Plisky.Plumbing;
using System;

namespace Plisky.Test.Mocks {

    public class MockTimedHub : TimedHub {
        public DateTime ReturnDate { get; set; }

        protected override DateTime GetDateTime() {
            if (ReturnDate == DateTime.MinValue) {
                ReturnDate = DateTime.Now;
            }
            return ReturnDate;
        }

        public MockTimedHub()
            : base() {
            CallBackTime = 1;
        }
    }
}
#endif