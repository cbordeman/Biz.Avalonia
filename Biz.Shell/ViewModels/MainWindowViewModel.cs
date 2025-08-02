using System;
using System.Threading.Tasks;
using Prism.Commands;
using Prism.Navigation.Regions;

namespace Biz.Shell.ViewModels
{
    public class MainWindowViewModel : NavigationAwareViewModelBase
    { 
        #region Main
        public MainMobileViewModel? Main
        {
            get => main;
            set => SetProperty(ref main, value);
        }
        MainMobileViewModel? main;        
        #endregion Main
        
        public MainWindowViewModel(IContainer container, MainMobileViewModel mainViewModelMobile) : base(container)
        {
            Main = mainViewModelMobile;
            Title = "Shell";
        }
    }
}