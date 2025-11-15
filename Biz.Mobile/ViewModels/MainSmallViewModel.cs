using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Biz.Modules.Dashboard.Core;
using Biz.Shared.Infrastructure;
using Biz.Shared.ViewModels;

namespace Biz.Mobile.ViewModels;

public class MainSmallViewModel : MainViewModelBase
{
    public List<SidebarItemViewModel> SidebarItems { get; }

    #region CurrentItem
    public SidebarItemViewModel? CurrentItem
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion CurrentItem
    
    public MainSmallViewModel()
    {
        IsDrawerOpen = false;

        SidebarItems =
        [
            new SideBarNavigationItemViewModel(
                viewName: DashboardConstants.DashboardView,
                displayName: "Dashboard",
                geometryStyleResourceName: ResourceNames.Home,
                moduleName: DashboardConstants.ModuleName),
        ];
    }
    
    protected override void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        // When CurrentArea changes, select the corresponding
        // item for highlight.
        if (args.PropertyName == nameof(CurrentArea)) 
            CurrentItem = SidebarItems.FirstOrDefault(
                x => x.ViewName == CurrentArea);
        
        base.OnPropertyChanged(args);
    }

    public override string Area => string.Empty;
}