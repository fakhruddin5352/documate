namespace Documate.BlockChain
{
    public class BlockChainOptionsBuilder
    {
        public string DocumateAbi {get; private set;}
        public string DocumentAbi {get; private set;}
        public string RpcEndpoint{get; private set;}
        public string DocumateAddress { get; private set; }

        public BlockChainOptionsBuilder UseDocumentApi(string abi) {
            DocumentAbi = abi;
            return this;
        }

          public BlockChainOptionsBuilder UseDocumateApi(string abi) {
            DocumateAbi = abi;
            return this;
        }

        public BlockChainOptionsBuilder UseRpcEndpoint(string endpoint) {
            RpcEndpoint = endpoint;
            return this;
        }

        public BlockChainOptionsBuilder UserDocumateAddress(string address) {
            DocumateAddress = address;
            return this;
        }

        internal BlockChainOptions Build() {
            if (string.IsNullOrEmpty(RpcEndpoint))
                throw new BlockChainConfigurationException($"{nameof(RpcEndpoint)} cannot be null");
            if (string.IsNullOrEmpty(DocumateAbi))
                throw new BlockChainConfigurationException($"{nameof(DocumateAbi)} cannot be null");
            if (string.IsNullOrEmpty(DocumentAbi))
                throw new BlockChainConfigurationException($"{nameof(DocumentAbi)} cannot be null");
            if (string.IsNullOrEmpty(DocumateAddress))
                throw new BlockChainConfigurationException($"{nameof(DocumateAddress)} cannot be null");
            return new BlockChainOptions(RpcEndpoint,DocumentAbi, DocumateAbi, DocumateAddress);
        }
        
    }
}