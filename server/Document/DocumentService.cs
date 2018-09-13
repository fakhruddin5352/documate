using System.IO;
using System.Threading.Tasks;
using Documate.BlockChain;
using Documate.Crypto;
using Documate.Data;
using Documate.Storage;
using Documate.ValueObjects;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Documate.Document
{
    public class DocumentService : IDocumentService {
        private readonly IBlockChainService blockChainService;
        private readonly IStorageService storageService;
        private readonly ICryptoService cryptoService;
        private readonly IDataService dataService;
        private readonly ILogger<DocumentService> logger;

        public DocumentService (IBlockChainService blockChainService, IStorageService storageService, ICryptoService cryptoService,
            IDataService dataService, ILogger<DocumentService> logger) {
            this.blockChainService = blockChainService ??
                throw new System.ArgumentNullException (nameof (blockChainService));
            this.storageService = storageService ??
                throw new System.ArgumentNullException (nameof (storageService));
            this.cryptoService = cryptoService ??
                throw new System.ArgumentNullException (nameof (cryptoService));
            this.dataService = dataService ??
                throw new System.ArgumentNullException (nameof (dataService));
            this.logger = logger ??
                throw new System.ArgumentNullException (nameof (logger));
        }
        public async Task<DocumentData> Load (Address sender, Signature signature, Hash hash) {
            Verify (sender, signature, hash);

            var document = await dataService.Load (hash);
            if (document == null)
                throw new NotFoundException ();

            bool canView = await blockChainService.CanViewDocument (sender, hash);
            if (!canView)
                throw new UnAuthorizedAccessException ();

            var stream = await storageService.Load (document.Hash);
            return new DocumentData (document.Name, stream, document.When, document.Hash);
        }

        public async Task<DocumentInfo> Store (Address sender, Signature signature, byte[] data, string name) {
            var hash = Verify (sender, signature, data);
            await Save (sender, hash, data, name);
            var serverSignature = Sign (sender, hash);
            return new DocumentInfo (hash, serverSignature);
        }

        private async Task Save (Address sender, Hash hash, byte[] data, string name) {
            var document = await dataService.Load (hash);
            if (document == null) {
                document = new Data.Document () {
                Hash = hash,
                Owner = sender,
                Name = name
                };
                using (var ms = new MemoryStream (data)) {
                    if (!await storageService.Store (hash, ms)) { 
                        throw new StorageException ();
                    }
                }
                await dataService.Save (document);
            }
        }

        private Signature Sign (Address sender, Hash hash) {
            var signature = cryptoService.Sign (Model.FromItems (hash,sender));
            return signature;
        }

        private Hash Verify (Address sender, Signature signature, byte[] data) {
            var recoverModel = cryptoService.EcRecover (sender, signature, data);
            var valid = (sender == recoverModel.Signer);
            if (!recoverModel.Valid)
                throw new UnAuthorizedAccessException ();
            return recoverModel.Hash;
        }
        private Hash Verify (Address sender, Signature signature, Hash hash) {
            var recoverModel = cryptoService.EcRecover (sender, signature, hash);
            var valid = (sender == recoverModel.Signer);
            if (!recoverModel.Valid)
                throw new UnAuthorizedAccessException ();
            return recoverModel.Hash;
        }

    }
}