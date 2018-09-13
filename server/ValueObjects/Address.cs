using System;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Documate.ValueObjects {
    public class Address : IDataType {
        private const int ADDRESS_LENGTH = /*2 hex digits for each byte*/ 20 * 2;
        private const int ADDRESS_LENGTH_WITH_PREFIX = /*for 0x*/ 2 + ADDRESS_LENGTH;
        private readonly string value;

        public Address (string value) {
            this.value = value;
        }

        // override object.Equals
        public override bool Equals (object obj) {

            if (obj == null || GetType () != obj.GetType ()) {
                return false;
            }

            return string.Equals ((Address) obj, value, System.StringComparison.OrdinalIgnoreCase);
        }

        // override object.GetHashCode
        public override int GetHashCode () {
            return value.GetHashCode ();
        }

        public string ToHexWithoutPrefix () {
            return value.RemoveHexPrefix ();
        }
        public byte[] GetBytes () {
            return value.HexToByteArray ();
        }

        public static Address Of (string value) {
            if (value == null) {
                throw new ArgumentNullException (nameof (value));
            }
            string prefixed = value.EnsureHexPrefix ();
            if (prefixed.Length != ADDRESS_LENGTH_WITH_PREFIX)
                throw new InvalidAddressException ();

            return new Address (prefixed);
        }
        public static implicit operator string (Address address) {
            return address.value;
        }
        public static bool operator == (Address address1, Address address2){
            if (ReferenceEquals(address1, address2))
                return true;
    
            return string.Equals (address1, address2, System.StringComparison.OrdinalIgnoreCase);
        }
        public static bool operator != (Address address1, Address address2){
            return !(address1 == address2);
        }

    }

}