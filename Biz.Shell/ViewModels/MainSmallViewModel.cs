namespace Biz.Shell.ViewModels;

public class MainSmallViewModel : MainViewModelBase
{
    #region Greeting
    string greeting = "Welcome to Shell (Small)";
    public string Greeting
    {
        get => greeting;
        set => SetProperty(ref greeting, value);
    }
    #endregion Greeting

    public MainSmallViewModel(IContainer container) : base(container)
    {
    }
}