using JetBrains.Annotations;

namespace Services.Config;

[UsedImplicitly]
public class AzureSettings
{
    public string? ConnectionString { get; set; }
    public string? FromEmailAddress { get; set; }
    
}
