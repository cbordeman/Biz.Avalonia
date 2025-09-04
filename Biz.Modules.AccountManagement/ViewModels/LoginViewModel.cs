using Biz.Core;
using Biz.Models;
using Biz.Modules.AccountManagement.Core;
using Biz.Shell.Services;
using Biz.Shell.Services.Authentication;
using DryIoc;
using Prism.Commands;
using Prism.Navigation;
using Prism.Navigation.Regions;

namespace Biz.Modules.AccountManagement.ViewModels;

public class LoginViewModel : PageViewModelBase
{
    CancellationTokenSource? loginCancellationTokenSource;
    readonly IPlatformDialogService dialogService;
    readonly IAuthenticationService authenticationService;

    public LoginViewModel(IContainer container)
        : base(container)
    {
        dialogService = container.Resolve<IPlatformDialogService>();
        authenticationService = container.Resolve<IAuthenticationService>();
        Title = $"Sign In to {AppConstants.AppShortName}";
    }

    #region LoginWithGoogleCommand
    public AsyncDelegateCommand? LoginWithGoogleCommand => field ??= new AsyncDelegateCommand(ExecuteLoginWithGoogleCommand, CanLoginWithGoogleCommand);
    bool CanLoginWithGoogleCommand() => true;
    async Task ExecuteLoginWithGoogleCommand()
    {
        try
        {
            IsBusy = true;
            loginCancellationTokenSource = new();
            var result = await authenticationService.LoginWithGoogleAsync(
                loginCancellationTokenSource.Token);
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
            loginCancellationTokenSource = null;
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
            loginCancellationTokenSource = new();
            var result = await authenticationService.LoginWithMicrosoftAsync(
                loginCancellationTokenSource.Token);
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
            loginCancellationTokenSource = null;
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
            loginCancellationTokenSource = new();
            var result = await authenticationService.LoginWithFacebookAsync(
                loginCancellationTokenSource.Token);
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
            loginCancellationTokenSource = new();
            var result = await authenticationService.LoginWithAppleAsync(
                loginCancellationTokenSource.Token);
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

        await NavigationService.NavigateAsync(
            AccountManagementConstants.ModuleName,
            AccountManagementConstants.TenantSelectionView,
            new NavigationParameters
            {
                {
                    nameof(TenantSelectionViewModel.AvailableTenants), 
                    availableTenants
                }
            });
    }

    #region CancelLoginCommand
    public AsyncDelegateCommand? CancelLoginCommand => field ??= new AsyncDelegateCommand(ExecuteCancelLoginCommand);
    Task ExecuteCancelLoginCommand()
    {
        loginCancellationTokenSource?.Cancel();
        return Task.CompletedTask;
    }
    #endregion CancelLoginCommand

    public override void OnNavigatedTo(NavigationContext ctx)
    {
        base.OnNavigatedTo(ctx);

        AuthenticationService.Logout(false);
    }

    public override bool PersistInHistory() => false;
}
