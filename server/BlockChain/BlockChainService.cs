using Nethereum.JsonRpc.Client;
using Nethereum.Web3;
using Nethereum.Hex.HexConvertors.Extensions;
using System.Threading.Tasks;

namespace Documate.BlockChain
{
    public class BlockChainService : IBlockChainService
    {
        private readonly string web3Provider;
        private readonly string documateAbi;

        private readonly string documateAddress;

        public BlockChainService(string web3Provider, string documateAddress, string documateAbi)
        {
            this.web3Provider = web3Provider;
            this.documateAbi = documateAbi;
            this.documateAddress = documateAddress;
        }

        public Task<bool> CanViewDocument(string by, string document)
        {
            var web3 = new Web3(web3Provider);
            var contract = web3.Eth.GetContract(documateAbi, documateAddress.EnsureHexPrefix());
            var function = contract.GetFunction("canViewDocument");
            return function.CallAsync<bool>(by.EnsureHexPrefix(), document.HexToByteArray());
        }
    }
}