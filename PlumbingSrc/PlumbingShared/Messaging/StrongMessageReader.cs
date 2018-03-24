namespace Plisky.Plumbing {

    using System;

    /// <summary>
    /// Holds the action which is performed when an opener executes, this implementation uses a strong reference which will ensure that the
    /// action is available to be called even when the origin of the action goes out of scope. However this will also keep that reference until
    /// explicitly removed or the hub goes out of scope.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    internal class StrongMessageReader<T1> : HubMessageBase {
        private Action<T1> opener;

        internal StrongMessageReader(Action<T1> addOpener) {
            opener = addOpener;
        }

        internal override void OpenNote() {
            opener(default(T1));
        }

        internal override void OpenNote(object onThis) {
            opener((T1)onThis);
        }

        /*Pretty sure this isnt used
        internal void OpenNote(T1 withParameter) {
            opener(withParameter);
        }
         * */

        internal override bool ContainsThisAction(object testSubject) {
            return object.ReferenceEquals(opener, testSubject);
        }
    }
}