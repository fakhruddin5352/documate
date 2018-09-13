using System;
using System.IO;
using System.Threading.Tasks;
using Documate.BlockChain;
using Documate.Crypto;
using Documate.Data;
using Documate.Storage;
using Documate.ValueObjects;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Documate.Document {
    public interface IDocumentService {
        Task<DocumentData> Load (Address sender, Signature signature, Hash hash);

        Task<DocumentInfo> Store (Address sender, Signature signature, byte[] data, string name);

    }

}