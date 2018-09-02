using System.IO;
using System.Threading.Tasks;

namespace Documate.Storage {
    public interface IStorageService {
        Task<bool> Store (string hash, Stream stream);
        Task<bool> Load (string hash, Stream stream);
    }
}