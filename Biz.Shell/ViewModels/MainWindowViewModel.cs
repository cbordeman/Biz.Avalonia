namespace Biz.Shell.ViewModels
{
    [UsedImplicitly]
    public class MainWindowViewModel : PageViewModelBase
    {
        #region Main
        public MainLargeViewModel Main
        {
            get;
            set => SetProperty(ref field, value);
        }
        #endregion Main

        public MainWindowViewModel(IContainer container, 
            MainLargeViewModel mainLargeViewModel)
            : base(container)
        {
            Main = mainLargeViewModel;
            Title = "Shell (Window)";
        }
        
        #region TryCloseCommand
        [field: AllowNull, MaybeNull]
        public AsyncRelayCommand TryCloseCommand => field ??= new AsyncRelayCommand(ExecuteTryCloseCommand, CanTryCloseCommand);
        bool CanTryCloseCommand() => true;
        async Task ExecuteTryCloseCommand()
        {
            if (await DesktopDialogService.Confirm(
                    "Close", "Do you really want to exit?",
                    "Yes", "No")) 
                Environment.Exit(0);
        }
        #endregion TryCloseCommand
    }
}