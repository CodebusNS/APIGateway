using APIGateway.Core.DbRepository.Entities;
using Microsoft.EntityFrameworkCore;

namespace APIGateway.Core.DbRepository
{
    public class GatewayDbContext : DbContext
    {
        public GatewayDbContext(DbContextOptions<GatewayDbContext> options) : base(options)
        {
        }

        public DbSet<GlobalConfiguration> GlobalConfiguration { get; set; }

        public DbSet<ReRoute> ReRoute { get; set; }

        public DbSet<Aggregates> Aggregates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GlobalConfiguration>().HasMany(x => x.ReRoutes).WithOne(x => x.GlobalConfiguration).HasForeignKey(x => x.GlobalConfigurationId);
        }
    }
}
