using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Documate.BlockChain;
using Documate.Crypto;
using Documate.Data;
using Documate.Document;
using Documate.DTO;
using Documate.Helpers;
using Documate.Storage;
using Documate.ValueObjects;
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
        private readonly IDocumentService documentService;

        public DocumentController (ILogger<DocumentController> logger, IDocumentService documentService) {
            this.logger = logger ??
                throw new ArgumentNullException (nameof (logger));
            this.documentService = documentService ??
                throw new ArgumentNullException (nameof (documentService));
        }

        [Route ("", Name = "Download")]
        [HttpGet ()]
        public async Task<IActionResult> DownloadRequest ([FromQuery] DownloadRequest request) {
            //check if 
            try {
                var data = await documentService.Load (Address.Of(request.Sender), Signature.Of(request.Signature), Hash.Of(request.Hash));
                var mime = MimeTypes.GetMimeType (data.Name);
                EntityTagHeaderValue entityTag = EntityTagHeaderValue.Parse ("\"" + data.Hash.ToHexWithoutPrefix() + "\"");
                return base.File (data.Data, mime, data.Name, data.CreatedOn, entityTag);
            } catch (InvalidSignatureException ex) {
                return base.BadRequest ("Invalid signature");
            } catch (UnAuthorizedAccessException ex) {
                return base.Unauthorized ();
            } catch (NotFoundException ex) {
                return base.NotFound (request.Hash);
            }

        }

        [Route ("", Name = "Upload")]
        [HttpPost ()]
        public async Task<IActionResult> UploadRequest ([FromForm] UploadRequest request) {

            byte[] data = new byte[request.File.Length];
            using (var stream = new MemoryStream (data)) {
                await request.File.CopyToAsync (stream);
                try {
                    var info = await documentService.Store (Address.Of(request.Sender),Signature.Of(request.Signature), data, request.Name);
                    return base.Created (Url.Link ("Download", info.Hash),
                        new UploadResponse { Signature = info.Signature });
                } catch (InvalidSignatureException ex) {
                    return base.BadRequest ("Invalid signature");
                } catch (Document.StorageException ex) {
                    return base.StatusCode (500, "Server error");
                }
            }

        }
    }
}