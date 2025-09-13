namespace CompositeFramework.Modules;

public class ModuleDataAndInstance
{
    public ModuleMetadata ModuleData { get; }
    public IModule Instance { get; }
    
    public ModuleDataAndInstance(ModuleMetadata moduleData, IModule instance)
    {
        ModuleData = moduleData;
        Instance = instance;
    }
}
