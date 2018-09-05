using System.Threading.Tasks;

namespace Documate.BlockChain {
    public interface IBlockChainService {
        Task<bool> CanViewDocument (string by, string document);
    }
}