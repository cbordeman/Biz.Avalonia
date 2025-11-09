using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Biz.Desktop.ViewModels;
using CompositeFramework.Core.Extensions;
using CompositeFramework.Core.Navigation;
using Splat;

namespace Biz.Desktop.Views
{
    /// <summary>Main window.</summary>
    public partial class MainWindow : ShadUI.Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Closing += OnClosing;

            ToolTip.SetTip(FullscreenButton, "Fullscreen");
            FullscreenButton.Click += OnFullScreen;
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            this.AddHandler(PointerPressedEvent, PointerPressedHandler, handledEventsToo: true);
            
            if (DataContext is IOnViewLoaded onViewLoaded)
                onViewLoaded.OnViewLoaded();
        }
        
        void PointerPressedHandler(object? sender, PointerPressedEventArgs e)
        {
            var properties = e.GetCurrentPoint(this).Properties;
            if (properties.IsXButton1Pressed)
            {
                // Handle "Back" button
                var nav = Locator.Current.Resolve<ISectionNavigationService>();
                if (nav.History.Count > 0)
                    nav.GoBackAsync().ContinueWith(t => { });
            }
            else if (properties.IsXButton2Pressed)
            {
                // Handle "Forward" button
                throw new NotImplementedException();
            }
        }

        void OnFullScreen(object? sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.FullScreen)
            {
                ExitFullScreen();
                ToolTip.SetTip(FullscreenButton, "Fullscreen");
            }
            else
            {
                WindowState = WindowState.FullScreen;
                ToolTip.SetTip(FullscreenButton, "Exit Fullscreen");
            }
        }

        void OnClosing(object? sender, WindowClosingEventArgs e)
        {
            e.Cancel = true;

            if (DataContext is MainWindowViewModel viewModel)
                viewModel.TryCloseCommand.Execute(null);
        }
    }
}