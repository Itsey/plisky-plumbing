#if false
namespace Plisky.Test {
    using Plisky.Plumbing;

    public class TestMemLeak {
        private Hub hu;

        public int PassCount { get; set; }

        public TestMemLeak(Hub h) {
            hu = h;
            hu.LookFor<TestMessage>(param => {
                PassCount++;
            });
        }
    }
}
#endif