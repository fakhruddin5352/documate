namespace Documate.ValueObjects
{
    [System.Serializable]
    public class InvalidSignatureException : System.Exception
    {
        public InvalidSignatureException() { }
        public InvalidSignatureException(string message) : base(message) { }
        public InvalidSignatureException(string message, System.Exception inner) : base(message, inner) { }
        protected InvalidSignatureException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}