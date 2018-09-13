using Documate.ValueObjects;

namespace Documate.Document
{
    public class DocumentInfo {
        public DocumentInfo (Hash hash, Signature signature) {
            this.Hash = hash;
            this.Signature = signature;

        }
        public Hash Hash { get; }
        public Signature Signature { get; }
    }
}