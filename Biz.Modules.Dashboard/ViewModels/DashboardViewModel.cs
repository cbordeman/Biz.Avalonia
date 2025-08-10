namespace Biz.Modules.Dashboard.ViewModels;

[Page("dashboard")]
public class DashboardViewModel : NavigationAwareViewModelBase
{
    readonly INotificationService notificationService;

    int counter;
    int listItemSelected = -1;
    ObservableCollection<string> listItems = [];
    string listItemText = string.Empty;

    public List<ThemeVariant> ThemeStyles =>
    [
        ThemeVariant.Default,
        ThemeVariant.Dark,
        ThemeVariant.Light
    ];
    
    public DashboardViewModel(IContainer container) : base(container)
    {
        notificationService = container.Resolve<INotificationService>();
        ThemeSelected = Application.Current!.RequestedThemeVariant!;
    }

    public DelegateCommand CmdAddItem => new(() =>
    {
        counter++;

        // Insert items at the top of the list
        ListItems.Insert(0, $"Item Number: {counter}");

        // Insert at the bottom
        // ListItems.Add($"Item Number: {_counter}");
    });

    public DelegateCommand CmdClearItems => new(ListItems.Clear);

    public DelegateCommand CmdNotification => new(() =>
    {
        notificationService.Show("Hello Prism!", "Notification Pop-up Message.");

        // Alternate OnClick action
        ////_notification.Show("Hello Prism!", "Notification Pop-up Message.", () =>
        ////{
        ////    // Action to perform
        ////});
    });

    public int ListItemSelected
    {
        get => listItemSelected;
        set
        {
            SetProperty(ref listItemSelected, value);

            if (value == -1)
                return;

            ListItemText = ListItems[ListItemSelected];
        }
    }

    public string ListItemText
    {
        get => listItemText;
        set => SetProperty(ref listItemText, value);
    }

    public ObservableCollection<string> ListItems
    {
        get => listItems;
        set => SetProperty(ref listItems, value);
    }

    #region ThemeSelected
    ThemeVariant themeSelected = ThemeVariant.Default;
    public ThemeVariant ThemeSelected
    {
        get => themeSelected;
        set
        {
            if (SetProperty(ref themeSelected, value))
                Application.Current!.RequestedThemeVariant = themeSelected;
        }
    }
    #endregion ThemeSelected
}