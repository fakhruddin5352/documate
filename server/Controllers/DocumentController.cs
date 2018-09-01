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

namespace Documate.Controllers {

    public class Model {
        public IFormFile Document { get; set; }
        public string Signature { get; set; }
    }

    [Route ("api/[controller]")]
    [ApiController]
    public class DocumentController : ControllerBase {


    	private static readonly FormOptions _defaultFormOptions = new FormOptions();
 
        private readonly ApplicationDbContext context;

        public DocumentController (ApplicationDbContext context) {
            this.context = context;
        }


        [HttpPost]
        public async Task<IActionResult> Upload () {
            if (!MultipartRequestHelper.IsMultipartContentType (Request.ContentType)) {
                return BadRequest ($"Expected a multipart request, but got {Request.ContentType}");
            }

            var document = new Document {
            };
            using (var stream = new FileStream("Files/test.jpeg",FileMode.OpenOrCreate)) {
                var formData = await  Request.StreamFile(stream);
                document.Hash = formData["Signature"];
            }

            await context.Documents.AddAsync (document);
            await context.SaveChangesAsync ();
            return base.Ok ("OK");
        }
    }
}