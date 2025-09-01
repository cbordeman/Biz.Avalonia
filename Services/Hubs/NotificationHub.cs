using JetBrains.Annotations;
using Microsoft.AspNetCore.SignalR;

namespace Services.Hubs;

public class NotificationHub : Hub
{
    [UsedImplicitly]
    public async Task OpenResetPasswordUi(string userId, string token)
    {
        await Clients.User(userId).SendAsync(nameof(OpenResetPasswordUi));
    }
}