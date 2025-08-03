using Avalonia.Controls;
using Biz.Shell.Core.Services;

namespace Biz.Shell.Services;

public class FormFactorService : BindableBase, IFormFactorService
{
    const double PhoneMaxWidth = 600;
    const double TabletMaxWidth = 900;

    double lastKnownWidth = 0;
    
    public FormFactor CurrentFormFactor { get; private set; }

    public event Notify? Changed;
    
    public void NotifyWidthChanged(double width)
    {
        if (width == lastKnownWidth) return;
        lastKnownWidth = width;
        
        FormFactor newFormFactor;
        switch (width)
        {
            case <= PhoneMaxWidth:
                newFormFactor = FormFactor.Phone;
                break;
            case <= TabletMaxWidth:
                newFormFactor = FormFactor.Tablet;
                break;
            default:
                newFormFactor = FormFactor.Desktop;
                break;
        }

        if (CurrentFormFactor != newFormFactor)
        {
            CurrentFormFactor = newFormFactor;
            Changed?.Invoke();
        }
    }
}

