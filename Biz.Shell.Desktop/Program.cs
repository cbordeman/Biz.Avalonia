using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Biz.Core;
using Biz.Core.Extensions;
using Biz.Shell.Desktop.Services;
using Biz.Shell.Infrastructure;
using Biz.Shell.Platform;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Microsoft.Win32;
using Prism.Ioc;

namespace Biz.Shell.Desktop;

#pragma warning disable CA1416 // unimportant platform warning

[UsedImplicitly]
sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        var singleInstanceMutex =
            new Mutex(
                true,
                $"{AppConstants.AppShortName}SingletonMutex",
                out bool createdNew);

        if (!createdNew)
        {
            // Another instance is running, send args[1] (the URI) via IPC to it
            if (args.Length > 1)
                SendUriToRunningInstance(args[1]);
            // Exit this new instance
            return;
        }

        // Start IPC server to listen for URI messages from future instances
        Task.Run(IpcServer);

        // Handle URI if app started by link
        if (args.Length > 1)
            PlatformHandleUri(args[1]);

        PlatformHelper.PlatformService = new DesktopPlatformService();

        // Register biz:... links
        RegisterCustomUriSchemeIfMissing();

        BuildAvaloniaApp()
            .StartWithClassicDesktopLifetime(args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    // ReSharper disable once MemberCanBePrivate.Global
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    static void RegisterCustomUriSchemeIfMissing()
    {
        // Customize this by modifying AppConstants.

        var schemeKey =
            @"SOFTWARE\Classes\" +
            AppConstants.CustomUriSchemeName;

        using var key = Registry.CurrentUser.OpenSubKey(schemeKey);

        if (key != null)
            return; // Already registered, do nothing

        using var newKey = Registry.CurrentUser.CreateSubKey(schemeKey);

        newKey.SetValue("", $"URL:{
            AppConstants.CustomUriProtocolFriendlyName}");
        newKey.SetValue("URL Protocol", "");

        // Set icon (optional)
        var exePath = Assembly.GetEntryAssembly()!.Location;
        using var iconKey = newKey.CreateSubKey("DefaultIcon");
        iconKey.SetValue("", exePath + ",1");

        // Set shell\open\command handler
        using var commandKey = newKey.CreateSubKey(@"shell\open\command");
        commandKey.SetValue("", $"\"{exePath}\" \"%1\"");
    }

    static void SendUriToRunningInstance(string uri)
    {
        try
        {
            using var client = new NamedPipeClientStream(".",
                AppConstants.IpcPipeName, PipeDirection.Out);
            client.Connect(5000); // 2-second timeout

            using var writer = new StreamWriter(client);
            writer.WriteLine(uri);
            writer.Flush();
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to send IPC message: " + ex.Message);
        }
    }

    static async Task IpcServer()
    {
        while (true)
        {
            try
            {
                await using var server = new NamedPipeServerStream(
                    AppConstants.IpcPipeName,
                    PipeDirection.In,
                    1,
                    PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                await server.WaitForConnectionAsync();

                using var reader = new StreamReader(server);

                // Read in a loop until client disconnects
                while (true)
                {
                    // Note that this line waits asyncronously for input
                    // and doesn't return without data, so no need
                    // to introduce a delay.
                    var uri = await reader.ReadLineAsync();
                    if (uri == null)
                        break; // client disconnected

                    if (!string.IsNullOrEmpty(uri))
                    {
                        Avalonia.Threading.Dispatcher.UIThread.Post(
                            () => PlatformHandleUri(uri));
                    }
                }
            }
            catch (Exception ex)
            {
                var logger = GetLogger();
                if (logger == null)
                    Console.WriteLine($"IPC server error: {ex}");
                else
                    logger.LogError("IPC server error: {Exception}", ex);

                // Wait a bit before restarting listener
                await Task.Delay(1000);
            }
        }
    }

    static ILogger? GetLogger()
    {
        try
        {
            return ContainerLocator.Container.Resolve<ILogger<Program>>();
        }
        catch (Exception)
        {
            return null;
        }
    }

    static async void PlatformHandleUri(string uriString)
    {
        try
        {
            var uriHandler = ContainerLocator.Container
                .Resolve<PlatformAppCustomUriHandlerBase>();
            await uriHandler.HandleUri(uriString);
        }
        catch (Exception e)
        {
            var logger = GetLogger();
            if (logger == null)
                Console.WriteLine($"Failed to handle URI {uriString}: {e.GetType().Name}: {e.Message}");
            else
                logger.LogError(e, $"Failed to handle URI {uriString}: {e.GetType().Name}: {e.Message}");
        }
    }
}