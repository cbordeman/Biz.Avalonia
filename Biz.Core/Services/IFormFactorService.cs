namespace Biz.Core.Services;

public enum FormFactor { NotSet, Phone, Tablet, Desktop }

public delegate void Notify();

public interface IFormFactorService
{
    FormFactor CurrentFormFactor { get; }
    event Notify? Changed;
    void NotifyWidthChanged(double width);
}