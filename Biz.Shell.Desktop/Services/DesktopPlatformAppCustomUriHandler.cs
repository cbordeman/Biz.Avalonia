using System;
using System.Threading.Tasks;
using Biz.Modules.AccountManagement.Core;
using Biz.Shell.Infrastructure;
using Biz.Shell.Platform;
using Biz.Shell.Services;
using Microsoft.Extensions.Logging;
using Prism.Ioc;
using Prism.Navigation;
using Prism.Navigation.Regions;
using ServiceClients;

namespace Biz.Shell.Desktop.Services;

public class DesktopPlatformAppCustomUriHandler(
    ILogger<DesktopPlatformAppCustomUriHandler> logger,
    IMainRegionNavigationService navService,
    IAccountApi accountApi)
    : PlatformAppCustomUriHandlerBase(logger)
{
    readonly ILogger<DesktopPlatformAppCustomUriHandler> logger = logger;
    readonly IMainRegionNavigationService navService = navService;
    readonly IAccountApi accountApi = accountApi;

    protected override async Task HandleConfirmUserRegistration(
        string token, string email)
    {
        await accountApi.ConfirmRegisteredEmail(email, token);
    }

    protected override Task HandleConfirmForgotPassword(
        string token, string email)
    {
        // Opens the page that hits the service and 
        // confirms to the user the account is activated.
        navService.RequestNavigate(
            AccountManagementConstants.ModuleName,
            AccountManagementConstants.ResetPasswordAfterEmailConfirmationView,
            new NavigationParameters
            {
                {
                    nameof(ResetPasswordAfterEmailConfirmationViewModel.Token), 
                    token!
                },
                {
                    nameof(ResetPasswordAfterEmailConfirmationViewModel.Email),
                    email
                }
            });
    }
}