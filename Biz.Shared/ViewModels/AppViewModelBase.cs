using CompositeFramework.Core.ViewModels;

namespace Biz.Shared.ViewModels;

public abstract class AppViewModelBase : BindingValidatingBase
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
