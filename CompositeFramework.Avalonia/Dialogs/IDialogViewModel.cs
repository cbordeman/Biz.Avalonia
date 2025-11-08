namespace CompositeFramework.Avalonia.Dialogs;

public interface IDialogViewModel : INotifyPropertyChanged
{
    Task OpenedAsync(params NavParam[] parameters);
    Task ClosedAsync();
}
