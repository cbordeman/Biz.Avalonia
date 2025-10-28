namespace Biz.Shared.Platform;

public interface IPlatformModuleCatalogService
{
    void ConfigureModuleCatalog(IModuleIndex moduleIndex);
}