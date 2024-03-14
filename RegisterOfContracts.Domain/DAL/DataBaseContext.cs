using ContractParser.Domain.Entity;
using Microsoft.EntityFrameworkCore;
using RegisterOfContracts.Domain.Entity;

namespace RegisterOfContracts.Domain.DAL
{
    public class DataBaseContext : DbContext
    {
        public DbSet<Entity.Contract> Contracts { get; set; }
        public DbSet<Attachment> FileAttachments { get; set; }
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contract>()
                .HasMany(c => c.Attachments)
                .WithOne(a => a.Contract)
                .HasForeignKey(a => a.contractId);
        }
    }
}
