using System.Collections.ObjectModel;

namespace Prism.Modularity;

public class ModuleInfo : IModuleInfo
{
    public ModuleInfo() { }

    public ModuleInfo(string moduleType, string moduleName)
        : this(moduleType, moduleName, InitializationMode.WhenAvailable) { }

    public ModuleInfo(string moduleType, string moduleName, InitializationMode initMode)
    {
        ModuleType = moduleType ?? throw new ArgumentNullException(nameof(moduleType));
        ModuleName = moduleName ?? throw new ArgumentNullException(nameof(moduleName));
        InitializationMode = initMode;
    }

    public string ModuleName { get; set; } = string.Empty;
    public string ModuleType { get; set; } = string.Empty;
    public InitializationMode InitializationMode { get; set; }
    public string? Ref { get; set; }
    public ModuleState State { get; set; }
    public Collection<string> DependsOn { get; set; } = new();
}
