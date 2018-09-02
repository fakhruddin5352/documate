using System;
using System.Threading.Tasks;
using Documate.Data;
using Nethereum.Hex.HexConvertors.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Documate.Data {
    class DataService : IDataService {
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger logger;

        public DataService (ApplicationDbContext dbContext,ILogger<DataService> logger) {
            this.dbContext = dbContext;
            this.logger = logger;
        }
        public Task<Document> Load (string hash) 
        {
            return dbContext.Documents.SingleOrDefaultAsync(x=>x.Hash == hash);
        }

        public async Task Save (Document document) {
            document.When = DateTime.Now;
            document.Hash = document.Hash.RemoveHexPrefix();
            document.Owner =  document.Owner.RemoveHexPrefix();
            logger.LogDebug("Saving document Hash {0} - Owner {1} ",document.Hash,document.Owner);
            await dbContext.Documents.AddAsync (document);
            await dbContext.SaveChangesAsync ();
        }
    }
}