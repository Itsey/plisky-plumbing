namespace Plisky.Plumbing {

    using System;

    internal class MessageReaderSimple : HubMessageBase {
        private string keyMatch;
        private Action<string> opener;

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

        internal override bool ContainsThisAction(object testSubject) {
            return object.ReferenceEquals(opener, testSubject);
        }

        internal void OpenNote(string onThis) {
            opener(onThis);
        }
    }
}