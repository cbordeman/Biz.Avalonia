using Data.Config;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Scalar.AspNetCore;
using Services.Auth;
using Services.Auth.Jwt;
using Services.Config;

namespace Services;

class Program
{
    public static void Main(string[] args)
    {
        try
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register configuration objects.
            builder.Services.AddSingleton<DatabaseSettings>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var dbSettings = new DatabaseSettings();
                config.GetSection("Database").Bind(dbSettings);
                return dbSettings;
            });
            builder.Services.AddSingleton<JwtIssuerSettings>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var jwtIssuerSettings = new JwtIssuerSettings();
                config.GetSection("JwtIssuer").Bind(jwtIssuerSettings);
                return jwtIssuerSettings;
            });
            builder.Services.AddSingleton<AzureSettings>(provider =>
            {
                var config = provider.GetRequiredService<IConfiguration>();
                var azureSettings = new AzureSettings();
                config.GetSection("Azure").Bind(azureSettings);
                return azureSettings;
            });

            builder.Services.AddSingleton<IJwtTokenIssuer, JwtTokenIssuer>();

            // Register DbContextFactory based on configuration
            builder.Services.AddSingleton<IDbContextFactory<AppDbContext>>(provider =>
            {
                var dbSettings = provider.GetRequiredService<DatabaseSettings>();

                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                optionsBuilder.ApplyDatabaseProvider(dbSettings)
                    // Must apply this here instead of on AppDbContext for some reason,
                    // otherwise it will not work with the PooledDbContextFactory and
                    // will throw an exception.
                    .UseSnakeCaseNamingConvention();

                return new PooledDbContextFactory<AppDbContext>(optionsBuilder.Options);
            });

            // Register ExternalAuthSettings from configuration
            //builder.Services.Configure<ExternalAuthSettings>(
            //    builder.Configuration.GetSection("Authentication"));

            var externalAuthSettings = new ExternalAuthSettings();
            builder.Configuration.GetSection("Authentication").Bind(externalAuthSettings);

            builder.Services
                .ConfigureAuthentication(externalAuthSettings);

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddSignalR();

            builder.Logging.AddConsole();
            builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Information);
            builder.Logging.AddFilter("Microsoft.AspNetCore.Authorization", LogLevel.Information);
            // Wider ASP.Net logs.
            //builder.Logging.AddFilter("Microsoft.AspNetCore", LogLevel.Information);

            var app = builder.Build();

            try
            {
                // Apply EF migrations on startup
                using var scope = app.Services.CreateScope();

                var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<AppDbContext>>();
                using var dbContext = dbContextFactory.CreateDbContext();
                dbContext.Database.Migrate();
            }
// ReSharper disable once RedundantCatchClause
            catch (Exception)
            {
                throw;
            }

// Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                // provides /openapi/v1.json
                app.MapOpenApi();
                // provides /scalar/v1
                app.MapScalarApiReference();
            }

            app.UseMiddleware<ExceptionMiddleware>();

            //app.MapHub<NotificationHub>("/hubs/notification");

            //app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            
        }
    }
}