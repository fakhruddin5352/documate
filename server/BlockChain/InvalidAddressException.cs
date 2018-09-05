namespace Documate.BlockChain
{
    public class InvalidAddressException : BlockChainException
    {
        public InvalidAddressException(string message) : base(message)
        {
        }
    }
}