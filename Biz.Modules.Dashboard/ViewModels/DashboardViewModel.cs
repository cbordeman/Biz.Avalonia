using System.Collections.Generic;
using System.Threading.Tasks;
using Biz.Shell.Infrastructure;
using Biz.Shell.ViewModels.Toolbar;
using Prism.Commands;

namespace Biz.Modules.Dashboard.ViewModels;

public sealed class DashboardViewModel : PageViewModelBase
{
    readonly INotificationService notificationService;

    int counter;
    int listItemSelected = -1;
    ObservableCollection<string> listItems = [];
    string listItemText = string.Empty;

    public static List<ThemeVariant> ThemeStyles =>
    [
        ThemeVariant.Default,
        ThemeVariant.Dark,
        ThemeVariant.Light
    ];
    
    public DashboardViewModel(IContainer container) : base(container)
    {
        Title = "Dashboard";
        TitleGeometryResourceName = ResourceNames.Gear;
        
        notificationService = container.Resolve<INotificationService>();
        ThemeSelected = Application.Current!.RequestedThemeVariant!;
        
        ToolbarEntries.Add(new ToolbarEntry(
            "Reply", ResourceNames.ArrowReply, Delay));
        ToolbarEntries.Add(new ToolbarEntry(
            "Settings", ResourceNames.Gear, Delay));
        ToolbarEntries.Add(new ToolbarSeparator());
        ToolbarEntries.Add(new ToolbarEntry(
            "Lock", ResourceNames.Lock, Delay));
        ToolbarEntries.Add(new ToolbarEntry(
            "Home", ResourceNames.Home, Delay));
    }

    Task Delay(object? _)
    {
        return Task.Delay(5000);
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