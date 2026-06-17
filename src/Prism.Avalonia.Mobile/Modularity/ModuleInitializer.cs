using Prism.Ioc;

namespace Prism.Modularity;

public class ModuleInitializer : IModuleInitializer
{
    private readonly IContainerExtension _container;

    public ModuleInitializer(IContainerExtension containerExtension)
        => _container = containerExtension ?? throw new ArgumentNullException(nameof(containerExtension));

    public void Initialize(IModuleInfo moduleInfo)
    {
        if (moduleInfo == null) throw new ArgumentNullException(nameof(moduleInfo));

        IModule? instance = null;
        try
        {
            instance = CreateModule(moduleInfo);
            if (instance != null) { instance.RegisterTypes(_container); instance.OnInitialized(_container); }
        }
        catch (Exception ex) { HandleError(moduleInfo, instance?.GetType().Assembly.FullName, ex); }
    }

    public virtual void HandleError(IModuleInfo info, string? assemblyName, Exception ex)
    {
        if (ex is ModuleInitializeException) throw ex;
        if (!string.IsNullOrEmpty(assemblyName))
            throw new ModuleInitializeException(info.ModuleName, assemblyName, ex.Message, ex);
        throw new ModuleInitializeException(info.ModuleName, ex.Message, ex);
    }

    protected virtual IModule? CreateModule(IModuleInfo info)
    {
        var type = Type.GetType(info.ModuleType, throwOnError: false);
        if (type is null)
            throw new ModuleInitializeException($"Type '{info.ModuleType}' not found. Ensure AOT registration.");
        return (IModule)_container.Resolve(type);
    }
}
