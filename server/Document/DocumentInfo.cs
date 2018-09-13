namespace Documate.Document
{
    public class DocumentInfo {
        public DocumentInfo (string hash, string signature) {
            this.Hash = hash;
            this.Signature = signature;

        }
        public string Hash { get; }
        public string Signature { get; }
    }
}