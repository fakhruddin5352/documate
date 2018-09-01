using Microsoft.EntityFrameworkCore;

namespace Documate.Data {
    public class ApplicationDbContext : DbContext {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base (options) { }

        public DbSet<Document> Documents{get;set;}
    }
}