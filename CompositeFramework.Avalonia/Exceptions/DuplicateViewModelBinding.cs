using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CompositeFramework.Avalonia.Exceptions;

public partial class DuplicateViewModelBindingException
    <TVm, TView>: Exception
{
    public Type TypeOfViewModel { get; }
    public Type TypeOfView { get; }
    
    public DuplicateViewModelBindingException()
        : base($"Duplicate VM/View binding: {typeof(TVm).Name} " +
               $"to {typeof(TView).Name}.")
    {
        TypeOfViewModel = typeof(TVm);
        TypeOfView = typeof(TView);
    }
}
