using CommunityToolkit.Mvvm.ComponentModel;

namespace Biz.Avalonia.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    #region IsBusy
    bool isBusy = false;
    public bool IsBusy
    {
        get => isBusy;
        set
        {
            if (SetProperty(ref isBusy, value))
                OnPropertyChanged(nameof(IsNotBusy));
        }
    }
    public bool IsNotBusy { get; set; }
    #endregion IsBusy

}
