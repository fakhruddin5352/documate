using System.Collections.Generic;
using System.Linq;
using Documate.ValueObjects;

namespace Documate.Crypto {
    public class CryptoOptions {
        public CryptoOptions (string privateKey, params IDataType[] items) {
            this.PrivateKey = privateKey;
            this.AdditionalSignatureItems = items.ToList ();
        }
        public string PrivateKey { get; private set; }

        public IList<IDataType> AdditionalSignatureItems { get; private set; }

    }
}