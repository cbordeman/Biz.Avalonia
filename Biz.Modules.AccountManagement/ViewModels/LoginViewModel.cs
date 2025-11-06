using System.Diagnostics.CodeAnalysis;

namespace Biz.Modules.AccountManagement.ViewModels;

public class LoginViewModel : PageViewModelBase
{
    CancellationTokenSource? loginCancellationTokenSource;
    readonly IDialogService dialogService;
    readonly IAuthenticationService authenticationService;

    #region Email
    [Required(ErrorMessage = "Required.")]
    [EmailAddressCustom]
    public string? Email
    {
        get => email;
        set => SetProperty(ref email, value);
    }
    string? email;        
    #endregion Email

    #region Password
    [Required(ErrorMessage = "Reqired")]
    [StringLength(50, MinimumLength = 8, ErrorMessage = "Too short.")]
    [RegularExpression(AppConstants.PasswordRegex, 
        ErrorMessage = "Too weak.")]
    public string? Password
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion Password
    
    public LoginViewModel()
    {
        dialogService = Locator.Current.Resolve<IDialogService>();
        authenticationService = Locator.Current.Resolve<IAuthenticationService>();
        Title = $"Sign In to {AppConstants.AppShortName}";
    }

    #region LoginCommand
    [field: AllowNull, MaybeNull]
    public AsyncRelayCommand<LoginProvider> LoginCommand => field ??= 
        new AsyncRelayCommand<LoginProvider>(ExecuteLoginCommand, CanLoginCommand);
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
                        (result.error == null ? "" : $"\n\nError: {result.error}"));
                    return;
                }
                else if (result.availableTenants.Length == 0)
                {
                    await dialogService.Confirm("Account Inactive",
                        "Your account is inactive in all organizations.");
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
                $"An error occurred during Microsoft login: {ex.Message}");
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

        var mainNavService = Locator.Current.Resolve<IMainNavigationService>();
        await mainNavService.NavigateWithModuleAsync(
            AccountManagementConstants.ModuleName,
            AccountManagementConstants.TenantSelectionView,
            new NavParam(
                nameof(TenantSelectionViewModel.AvailableTenants),
                availableTenants));
    }

    #region CancelLoginCommand
    public AsyncRelayCommand? CancelLoginCommand => field ??= new AsyncRelayCommand(ExecuteCancelLoginCommand);
    Task ExecuteCancelLoginCommand()
    {
        loginCancellationTokenSource?.Cancel();
        return Task.CompletedTask;
    }
    #endregion CancelLoginCommand

    public override async Task OnNavigatedToAsync(NavigationContext ctx)
    {
        await AuthenticationService.LogoutAsync(false, false);
        IsValidating = true;
    }

    #region RegisterCommand
    AsyncRelayCommand? registerCommand;
    public AsyncRelayCommand RegisterCommand => registerCommand ??= new AsyncRelayCommand(ExecuteRegisterCommand, CanRegisterCommand);
    static bool CanRegisterCommand() => true;
    Task ExecuteRegisterCommand()
    {
        return Task.CompletedTask;
    }
    #endregion RegisterCommand
    
    #region ForgotPasswordCommand
    AsyncRelayCommand? forgotPasswordCommand;
    public AsyncRelayCommand ForgotPasswordCommand => forgotPasswordCommand ??= new AsyncRelayCommand(ExecuteForgotPasswordCommand, CanForgotPasswordCommand);
    static bool CanForgotPasswordCommand() => true;
    Task ExecuteForgotPasswordCommand()
    {
        return Task.CompletedTask;
    }
    #endregion ForgotPasswordCommand
    
    public override bool PersistInHistory() => false;
    public override string Area => "Account";   
}