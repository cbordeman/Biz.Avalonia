using System;
using Biz.Core.ViewModels;
using JetBrains.Annotations;

namespace Biz.Shell.ViewModels
{
    [UsedImplicitly]
    public class MainWindowViewModel : NavigationAwareViewModelBase
    {
        #region Main
        public MainLargeViewModel? Main
        {
            get => main;
            set => SetProperty(ref main, value);
        }
        MainLargeViewModel? main;

        #endregion Main

        public MainWindowViewModel(IContainer container, MainLargeViewModel mainLargeViewModel)
            : base(container)
        {
            Main = mainLargeViewModel;
            Title = "Shell (Window)";
        }
    }
}