using IFormFactorService = Biz.Shell.Services.IFormFactorService;

namespace Biz.Shell.ViewModels;

public abstract class FormFactorAwareViewModel : AppViewModelBase, IDisposable
{
    readonly IFormFactorService formFactorService;
    
    #region CurrentFormFactor
    public FormFactor CurrentFormFactor
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion CurrentFormFactor
    
    #region IsPhone
    public bool IsPhone
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion IsPhone

    #region IsTablet
    public bool IsTablet
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion IsTablet

    #region IsDesktop
    public bool IsDesktop
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion IsDesktop

    #region IsTabletOrDesktop
    public bool IsTabletOrDesktop
    {
        get;
        set => SetProperty(ref field, value);
    }
    #endregion IsTabletOrDesktop
    
    protected FormFactorAwareViewModel()
    {
        this.formFactorService = Locator.Current.Resolve<IFormFactorService>();
        formFactorService.Changed += FormFactorServiceOnChanged;
    }

    void FormFactorServiceOnChanged()
    {
        CurrentFormFactor = formFactorService.CurrentFormFactor;
        
        switch (this.formFactorService.CurrentFormFactor)
        {
            case FormFactor.NotSet:
                IsPhone = false;
                IsTablet = false;
                IsDesktop = false;
                IsTabletOrDesktop = false;
                break;
            case FormFactor.Phone:
                IsPhone = true;
                IsTablet = false;
                IsDesktop = false;
                IsTabletOrDesktop = false;
                break;
            case FormFactor.Tablet:
                IsPhone = false;
                IsTablet = true;
                IsDesktop = false;
                IsTabletOrDesktop = true;
                break;
            case FormFactor.Desktop:
                IsPhone = false;
                IsTablet = false;
                IsDesktop = true;
                IsTabletOrDesktop = true;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public virtual void Dispose()
    {
        formFactorService.Changed -= FormFactorServiceOnChanged;
    }
}