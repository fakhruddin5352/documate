using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Documate.ValueObjects;
using Microsoft.Extensions.Logging;
using Nethereum.ABI;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Web3;

namespace Documate.Crypto {
    class CryptoService : ICryptoService {

        private readonly ILogger logger;
        private readonly string privateKey;
        private readonly IEnumerable<IDataType> additionalSignatureData;

        public CryptoService (ILogger<CryptoService> logger, string privateKey, params IDataType[] additionalSignatureData) {
            this.privateKey = privateKey;
            this.additionalSignatureData = additionalSignatureData.ToList ();
            this.logger = logger;
        }
        public CryptoService (ILogger<CryptoService> logger) {
            this.logger = logger;
        }
        public RecoverModel EcRecover (Address sender, Signature signature, byte[] data) {
            var hash = Hash.Of(Web3.Sha3 (System.Convert.ToBase64String (data)));
            return EcRecover (sender, signature, hash);

        }

        public RecoverModel EcRecover (Address sender, Signature signature, Hash hash) {
            var ethereumSigner = new EthereumMessageSigner ();
            var signer = Address.Of(ethereumSigner.EcRecover (hash.GetBytes(), signature));
            logger.LogDebug ("Hash {0} , Signature {1}, Signer {2}, Sender {3}", hash, signature, signer, sender);

            return new RecoverModel {
                Signer = signer,
                    Hash = hash,
                    Valid = signer == sender
            };
        }

        public Signature Sign (Model model) {
            var signer = new EthereumMessageSigner ();
            var hash = SoliditySHA3 (model);

            if (logger.IsEnabled (LogLevel.Debug)) {
                logger.LogDebug ("signing model {0} , {1} bytes", model, hash.ToHex ().EnsureHexPrefix ());
            }

            return Signature.Of(signer.HashAndSign (hash, new EthECKey (privateKey)));

        }

        private string Keccak256 (byte[] data) {
            var signer = new EthereumMessageSigner ();
            return signer.Hash (data).ToHex (true);
        }

        private byte[] SoliditySHA3 (Model model) {
            Nethereum.ABI.AddressType a;
            using (var stream = new MemoryStream ()) {
                foreach (var item in model.Items.Concat (additionalSignatureData)) {
                    stream.Write (item.GetBytes());
                }
                var packed = stream.ToArray ();
                if (logger.IsEnabled (LogLevel.Debug))
                    logger.LogDebug ("Packed model for signing {0}", packed.ToHex ().EnsureHexPrefix ());
                return packed;
            }
        }


    }
} 