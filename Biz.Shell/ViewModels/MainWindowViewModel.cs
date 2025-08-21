namespace Biz.Shell.ViewModels
{
    [UsedImplicitly]
    public class MainWindowViewModel : PageViewModelBase
    {
        #region Main
        public MainLargeViewModel Main
        {
            get => main;
            set => SetProperty(ref main, value);
        }
        MainLargeViewModel main;
        
        #endregion Main

        public MainWindowViewModel(IContainer container, 
            MainLargeViewModel mainLargeViewModel)
            : base(container)
        {
            main = mainLargeViewModel;
            Title = "Shell (Window)";
        }
        
        #region TryCloseCommand
        AsyncDelegateCommand? tryCloseCommand;
        public AsyncDelegateCommand TryCloseCommand => tryCloseCommand ??= new AsyncDelegateCommand(ExecuteTryCloseCommand, CanTryCloseCommand);
        bool CanTryCloseCommand() => true;
        async Task ExecuteTryCloseCommand()
        {
            if (await DialogService.Confirm(
                    "Close", "Do you really want to exit?",
                    "Yes", "No")) 
                Environment.Exit(0);
        }
        #endregion TryCloseCommand
    }
}