using System.Diagnostics;
using Prism.Commands;
using Prism.Navigation;
using Prism.Navigation.Regions;
using System.Collections.Generic;
using Biz.Shell.Views;

namespace Biz.Shell.ViewModels
{
    public class SettingsViewModel : NavigationAwareViewModelBase
    {
        public SettingsViewModel(IContainer container) : base(container)
        {
            Title = "Settings";
        }

        public DelegateCommand CmdNavigateToChild => new(() =>
        {
            Debug.WriteLine("CmdNavigateToChild() - Navigating away...");
            var navParams = new NavigationParameters
            {
                { "key1", "Some text" },
                { "key2", 999 }
            };

            RegionManager.RequestNavigate(
                RegionNames.MainContentRegion,
                nameof(SettingsSubView),
                navParams);
        });

        public override void OnNavigatedFrom(NavigationContext navigationContext)
        {
            Debug.WriteLine("OnNavigatedFrom()");
            base.OnNavigatedFrom(navigationContext);
        }
    }
}
