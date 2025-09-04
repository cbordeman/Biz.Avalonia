namespace Biz.Shell.Views;

public partial class ProgressBorder : UserControl
{
    public ProgressBorder()
    {
        InitializeComponent();
    }

    #region IsBusy
    private bool isBusy;
    public static readonly DirectProperty<ProgressBorder, bool> IsBusyProperty = AvaloniaProperty.RegisterDirect<ProgressBorder, bool>(
        nameof(IsBusy), o => o.IsBusy, (o, v) => o.IsBusy = v);
    public bool IsBusy
    {
        get => isBusy;
        set => SetAndRaise(IsBusyProperty, ref isBusy, value);
    }
    #endregion IsBusy
    
    #region CancelCommand
    private ICommand cancelCommand;
    public static readonly DirectProperty<ProgressBorder, ICommand> CancelCommandProperty = AvaloniaProperty.RegisterDirect<ProgressBorder, ICommand>(
        nameof(CancelCommand), o => o.CancelCommand, (o, v) => o.CancelCommand = v);
    public ICommand CancelCommand
    {
        get => cancelCommand;
        set => SetAndRaise(CancelCommandProperty, ref cancelCommand, value);
    }
    #endregion CancelCommand
    
    #region CancelContent
    private object cancelContent;
    public static readonly DirectProperty<ProgressBorder, object> cancelContentProperty = AvaloniaProperty.RegisterDirect<ProgressBorder, object>(
        nameof(CancelContent), o => o.CancelContent, (o, v) => o.CancelContent = v);
    public object CancelContent
    {
        get => cancelContent;
        set => SetAndRaise(cancelContentProperty, ref cancelContent, value);
    }
    #endregion CancelContent
}

