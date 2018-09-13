using System;
using System.IO;
using System.Threading.Tasks;
using Documate.BlockChain;
using Documate.Crypto;
using Documate.Data;
using Documate.Storage;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Documate.Document {
    public interface IDocumentService {
        Task<DocumentData> Load (string sender, string signature, string hash);

        Task<DocumentInfo> Store (string sender, string hash, byte[] data, string name);

    }

}