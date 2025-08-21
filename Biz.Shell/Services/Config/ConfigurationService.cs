using Biz.Services.Config;
using Microsoft.Extensions.Configuration;

namespace Biz.Shell.Services.Config
{
    public interface IConfigurationService
    {
        AuthSettings Authentication { get; }
        MapsSettings Maps { get; }
        ServerSettings Server { get; }
    }

    public class ConfigurationService : IConfigurationService
    {
        readonly AuthSettings auth;
        readonly MapsSettings maps;
        readonly ServerSettings server;

        public ConfigurationService()
        {
            var builder = new ConfigurationBuilder();
            
            // Load base appsettings.json
            var assembly = typeof(ConfigurationService).Assembly;
            using var appsettingsStream = assembly.GetManifestResourceStream("Biz.appsettings.json");
            if (appsettingsStream != null)
            {
                builder.AddJsonStream(appsettingsStream);
            }

            // Load environment-specific settings
#if DEBUG
            using var devStream = assembly.GetManifestResourceStream("Biz.appsettings.Development.json");
            if (devStream != null)
            {
                builder.AddJsonStream(devStream);
            }
#endif

            IConfiguration configuration1 = builder.Build();

            // Bind configuration sections to strongly typed objects
            auth = new AuthSettings();
            configuration1.GetSection("Authentication").Bind(auth);
            
            maps = new MapsSettings();
            configuration1.GetSection("Maps").Bind(maps);

            server = new ServerSettings();
            configuration1.GetSection("Server").Bind(server);
        }

        public AuthSettings Authentication => auth;
        public MapsSettings Maps => maps;
        public ServerSettings Server => server;
    }
} 