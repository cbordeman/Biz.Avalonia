namespace Biz.Shell.ViewModels
{
    public class MainWindowViewModel : NavigationAwareViewModelBase
    {
        #region Main

        public MainSmallViewModel? Main
        {
            get => main;
            set => SetProperty(ref main, value);
        }

        MainSmallViewModel? main;

        #endregion Main

        public MainWindowViewModel(IContainer container, MainSmallViewModel mainViewModelSmall) : base(container)
        {
            Main = mainViewModelSmall;
            Title = "Shell";
        }
    }
}