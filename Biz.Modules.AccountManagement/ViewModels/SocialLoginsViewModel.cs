using Biz.Models;
using Biz.Modules.AccountManagement.Core;
using Biz.Shell.Services;
using Biz.Shell.Services.Authentication;
using DryIoc;
using JetBrains.Annotations;
using Prism.Commands;
using Prism.Navigation;

namespace Biz.Modules.AccountManagement.ViewModels;

[UsedImplicitly]
public class SocialLoginsViewModel(IContainer container) : NavigationAwareViewModelBase(container)
{
    readonly IPlatformDialogService dialogService = container.Resolve<IPlatformDialogService>();
    readonly IAuthenticationService authenticationService = container.Resolve<IAuthenticationService>();

    public CancellationTokenSource? LoginCancellationTokenSource { get; private set; }

    #region LoginWithGoogleCommand
    public AsyncDelegateCommand? LoginWithGoogleCommand => field ??= new AsyncDelegateCommand(ExecuteLoginWithGoogleCommand, CanLoginWithGoogleCommand);
    bool CanLoginWithGoogleCommand() => true;
    async Task ExecuteLoginWithGoogleCommand()
    {
        try
        {
            IsBusy = true;
            LoginCancellationTokenSource = new();
            var result = await authenticationService.LoginWithGoogleAsync(
                LoginCancellationTokenSource.Token);
            if (!result.isLoggedIn)
            {
                if (result.availableTenants == null)
                {
                    await dialogService.Confirm("Cannot Login",
                        "Error logging in.  Please try again.",
                        cancelText: null);
                    IsBusy = false;
                    return;
                }
                else if (result.availableTenants.Length == 0)
                {
                    await dialogService.Confirm("Account Inactive",
                        "Your account is inactive in all organizations.",
                        cancelText: null);
                    IsBusy = false;
                    return;
                }
                await GoToTenantSelectionPage(result.availableTenants);
            }
        }
        catch (OperationCanceledException)
        {
            IsBusy = false;
        }
        catch (Exception ex)
        {
            await dialogService.Confirm(
                "Error",
                $"An error occurred during Google login: {ex.Message}",
                cancelText: null);
            IsBusy = false;
        }
        finally
        {
            IsBusy = false;
            LoginCancellationTokenSource = null;
        }
    }
    #endregion LoginWithGoogleCommand

    #region LoginWithMicrosoftCommand
    public AsyncDelegateCommand? LoginWithMicrosoftCommand => field ??= new AsyncDelegateCommand(ExecuteLoginWithMicrosoftCommand);
    async Task ExecuteLoginWithMicrosoftCommand()
    {
        try
        {
            IsBusy = true;
            LoginCancellationTokenSource = new();
            var result = await authenticationService.LoginWithMicrosoftAsync(
                LoginCancellationTokenSource.Token);
            if (!result.isLoggedIn)
            {
                if (result.availableTenants == null || result.error != null)
                {
                    await dialogService.Confirm("Cannot Login",
                        "Error logging in.  Please try again." +
                        (result.error == null ? "" : $"\n\nError: {result.error}"),
                        cancelText: null);
                    return;
                }
                else if (result.availableTenants.Length == 0)
                {
                    await dialogService.Confirm("Account Inactive",
                        "Your account is inactive in all organizations.",
                        cancelText: null);
                    return;
                }
                await GoToTenantSelectionPage(result.availableTenants);
            }
        }
        catch (OperationCanceledException)
        {
            // Eat
        }
        catch (Exception ex)
        {
            await dialogService.Confirm(
                "Error",
                $"An error occurred during Microsoft login: {ex.Message}",
                cancelText: null);
        }
        finally
        {
            IsBusy = false;
            LoginCancellationTokenSource = null;
        }
        
    }
    #endregion LoginWithMicrosoftCommand

    #region LoginWithFacebookCommand
    public AsyncDelegateCommand? LoginWithFacebookCommand =>
        field ??= new AsyncDelegateCommand(ExecuteLoginWithFacebookCommand, CanLoginWithFacebookCommand);
    bool CanLoginWithFacebookCommand() => true;
    async Task ExecuteLoginWithFacebookCommand()
    {
        try
        {
            IsBusy = true;
            LoginCancellationTokenSource = new();
            var result = await authenticationService.LoginWithFacebookAsync(
                LoginCancellationTokenSource.Token);
            if (!result.isLoggedIn)
            {
                if (result.availableTenants == null)
                {
                    await dialogService.Confirm("Cannot Login",
                        "Error logging in.  Please try again.",
                        cancelText: null);
                    IsBusy = false;
                    return;
                }
                else if (result.availableTenants.Length == 0)
                {
                    await dialogService.Confirm("Account Inactive",
                        "Your account is inactive in all organizations.",
                        cancelText: null);
                    IsBusy = false;
                    return;
                }
                await GoToTenantSelectionPage(result.availableTenants);
                IsBusy = false;
            }
            else
                IsBusy = false;
        }
        catch (OperationCanceledException)
        {
            IsBusy = false;
        }
        catch (Exception ex)
        {
            await dialogService.Confirm("Error", $"An error occurred during Facebook login: {ex.Message}",
                cancelText: null);
            IsBusy = false;
        }
    }
    #endregion LoginWithFacebookCommand

    #region LoginWithAppleCommand
    public AsyncDelegateCommand? LoginWithAppleCommand => field ??= new AsyncDelegateCommand(ExecuteLoginWithAppleCommand, CanLoginWithAppleCommand);
    static bool CanLoginWithAppleCommand() => true;
    async Task ExecuteLoginWithAppleCommand()
    {
        try
        {
            IsBusy = true;
            LoginCancellationTokenSource = new();
            var result = await authenticationService.LoginWithAppleAsync(
                LoginCancellationTokenSource.Token);
            if (!result.isLoggedIn)
            {
                if (result.availableTenants == null)
                {
                    await dialogService.Confirm("Cannot Login",
                        "Error logging in.  Please try again.",
                        cancelText: null);
                    IsBusy = false;
                    return;
                }
                else if (result.availableTenants.Length == 0)
                {
                    await dialogService.Confirm("Account Inactive",
                        "Your account is inactive in all organizations.",
                        cancelText: null);
                    IsBusy = false;
                    return;
                }
                await GoToTenantSelectionPage(result.availableTenants);
            }
            IsBusy = false;
        }
        catch (OperationCanceledException)
        {
            IsBusy = false;
        }
        catch (Exception ex)
        {
            await dialogService.Confirm("Error", $"An error occurred during Apple login: {ex.Message}",
                cancelText: null);
            IsBusy = false;
        }
    }
    #endregion LoginWithAppleCommand

    async Task GoToTenantSelectionPage(Tenant[] availableTenants)
    {
        ArgumentNullException.ThrowIfNull(availableTenants);
        
        await NavigationService!.NavigateAsync(
            AccountManagementConstants.ModuleName,
            AccountManagementConstants.TenantSelectionView,
            new NavigationParameters
            { { nameof(TenantSelectionViewModel.AvailableTenants), availableTenants! } });
    }
}