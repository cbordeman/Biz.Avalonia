namespace Biz.Shell.ViewModels
{
    public class SettingsViewModel : PageViewModelBase
    {
        public SettingsViewModel()
        {
            Title = "Settings";
        }

        // public DelegateCommand CmdNavigateToChild => new(() =>
        // {
        //     Debug.WriteLine("CmdNavigateToChild() - Navigating away...");
        //     var navParams = new NavigationParameters
        //     {
        //         { "key1", "Some text" },
        //         { "key2", 999 }
        //     };
        //
        //     NavigationService.RequestNavigate(
        //         null,
        //         nameof(SettingsSubView),
        //         navParams);
        // });

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Debug.WriteLine("OnNavigatedFrom()");
            base.OnNavigatedFrom(navigationContext);
        }
        public override string Area => "Settings";
    }
}
