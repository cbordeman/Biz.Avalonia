using System.Diagnostics.CodeAnalysis;
using Biz.Shared.ViewModels;
using CommunityToolkit.Mvvm.Input;
using CompositeFramework.Core.Dialogs;
using CompositeFramework.Core.Extensions;

namespace Biz.Shell.Desktop.ViewModels
{
    public class MainWindowViewModel : PageViewModelBase
    {
        #region Main
        public MainLargeViewModel Main
        {
            get;
            set => SetProperty(ref field, value);
        }
        #endregion Main

        // This must be public so MainWindow can bind to its DialogHost property.
        public IDialogService DialogService { get; }
        
        public MainWindowViewModel( 
            MainLargeViewModel mainLargeViewModel)
        {
            Main = mainLargeViewModel;
            DialogService = Locator.Current
                .Resolve<IDialogService>();
            Title = "Shell (Window)";
        }
        
        #region TryCloseCommand
        [field: AllowNull, MaybeNull]
        public AsyncRelayCommand TryCloseCommand => field ??= new AsyncRelayCommand(ExecuteTryCloseCommand, CanTryCloseCommand);
        bool CanTryCloseCommand() => true;
        async Task ExecuteTryCloseCommand()
        {
            if (await DialogService.Confirm(
                    "Close", "Do you really want to exit?",
                    "Yes", "No")) 
                Environment.Exit(0);
        }
        #endregion TryCloseCommand
        
        public override string Area => string.Empty;
    }
}