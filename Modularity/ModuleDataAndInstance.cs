namespace Modularity;

public class ModuleDataAndInstance
{
    public ModuleData ModuleData { get; }
    public IModule Instance { get; }
    
    public ModuleDataAndInstance(ModuleData moduleData, IModule instance)
    {
        ModuleData = moduleData;
        Instance = instance;
    }
}
