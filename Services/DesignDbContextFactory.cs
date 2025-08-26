using Microsoft.EntityFrameworkCore.Design;

namespace Services;

public class DesignDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        //// Build configuration
        //IConfiguration config = new ConfigurationBuilder()
        //    .SetBasePath(Directory.GetCurrentDirectory())
        //    .AddJsonFile("appsettings.Development.json", optional: false)
        //    .Build();

        //// Get connection string
        //var dbSettings = config.GetValue<DatabaseSettings>(nameof(DatabaseSettings));

        //// Configure DbContextOptions
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        //optionsBuilder.ApplyDatabaseProvider(dbSettings);

        // Return new context
        //return new AppDbContext(optionsBuilder.Options);
        optionsBuilder.UseNpgsql("Host=localhost;Database=Biz;Username=postgres;Password=1")
            .UseNpgsql("Host=localhost;Database=Biz;Username=postgres;Password=1")
            .UseSnakeCaseNamingConvention();
        return new AppDbContext(optionsBuilder.Options);
    }
}