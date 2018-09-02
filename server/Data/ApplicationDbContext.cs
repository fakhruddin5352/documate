using Microsoft.EntityFrameworkCore;

namespace Documate.Data {
    class ApplicationDbContext : DbContext {
        public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options) : base (options) { }

        public DbSet<Document> Documents{get;set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<Document>().HasIndex( index=>index.Hash) .IsUnique();
            modelBuilder.Entity<Document>().HasIndex( index=>index.Owner);
            modelBuilder.Entity<Document>().HasIndex( index=>index.When);
        }
    }
}