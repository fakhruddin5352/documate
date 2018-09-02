using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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

namespace Documate.Controllers {


    [Route ("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase {
        private readonly ILogger logger;
        private readonly IDataService dataService;
        private readonly ICryptoService cryptoService;

        private readonly IStorageService storageService;

        public DocumentController (ILogger<DocumentController> logger, ICryptoService cryptoService,
        IStorageService storageService, IDataService dataService) {
            this.logger = logger;
            this.dataService = dataService;
            this.cryptoService = cryptoService;
            this.storageService = storageService;
        }

        [Route("", Name="Download")]
        [HttpGet()]
        public async Task<IActionResult> DownloadRequest (DownloadRequest request) {
            //check if 
            var recoverModel = cryptoService.EcRecover (request.Sender, request.Signature,  request.Hash);
            if (!recoverModel.Valid)
                return base.BadRequest ("Invalid Signature");

            return base.Ok("OK");
       }
        

        [Route("", Name="Upload")]
        [HttpPost ()]
        public async Task<IActionResult> UploadRequest ([FromForm] UploadRequest request) {
            byte[] data = new byte[request.File.Length];
            using (var stream = new MemoryStream (data)) {
                request.File.CopyTo (stream);
                var recoverModel = cryptoService.EcRecover (request.Sender, request.Signature,  data);
                var valid = (request.Sender == recoverModel.Signer);
                if (!recoverModel.Valid)
                    return base.BadRequest ("Invalid Signature");

                var document = new Document () {
                    Hash = recoverModel.Hash,
                    Owner = recoverModel.Signer,
                    Name = request.Name
                };
                stream.Seek(0, SeekOrigin.Begin);
                if (!await storageService.Store(recoverModel.Hash, stream)){
                    return base.StatusCode(500, "Server error");
                }
                await dataService.Save(document);                
                return base.Created(Url.Link("Download",recoverModel.Hash),recoverModel.Hash);

            }

        }
    }
}