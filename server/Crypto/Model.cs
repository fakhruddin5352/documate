using System.Collections.Generic;

namespace Documate.Crypto {
    public class Model {
        public IEnumerable<ModelItem> Items { get; set; }

        public static Model FromItems (params ModelItem[] items) {
            return new Model { Items = items };
        }
    }
}