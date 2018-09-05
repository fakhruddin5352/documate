namespace Documate.Storage {
    public class LocalStorageOptions {
        public LocalStorageOptions (string path) {
            this.Path = path;

        }
        public string Path { get; private set; }

    }
}