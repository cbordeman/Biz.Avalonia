using Avalonia.Interactivity;

namespace Biz.Shell.Views
{
    /// <summary>Main window view.</summary>
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

            if (DataContext is IOnViewLoaded onViewLoaded)
                onViewLoaded.OnViewLoaded();
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
            {
                viewModel.TryCloseCommand.Execute().LogException("MainWindow.OnClosing dialog");
            }
        }
    }
}