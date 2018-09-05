namespace Documate.BlockChain {
    public class BlockChainOptions {
        public string DocumateAbi { get; private set; }
        public string DocumentAbi { get; private set; }
        public string RpcEndpoint { get; private set; }
        public string DocumateAddress { get; private set; }

        public BlockChainOptions (string rpcEndpoint, string documentAbi, string documateAbi, string documateAddress) {
            this.RpcEndpoint = rpcEndpoint;
            this.DocumentAbi = documentAbi;
            this.DocumateAbi = documateAbi;
            this.DocumateAddress = documateAddress;
        }
    }
}