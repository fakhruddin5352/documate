using Microsoft.AspNetCore.Http;

namespace Documate.DTO {

    public class UploadRequest {
        public string Signature { get; set; }
        public string Sender { get; set; }
        public IFormFile File { get; set; }
        public string Name { get; set; }
    }
}