using System.IO.Pipes;
using System.Reflection;
using Avalonia;
using Avalonia.Threading;
using Biz.Core;
using Biz.Shared;
using Biz.Shared.Platform;
using Biz.Shell.Desktop.Services;
using CompositeFramework.Core.Extensions;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace Biz.Shell.Desktop;

#pragma warning disable CA1416 // unimportant platform warning

[UsedImplicitly]
sealed class Program
{
    const string IpcPipeName = $"{AppConstants.AppInternalName}.Shell.IpcPipe";

    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        ClientLogging.Initialize();
        
        try
        {
            // ReSharper disable once UnusedVariable
            var singleInstanceMutex =
                new Mutex(
                    true,
                    $"{AppConstants.AppShortName}.SingletonMutex",
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
        catch (Exception e)
        {
            Log.Logger.Error(e, "Error during startup: {Message}.", e.Message);
            throw;
        }
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
                IpcPipeName, PipeDirection.Out);
            client.Connect(2000); // 2-second timeout

            using var writer = new StreamWriter(client);
            writer.WriteLine(uri);
            writer.Flush();
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, $"Failed to send IPC message \"{uri}\": {e.Message}");
        }
    }

    static async Task IpcServer()
    {
        while (true)
        {
            try
            {
                await using var server = new NamedPipeServerStream(
                    IpcPipeName,
                    PipeDirection.In,
                    1,
                    PipeTransmissionMode.Message, PipeOptions.Asynchronous);
                await server.WaitForConnectionAsync();

                using var reader = new StreamReader(server);

                // Read in a loop until client disconnects
                while (true)
                {
                    // This line waits asyncronously for input
                    // and doesn't return without data, so no need
                    // to introduce a delay.
                    var uri = await reader.ReadLineAsync();
                    if (uri == null)
                        break; // client disconnected

                    if (!string.IsNullOrEmpty(uri))
                    {
                        Dispatcher.UIThread.Post(
                            () => PlatformHandleUri(uri));
                    }
                }
            }
            catch (Exception e)
            {
                Log.Logger.Error(e, "IPC server error: {Message}.", e.Message);
                
                // Wait a bit before restarting listener
                await Task.Delay(1000);
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }

    static async void PlatformHandleUri(string uriString)
    {
        try
        {
            var uriHandler = Locator.Current
                .Resolve<PlatformAppCustomUriHandlerBase>();
            await uriHandler.HandleUri(uriString);
        }
        catch (Exception e)
        {
            Log.Logger.Error(e, "Failed to handle URI {uriString}: {ExceptionType}: {Message}",
                uriString, e.GetType().Name, e.Message);
        }
    }
}
