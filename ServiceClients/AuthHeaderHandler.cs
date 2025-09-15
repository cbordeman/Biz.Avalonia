using System.Net;
using System.Net.Http.Headers;
using Biz.Models;
using JetBrains.Annotations;

namespace ServiceClients;

[UsedImplicitly]
public class ServicesAuthHeaderHandler
    : DelegatingHandler
{
    private readonly IAuthDataStore authDataStore;
    public ServicesAuthHeaderHandler(IAuthDataStore authDataStore)
    {
        this.authDataStore = authDataStore;
        InnerHandler = new HttpClientHandler();
    }
    
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var tokenAndProvider = await authDataStore.GetTokenAndProvider();
        
        if (tokenAndProvider == null)
        {
            // No token available, return an unauthorized response.
            return new HttpResponseMessage(HttpStatusCode.Unauthorized)
            {
                ReasonPhrase = "No authentication token available.",
                RequestMessage = new HttpRequestMessage()
            };
        }
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenAndProvider.Token);
        request.Headers.Add(nameof(LoginProvider), tokenAndProvider.LoginProvider.ToString());

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}