namespace Biz.Shared;

public static class ClientLogging
{
    public static void Initialize()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Debug()
            .CreateLogger();
    }
   
}