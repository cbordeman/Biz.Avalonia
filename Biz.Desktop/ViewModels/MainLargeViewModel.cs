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
                        viewName: DashboardConstants.DashboardView,
                        displayName: "Dashboard", 
                        geometryStyleResourceName: ResourceNames.Home,
                        moduleName: DashboardConstants.ModuleName),
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