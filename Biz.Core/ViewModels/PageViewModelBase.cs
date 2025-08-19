using System.Collections.ObjectModel;
using Biz.Core.ViewModels.Toolbar;
using DryIoc;

namespace Biz.Core.ViewModels;

public abstract class PageViewModelBase(IContainer container)
    : NavigationAwareViewModelBase(container)
{
    #region Title
    public string? Title
    {
        get => title;
        set => SetProperty(ref title, value);
    }
    string? title;        
    #endregion Title

    #region IsMinimalUi
    public bool IsMinimalUi
    {
        get => isMinimalUi;
        set
        {
            if (SetProperty(ref isMinimalUi, value))
                RaisePropertyChanged(nameof(IsFullUi));
        }
    }
    bool isMinimalUi;
    public bool IsFullUi => !IsMinimalUi;
    #endregion MinimalUi

    public virtual ObservableCollection<ToolbarEntry> ToolbarEntries { get; } = [];
}