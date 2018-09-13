using System;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Documate.ValueObjects {
    public class Hash : IDataType{
        private const int HASH_LENGTH = /*2 hex digits for each byte*/32*2;
        private const int HASH_LENGTH_WITH_PREFIX =/*for 0x*/ 2 + HASH_LENGTH;
        private readonly string value;

        public Hash (string value) {
            this.value = value;
        }

        public static implicit operator string (Hash hash) {
            return hash.value;
        }
        // override object.Equals
        public override bool Equals (object obj) {

            if (obj == null || GetType () != obj.GetType ()) {
                return false;
            }

            return string.Equals ((Hash) obj, value, StringComparison.OrdinalIgnoreCase);
        }

        // override object.GetHashCode
        public override int GetHashCode () {
            return value.GetHashCode();
        }

        public string ToHexWithoutPrefix(){
            return value.RemoveHexPrefix();
        }
        public byte[] GetBytes(){
            return value.HexToByteArray();
        }
        public static Hash Of (string value) {
            if (value == null) 
            {
                throw new ArgumentNullException(nameof(value));
            }
            string prefixed  = value.EnsureHexPrefix();
            if (prefixed.Length != HASH_LENGTH_WITH_PREFIX)
                throw new InvalidHashException();


            return new Hash (prefixed);
        }
    }
}