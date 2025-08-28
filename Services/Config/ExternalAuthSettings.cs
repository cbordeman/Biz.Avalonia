using JetBrains.Annotations;

namespace Services.Config
{
    public class ExternalAuthSettings
    {
        public ProviderSettings Local { get; set; } = null!;
        public ProviderSettings Google { get; set; } = null!;
        public ProviderSettings Microsoft { get; set; } = null!;
        public ProviderSettings Facebook { get; set; } = null!;
        public ProviderSettings Apple { get; set; } = null!;
    }

    [UsedImplicitly]
    public class ProviderSettings
    {
        public string Authority { get; set; } = null!;

        // ClientId
        public string Audience { get; set; } = null!;
        
        // Signing Key for Local only
        public string? SigningKey { get; set; }
    }
} 