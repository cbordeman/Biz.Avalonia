using System.ComponentModel;

namespace CompositeFramework.Core.Dialogs;

public interface IDialogViewModel : INotifyPropertyChanged
{
    Task OpenedAsync(params NavParam[] parameters);
    Task ClosedAsync();
}
