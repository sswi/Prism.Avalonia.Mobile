using DryIoc;
using Prism;
using Prism.Ioc;

namespace Prism.DryIoc;

/// <summary>
/// DryIoc-specific bootstrapper for Prism.Avalonia.Mobile applications
/// that prefer the bootstrapper pattern over the Application subclass pattern.
/// </summary>
/// <remarks>
/// <para>The bootstrapper pattern is for scenarios where you cannot or do not
/// want to inherit from <see cref="PrismApplication"/>. In most cases,
/// inheriting from <see cref="PrismApplication"/> is the recommended approach.</para>
/// </remarks>
public abstract class PrismBootstrapper
{
    private IContainerExtension _containerExtension = null!;

    /// <summary>
    /// Gets the DI container provider.
    /// </summary>
    protected IContainerProvider Container => _containerExtension;

    /// <summary>
    /// Creates the container rules for DryIoc.
    /// </summary>
    protected virtual Rules CreateContainerRules()
    {
        return DryIocContainerExtension.DefaultRules;
    }

    /// <summary>
    /// Creates the container extension.
    /// </summary>
    protected virtual IContainerExtension CreateContainerExtension()
    {
        return new DryIocContainerExtension(CreateContainerRules());
    }

    /// <summary>
    /// Runs the bootstrapper. This is the entry point for the bootstrapper pattern.
    /// </summary>
    public virtual void Run()
    {
        ContainerLocator.SetContainerExtension(CreateContainerExtension());
        _containerExtension = ContainerLocator.Current;

        RegisterRequiredTypes(_containerExtension);
        RegisterTypes(_containerExtension);

        var shell = CreateShell();
        if (shell != null)
        {
            Prism.Mvvm.ViewModelLocator.Autowire(shell);
            InitializeShell(shell);
        }

        InitializeModules();
        OnInitialized();
    }

    /// <summary>
    /// Registers required Prism services.
    /// </summary>
    protected virtual void RegisterRequiredTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterRequiredTypes(CreateModuleCatalog());
    }

    /// <summary>
    /// Registers application-specific types.
    /// </summary>
    protected abstract void RegisterTypes(IContainerRegistry containerRegistry);

    /// <summary>
    /// Creates the application shell.
    /// </summary>
    protected abstract Avalonia.AvaloniaObject CreateShell();

    /// <summary>
    /// Initializes the shell.
    /// </summary>
    protected virtual void InitializeShell(Avalonia.AvaloniaObject shell)
    {
    }

    /// <summary>
    /// Creates the module catalog.
    /// </summary>
    protected virtual Prism.Modularity.IModuleCatalog CreateModuleCatalog()
    {
        return new Prism.Modularity.ModuleCatalog();
    }

    /// <summary>
    /// Initializes modules.
    /// </summary>
    protected virtual void InitializeModules()
    {
        Container.RunModuleManager();
    }

    /// <summary>
    /// Called after the application is fully initialized.
    /// </summary>
    protected virtual void OnInitialized()
    {
    }
}
