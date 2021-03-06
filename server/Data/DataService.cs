using System;
using System.Threading.Tasks;
using Documate.Data;
using Nethereum.Hex.HexConvertors.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Documate.ValueObjects;

namespace Documate.Data {
    class DataService : IDataService {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger logger;

        public DataService (ApplicationDbContext dbContext,ILogger<DataService> logger) {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        public async Task<Document> Load (Hash hash) 
        {
            string hash1=  hash.ToHexWithoutPrefix();
            var document =  await dbContext.Documents.SingleOrDefaultAsync(x=>  EF.Property<string>(x,"hash") == hash1);
            if (document != null) {
                document.Hash = document.Hash;
                document.Owner = document.Owner.EnsureHexPrefix();
            }
            return document;
        }

        public async Task Save (Document document) {
            document.When = DateTime.Now;
            document.Owner =  document.Owner.RemoveHexPrefix();
            logger.LogDebug("Saving document Hash {0} - Owner {1} ",document.Hash,document.Owner);
            await dbContext.Documents.AddAsync (document);
            await dbContext.SaveChangesAsync ();
        }
    }
} 