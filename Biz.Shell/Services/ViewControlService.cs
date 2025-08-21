using Avalonia.Controls;
using Biz.Shell.Services;

namespace Biz.Shell.Services;

public class ViewControlService : IFormFactorService
{
    const double PhoneMaxWidth = 600;
    const double TabletMaxWidth = 900;

    double lastKnownWidth = 0;
    
    public FormFactor CurrentFormFactor { get; private set; }

    public event Notify? Changed;
    
    public void NotifyWidthChanged(double width)
    {
        if (Math.Abs(width - lastKnownWidth) < 0.01) return;
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

