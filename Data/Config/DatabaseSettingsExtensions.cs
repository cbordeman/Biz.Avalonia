using Microsoft.EntityFrameworkCore;

namespace Data.Config;

public static class DatabaseSettingsExtensions
{
    public static DbContextOptionsBuilder<AppDbContext> ApplyDatabaseProvider(
        this DbContextOptionsBuilder<AppDbContext> builder, 
        DatabaseSettings settings)
    {
        var provider = GetProvider(settings);
        switch (provider)
        {
            case DatabaseProvider.SqlServer:
                builder.UseSqlServer(settings.MsSql);
                break;
            case DatabaseProvider.PostgreSql:
                builder.UseNpgsql(settings.PostgreSQL);
                break;
            default:
                throw new InvalidOperationException($"Unsupported database backend: {settings.Backend}");
        }
        return builder;
    }

    public static DatabaseProvider GetProvider(this DatabaseSettings settings)
    {
        return settings.Backend?.ToUpperInvariant() switch
        {
            "MSSQL" => DatabaseProvider.SqlServer,
            "POSTGRESQL" => DatabaseProvider.PostgreSql,
            _ => throw new InvalidOperationException($"Unsupported database backend: {settings.Backend}")
        };
    }

    public enum DatabaseProvider
    {
        SqlServer,
        PostgreSql,
    }
}