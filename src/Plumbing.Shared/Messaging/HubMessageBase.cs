namespace Plisky.Plumbing {

    /// <summary>
    /// HubMessageBase is required as a base type for all messages that are to be sent.  It allows us to operate on all message types and also to use
    /// either the weak or strong implementations.
    /// </summary>
    internal abstract class HubMessageBase {

        internal virtual bool Accept(object onThis) {
            return true;
        }

        internal virtual bool ContainsThisAction(object opener) {
            return false;
        }

        internal virtual void OpenNote() {
        }

        internal virtual void OpenNote(object onThis) {
        }

        internal virtual void OpenNoteAsync(object onThis) {
            OpenNote(onThis);
        }

        /// <summary>
        /// Is this message valid.  If this method returns false then the hub will remove it from the listeners.
        /// </summary>
        /// <returns></returns>
        internal virtual bool Valid() {
            return true;
        }
    }
}