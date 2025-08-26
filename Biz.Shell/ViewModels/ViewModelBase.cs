using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Biz.Shell.ViewModels;

public abstract class ViewModelBase : BindableBase, 
    IRegionMemberLifetime, INotifyDataErrorInfo
{
    protected readonly DryIoc.IContainer Container;
    protected readonly IRegionManager? RegionManager;
    
    #region INotifyDataErrorInfo
    readonly Dictionary<string, List<string>> errors = new();
    
    public bool HasErrors => errors.Count != 0;
    
    public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;
    
    public IEnumerable GetErrors(string? propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
        {
            return Array.Empty<string>();
        }

        return this.errors.TryGetValue(propertyName, out var getErrors)
            ? getErrors : [];
    }

    void ValidateProperty<T>(T value, string propertyName)
    {
        ClearErrors(propertyName);

        var validationContext = new ValidationContext(this)
        {
            MemberName = propertyName
        };
        var validationResults = new List<ValidationResult>();

        if (Validator.TryValidateProperty(value, validationContext, validationResults)) 
            return;

        foreach (var validationResult in validationResults) 
            AddError(propertyName, validationResult.ErrorMessage ?? string.Empty);
    }

    void AddError(string propertyName, string error)
    {
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
    public bool IsNotBusy => !IsBusy;
    #endregion IsBusy
    
    protected ViewModelBase(DryIoc.IContainer container)
    {
        this.Container = container;
        this.RegionManager = container.Resolve<IRegionManager>();
    }
    
    protected override bool SetProperty<T>(ref T storage, T value, 
        [CallerMemberName] string? propertyName = null!)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

        storage = value;
        RaisePropertyChanged(propertyName);
        ValidateProperty(value, propertyName!);

        return true;
    }

    #region OpenUrlCommand
    AsyncDelegateCommandWithParam<string>? openUrlCommand;
    public AsyncDelegateCommandWithParam<string> OpenUrlCommand => openUrlCommand ??= new AsyncDelegateCommandWithParam<string>(ExecuteOpenUrlCommand, CanOpenUrlCommand);
    bool CanOpenUrlCommand(string url) => true;
    Task ExecuteOpenUrlCommand(string url)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Process.Start(new ProcessStartInfo(url.Replace("&", "^&")) { UseShellExecute = true });
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