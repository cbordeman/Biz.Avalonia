using Biz.Modules.Dashboard;
using ShadUI;

namespace Biz.Shell.ViewModels
{
    public class MainWindowViewModel : NavigationAwareViewModelBase
    {
        public IPlatformDialogService DialogService { get; }
        
        #region Main
        public MainLargeViewModel Main
        {
            get => main;
            set => SetProperty(ref main, value);
        }
        MainLargeViewModel main;
        readonly IModuleManager moduleManager;

        #endregion Main

        public MainWindowViewModel(IContainer container, 
            MainLargeViewModel mainLargeViewModel, 
            IPlatformDialogService platformDialogService,
            IModuleManager moduleManager)
            : base(container)
        {
            this.DialogService = platformDialogService;
            
            main = mainLargeViewModel;
            this.moduleManager = moduleManager;
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