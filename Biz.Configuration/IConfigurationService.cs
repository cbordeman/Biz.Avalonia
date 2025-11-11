namespace Biz.Configuration;

public interface IConfigurationService
{
    AuthSettings Authentication { get; }
    MapsSettings Maps { get; }
    ServerSettings Server { get; }
}
