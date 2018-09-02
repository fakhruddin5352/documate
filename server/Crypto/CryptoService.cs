using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Web3;

namespace Documate.Crypto {
    class CryptoService : ICryptoService {

        private readonly ILogger logger;

        public CryptoService(ILogger<CryptoService> logger){
            this.logger = logger;
        }
        private string Keccak256 (byte[] data) {
            var signer = new EthereumMessageSigner();
            return signer.Hash(data).ToHex(true);
        }

        private string EncodedPacked (Model model) {
            using (var stream = new MemoryStream()){
                foreach (var item in model.Items) {
                    var data = Nethereum.ABI.AddressType.CreateABIType(item.Type.ToString()).Encode(item.Value);
                    stream.Write(data);
                }
                return stream.ToArray().ToHex();
            }
        }

        private Model ToModel (IEnumerable<object> values, IList<DataType> types) {
            return new Model {
                Items = values.Select((v,i) => new ModelItem{Value=v, Type = types[i]})
            };
        }

        public RecoverModel EcRecover(string sender, string signature, byte[] data) {
            var ethereumSigner = new EthereumMessageSigner();
            var hash = Web3.Sha3(System.Convert.ToBase64String(data)).EnsureHexPrefix();
            return EcRecover(sender, signature, hash);

        }

        public RecoverModel EcRecover(string sender, string signature, string hash)
        {
            var ethereumSigner = new EthereumMessageSigner();
            var signer = ethereumSigner.EcRecover(hash.HexToByteArray(), signature);
            logger.LogDebug("Hash {0} , Signature {1}, Signer {2}, Sender {3}", hash, signature, signer, sender);
  
            return new RecoverModel
            {
                Signer = signer.ToLowerInvariant(),
                Hash = hash,
                Valid = string.Equals(signer, sender, StringComparison.OrdinalIgnoreCase)
            };
        }

        public string Sign(Model model, string privateKey ) {
            var signer = new EthereumMessageSigner();
            var bytes = EncodedPacked(model).HexToByteArray();
            return signer.HashAndSign(bytes, new EthECKey(privateKey) );

        }
 

    }
}