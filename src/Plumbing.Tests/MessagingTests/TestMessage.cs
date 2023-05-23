namespace Plisky.Test {

    /// <summary>
    /// Test message type used in testing plisky messaging.
    /// </summary>
    public class TestMessage {

        public TestMessage() {
            Data = "Default";
        }

        public TestMessage(string testMsg) {
            Data = testMsg;
        }

        public string Data { get; set; }
    }
}