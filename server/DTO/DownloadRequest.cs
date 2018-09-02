namespace Documate.DTO
{
    public class DownloadRequest
    {
        public string Sender{get;set;}

        public string Signature{get;set;}
        public string Hash{get;set;}
    }
}