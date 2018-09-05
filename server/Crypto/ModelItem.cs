namespace Documate.Crypto {
    public class ModelItem {
        public object Value { get; set; }
        public DataType Type { get; set; }

        public static ModelItem From (DataType type, object value) {
            return new ModelItem { Type = type, Value = value };
        }

        public override string ToString(){
            return $"{Type.ToString()} - {Value?.ToString()}";

        }

    }
}