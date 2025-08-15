using System.Collections.Generic;
using Biz.Modules.Dashboard;

namespace Biz.Shell.ViewModels;

public class MainLargeViewModel : MainViewModelBase
{
    public List<SidebarHeaderViewModel> SidebarHeaders { get; }

    #region Greeting
    string greeting = "Welcome to Shell (Large)";
    public string Greeting
    {
        get => greeting;
        set => SetProperty(ref greeting, value);
    }
    #endregion Greeting

    #region SidebarIsExpanded
    public bool SidebarIsExpanded
    {
        get => sidebarIsExpanded;
        set => SetProperty(ref sidebarIsExpanded, value);
    }
    bool sidebarIsExpanded = true;        
    #endregion SidebarIsExpanded
    
    public MainLargeViewModel(IContainer container) : base(container)
    {
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
                        null)
                ]
            }
        ];
    }
}