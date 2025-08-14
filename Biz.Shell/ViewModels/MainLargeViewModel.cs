using System.Threading.Tasks;
using Prism.Commands;
using ShadUI;

namespace Biz.Shell.ViewModels;

public class MainLargeViewModel : MainViewModelBase
{
    #region SidebarItems
    public ObservableCollection<SidebarHeaderViewModel> SidebarItems
    {
        get => sidebarItems;
        set => SetProperty(ref sidebarItems, value);
    }
    ObservableCollection<SidebarHeaderViewModel> sidebarItems;        
    #endregion SidebarItems
    
    #region Greeting
    string greeting = "Welcome to Shell (Large)";
    public string Greeting
    {
        get => greeting;
        set => SetProperty(ref greeting, value);
    }
    #endregion Greeting

    public MainLargeViewModel(IContainer container) : base(container)
    {
    }
}