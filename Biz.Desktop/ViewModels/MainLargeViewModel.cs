using System.Collections.Generic;
using System.Threading.Tasks;
using Biz.Models;
using Biz.Modules.Dashboard.Core;
using Biz.Shared.Infrastructure;
using Biz.Shared.ViewModels;
using CompositeFramework.Avalonia.Commands;

namespace Biz.Desktop.ViewModels;

public class MainLargeViewModel : MainViewModelBase
{
    public List<SidebarHeaderViewModel> SidebarHeaders
    {
        get;
        protected set;
    }
    public override string Area => string.Empty;
    public List<BaseMenuItemViewModel>? ProfileMenu { get; set; }

    public MainLargeViewModel()
    {
        IsDrawerOpen = true;

        SidebarHeaders =
        [
            new SidebarHeaderViewModel()
            {
                Header = "Item 1",
                Children =
                [
                    new SideBarNavigationItemViewModel(
                        viewName: DashboardConstants.DashboardView,
                        displayName: "Dashboard",
                        geometryStyleResourceName: ResourceNames.Home,
                        moduleName: DashboardConstants.ModuleName),
                    // new SideBarNavigationItemViewModel(
                    //     "SettingsView",
                    //     "Settings",
                    //     ResourceNames.Gear,
                    //     null!)
                ]
            }
        ];

        BuildProfileMenu();
        
        AuthService.AuthenticationStateChanged.Subscribe(BuildProfileMenu);
    }
    Task BuildProfileMenu()
    {
        ProfileMenu = new List<BaseMenuItemViewModel>();
        var isAuthenticated = AuthService.IsAuthenticated().Result;
        User? user = null;
        if (isAuthenticated)
            user = AuthService.GetCurrentUserAsync().Result;

        // Add profile or sign in / register commands
        if (isAuthenticated && user != null)
        {
            ProfileMenu.Add(new ProfileMenuItemViewModel(
                user.Name,
                user.SourceAvaRes ?? "avares://Biz.Shared/Assets/user.png",
                user.Email,
                new AsyncCommand(async () =>
                {
                    await AuthService.LogoutAsync(true, true);
                })));
        }

        // Add other commands here...

        // Add sign out
        if (isAuthenticated)
            ProfileMenu.Add(new MenuItemViewModel("",
                "Sign out",
                "",
                "",
                new AsyncCommand(async () =>
                {
                    await AuthService.LogoutAsync(true, true);
                }),
                null));
        RaisePropertyChanged(nameof(ProfileMenu));
        return Task.CompletedTask;
    }
}
