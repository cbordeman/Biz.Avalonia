using System.Diagnostics.CodeAnalysis;

namespace Biz.Modules.AccountManagement.ViewModels;

public partial class LoginViewModel : PageViewModelBase
{
    CancellationTokenSource? loginCancellationTokenSource;
    readonly DesktopDialogService desktopDialogService;
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
    
    public LoginViewModel() : base()
    {
        desktopDialogService = Locator.Current.Resolve<DesktopDialogService>();
        authenticationService = Locator.Current.Resolve<IAuthenticationService>();
        Title = $"Sign In to {AppConstants.AppShortName}";
        IsValidating = true;
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
                    await desktopDialogService.Confirm(
                        $"{provider} Login",
                        $"Error logging in." +
                        (result.error == null ? "" : $"\n\nError: {result.error}"),
                        cancelText: null);
                    return;
                }
                else if (result.availableTenants.Length == 0)
                {
                    await desktopDialogService.Confirm("Account Inactive",
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
            await desktopDialogService.Confirm(
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
        ArgumentChecker.ThrowIfNull(availableTenants);

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

    public override void OnNavigatedTo(NavigationContext ctx)
    {
        base.OnNavigatedTo(ctx);

        AuthenticationService.Logout(false, false);
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