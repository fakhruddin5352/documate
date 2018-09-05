using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Documate.Storage {
    class LocalStorageService : IStorageService {
        private readonly string path;
        private readonly ILogger logger;
        public LocalStorageService (string path, ILogger<LocalStorageService> logger) {
            this.path = path;
            this.logger = logger;
        }

        public Task<Stream> Load (string hash) {
            string fullPath = string.Empty;

            fullPath = Path.Combine (path, hash);
            if (!File.Exists (fullPath)) {
                logger.LogError ("File {0} does not exists for hash {1}", fullPath, hash);
                throw new FileNotFoundException($"File does not exists for hash {hash}");
            }

            return Task.FromResult<Stream>(new FileStream (fullPath, FileMode.Open, FileAccess.Read, FileShare.Read));
        }

        public async Task<bool> Store (string hash, Stream stream) {
            string fullPath = string.Empty;
            try {
                fullPath = Path.Combine (path, hash);
                if (File.Exists (fullPath)) {
                    logger.LogError ("File {0} already  exists for hash {1}", fullPath, hash);
                    return true;
                }
                using (FileStream fileStream = new FileStream (fullPath, FileMode.CreateNew, FileAccess.Write, FileShare.None)) {
                    await stream.CopyToAsync (fileStream);
                }
            } catch (Exception ex) {
                logger.LogError (ex, "Failed loading file {0} for hash {1}", fullPath, hash);
                return false;
            }
            return true;
        }

    }
}