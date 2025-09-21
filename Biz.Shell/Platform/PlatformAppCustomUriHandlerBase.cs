namespace Biz.Shell.Platform;

public abstract class PlatformAppCustomUriHandlerBase(
    ILogger<PlatformAppCustomUriHandlerBase> logger)
{
    readonly ILogger<PlatformAppCustomUriHandlerBase> logger = logger;

    protected abstract Task HandleConfirmUserRegistration(
        string token, string email);
    protected abstract Task HandleConfirmForgotPassword(
        string token, string email);

    public async Task HandleUri(string uriString)
    {
        try
        {
            var uri = new Uri(uriString);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);

            string? token = query["token"];
            string? email = query["email"];

            // TODO: Open views
            switch (uri.Host)
            {
                case "confirm-user-registration":
                    await HandleConfirmUserRegistration(
                        token ?? throw new Exception("Missing token"), 
                        email ?? throw new Exception("Missing email"));
                    break;

                case "confirm-forgot-password":
                    await HandleConfirmForgotPassword(
                        token ?? throw new Exception("Missing token"), 
                        email ?? throw new Exception("Missing email"));
                    break;
                
                default:
                    throw new Exception($"Unknown URI: {uri.Host}");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Failed to handle URI {uriString}: {e.GetType().Name}: {e.Message}");
        }
    }
}