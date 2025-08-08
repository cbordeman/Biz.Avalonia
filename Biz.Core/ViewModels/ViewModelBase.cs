using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
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

public abstract class ViewModelBase : BindableBase, IRegionMemberLifetime,
    INotifyDataErrorInfo
{
    protected readonly DryIoc.IContainer Container;
    
    #region INotifyDataErrorInfo
    
    private readonly Dictionary<string, List<string>> errors = new();
    
    public bool HasErrors => errors.Count != 0;
    
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    
    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return Array.Empty<string>();
        }

        return this.errors.TryGetValue(propertyName, out var errors)
            ? errors
            : Array.Empty<string>();
    }
    
    protected void ValidateProperty<T>(T value, string propertyName)
    {
        ClearErrors(propertyName);

        var validationContext = new ValidationContext(this)
        {
            MemberName = propertyName
        };
        var validationResults = new List<ValidationResult>();

        if (Validator.TryValidateProperty(value, 
                validationContext, validationResults)) 
            return;

        foreach (var validationResult in validationResults) 
            AddError(propertyName, validationResult.ErrorMessage ?? string.Empty);
    }

    protected void AddError(string propertyName, string error)
    {
        if (!errors.ContainsKey(propertyName)) 
            errors[propertyName] = new List<string>();

        if (errors[propertyName].Contains(error)) 
            return;

        errors[propertyName].Add(error);
        OnErrorsChanged(propertyName);
    }

    protected void ClearErrors(string propertyName)
    {
        if (errors.Remove(propertyName)) 
            OnErrorsChanged(propertyName);
    }

    protected void ClearAllErrors()
    {
        var properties = errors.Keys.ToList();
        errors.Clear();
        foreach (var property in properties) 
            OnErrorsChanged(property);
    }

    private void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    protected void ValidateAllProperties()
    {
        var properties = GetType().GetProperties()
            .Where(prop => prop.GetCustomAttributes(
                typeof(ValidationAttribute), true).Length != 0);

        foreach (var property in properties)
        {
            var value = property.GetValue(this);
            ValidateProperty(value, property.Name);
        }
    }
    #endregion INotifyDataErrorInfo

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
    
    protected ViewModelBase(DryIoc.IContainer container)
    {
        this.Container = container;
    }
    
    protected virtual bool SetProperty<T>(ref T storage, T value, 
        bool validate = false, 
        [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

        storage = value;
        RaisePropertyChanged(propertyName);
        if (validate) ValidateProperty(value, propertyName);

        return true;
    }
    
    public virtual bool KeepAlive => true;
}
