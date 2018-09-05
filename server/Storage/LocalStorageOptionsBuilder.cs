namespace Documate.Storage {
    public class LocalStorageOptionsBuilder {
        public string Path { get; private set; }

        public LocalStorageOptionsBuilder UsePath (string path) {
            Path = path;
            return this;
        }

        internal LocalStorageOptions Build() {
            return new LocalStorageOptions(Path);
        }

    }
}