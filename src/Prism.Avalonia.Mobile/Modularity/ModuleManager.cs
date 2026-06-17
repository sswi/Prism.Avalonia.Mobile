namespace Prism.Modularity;

/// <summary>
/// Component responsible for coordinating module loading and initialization.
/// </summary>
public class ModuleManager : IModuleManager, IDisposable
{
    private readonly IModuleInitializer _moduleInitializer;
    private readonly IModuleCatalog _moduleCatalog;
    private bool _isDisposed;

    /// <inheritdoc />
    public IEnumerable<IModuleInfo> Modules => _moduleCatalog.Modules;

    /// <inheritdoc />
    public event EventHandler<ModuleDownloadProgressChangedEventArgs>? ModuleDownloadProgressChanged;
    /// <inheritdoc />
    public event EventHandler<LoadModuleCompletedEventArgs>? LoadModuleCompleted;

    /// <summary>
    /// Initializes a new instance of <see cref="ModuleManager"/>.
    /// </summary>
    public ModuleManager(IModuleInitializer moduleInitializer, IModuleCatalog moduleCatalog)
    {
        _moduleInitializer = moduleInitializer ?? throw new ArgumentNullException(nameof(moduleInitializer));
        _moduleCatalog = moduleCatalog ?? throw new ArgumentNullException(nameof(moduleCatalog));
    }

    /// <inheritdoc />
    public void Run()
    {
        LoadModulesWhenAvailable();
    }

    /// <inheritdoc />
    public void LoadModule(string moduleName)
    {
        var modules = _moduleCatalog.Modules
            .Where(m => m.ModuleName == moduleName)
            .ToList();

        if (modules.Count == 0)
            throw new ModuleNotFoundException(moduleName);

        foreach (var module in modules)
        {
            if (module.State == ModuleState.NotStarted)
            {
                module.State = ModuleState.Initializing;
                _moduleInitializer.Initialize(module);
                module.State = ModuleState.Initialized;

                LoadModuleCompleted?.Invoke(this, new LoadModuleCompletedEventArgs(module, null));
            }
        }
    }

    /// <summary>
    /// Loads all modules marked with <see cref="InitializationMode.WhenAvailable"/>.
    /// </summary>
    protected virtual void LoadModulesWhenAvailable()
    {
        var whenAvailable = _moduleCatalog.Modules
            .Where(m => m.InitializationMode == InitializationMode.WhenAvailable
                     && m.State == ModuleState.NotStarted);

        foreach (var module in whenAvailable)
        {
            if (module.State == ModuleState.NotStarted)
            {
                module.State = ModuleState.Initializing;
                _moduleInitializer.Initialize(module);
                module.State = ModuleState.Initialized;

                LoadModuleCompleted?.Invoke(this, new LoadModuleCompletedEventArgs(module, null));
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!_isDisposed)
        {
            _isDisposed = true;
        }
    }
}
