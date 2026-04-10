using Fomo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fomo.Infrastructure.Persistence
{
    public class EFCoreDbContext : DbContext
    {
        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.TradeResults)
                .WithOne(tr => tr.User)
                .HasForeignKey(tr => tr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Auth0Id)
                .IsUnique();

            modelBuilder.Entity<TradeResult>()
                .HasOne(tr => tr.TradeMethod)
                .WithOne(tm => tm.TradeResult)
                .HasForeignKey<TradeMethod>(tm => tm.TradeResultId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TradeResult>()
                .ToTable(t =>
                {
                    t.HasCheckConstraint("CK_Product_EntryPrice_NonNegative", "\"EntryPrice\" >= 0");
                    t.HasCheckConstraint("CK_Product_ExitPrice_NonNegative", "\"ExitPrice\" >= 0");
                    t.HasCheckConstraint("CK_Product_NumberOfStocks_NonNegative", "\"NumberOfStocks\" >= 0");
                });

            modelBuilder.Entity<TradeResult>()
                .Property(tr => tr.EntryPrice)
                .HasPrecision(10, 4);
            
            modelBuilder.Entity<TradeResult>()
                .Property(tr => tr.ExitPrice)
                .HasPrecision(10, 4);            
            
            modelBuilder.Entity<Stock>()
                .HasIndex(s => s.Symbol);

            modelBuilder.Entity<Stock>()
                .HasIndex(s => s.Name);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<TradeResult> TradeResults { get; set; }
        public DbSet<TradeMethod> TradeMethods { get; set; }
        public DbSet<Stock> Stocks { get; set; }
    }
}