namespace Plisky.Plumbing {

    using Plisky.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    /// Holds a hub for messaging, sending messages from one part of the application to another.
    /// </summary>
    public class Hub {
        protected Bilge b = new Bilge("Hub");  // Diags.PliskyMessagingSwitch

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        private Dictionary<Type, List<HubMessageBase>> peopleSearching = new Dictionary<Type, List<HubMessageBase>>();

        private Dictionary<int, List<HubMessageBase>> simpleMessagePeopleSearching = new Dictionary<int, List<HubMessageBase>>();

        #region static areas for single instance hubs

        private static object hubLock = new object();
        private static Hub currentInstance;
        private static int instanceCount = 0;

        /// <summary>
        /// Destroys the static "current" in order that all listeners are lost.
        /// </summary>
        public static void Relinquish() {
            lock (hubLock) {
                currentInstance = new Hub();
            }
        }

        public static Hub Current {
            get {
                if (currentInstance == null) {
                    lock (hubLock) {
                        currentInstance = new Hub();
                    }
                }
                return currentInstance;
            }
        }

        #endregion

        #region privates

        private string tracePrefix = null;

        private string GetTraceMessage(string v) {
            return tracePrefix + " " + v;
        }

        private void RemoveIfItsThere(List<HubMessageBase> list, object opener) {
            for (int i = 0; i < list.Count; i++) {
                if (list[i].ContainsThisAction(opener)) {
                    list.RemoveAt(i);
                    break;
                }
            }
        }

        private void LaunchSimple(int messageIdentity, bool async) {
            int messagesDispatched = 0;
            if (simpleMessagePeopleSearching.ContainsKey(messageIdentity)) {
                foreach (var subscribers in simpleMessagePeopleSearching[messageIdentity]) {
                    if (subscribers.Accept(messageIdentity)) {
                        messagesDispatched++;
                        if (async) {
                            ThreadPool.QueueUserWorkItem(new WaitCallback(subscribers.OpenNoteAsync), messageIdentity);
                        } else {
                            subscribers.OpenNote(messageIdentity);
                        }
                    }
                }
            }

            if (messagesDispatched == 0) {
                b.Verbose.Log(GetTraceMessage("message was sent but there were no local recipients"));
            }
        }

        #endregion

        /// <summary>
        /// Indicates whether the hub should hold a reference to the Actions that are passed into the LookFor methods, in the case where you are using
        /// inline Actions and are able to manage your references yourself (with Unregistering correctly) then this can be set to True to manage the lifetime
        /// of the Actions without referencing them as member variables.
        /// </summary>
        public bool UseStrongReferences { get; set; }

        public string InstanceName { get; internal set; }

        #region constructors

        public Hub() {
            ConstructMe(false, null);
            ;
        }

        private void ConstructMe(bool v, string instanceNameHint) {
            UseStrongReferences = v;
            if (string.IsNullOrEmpty(instanceNameHint)) {
                instanceNameHint = "HubDefault";
            }
            instanceCount++;
            InstanceName += instanceNameHint + "_" + instanceCount.ToString();
            tracePrefix = "[Hub>>" + InstanceName + "]";
            b.Verbose.Log(GetTraceMessage($"Messaging hub is online [{InstanceName}]"));
        }

        public Hub(bool useStrongReferences) {
            ConstructMe(useStrongReferences, null);
        }

        public Hub(string v) {
            ConstructMe(false, v);
        }

        #endregion

        // public virtual Func<T1, T1> LookFor<T1>(Func<T1,T1> openMessage) {
        public virtual Action<T1> LookFor<T1>(Action<T1> openMessage) {
            Type msgType = typeof(T1);

            b.Verbose.Log(GetTraceMessage($"Adding listener for message type {msgType}"));
            if (!peopleSearching.ContainsKey(msgType)) {
                peopleSearching.Add(msgType, new List<HubMessageBase>());
            }

            HubMessageBase msg;
            if (UseStrongReferences) {
                msg = new StrongMessageReader<T1>(openMessage);
            } else {
                msg = new WeakMessageReader<T1>(openMessage);
            }
            peopleSearching[msgType].Add(msg);
            return openMessage;
        }

