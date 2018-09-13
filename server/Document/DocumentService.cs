using System.IO;
using System.Threading.Tasks;
using Documate.BlockChain;
using Documate.Crypto;
using Documate.Data;
using Documate.Storage;
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
        public async Task<DocumentData> Load (string sender, string signature, string hash) {
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

        public async Task<DocumentInfo> Store (string sender, string signature, byte[] data, string name) {
            string hash = Verify (sender, signature, data);
            await Save (sender, hash, data, name);
            var serverSignature = Sign (sender, hash);
            return new DocumentInfo (hash, serverSignature);
        }

        private async Task Save (string sender, string hash, byte[] data, string name) {
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

        private string Sign (string sender, string hash) {
            var signature = cryptoService.Sign (Model.FromItems (
                ModelItem.From (DataType.bytes32, hash.HexToByteArray ()),
                ModelItem.From (DataType.address, sender)
            ));
            return signature;
        }

        private string Verify (string sender, string signature, byte[] data) {
            var recoverModel = cryptoService.EcRecover (sender, signature, data);
            var valid = (sender == recoverModel.Signer);
            if (!recoverModel.Valid)
                throw new InvalidSignatureException ();
            return recoverModel.Hash;
        }
        private string Verify (string sender, string signature, string hash) {
            var recoverModel = cryptoService.EcRecover (sender, signature, hash);
            var valid = (sender == recoverModel.Signer);
            if (!recoverModel.Valid)
                throw new InvalidSignatureException ();
            return recoverModel.Hash;
        }

    }
}