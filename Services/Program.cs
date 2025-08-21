using Data.Config;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Scalar.AspNetCore;
using Services.Auth;
using Services.Config;

var builder = WebApplication.CreateBuilder(args);

// Register DbContextFactory based on configuration
builder.Services.AddSingleton<IDbContextFactory<AppDbContext>>(provider =>
{
    IConfiguration config = provider.GetRequiredService<IConfiguration>();
    var dbSettings = new DatabaseSettings();
    config.GetSection("Database").Bind(dbSettings);

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

builder.Services.ConfigureAuthentication(externalAuthSettings);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Logging.AddConsole(); // Already present in most templates
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authorization", LogLevel.Debug);
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
#pragma warning disable CS0168 // Variable is declared but never used
catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
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

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();