using CompositeFramework.Modules;

namespace Biz.Shell.Platform;

public interface IPlatformModuleCatalogService
{
    void ConfigureModuleCatalog(IModuleIndex moduleIndex);
}