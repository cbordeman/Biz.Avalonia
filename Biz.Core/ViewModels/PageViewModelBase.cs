using System.Collections.ObjectModel;
using Avalonia.Media;
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

    #region TitleGeometryResourceName
    public string? TitleGeometryResourceName
    {
        get => titleGeometryResourceName;
        set => SetProperty(ref titleGeometryResourceName, value);
    }
    string? titleGeometryResourceName;        
    #endregion TitleGeometryResourceName
    
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

    public ObservableCollection<IToolbarEntry> ToolbarEntries { get; } = [];
}