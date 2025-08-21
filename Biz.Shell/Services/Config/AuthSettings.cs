namespace Biz.Services.Config
{
    public class AuthSettings
    {
        public GoogleAuth Google { get; set; } = new();
        public MicrosoftAuth Microsoft { get; set; } = new();
        public FacebookAuth Facebook { get; set; } = new();
        public AppleAuth Apple { get; set; } = new();
    }

    public class GoogleAuth
    {
        public string ClientId { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string[] Scopes { get; set; } = Array.Empty<string>();
    }

    public class MicrosoftAuth
    {
        public string TenantId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string MobileRedirectUri { get; set; } = string.Empty;
        public string DesktopRedirectUri { get; set; }
        public string[] Scopes { get; set; } = Array.Empty<string>();
    }

    public class FacebookAuth
    {
        public string ClientId { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string[] Scopes { get; set; } = Array.Empty<string>();
    }

    public class AppleAuth
    {
        public string ClientId { get; set; } = string.Empty;
        public string RedirectUri { get; set; } = string.Empty;
        public string[] Scopes { get; set; } = Array.Empty<string>();
    }

    public class MapsSettings
    {
        public string BingMapsApiKey { get; set; } = string.Empty;
    }
} 