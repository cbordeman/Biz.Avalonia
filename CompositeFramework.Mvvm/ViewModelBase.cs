using System.Collections;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace CompositeFramework.Mvvm;

/// <summary>
/// Provides binding and validation support, with
/// IsValidating, IsBusy, and IsNotBusy flags.
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged,
    INotifyDataErrorInfo
{
    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Sets a property and returns true if it changed.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="forceValidate">If IsValidating is false, allows
    /// you to force just this property to validate anyway.</param>
    /// <param name="propertyName"></param>
    /// <param name="storage"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
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
        get => field;
        set
        {
            if (SetProperty(ref field, value))
            {
                if (!field)
                    ClearAllErrors();

                // If isValidating is true, still
                // don't validate immediately because
                // we want to be let fields show
                // errors only after the user starts
                // typing in them.
            }
        }
    }
    #endregion IsValidating

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

        if (!IsValidating && !forceValidate)
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
        if (!IsValidating && !forceValidate)
            return;

        if (!errors.ContainsKey(propertyName))
            errors[propertyName] = [];

        if (errors[propertyName].Contains(error))
            return;

        errors[propertyName].Add(error);
        RaiseErrorsChanged(propertyName);
    }

    void ClearErrors(string propertyName)
    {
        if (errors.Remove(propertyName))
            RaiseErrorsChanged(propertyName);
    }

    protected void ClearAllErrors()
    {
        var properties = errors.Keys.ToList();
        foreach (var property in properties)
            ClearErrors(property);
    }

    void RaiseErrorsChanged(string propertyName)
    {
        ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
    }

    protected void ValidateAllProperties(bool forceValidate)
    {
        if (!IsValidating && !forceValidate)
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
}