        public virtual Action<int> LookFor(int thisMessage, Action<int> openMessage) {
            if (!simpleMessagePeopleSearching.ContainsKey(thisMessage)) {
                simpleMessagePeopleSearching.Add(thisMessage, new List<HubMessageBase>());
            }

            b.Verbose.Log(GetTraceMessage($"Adding listener for simple int message type {thisMessage}"));
            HubMessageBase msg;
            if (UseStrongReferences) {
                msg = new StrongMessageReader<int>(openMessage);
            } else {
                msg = new WeakMessageReader<int>(openMessage);
            }
            simpleMessagePeopleSearching[thisMessage].Add(msg);
            return openMessage;
        }

        public virtual Action<string> LookFor(string thisMessage, Action<string> openMessage) {
            Type msgType = typeof(string);
            b.Verbose.Log(GetTraceMessage($"Adding listener for simple string message type {thisMessage}"));
            if (!peopleSearching.ContainsKey(msgType)) {
                peopleSearching.Add(msgType, new List<HubMessageBase>());
            }

            peopleSearching[msgType].Add(new MessageReaderSimple(thisMessage, openMessage));

            return openMessage;
        }

        public virtual void Launch<TMessage>(TMessage message, bool async = false) {
            int messagesDispatched = 0;

            Type msgType = typeof(TMessage);
            b.Verbose.Log(GetTraceMessage($"Message Type {msgType.ToString()} recieved by Launcher"));

            if (peopleSearching.ContainsKey(msgType)) {
                foreach (var subscribers in peopleSearching[msgType]) {
                    if (subscribers.Accept(message)) {
                        messagesDispatched++;
                        if (async) {
                            b.Info.Log(GetTraceMessage($"Async message launched {message.ToString()}"));
                            ThreadPool.QueueUserWorkItem(new WaitCallback(subscribers.OpenNoteAsync), message);
                        } else {
                            b.Info.Log(GetTraceMessage($"Message launched {message.ToString()}"));
                            subscribers.OpenNote(message);
                        }
                    }
                }
            } else {
                b.Warning.Log(GetTraceMessage("Message type could not be found, no one being notified"));
            }

            if (messagesDispatched == 0) {
                b.Warning.Log(GetTraceMessage("Message sent but no one received it."));
            } else {
                b.Verbose.Log(GetTraceMessage($"{messagesDispatched} messages were dispatched to active listeners"));
            }
        }

        /// <summary>
        /// Sends a simple integer message to the listeners
        /// </summary>
        /// <param name="messageIdentity">The integer message identity</param>
        public virtual void Launch(int messageIdentity, bool async = false) {
            LaunchSimple(messageIdentity, async);
        }

        /// <summary>
        /// Sends a simple string message to the listeners.
        /// </summary>
        /// <param name="messageContext">The string message identifier</param>
        public virtual void Launch(string messageContext, bool async = false) {
            Launch<string>(messageContext, async);
        }

        /// <summary>
        /// Removes the registered action from the list of actions.  Pass in the message identifier that was first associated with this action
        /// and the action to have it removed.  If the action is not found in the list nothing happens.
        /// </summary>
        /// <param name="targetMessage">The generic message identifier</param>
        /// <param name="opener">The action that was passed to LookFor</param>
        public virtual void StopLooking<T1>(Action<T1> opener) {
            Type msgType = typeof(T1);
            if (peopleSearching.ContainsKey(msgType)) {
                RemoveIfItsThere(peopleSearching[msgType], opener);
            }
        }

        /// <summary>
        /// Removes the registered action from the list of actions.  Pass in the message identifier that was first associated with this action
        /// and the action to have it removed.  If the action is not found in the list nothing happens.
        /// </summary>
        /// <param name="targetMessage">The string message identifier</param>
        /// <param name="opener">The action that was passed to LookFor</param>
        public virtual void StopLooking(string simpleMessage, Action<string> opener) {
            Type msgType = typeof(string);
            if (peopleSearching.ContainsKey(msgType)) {
                RemoveIfItsThere(peopleSearching[msgType], opener);
            }
        }

        /// <summary>
        /// Removes the registered action from the list of actions.  Pass in the message identifier that was first associated with this action
        /// and the action to have it removed.  If the action is not found in the list nothing happens.
        /// </summary>
        /// <param name="targetMessage">The int message identifier</param>
        /// <param name="opener">The action that was passed to LookFor</param>
        public virtual void StopLooking(int targetMessage, Action<int> opener) {
            if (simpleMessagePeopleSearching.ContainsKey(targetMessage)) {
                RemoveIfItsThere(simpleMessagePeopleSearching[targetMessage], opener);
            }
        }
    }
}