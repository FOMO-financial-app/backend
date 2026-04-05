using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Fomo.Infrastructure.Persistence;

public class EFCoreDbContextFactory : IDesignTimeDbContextFactory<EFCoreDbContext>
{
    public EFCoreDbContext CreateDbContext(string[] args)
    {
        var connectionString = "";

        var builder = new DbContextOptionsBuilder<EFCoreDbContext>();
        builder.UseNpgsql(connectionString);

        return new EFCoreDbContext(builder.Options);
    }
}