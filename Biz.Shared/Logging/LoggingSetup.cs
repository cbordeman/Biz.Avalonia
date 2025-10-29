using Splat.Microsoft.Extensions.Logging;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Biz.Shared.Logging;

public static class LoggingSetup
{
    private static ILoggerFactory? loggerFactory;

    public static void RegisterMicrosoftLoggerFactoryWithSplat()
    {
        try
        {
            loggerFactory = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole()
                    .SetMinimumLevel(LogLevel.Debug);
            });

            // Register Microsoft ILoggerFactory with Splat's locator
            // and enable forwarding Splat logs
            Locator.CurrentMutable
                .UseMicrosoftExtensionsLoggingWithWrappingFullLogger(loggerFactory);

            // Register the ILoggerFactory instance directly for later resolution
            Locator.CurrentMutable.RegisterConstant(loggerFactory);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}