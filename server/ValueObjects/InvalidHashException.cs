namespace Documate.ValueObjects
{
    [System.Serializable]
    public class InvalidHashException : System.Exception
    {
        public InvalidHashException() { }
        public InvalidHashException(string message) : base(message) { }
        public InvalidHashException(string message, System.Exception inner) : base(message, inner) { }
        protected InvalidHashException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}