using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Util;
using Avalonia.Threading;
using Biz.Core.Services;
using Biz.Mobile.Services;
using Biz.Mobile.ViewModels;
using Biz.Mobile.Views;
using Biz.Shell.Infrastructure;
using Biz.Shell.Services;
using Biz.Shell.Services.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;
using Prism.Ioc;
using LogLevel = Microsoft.Identity.Client.LogLevel;

namespace Biz.Shell.Android.Services;

public class AndroidPlatformService : IPlatformService
{
    public void RegisterPlatformTypes(IContainerRegistry containerRegistry)
    {
        // Register Android-specific types, except dialogs, which are 
        // registered in RegisterDialogs().
        containerRegistry.RegisterSingleton<IPlatformModuleCatalogService, MobileModuleCatalogService>();
        containerRegistry.RegisterSingleton<IPlatformDialogService, MobileDialogService>();
        containerRegistry.RegisterSingleton<ISafeStorage, AndroidSafeStorage>();
        containerRegistry.RegisterSingleton<IPlatformMsalService, AndroidMsalService>();

        // Prism style dialog registration.
        containerRegistry.RegisterDialog<MessageDialogView, MessageDialogViewModel>();
    }

    public void InitializePlatform(IContainerProvider container)
    {
    }
}
