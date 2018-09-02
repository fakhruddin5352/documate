namespace Documate.Storage {
    public class LocalStorageOptionsBuilder {
        public string Path { get; private set; }

        public LocalStorageOptionsBuilder UsePath (string path) {
            Path = path;
            return new LocalStorageOptionsBuilder{Path = path};
        }

    }
}