using System.ComponentModel;

namespace CompositeFramework.Core.Navigation;

public record ViewModelViewBinding
{
    public Type ViewModelType { get; init; }
    public Type ViewType { get; init; }
    
    public ViewModelViewBinding(Type ViewModelType, Type ViewType)
    {
        if (!ViewModelType.IsAssignableTo(typeof(INotifyPropertyChanged)))
            throw new ArgumentException("ViewModelType must implement INotifyPropertyChanged", nameof(ViewModelType));
        
        this.ViewModelType = ViewModelType;
        this.ViewType = ViewType;
    }
    
    // ReSharper disable ParameterHidesMember
    // ReSharper disable InconsistentNaming
    public void Deconstruct(out Type ViewModelType, out Type ViewType)
    {
        ViewModelType = this.ViewModelType;
        ViewType = this.ViewType;
    }
}
