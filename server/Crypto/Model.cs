using System.Collections.Generic;
using Documate.ValueObjects;

namespace Documate.Crypto {
    public class Model {
        public IEnumerable<IDataType> Items { get; set; }

        public static Model FromItems (params IDataType[] items) {
            return new Model { Items = items };
        }
    }
}