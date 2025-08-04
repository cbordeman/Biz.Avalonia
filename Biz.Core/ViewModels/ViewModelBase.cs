using System;
using Biz.Core.Services;
using Biz.Shell.Services;

namespace Biz.Shell.ViewModels;

public abstract class FormFactorAwareViewModel : ViewModelBase
{
    public IFormFactorService FormFactorService { get; }

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
    
    protected FormFactorAwareViewModel(IContainer container) : base(container)
    {
        this.FormFactorService = container.Resolve<IFormFactorService>();
        FormFactorService.Changed += FormFactorServiceOnChanged;
    }

    void FormFactorServiceOnChanged()
    {
        switch (this.FormFactorService.CurrentFormFactor)
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
}

public abstract class ViewModelBase : BindableBase, IRegionMemberLifetime
{
    protected readonly IContainer Container;
    
    #region IsBusy
    bool isBusy;
    public bool IsBusy
    {
        get => isBusy;
        set
        {
            if (SetProperty(ref isBusy, value)) 
                RaisePropertyChanged(nameof(IsNotBusy));
        }
    }
    public bool IsNotBusy { get; set; }
    #endregion IsBusy

    protected ViewModelBase(IContainer container)
    {
        this.Container = container;
    }
    
    public virtual bool KeepAlive => true;
}
