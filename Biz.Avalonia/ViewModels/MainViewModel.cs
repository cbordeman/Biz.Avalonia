using System.Collections.Generic;

namespace Biz.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    #region Greeting
    string greeting = "Welcome to Avalonia";
    public string Greeting
    {
        get => greeting;
        set => SetProperty(ref greeting, value);
    }
    #endregion Greeting

}
