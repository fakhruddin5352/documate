using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Documate.BlockChain;
using Documate.Crypto;
using Documate.Data;
using Documate.DTO;
using Documate.Helpers;
using Documate.Storage;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Documate.Controllers {

    [Route ("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase {
        private readonly ILogger logger;
        private readonly IDataService dataService;
        private readonly ICryptoService cryptoService;

        private readonly IStorageService storageService;
        private readonly IBlockChainService blockChainService;

        public DocumentController (ILogger<DocumentController> logger, ICryptoService cryptoService,
            IStorageService storageService, IDataService dataService, IBlockChainService blockChainService) {
            this.logger = logger;
            this.dataService = dataService;
            this.cryptoService = cryptoService;
            this.storageService = storageService;
            this.blockChainService = blockChainService;
        }

        [Route ("", Name = "Download")]
        [HttpGet ()]
        public async Task<IActionResult> DownloadRequest ([FromQuery] DownloadRequest request) {
            //check if 

            var recoverModel = cryptoService.EcRecover (request.Sender, request.Signature, request.Hash);
            if (!recoverModel.Valid)
                return base.BadRequest ("Invalid signature");

            var document = await dataService.Load (request.Hash);
            if (document == null)
                return base.NotFound (request.Hash);

            bool canView = await blockChainService.CanViewDocument (request.Sender, request.Hash);
            if (!canView)
                return base.Forbid ("Not allowed to access the document");

            var stream = await storageService.Load (document.Hash);
            var mime = MimeTypes.GetMimeType (document.Name);
            EntityTagHeaderValue entityTag = EntityTagHeaderValue.Parse("\"" + document.Hash.RemoveHexPrefix() + "\"");
            return base.File (stream, mime, document.Name, document.When, entityTag);
        }

        [Route ("", Name = "Upload")]
        [HttpPost ()]
        public async Task<IActionResult> UploadRequest ([FromForm] UploadRequest request) {

            byte[] data = new byte[request.File.Length];
            using (var stream = new MemoryStream (data)) {
                request.File.CopyTo (stream);
                var recoverModel = cryptoService.EcRecover (request.Sender, request.Signature, data);
                var valid = (request.Sender == recoverModel.Signer);
                if (!recoverModel.Valid)
                    return base.BadRequest ("Invalid signature");

                var document = await dataService.Load (recoverModel.Hash);
                if (document == null) {
                    document = new Document () {
                    Hash = recoverModel.Hash,
                    Owner = recoverModel.Signer,
                    Name = request.Name
                    };
                    stream.Seek (0, SeekOrigin.Begin);
                    if (!await storageService.Store (recoverModel.Hash, stream)) {
                        return base.StatusCode (500, "Server error");
                    }
                    await dataService.Save (document);
                }

                var signature = cryptoService.Sign (Model.FromItems (
                    ModelItem.From (DataType.bytes32, document.Hash.HexToByteArray ()),
                    ModelItem.From (DataType.address, document.Owner)
                ));

                return base.Created (Url.Link ("Download", recoverModel.Hash),
                    new UploadResponse { Signature = signature });

            }

        }
    }
}