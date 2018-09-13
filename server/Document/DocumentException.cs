namespace Documate.Document
{
    [System.Serializable]
    public class DocumentException : System.Exception {
        public DocumentException () { }
        public DocumentException (string message) : base (message) { }
        public DocumentException (string message, System.Exception inner) : base (message, inner) { }
        protected DocumentException (
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base (info, context) { }
    }
}