using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Biz.Shared.ViewModels;
using CommunityToolkit.Mvvm.Input;

namespace Biz.Desktop.ViewModels
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

        public MainWindowViewModel( 
            MainLargeViewModel mainLargeViewModel)
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
            if (await Main.DialogService.Confirm(
                    "Close", "Do you really want to exit?",
                    "Yes", true, "No")) 
                Environment.Exit(0);
        }
        #endregion TryCloseCommand
        
        public override string Area => string.Empty;
    }
}