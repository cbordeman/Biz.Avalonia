using System.Collections.Generic;
using Biz.Modules.Dashboard.Core;
using Biz.Shared.Infrastructure;
using Biz.Shared.ViewModels;

namespace Biz.Desktop.ViewModels;

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