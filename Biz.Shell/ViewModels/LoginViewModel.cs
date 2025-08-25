using Biz.Models;

namespace Biz.Shell.ViewModels;

public class LoginViewModel : PageViewModelBase
{
    CancellationTokenSource? loginCancellationTokenSource;
    
    #region LoginWithCoogleCommand
    AsyncDelegateCommand? loginWithGoogleCommand;
    public AsyncDelegateCommand LoginWithGoogleCommand => loginWithGoogleCommand ??= new AsyncDelegateCommand(ExecuteLoginWithGoogleCommand, CanLoginWithGoogleCommand);
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
    AsyncDelegateCommand? loginWithMicrosoftCommand;
    public AsyncDelegateCommand LoginWithMicrosoftCommand => loginWithMicrosoftCommand ??= new AsyncDelegateCommand(ExecuteLoginWithMicrosoftCommand);
    async Task ExecuteLoginWithMicrosoftCommand()
    {
        try
        {
            IsBusy = true;
            loginCancellationTokenSource = new();
            var result = await AuthenticationService.LoginWithMicrosoftAsync(loginCancellationTokenSource.Token);
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
                $"An error occurred during Microsoft login: {ex.Message}",
                cancelText: null);
            IsBusy = false;
        }
    }

    #endregion LoginWithMicrosoftCommand
    
    #region LoginWithFacebookCommand
    AsyncDelegateCommand? loginWithFacebookCommand;
    public AsyncDelegateCommand LoginWithFacebookCommand => loginWithFacebookCommand ??= new AsyncDelegateCommand(ExecuteLoginWithFacebookCommand, CanLoginWithFacebookCommand);
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
    AsyncDelegateCommand? loginWithAppleCommand;
    public AsyncDelegateCommand LoginWithAppleCommand => loginWithAppleCommand ??= new AsyncDelegateCommand(ExecuteLoginWithAppleCommand, CanLoginWithAppleCommand);
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
    AsyncDelegateCommand? cancelLoginCommand;
    public AsyncDelegateCommand CancelLoginCommand => cancelLoginCommand ??= new AsyncDelegateCommand(ExecuteCancelLoginCommand);
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

    Task GoToTenantSelectionPage(Tenant[] availableTenants)
    {
        NavigationService.RequestNavigate(
            nameof(TenantSelectionView),
            new NavigationParameters
            {
                { nameof(TenantSelectionViewModel.AvailableTenants), 
                    availableTenants }
            });
        return Task.CompletedTask;
    }

    public override bool PersistInHistory() => false;
}