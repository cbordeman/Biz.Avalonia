using System.Diagnostics.CodeAnalysis;
using Biz.Models;

namespace Biz.Shell.ViewModels;

public class LoginViewModel : PageViewModelBase
{
    CancellationTokenSource? loginCancellationTokenSource;
    
    #region LoginWithCoogleCommand
    [field: AllowNull, MaybeNull]
    public AsyncDelegateCommand LoginWithGoogleCommand => field ??= new AsyncDelegateCommand(ExecuteLoginWithGoogleCommand, CanLoginWithGoogleCommand);
    bool CanLoginWithGoogleCommand() => true;
    async Task ExecuteLoginWithGoogleCommand()
    {
        try
        {
            IsBusy = true;
            loginCancellationTokenSource = new();
            var result = await AuthenticationService.LoginWithGoogleAsync(loginCancellationTokenSource.Token);
            if (!result.isLoggedIn)
            {
                if (result.availableTenants == null)
                {
                    await DialogService.Confirm("Cannot Login",
                        "Error logging in.  Please try again.",
                        cancelText: null);
                    IsBusy = false;
                    return;
                }
                else if (result.availableTenants.Length == 0)
                {
                    await DialogService.Confirm("Account Inactive",
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
            await DialogService.Confirm(
                "Error", 
                $"An error occurred during Google login: {ex.Message}",
                cancelText: null);
            IsBusy = false;
        }
    }
    #endregion LoginWithCoogleCommand

    #region LoginWithMicrosoftCommand
    [field: AllowNull, MaybeNull]
    public AsyncDelegateCommand LoginWithMicrosoftCommand => field ??= new AsyncDelegateCommand(ExecuteLoginWithMicrosoftCommand);
    async Task ExecuteLoginWithMicrosoftCommand()
    {
        try
        {
            IsBusy = true;
            loginCancellationTokenSource = new();
            var result = await AuthenticationService.LoginWithMicrosoftAsync(loginCancellationTokenSource.Token);
            if (!result.isLoggedIn)
            {
                if (result.availableTenants == null || result.error != null)
                {
                    await DialogService.Confirm("Cannot Login",
                        "Error logging in.  Please try again." +
                        (result.error == null ? "" : $"\n\nError: {result.error}"),
                        cancelText: null);
                    IsBusy = false;
                    return;
                }
                else if (result.availableTenants.Length == 0)
                {
                    await DialogService.Confirm("Account Inactive",
                        "Your account is inactive in all organizations.",
                        cancelText: null);
                    IsBusy = false;
                    return;
                }
                IsBusy = false;
                await GoToTenantSelectionPage(result.availableTenants);
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
            await DialogService.Confirm(
                "Error", 
                $"An error occurred during Microsoft login: {ex.Message}",
                cancelText: null);
            IsBusy = false;
        }
    }

    #endregion LoginWithMicrosoftCommand
    
    #region LoginWithFacebookCommand
    [field: AllowNull, MaybeNull]
    public AsyncDelegateCommand LoginWithFacebookCommand => field ??= new AsyncDelegateCommand(ExecuteLoginWithFacebookCommand, CanLoginWithFacebookCommand);
    bool CanLoginWithFacebookCommand() => true;
    async Task ExecuteLoginWithFacebookCommand()
    {
        try
        {
            IsBusy = true;
            loginCancellationTokenSource = new();
            var result = await AuthenticationService.LoginWithFacebookAsync(loginCancellationTokenSource.Token);
            if (!result.isLoggedIn)
            {
                if (result.availableTenants == null)
                {
                    await DialogService.Confirm("Cannot Login",
                        "Error logging in.  Please try again.",
                        cancelText: null);
                    IsBusy = false;
                    return;
                }
                else if (result.availableTenants.Length == 0)
                {
                    await DialogService.Confirm("Account Inactive",
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
            await DialogService.Confirm("Error", $"An error occurred during Facebook login: {ex.Message}",
                cancelText: null);
            IsBusy = false;
        }
    }
    #endregion LoginWithFacebookCommand

    #region LoginWithAppleCommand
    [field: AllowNull, MaybeNull]
    public AsyncDelegateCommand LoginWithAppleCommand => field ??= new AsyncDelegateCommand(ExecuteLoginWithAppleCommand, CanLoginWithAppleCommand);
    bool CanLoginWithAppleCommand() => true;
    async Task ExecuteLoginWithAppleCommand()
    {
        try
        {
            IsBusy = true;
            loginCancellationTokenSource = new();
            var result = await AuthenticationService.LoginWithAppleAsync(loginCancellationTokenSource.Token);
            if (!result.isLoggedIn)
            {
                if (result.availableTenants == null)
                {
                    await DialogService.Confirm("Cannot Login",
                        "Error logging in.  Please try again.",
                        cancelText: null);
                    IsBusy = false;
                    return;
                }
                else if (result.availableTenants.Length == 0)
                {
                    await DialogService.Confirm("Account Inactive",
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
            await DialogService.Confirm("Error", $"An error occurred during Apple login: {ex.Message}",
                cancelText: null);
            IsBusy = false;
        }
    }
    #endregion LoginWithAppleCommand
    
    #region CancelLoginCommand
    [field: AllowNull, MaybeNull]
    public AsyncDelegateCommand CancelLoginCommand => field ??= new AsyncDelegateCommand(ExecuteCancelLoginCommand);
    Task ExecuteCancelLoginCommand()
    {
        loginCancellationTokenSource?.Cancel();
        return Task.CompletedTask;
    }
    #endregion CancelLoginCommand

    public LoginViewModel(IContainer container) 
        : base(container)
    {
        Title = "Sign In to Biz";
    }

    Task GoToTenantSelectionPage(Tenant[]? availableTenants)
    {
        NavigationService.RequestNavigate(
            nameof(TenantSelectionView),
            new NavigationParameters
            {
                {
                    nameof(TenantSelectionViewModel.AvailableTenants), 
                    availableTenants!
                }
            });
        return Task.CompletedTask;
    }

    public override void OnNavigatedTo(NavigationContext ctx)
    {
        base.OnNavigatedTo(ctx);
        
        AuthenticationService.Logout(false);
    }

    public override bool PersistInHistory() => false;
}