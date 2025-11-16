using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Biz.Desktop.ViewModels;
using Biz.Modules.Dashboard.Core;
using CompositeFramework.Core.Extensions;
using CompositeFramework.Core.Navigation;
using Splat;

namespace Biz.Desktop.Views
{
    /// <summary>Main window.</summary>
    public partial class MainWindow : ShadUI.Window
    {
        readonly Thickness thickness0 = new Thickness(0);
        readonly Thickness thickness1 = new Thickness(1);

        bool navigating;

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
            this.AddHandler(KeyDownEvent, KeyPressedEventHandler);

            if (DataContext is IOnViewLoaded onViewLoaded)
                onViewLoaded.OnViewLoaded();
        }

        void KeyPressedEventHandler(object? sender, KeyEventArgs e)
        {
            if (navigating)
                return;

            if (e.Key == Key.F5)
            {
                try
                {
                    var nav = Locator.Current.Resolve<ISectionNavigationService>();
                    navigating = true;
                    nav
                        .Refresh(
                            DashboardConstants.DashboardView,
                            DashboardConstants.ModuleName)
                        .ContinueWith(t =>
                        {
                            if (t.IsFaulted)
                                throw t.Exception;
                        });
                }
                finally
                {
                    navigating = false;
                }
            }
        }

        void PointerPressedHandler(object? sender, PointerPressedEventArgs e)
        {
            var properties = e.GetCurrentPoint(this).Properties;
            if (properties.IsXButton1Pressed)
            {
                try
                {
                    // Handle "Back" button
                    navigating = true;
                    var nav = Locator.Current.Resolve<ISectionNavigationService>();
                    if (nav.History.Length > 0)
                        nav
                            .GoBackAsync()
                            .ContinueWith(t =>
                            {
                                if (t.IsFaulted)
                                    throw t.Exception;
                            });
                }
                finally
                {
                    navigating = false;
                }
            }
            else if (properties.IsXButton2Pressed)
            {
                // Handle "Forward" button
                try
                {
                    // Handle "Back" button
                    navigating = true;
                    var nav = Locator.Current.Resolve<ISectionNavigationService>();
                    if (nav.History.Length > 0)
                        nav
                            .GoForwardAsync()
                            .ContinueWith(t =>
                            {
                                if (t.IsFaulted)
                                    throw t.Exception;
                            });
                }
                finally
                {
                    navigating = false;
                }
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
        
        void WindowBase_OnResized(object? sender, 
            WindowResizedEventArgs args)
        {
            if (WindowState == WindowState.FullScreen ||
                WindowState == WindowState.Maximized)
                BorderThickness = thickness0;
            else
                BorderThickness = thickness1;
        }
    }
}
