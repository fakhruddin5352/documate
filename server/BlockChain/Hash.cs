using Nethereum.Hex.HexConvertors.Extensions;

namespace server.BlockChain
{
    public class Hash
    {
        public string Value{get;private set;}

        private Hash(string value){
            this.Value = value;
        }

        public static Hash Parse(string value) {
            return null;
        }
    }
}