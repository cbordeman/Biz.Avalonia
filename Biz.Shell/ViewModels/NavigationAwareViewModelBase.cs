using System.Reflection;

namespace Biz.Shell.ViewModels;

public abstract class NavigationAwareViewModelBase
    : FormFactorAwareViewModel, ILocation
{
    public virtual Task OnNavigatedToAsync(NavigationContext ctx)
    {
        return Task.CompletedTask;
    }
    
    public Task<bool> OnNavigatingFromAsync(NavigationContext newContext)
    {
        return Task.FromResult(true);
    }

    public virtual Task OnNavigatedFromAsync(NavigationContext navigationContext)
    {
        return Task.CompletedTask;
    }
    
    protected void SetPropertiesFromParameters(NavigationContext ctx)
    {
        // Public non-static properties with a setter. 
        var properties = this.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.SetMethod != null && p.SetMethod.IsPublic);
        properties.ShouldNotBeNull();
        foreach (var p in ctx.Parameters)
        {
            var foundProp = properties.FirstOrDefault(prop => prop.Name == p.Key);
            if (foundProp == null)
                throw new InvalidOperationException($"There is no property on {this.GetType().FullName} named \"{p.Key}\".");
            foundProp.SetValue(this, p.Value);
        }
    }

    public virtual bool PersistInHistory() => true;
    
    public abstract string Area { get; }
}