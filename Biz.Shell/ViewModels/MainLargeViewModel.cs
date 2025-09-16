using Biz.Modules.Dashboard.Core;

namespace Biz.Shell.ViewModels;

public class MainLargeViewModel : MainViewModelBase
{
    public List<SidebarHeaderViewModel> SidebarHeaders 
        { get; protected set; }
    
    public MainLargeViewModel()
    {
        IsDrawerOpen = true;
        
        SidebarHeaders =
        [
            new SidebarHeaderViewModel()
            {
                Header = "Item 1",
                Children =
                [
                    new SideBarNavigationItemViewModel(
                        DashboardConstants.DashboardView,
                        DashboardConstants.ModuleName, 
                        ResourceNames.Home,
                        DashboardConstants.ModuleName),
                    // new SideBarNavigationItemViewModel(
                    //     "SettingsView",
                    //     "Settings",
                    //     ResourceNames.Gear,
                    //     null!)
                ]
            }
        ];
    }

    public override string Area => string.Empty;
}