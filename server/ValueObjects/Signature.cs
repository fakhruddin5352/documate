using System;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Documate.ValueObjects {
    public class Signature {
        private const int SIGNATURE_LENGTH = /*2 hex digits for each byte*/65*2;
        private const int SIGNATURE_LENGTH_WITH_PREFIX =/*for 0x*/ 2 + SIGNATURE_LENGTH;
        private readonly string value;

        public Signature (string value) {
            this.value = value;
        }

        public static implicit operator string (Signature signature) {
            return signature.value;
        }
        // override object.Equals
        public override bool Equals (object obj) {

            if (obj == null || GetType () != obj.GetType ()) {
                return false;
            }

            return string.Equals ((Signature) obj, value, StringComparison.OrdinalIgnoreCase);
        }

        // override object.GetHashCode
        public override int GetHashCode () {
            return value.GetHashCode();
        }
        public static Signature Of (string value) {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            string prefixed  = value.EnsureHexPrefix().ToLowerInvariant();
            if (prefixed.Length != SIGNATURE_LENGTH_WITH_PREFIX)
                throw new InvalidSignatureException();


            return new Signature (prefixed);
        }
    }
}