using System.Collections.Generic;
using System.ComponentModel;
using Biz.Modules.Dashboard;
using IContainer = DryIoc.IContainer;

namespace Biz.Shell.ViewModels;

[UsedImplicitly]
public class MainSmallViewModel : MainViewModelBase
{
    public List<SidebarItemViewModel> SidebarItems { get; }

    #region CurrentItem
    public SidebarItemViewModel? CurrentItem
    {
        get => currentItem;
        set => SetProperty(ref currentItem, value);
    }
    SidebarItemViewModel? currentItem;
    #endregion CurrentItem
    
    public MainSmallViewModel(IContainer container) : base(container)
    {
        IsDrawerOpen = false;

        SidebarItems =
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
        ];
    }
    
    protected override void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        // When CurrentArea changes, select the corresponding
        // item for highlight.
        if (args.PropertyName == nameof(CurrentArea)) 
            CurrentItem = SidebarItems.FirstOrDefault(x => x.ViewName == args.PropertyName);
        
        if (CurrentItem != null)
            Debugger.Break();
        
        base.OnPropertyChanged(args);
    }
    
}