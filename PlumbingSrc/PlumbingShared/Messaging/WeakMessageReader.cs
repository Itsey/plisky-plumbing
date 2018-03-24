namespace Plisky.Plumbing {

    using System;

    /// <summary>
    /// Holds the action which is performed when an opener executes, this implementation uses a weak reference which will ensure that the
    /// action is not held open by the hub. This is more useful when the objects are being replaced on the cliend side.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    internal class WeakMessageReader<T1> : HubMessageBase {
        private WeakReference openerReference;

        internal WeakMessageReader(Action<T1> addOpener) {
            openerReference = new WeakReference(addOpener);
        }

        internal override void OpenNote() {
            Action<T1> reciever = (Action<T1>)openerReference.Target;
            if (reciever != null) {
                reciever(default(T1));
            }
        }

        internal override void OpenNote(object onThis) {
            Action<T1> reciever = (Action<T1>)openerReference.Target;
            if (reciever != null) {
                reciever((T1)onThis);
            }
        }

        /* Pretty sure this isnt used.
         * internal void OpenNote(T1 withParameter) {
              Action<T1> reciever = (Action<T1>)openerReference.Target;
              if (reciever != null) {
                  reciever((T1)withParameter);
              }
          }*/

        internal override bool Valid() {
            return openerReference.IsAlive;
        }

        internal override bool ContainsThisAction(object testSubject) {
            return object.ReferenceEquals(openerReference.Target, testSubject);
        }
    }
}