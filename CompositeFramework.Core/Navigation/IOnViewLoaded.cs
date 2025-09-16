namespace CompositeFramework.Core.Navigation;

/// <summary>
/// When used with UserControlEx (or invoked on your view's OnLoaded(),
/// provides way to execute code after the view has been fully loaded.
/// </summary>
public interface IOnViewLoaded
{
    void OnViewLoaded();
}