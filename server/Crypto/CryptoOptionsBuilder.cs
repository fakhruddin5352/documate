using System.Collections.Generic;
using System.Linq;

namespace Documate.Crypto {
    public class CryptoOptionsBuilder {
        public string PrivateKey { get; private set; }

        public IList<ModelItem> AdditionalSignatureItems { get; private set; }

        public CryptoOptionsBuilder () {
            AdditionalSignatureItems = new List<ModelItem> ();
        }

        public CryptoOptionsBuilder UsePrivateKey (string privateKey) {
            PrivateKey = privateKey;
            return this;
        }

        public CryptoOptionsBuilder AddAdditionalSignatureItem (ModelItem item) {
            AdditionalSignatureItems.Add (item);
            return this;
        }

        internal CryptoOptions Build () {
            if (string.IsNullOrEmpty(PrivateKey))
                throw new CryptoConfigurationException($"{nameof(PrivateKey)} cannot be null");
            return new CryptoOptions (PrivateKey, AdditionalSignatureItems.ToArray ());
        }

    }
}