using Biz.Core.Services;

namespace Biz.Core.ViewModels;

public abstract class FormFactorAwareViewModel : ViewModelBase, IDisposable
{
    readonly IFormFactorService formFactorService;
    
    #region CurrentFormFactor
    public FormFactor CurrentFormFactor
    {
        get => currentFormFactor;
        set => SetProperty(ref currentFormFactor, value);
    }
    FormFactor currentFormFactor;        
    #endregion CurrentFormFactor
    
    #region IsPhone
    public bool IsPhone
    {
        get => isPhone;
        set => SetProperty(ref isPhone, value);
    }
    bool isPhone;        
    #endregion IsPhone

    #region IsTablet
    public bool IsTablet
    {
        get => isTablet;
        set => SetProperty(ref isTablet, value);
    }
    bool isTablet;        
    #endregion IsTablet

    #region IsDesktop
    public bool IsDesktop
    {
        get => isDesktop;
        set => SetProperty(ref isDesktop, value);
    }
    bool isDesktop;        
    #endregion IsDesktop

    #region IsTabletOrDesktop
    public bool IsTabletOrDesktop
    {
        get => isTabletOrDesktop;
        set => SetProperty(ref isTabletOrDesktop, value);
    }
    bool isTabletOrDesktop;        
    #endregion IsTabletOrDesktop
    
    protected FormFactorAwareViewModel(DryIoc.IContainer container) : base(container)
    {
        this.formFactorService = container.Resolve<IFormFactorService>();
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