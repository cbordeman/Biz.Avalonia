using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using IContainer = DryIoc.IContainer;

namespace Biz.Shell.ViewModels;

public abstract class ViewModelBase : INotifyPropertyChanged,
    IRegionMemberLifetime, INotifyDataErrorInfo
{
    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual bool SetProperty<T>(
        ref T storage, T value,
        bool forceValidate = false,
        [CallerMemberName] string? propertyName = null!)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

        storage = value;
        RaisePropertyChanged(propertyName);
        ValidateProperty(value, propertyName!, forceValidate);

        return true;
    }
    
    protected virtual bool SetPropertyForceValidate<T>(
        ref T storage, T value,
        bool forceValidate = false,
        [CallerMemberName] string? propertyName = null!)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

        storage = value;
        RaisePropertyChanged(propertyName);
        ValidateProperty(value, propertyName!, forceValidate);

        return true;
    }

    
    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
    }

    protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        PropertyChanged?.Invoke(this, args);
    }
    #endregion INotifyPropertyChanged

    #region IsValidating
    public bool IsValidating
    {
        get => isValidating;
        set
        {
            if (SetProperty(ref isValidating, value))
            {
                if (!isValidating)
                    ClearAllErrors();

                // If isValidating is true, still
                // don't validate immediately because
                // we want to be let fields show
                // errors only after the user starts
                // typing in them.
            }
        }
    }
    bool isValidating;
    #endregion IsValidating

    protected readonly IContainer Container;
    protected IMainRegionNavigationService NavigationService
    {
        get;
        private set;
    }

    #region INotifyDataErrorInfo
    readonly Dictionary<string, List<string>> errors = new();

    public bool HasErrors => errors.Count != 0;

    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return Array.Empty<string>();

        return this.errors.TryGetValue(propertyName, out var getErrors)
            ? getErrors : [];
    }

    void ValidateProperty<T>(
        T value, 
        string propertyName, 
        bool forceValidate)
    {
        ClearErrors(propertyName);

        if (!isValidating && !forceValidate)
            return;

        var validationContext = new ValidationContext(this)
        {
            MemberName = propertyName
        };
        var validationResults = new List<ValidationResult>();

        if (Validator.TryValidateProperty(value, validationContext, validationResults))
            return;

        foreach (var validationResult in validationResults)
            AddError(propertyName, validationResult.ErrorMessage ?? string.Empty,
                forceValidate);
    }

    void AddError(string propertyName, string error,
        bool forceValidate)
    {
        if (!isValidating && !forceValidate)
            return;

        if (!errors.ContainsKey(propertyName))
            errors[propertyName] = [];

        if (errors[propertyName].Contains(error))
            return;

        errors[propertyName].Add(error);
        OnErrorsChanged(propertyName);
    }

    void ClearErrors(string propertyName)
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

    void OnErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    protected void ValidateAllProperties(bool forceValidate)
    {
        if (!isValidating && !forceValidate)
            return;

        var properties = GetType().GetProperties()
            .Where(prop => prop.GetCustomAttributes(
                typeof(ValidationAttribute), true).Length != 0);

        foreach (var property in properties)
        {
            var value = property.GetValue(this);
            ValidateProperty(value, property.Name, forceValidate);
        }
    }
    #endregion INotifyDataErrorInfo

    #region IsBusy
    // ReSharper disable once MemberCanBeProtected.Global
    public bool IsBusy
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
                RaisePropertyChanged(nameof(IsNotBusy));
        }
    }
    public bool IsNotBusy => !IsBusy;
    #endregion IsBusy

    protected ViewModelBase(IContainer container)
    {
        this.Container = container;
        this.NavigationService = container.Resolve<IMainRegionNavigationService>();
    }
    
    #region OpenUrlCommand
    public AsyncDelegateCommandWithParam<string>? OpenUrlCommand => field ??= new AsyncDelegateCommandWithParam<string>(ExecuteOpenUrlCommand, CanOpenUrlCommand);
    protected virtual bool CanOpenUrlCommand(string url) => true;
    Task ExecuteOpenUrlCommand(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo(url.Replace("&", "^&"))
            {
                UseShellExecute = true
            });
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            Process.Start("xdg-open", url);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            Process.Start("open", url);
        }

        return Task.CompletedTask;
    }
    #endregion OpenUrlCommand

    public virtual bool KeepAlive => true;
}
