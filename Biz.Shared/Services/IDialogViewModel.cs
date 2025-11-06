using System.ComponentModel;

namespace Biz.Shared.Services;

public interface IDialogViewModel : INotifyPropertyChanged
{
    Task OpenedAsync(params NavParam[] parameters);
}

public interface IDialogViewModel<T> : IDialogViewModel
{
    T Result { get; set; }
}
