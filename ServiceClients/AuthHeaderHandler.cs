using System.Net;
using System.Net.Http.Headers;
using Biz.Core.Models;
using JetBrains.Annotations;

namespace ServiceClients;

[UsedImplicitly]
public class ServicesAuthHeaderHandler(IAuthDataStore authTokenStore)
    : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var tokenAndProvider = await authTokenStore.GetTokenAndProvider();
        
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