namespace Documate.BlockChain
{
    public class InvalidHashException : BlockChainException
    {
        public InvalidHashException(string message) : base(message)
        {
        }
    }
}