using Biz.Modules.Dashboard;

namespace Biz.Shell.ViewModels;

[UsedImplicitly]
public class MainLargeViewModel : MainViewModelBase
{
    public MainLargeViewModel(IContainer container) : base(container)
    {
        SidebarIsExpanded = true;
        
        SidebarHeaders =
        [
            new SidebarHeaderViewModel()
            {
                Header = "Item 1",
                Children =
                [
                    new SideBarNavigationItemViewModel(
                        DashboardConstants.DashboardView,
                        "Dashboard", 
                        ResourceNames.Home,
                        DashboardConstants.ModuleName),
                    new SideBarNavigationItemViewModel(
                        "SettingsView",
                        "Settings",
                        ResourceNames.Gear,
                        null!)
                ]
            }
        ];
    }
}