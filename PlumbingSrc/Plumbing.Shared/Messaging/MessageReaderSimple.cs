namespace Plisky.Plumbing {

    using System;

    internal class MessageReaderSimple : HubMessageBase {
        private Action<string> opener;
        private string keyMatch;

        public MessageReaderSimple(string targetMessage, Action<string> addOpener) {
            keyMatch = targetMessage;
            opener = addOpener;
        }

        internal override bool Accept(object onThis) {
            return (string)onThis == keyMatch;
        }

        /*
        internal override object OpenNote(object onThis) {
            return opener((string)onThis);
        }*/

        internal void OpenNote(string onThis) {
            opener(onThis);
        }

        internal override bool ContainsThisAction(object testSubject) {
            return object.ReferenceEquals(opener, testSubject);
        }
    }
}