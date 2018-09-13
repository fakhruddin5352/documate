using System.Collections.Generic;
using System.Linq;
using Documate.ValueObjects;

namespace Documate.Crypto {
    public class CryptoOptionsBuilder {
        public string PrivateKey { get; private set; }

        public IList<IDataType> AdditionalSignatureItems { get; private set; }

        public CryptoOptionsBuilder () {
            AdditionalSignatureItems = new List<IDataType> ();
        }

        public CryptoOptionsBuilder UsePrivateKey (string privateKey) {
            PrivateKey = privateKey;
            return this;
        }

        public CryptoOptionsBuilder AddAdditionalSignatureItem (IDataType item) {
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