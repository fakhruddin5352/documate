using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using Nethereum.ABI;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Web3;

namespace Documate.Crypto {
    class CryptoService : ICryptoService {

        private readonly ILogger logger;
        private readonly string privateKey;
        private readonly IEnumerable<ModelItem> additionalSignatureData;

        public CryptoService(ILogger<CryptoService> logger,string privateKey, params ModelItem[] additionalSignatureData){
            this.privateKey = privateKey;
            this.additionalSignatureData = additionalSignatureData.ToList();
            this.logger = logger;
        }
        public CryptoService(ILogger<CryptoService> logger){
            this.logger = logger;
        }
        private string Keccak256 (byte[] data) {
            var signer = new EthereumMessageSigner();
            return signer.Hash(data).ToHex(true);
        }

        private byte[] SoliditySHA3 (Model model) {
            Nethereum.ABI.AddressType a;
            using (var stream = new MemoryStream()){
                foreach (var item in model.Items.Concat(additionalSignatureData)) {
                    byte[] data;
                    if (item.Type == DataType.address){
                        data = item.Value.ToString().HexToByteArray();
                    }else{
                        var abiType = ABIType.CreateABIType(item.Type.ToString());
                        data = abiType.Encode(item.Value);
                        logger.LogDebug("Encoding {0} to {1} with {2}", item, data.ToHex(), abiType.FixedSize);
                    }
                    stream.Write(data);
                }
                var packed = stream.ToArray();
                if (logger.IsEnabled(LogLevel.Debug))
                    logger.LogDebug("Packed model for signing {0}", packed.ToHex().EnsureHexPrefix());
                return packed;
            }
        }

        private Model ToModel (IEnumerable<object> values, IList<DataType> types) {
            return new Model {
                Items = values.Select((v,i) => new ModelItem{Value=v, Type = types[i]})
            };
        }

        public RecoverModel EcRecover(string sender, string signature, byte[] data) {
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

        public string Sign(Model model ) {
            var signer = new EthereumMessageSigner();
            var hash = SoliditySHA3(model);
    
            if (logger.IsEnabled(LogLevel.Debug)){
                logger.LogDebug("signing model {0} , {1} bytes", model, hash.ToHex().EnsureHexPrefix());
            }

            return signer.HashAndSign(hash, new EthECKey(privateKey) );

        }
 

    }
}