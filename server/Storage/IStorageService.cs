using System.IO;
using System.Threading.Tasks;
using Documate.ValueObjects;

namespace Documate.Storage {
    public interface IStorageService {
        Task<bool> Store (Hash hash, Stream stream);
        Task<Stream> Load (Hash hash);
    }
}