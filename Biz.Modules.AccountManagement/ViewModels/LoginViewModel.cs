using Biz.Core;
using DryIoc;
using Prism.Commands;
using Prism.Navigation.Regions;

namespace Biz.Modules.AccountManagement.ViewModels;

public class LoginViewModel : PageViewModelBase
{
    public SocialLoginsViewModel SocialLogins { get; }
   
    public LoginViewModel(IContainer container) 
        : base(container)
    {
        SocialLogins = container.Resolve<SocialLoginsViewModel>();
        Title = $"Sign In to {AppConstants.AppShortName}";
    }

    #region CancelLoginCommand
    public AsyncDelegateCommand? CancelLoginCommand => field ??= new AsyncDelegateCommand(ExecuteCancelLoginCommand);
    Task ExecuteCancelLoginCommand()
    {
        SocialLogins.LoginCancellationTokenSource?.Cancel();
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