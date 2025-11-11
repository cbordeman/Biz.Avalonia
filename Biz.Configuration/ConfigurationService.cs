using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace Biz.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        const string NameOfConfigurationAssembly = "Biz.Configuration";
        
        readonly AuthSettings auth;
        readonly MapsSettings maps;
        readonly ServerSettings server;

        public ConfigurationService()
        {
            var builder = new ConfigurationBuilder();
            
            // Load base appsettings.json
            var assembly = Assembly.GetAssembly(typeof(IConfigurationService));
            using var appsettingsStream = assembly!.GetManifestResourceStream($"{NameOfConfigurationAssembly}.appsettings.json");
            if (appsettingsStream != null)
                builder.AddJsonStream(appsettingsStream);

            // Load environment-specific settings
#if DEBUG
            using var devStream = assembly.GetManifestResourceStream($"{NameOfConfigurationAssembly}.appsettings.Development.json");
            if (devStream != null)
            {
                builder.AddJsonStream(devStream);
            }
#endif

            IConfiguration config = builder.Build();

            // Bind configuration sections to strongly typed objects
            auth = new AuthSettings();
            config.GetSection(nameof(Authentication)).Bind(auth);
            
            maps = new MapsSettings();
            config.GetSection(nameof(Maps)).Bind(maps);

            server = new ServerSettings();
            config.GetSection(nameof(Server)).Bind(server);
        }

        public AuthSettings Authentication => auth;
        public MapsSettings Maps => maps;
        public ServerSettings Server => server;
    }
} 