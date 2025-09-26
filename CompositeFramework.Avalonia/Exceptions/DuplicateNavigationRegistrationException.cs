using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CompositeFramework.Avalonia.Exceptions;

public class DuplicateNavigationRegistrationException
    <TVm, TView>: Exception
{
    public string LocationName { get; }
    public Type TypeOfViewModel { get; }
    public Type TypeOfView { get; }
    
    public DuplicateNavigationRegistrationException(
        string locationName)
        : base($"Duplicate VM/View binding: {typeof(TVm).Name} " +
               $"to {typeof(TView).Name} with vew name {locationName}")
    {
        LocationName = locationName;
        TypeOfViewModel = typeof(TVm);
        TypeOfView = typeof(TView);
    }
}
