using Documate.ValueObjects;

namespace Documate.Crypto
{
    public class RecoverModel
    {
        public Hash Hash{get;set;}

        public Address Signer{get;set;}

        public bool Valid{get;set;}
    }
}