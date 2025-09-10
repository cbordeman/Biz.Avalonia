namespace Modularity;

public class ModuleInstance
{
    public ModuleData ModuleData { get; }
    public IModule Instance { get; }
    
    public ModuleInstance(ModuleData moduleData, IModule instance)
    {
        ModuleData = moduleData;
        Instance = instance;
    }
}
