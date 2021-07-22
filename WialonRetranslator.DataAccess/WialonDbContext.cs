using Microsoft.EntityFrameworkCore;
using WialonRetranslator.DataAccess.Configurations;
using WialonRetranslator.DataAccess.Entities;

namespace WialonRetranslator.DataAccess
{
    public class WialonDbContext: DbContext
    {
        public DbSet<Point> Points { get; set; }
        
        public WialonDbContext(DbContextOptions<WialonDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PointConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}