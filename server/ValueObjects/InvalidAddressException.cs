namespace Documate.ValueObjects
{
    [System.Serializable]
    public class InvalidAddressException : System.Exception
    {
        public InvalidAddressException() { }
        public InvalidAddressException(string message) : base(message) { }
        public InvalidAddressException(string message, System.Exception inner) : base(message, inner) { }
        protected InvalidAddressException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}