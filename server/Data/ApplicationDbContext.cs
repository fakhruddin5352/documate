using Documate.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Documate.Data {
    class ApplicationDbContext : DbContext {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base (options) { }

        public DbSet<Document> Documents{get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<Document>().HasIndex( index=>index.When);
            modelBuilder.Entity<Document>().Property("hash").HasColumnName("Hash");
            modelBuilder.Entity<Document>().HasIndex("hash");
        }
    }
}