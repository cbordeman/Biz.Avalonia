using Biz.Modules.AccountManagement.Core;
using Biz.Shared.Platform;
using Biz.Shared.Services;
using CompositeFramework.Avalonia.Dialogs;
using ServiceClients;

namespace Biz.Shell.Desktop.Services;

public class DesktopPlatformAppCustomUriHandler(
    IMainNavigationService navService,
    IAccountApi accountApi,
    IDialogService dialogService)
    : PlatformAppCustomUriHandlerBase()
{
    readonly IAccountApi accountApi = accountApi;
    readonly IDialogService dialogService = dialogService;
    
    protected override async Task HandleConfirmUserRegistration(
        string token, string email)
    {
        // for security reasons, the WebAPI doesn't confirm
        // whether the token was good or the action was successful.
        // But we still tell the user it was.
        await accountApi.ConfirmRegisteredEmail(email, token);
        await dialogService.Confirm("Account Activated",
            "Your email has been confirmed.  You may log in.");
        await navService.NavigateWithModuleAsync(
            AccountManagementConstants.ModuleName,
            AccountManagementConstants.LoginView);
    }

    protected override Task HandleConfirmForgotPassword(
        string token, string email)
    {
        // // Opens the page that hits the service and 
        // // confirms to the user the account is activated.
        // navService.RequestNavigate(
        //     AccountManagementConstants.ModuleName,
        //     AccountManagementConstants.ResetPasswordAfterEmailConfirmationView,
        //     new NavigationParameters
        //     {
        //         {
        //             nameof(ResetPasswordAfterEmailConfirmationViewModel.Token), 
        //             token!
        //         },
        //         {
        //             nameof(ResetPasswordAfterEmailConfirmationViewModel.Email),
        //             email
        //         }
        //     });
        return Task.CompletedTask;
    }
}