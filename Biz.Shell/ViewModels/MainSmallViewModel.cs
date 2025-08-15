using Biz.Modules.Dashboard;

namespace Biz.Shell.ViewModels;

[UsedImplicitly]
public class MainSmallViewModel : MainViewModelBase
{
    public MainSmallViewModel(IContainer container) : base(container)
    {
        SidebarIsExpanded = false;

        SidebarHeaders =
        [
            new SidebarHeaderViewModel()
            {
                Header = "Item 1",
                Children =
                [
                    new SideBarNavigationItemViewModel(
                        DashboardConstants.DashboardView,
                        "Dashboard", "",
                        DashboardConstants.ModuleName),
                    new SideBarNavigationItemViewModel(
                        "SettingsView",
                        "Settings", "",
                        null!)
                ]
            }
        ];
    }
}