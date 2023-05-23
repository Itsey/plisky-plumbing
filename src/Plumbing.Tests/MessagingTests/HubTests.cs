namespace Plisky.Test {

    using System;
    using System.Threading;
    using Plisky.Plumbing;
    using Xunit;

    public class HubTests {
        private UnitTestHelper uth = new UnitTestHelper();
        private SampleTestData td = new SampleTestData();

        public HubTests() {
            Hub.Relinquish();
        }

        public void Hub_HasNameOnCreate() {
            Hub sut = new Hub();
            Assert.NotNull(sut.InstanceName);
        }

        public void Hub_CreatedNamesAreUnique() {
            Hub sut = new Hub();
            Hub sut2 = new Hub();
            Assert.NotEqual(sut.InstanceName, sut2.InstanceName);
        }

        public void InstanceName_Provided_IsUSed() {
            Hub sut = new Hub("systesthub");
            Assert.True(sut.InstanceName.Contains("systesthub"), "The identifier was not used in the intsance name");
            Assert.NotEqual("systesthub", sut.InstanceName);
        }

        public void Hub_StaticCurrent_NotNull() {
            Assert.NotNull(Hub.Current);
        }

        public void Hub_Launch_NothingListening() {
            Hub sut = new Hub();
            sut.Launch(SampleTestData.GENERIC_STRING1);
            sut.Launch<TestMessage>(new TestMessage());
        }

        public void Hub_DefaultConstructorUsesWeakReferences() {
            Hub sut = new Hub();
            Assert.False(sut.UseStrongReferences, "The strong references should not be the default");
        }

        public void Hub_SetReferencesConstructorWorks() {
            Hub sut = new Hub(true);
            Hub sut2 = new Hub(false);
            Assert.True(sut.UseStrongReferences, "The constructor should set the strong references to true.");
            Assert.False(sut2.UseStrongReferences, "The constructor should set the strong references to false");
        }

        public void SendMessage_CustomType_DoesReachRecipient() {
            Hub sut = new Hub();
            string testMsg = SampleTestData.GENERIC_STRING1;
            bool wasExecuted = false;

            sut.LookFor<TestMessage>((TestMessage msg) => {
                Assert.True(msg.Data == testMsg, "A notification message was received but with the wrong data");
                wasExecuted = true;
            });

            sut.Launch<TestMessage>(new TestMessage(testMsg));

            Assert.True(wasExecuted, "The hub message recipient did not get notified of the call that was made.");
        }

        public void SendMessage_SimpleString_DoesReachRecipient() {
            Hub sut = new Hub();
            string testMsg = SampleTestData.GENERIC_STRING1;
            bool notification = false;

            sut.LookFor<string>((string s) => {
                if (s == testMsg) {
                    notification = true;
                }
            });

            sut.Launch(SampleTestData.GENERIC_STRING1);

            Assert.True(notification, "The notification was not hit with a simple string message");
        }

        public void SendMessage_LookForDifferentString_DoesNotNotify() {
            Hub sut = new Hub();

            string testMsg = SampleTestData.GENERIC_STRING1;
            bool notification = false;

            sut.LookFor<string>((string s) => {
                if (s == testMsg) {
                    notification = true;
                }
            });

            sut.Launch(SampleTestData.GENERNIC_STRING2);

            Assert.False(notification, "The notification was not hit with a simple string message");
        }

        public void LookFor_NoMessagesSent_WasNotCalled() {
            Hub sut = new Hub();
            string testMsg = "aTestString";
            bool notification = false;

            sut.LookFor<string>((string s) => {
                if (s == testMsg) {
                    notification = true;
                }
            });

            Assert.False(notification, "The notification in look for should not be hit if no messages were sent to it.");
        }

        public void StringRecipeint_NotCalledIfWrongTypeSent() {
            Hub sut = new Hub();
            string testMsg = "aTestString";
            bool notification = false;

            sut.LookFor<string>((string s) => {
                if (s == testMsg) {
                    notification = true;
                }
            });

            sut.Launch<TestMessage>(new TestMessage("arfle"));

            Assert.False(notification, "The notification was hit when it should not have been.");
        }

        // DISABLED - TODO
        public void StringRecipeint_IsCalledRightStringSent() {
            // TODO : FIX
            Hub sut = new Hub();
            string testmsg = "arflebarflegloop";
            bool wasHit = false;

            sut.LookFor(testmsg, (string s) => {
                Assert.True(s == testmsg, "The wrong simple message was sent through");
                wasHit = true;
            });

            sut.Launch(testmsg);

            Assert.True(wasHit, "The action was not fired for the correct message");
        }

        public void StringRecipeint_NotCalledWrongStringSent() {
            Hub sut = new Hub();
            string testmsg = "arflebarflegloop";
            bool wasHit = false;

            sut.LookFor(testmsg, (string s) => {
                wasHit = true;
            });

            sut.Launch("notarflebarflegloop");

            Assert.False(wasHit, "The action was not fired for the correct message");
        }

        public void IntRecipient_IsCalledRightIntSent() {
            Hub sut = new Hub();
            int testmsg = 1234;
            bool wasHit = false;

            sut.LookFor(testmsg, (int s) => {
                Assert.True(s == testmsg, "The wrong simple message was sent through");
                wasHit = true;
            });

            sut.Launch(testmsg);

            Assert.True(wasHit, "The action was not fired for the correct message");
        }

        public void IntRecipient_NotCalledWrongIntSent() {
            Hub sut = new Hub();
            int testmsg = 1234;
            bool wasHit = false;

            sut.LookFor(testmsg, (int s) => {
                wasHit = true;
            });

            sut.Launch(1235);
            sut.Launch(0);
            sut.Launch(1);
            sut.Launch(1233);

            Assert.False(wasHit, "The action was not fired for the correct message");
        }

        public void IntRecipeint_NotCalledWrongTypeSent() {
            Hub sut = new Hub();
            int testMsg = 1324;
            bool notification = false;

            sut.LookFor((int s) => {
                if (s == testMsg) {
                    notification = true;
                }
            });

            sut.Launch<TestMessage>(new TestMessage("arfle"));
            sut.Launch("1234");
            sut.Launch<int>(1234);

            Assert.False(notification, "The notification was hit when it should not have been.");
        }

#if false
        UNKNOWN AGED CODE - Likely related to removing memory leak
        private WeakReference GetInstanceThatHasHooked(Hub sut) {
            TestMemLeak tml = new TestMemLeak(sut);
            return new WeakReference(tml);
        }

        public void UseStrong_False_LookForDoesNotHoldOn() {
            // Bug 57.  Its quite hard to actually test for memory leaks and fixes to them but this is the best i can come up with.
            Hub sut = new Hub();
            sut.UseStrongReferences = false;

            WeakReference wr = GetInstanceThatHasHooked(sut);

            Assert.True(wr.IsAlive, "Should be alive right after the allocation");
            GC.Collect(2, GCCollectionMode.Forced);
            Assert.False(wr.IsAlive, "Should be dead after a GC");
        }

        public void UseStrong_True_LookForDoesHoldOn() {
            // Bug 57.  Its quite hard to actually test for memory leaks and fixes to them but this is the best i can come up with.
            Hub sut = new Hub();
            sut.UseStrongReferences = true;

            WeakReference wr = GetInstanceThatHasHooked(sut);

            Assert.True(wr.IsAlive, "Should be alive right after the allocation");
            GC.Collect(2, GCCollectionMode.Forced);
            Assert.True(wr.IsAlive, "The reference should still be alive when using strong references");
        }
#endif

        public void LookFor_ReturnsSameActionItWasGiven() {
            Hub sut = new Hub();
            Action<TestMessage> tma = new Action<TestMessage>(param => {
            });
            Action<TestMessage> res = sut.LookFor<TestMessage>(tma);

            Assert.Same(tma, res);
        }

        public void LookFor_SimpleInt_ReturnsSameActionItWasGiven() {
            Hub sut = new Hub();
            Action<int> tma = new Action<int>(param => {
            });
            Action<int> res = sut.LookFor(1, tma);

            Assert.Same(tma, res);
        }

        public void LookFor_SimpleString_ReturnsSameActionItWasGiven() {
            Hub sut = new Hub();
            Action<string> tma = new Action<string>(param => {
            });
            Action<string> res = sut.LookFor("Hello", tma);

            Assert.Same(tma, res);
        }

        public void StopLooking_SimpleStringMessage_DoesStopLooking() {
            Hub sut = new Hub();
            bool wasCalled = false;
            Action<string> tma = sut.LookFor("hello", param => {
                wasCalled = true;
            });
            sut.StopLooking("hello", tma);
            sut.Launch("hello");
            Assert.False(wasCalled, "the call did get executed even after removing stop looking");
        }

        public void StopLooking_SimpleIntMessage_DoesStopLooking() {
            Hub sut = new Hub();
            bool wasCalled = false;
            Action<int> tma = sut.LookFor(1, param => {
                wasCalled = true;
            });
            sut.StopLooking(1, tma);
            sut.Launch(1);
            Assert.False(wasCalled, "the call did get executed even after removing stop looking");
        }

        public void StopLooking_GenericMessage_DoesStopLooking() {
            Hub sut = new Hub();
            bool wasCalled = false;
            Action<TestMessage> tma = sut.LookFor<TestMessage>(param => {
                wasCalled = true;
            });
            sut.StopLooking(tma);
            sut.Launch(new TestMessage());
            Assert.False(wasCalled, "the call did get executed even after removing stop looking");
        }

        public void StopLooking_GenericMessage_StartAndStopWorksTogether() {
            Hub sut = new Hub();
            int callcount = 0;
            Action<TestMessage> tma = sut.LookFor<TestMessage>(param => {
                callcount++;
            });
            sut.Launch(new TestMessage());
            sut.StopLooking(tma);
            sut.Launch(new TestMessage());
            Assert.Equal<int>(1, callcount);
        }

        public void NonAsyncUsesSameThread() {
            Hub sut = new Hub();
            int callout = 0;
            bool sameThreadId = false;

            Action<TestMessage> tma = sut.LookFor<TestMessage>(param => {
                Interlocked.Increment(ref callout);
                if (Thread.CurrentThread.ManagedThreadId.ToString() == param.Data) {
                    sameThreadId = true;
                }
            });

            sut.Launch<TestMessage>(new TestMessage(Thread.CurrentThread.ManagedThreadId.ToString()), false);

            Thread.Sleep(100);
            Assert.True(callout > 0, "The call was not made");
            Assert.True(sameThreadId, "The call was not made on a different thread");
        }

        public void AsyncLaunchPerformsTask() {
            Hub sut = new Hub();
            int callout = 0;
            bool sameThreadId = false;

            Action<TestMessage> tma = sut.LookFor<TestMessage>(param => {
                Interlocked.Increment(ref callout);
                if (Thread.CurrentThread.ManagedThreadId.ToString() == param.Data) {
                    sameThreadId = true;
                }
            });

            sut.Launch<TestMessage>(new TestMessage(Thread.CurrentThread.ManagedThreadId.ToString()), true);

            Thread.Sleep(100);
            Assert.True(callout > 0, "The call was not made");
            Assert.False(sameThreadId, "The call was not made on a different thread");
        }
    }
}