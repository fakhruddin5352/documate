using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Documate.Helpers;
using Documate.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Http.Features;
using Documate.Crypto;
using Microsoft.Extensions.Logging;

namespace Documate.Controllers {

    [Route ("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase {


    	private static readonly FormOptions _defaultFormOptions = new FormOptions();
        private readonly ILogger logger;
        private readonly ApplicationDbContext context;
        private ICryptoService cryptoService;

        public DocumentController (ApplicationDbContext context, ILogger<DocumentController> logger,ICryptoService cryptoService) {
            this.context = context;
            this.logger = logger;
            this.cryptoService = cryptoService;
        }

        [HttpPost]
        public async Task<IActionResult> Upload () {
            if (!MultipartRequestHelper.IsMultipartContentType (Request.ContentType)) {
                return BadRequest ($"Expected a multipart request, but got {Request.ContentType}");
            }

            var document = new Document {
            };
            using (var stream = new MemoryStream()) {
                var formData = await  Request.StreamFile(stream);
                document.Hash = formData["Signature"];
                var signer = formData["Signer"];
                var signature = formData["Signature"].ToString();
                var valid =  (signer == cryptoService.EcRecover(signature,stream.ToArray()));
                if (!valid)
                    return base.BadRequest("Invalid Signature");
            }


            await context.Documents.AddAsync (document);
            await context.SaveChangesAsync ();
            return base.Ok ("OK");
        }
    }
}