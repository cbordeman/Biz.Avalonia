namespace Biz.Shell.ViewModels;

public abstract class AppViewModelBase : ViewModelBase
{
    #region OpenUrlCommand
    public AsyncRelayCommand<string?>? OpenUrlCommand =>
        field ??= new AsyncRelayCommand<string?>(
            ExecuteOpenUrlCommand, CanOpenUrlCommand);
    static bool CanOpenUrlCommand(string? url) => true;
    Task ExecuteOpenUrlCommand(string? url)
    {
        url?.OpenUrlCrossPlatform();
        return Task.CompletedTask;
    }
    #endregion OpenUrlCommand
}
