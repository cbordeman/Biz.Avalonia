using System.ComponentModel.DataAnnotations;
using Biz.Core;
using Biz.Core.Models;
using Biz.Models;
using Biz.Modules.AccountManagement.Core;
using Biz.Shell.Infrastructure;
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

    #region Email
    [Required(ErrorMessage = "Required.")]
    [StringLength(100, ErrorMessage = "Too long.")]
    [RegularExpression(AppConstants.EmailRegex, ErrorMessage = "Invalid email address.")]
    public string? Email
    {
        get => email;
        set => SetProperty(ref email, value);
    }
    string? email;        
    #endregion Email

    #region Password
    [Required]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Not a good password.")]
    public string? Password
    {
        get => password;
        set => SetProperty(ref password, value);
    }
    string? password;        
    #endregion Password

    
    public LoginViewModel(IContainer container)
        : base(container)
    {
        dialogService = container.Resolve<IPlatformDialogService>();
        authenticationService = container.Resolve<IAuthenticationService>();
        Title = $"Sign In to {AppConstants.AppShortName}";
    }

    #region LoginCommand
    AsyncDelegateCommandWithParam<LoginProvider>? loginCommand;
    public AsyncDelegateCommandWithParam<LoginProvider> LoginCommand => loginCommand ??= 
        new AsyncDelegateCommandWithParam<LoginProvider>(ExecuteLoginCommand, CanLoginCommand);
    static bool CanLoginCommand(LoginProvider provider) => true;
    async Task ExecuteLoginCommand(LoginProvider provider)
    {
        try
        {
            IsBusy = true;
            loginCancellationTokenSource = new();
            var result = await authenticationService.LoginWithProviderAsync(
                provider, loginCancellationTokenSource.Token);
            if (!result.isLoggedIn)
            {
                if (result.availableTenants == null || result.error != null)
                {
                    await dialogService.Confirm(
                        $"{provider} Login",
                        $"Error logging in." +
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
    #endregion LoginCommand

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

        AuthenticationService.Logout(false, false);
    }

    public override bool PersistInHistory() => false;
}