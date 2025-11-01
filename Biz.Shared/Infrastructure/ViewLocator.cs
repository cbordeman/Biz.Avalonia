using System.ComponentModel;
using Avalonia.Controls.Templates;

namespace Biz.Shared.Infrastructure;

public class ViewLocator : IDataTemplate
{
    public Control? Build(object? param)
    {
        if (param is null)
            return null;
        
        var vmType = param.GetType();
        string name = vmType.FullName!.Replace("ViewModel", "View", StringComparison.Ordinal);
        var vwType = vmType.Assembly.GetType(name);

        if (vwType == null)
            return new TextBlock
            {
                Text = $"Not Found: {name}"
            };

        try
        {
            var control = Locator.Current.Resolve(vwType);
            return (Control)control;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new TextBlock
            {
                Text = $"Couldn't resolve: {vwType.AssemblyQualifiedName}"
            };
        }
    }

    public bool Match(object? data)
    {
        return data is INotifyPropertyChanged;
    }
}
