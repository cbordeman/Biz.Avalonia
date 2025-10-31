using System.Reflection;

namespace Biz.Shared.ViewModels;

public abstract class NavigationAwareViewModelBase
    : FormFactorAwareViewModel, ILocation
{
    public virtual Task OnNavigatedToAsync(NavigationContext ctx)
    {
        return Task.CompletedTask;
    }
    
    public virtual Task<bool> CanNavigateToAsync(NavigationContext navigationContext)
    {
        return Task.FromResult(true);
    }
    
    public Task<bool> OnNavigatingFromAsync(NavigationContext newContext)
    {
        return Task.FromResult(true);
    }

    public virtual Task OnNavigatedFromAsync(NavigationContext navigationContext)
    {
        return Task.CompletedTask;
    }
    
    /// <summary>
    /// Optionally call this in OnNavigatedToAsync() to set any public
    /// properties using the navigation parameters using reflection.
    /// Not called automatically. 
    /// </summary>
    /// <param name="ctx"></param>
    /// <exception cref="InvalidOperationException"></exception>
    protected void SetPropertiesFromParameters(NavigationContext ctx)
    {
        // Public non-static properties with a setter. 
        var properties = this.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.SetMethod != null && p.SetMethod.IsPublic);
        properties.ShouldNotBeNull();
        foreach (var p in NavigationParameters)
        {
            var foundProp = properties.FirstOrDefault(prop => prop.Name == p.Name);
            if (foundProp == null)
                throw new InvalidOperationException($"There is no property on {this.GetType().FullName} named \"{p.Name}\".");
            foundProp.SetValue(this, p.Value);
        }
    }

    public virtual bool PersistInHistory() => true;
    
    public abstract string Area { get; }
    
    public string LocationName { get; set; } = null!;
    public NavParam[] NavigationParameters { get; set; } = null!;
}