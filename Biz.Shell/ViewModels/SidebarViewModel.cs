using System.Threading.Tasks;
using Biz.Shell.Views;
using Prism.Commands;
using Prism.Navigation.Regions;

namespace Biz.Shell.ViewModels
{
    public class SidebarViewModel : NavigationAwareViewModelBase
    {
        private const int CollapsedWidth = 40;
        private const int ExpandedWidth = 200;

        #region FlyoutWidth
        public int FlyoutWidth
        {
            get => flyoutWidth;
            set => SetProperty(ref flyoutWidth, value);
        }
        int flyoutWidth;        
        #endregion FlyoutWidth
        
        public SidebarViewModel(IContainer container) : base(container)
        {
            Title = "Navigation";
            FlyoutWidth = ExpandedWidth;
        }

        #region DashboardCommand
        AsyncDelegateCommand? dashboardCommand;
        public AsyncDelegateCommand DashboardCommand => dashboardCommand ??= new AsyncDelegateCommand(ExecuteDashboardCommand, CanDashboardCommand);
        bool CanDashboardCommand() => true;
        Task ExecuteDashboardCommand()
        {
            RegionManager.RequestNavigate(RegionNames.MainContentRegion, "DashboardView");
            return Task.CompletedTask;
        }
        #endregion DashboardCommand
        
        #region FlyoutMenuCommand
        AsyncDelegateCommand? flyoutMenuCommand;
        public AsyncDelegateCommand FlyoutMenuCommand => flyoutMenuCommand ??= new AsyncDelegateCommand(ExecuteFlyoutMenuCommand, CanFlyoutMenuCommand);
        bool CanFlyoutMenuCommand() => true;
        Task ExecuteFlyoutMenuCommand()
        {
            var isExpanded = FlyoutWidth == ExpandedWidth;
            FlyoutWidth = isExpanded ? CollapsedWidth : ExpandedWidth;
            return Task.CompletedTask;
        }
        #endregion FlyoutMenuCommand
        
        #region SettingsCommand
        AsyncDelegateCommand? settingsCommand;
        public AsyncDelegateCommand SettingsCommand => settingsCommand ??= new AsyncDelegateCommand(ExecuteSettingsCommand, CanSettingsCommand);
        bool CanSettingsCommand() => true;
        Task ExecuteSettingsCommand()
        {
            RegionManager.RequestNavigate(
                RegionNames.MainContentRegion, nameof(SettingsView));
            return Task.CompletedTask;
        }
        #endregion SettingsCommand
    }
}