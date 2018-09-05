using System.Collections.Generic;
using System.Linq;

namespace Documate.Crypto {
    public class CryptoOptions {
        public CryptoOptions (string privateKey, params ModelItem[] items) {
            this.PrivateKey = privateKey;
            this.AdditionalSignatureItems = items.ToList ();
        }
        public string PrivateKey { get; private set; }

        public IList<ModelItem> AdditionalSignatureItems { get; private set; }

    }
}