
namespace Plisky.Test {

    /// <summary>
    /// Test message type used in testing plisky messaging.
    /// </summary>
    public class TestMessage {
        public string Data { get; set; }

        public TestMessage() {
            Data = "Default";
        }

        public TestMessage(string testMsg) {
            Data = testMsg;
        }
    }
}
