#nullable enable
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Prism.Common;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Adapters;
using Prism.Navigation.Regions.Behaviors;

namespace Prism;

/// <summary>
/// Base application class for Prism.Avalonia.Mobile. Provides page-based
/// navigation using Avalonia 12's <c>NavigationPage</c>, Region-based
/// composition, modularity, and MVVM support — with AOT-safe conventions.
/// </summary>
/// <remarks>
/// <para>Usage:</para>
/// <code>
/// public class App : PrismApplication
/// {
///     protected override IContainerExtension CreateContainerExtension()
///         => new DryIocContainerExtension();
///
///     protected override AvaloniaObject CreateShell()
///         => Container.Resolve&lt;NavigationPage&gt;();
///
///     protected override void RegisterTypes(IContainerRegistry cr)
///     {
///         cr.RegisterForNavigation&lt;MainPage, MainViewModel&gt;("MainPage");
///         cr.RegisterForNavigation&lt;DetailPage, DetailViewModel&gt;("DetailPage");
///     }
/// }
/// </code>
/// </remarks>
public abstract class PrismApplication : Application
{
    private IContainerExtension _containerExtension = null!;
    private IModuleCatalog _moduleCatalog = null!;

    // ── Public state ─────────────────────────────────────────────────

    /// <summary>
    /// Gets or sets the main view (shell) of the application.
    /// </summary>
    public AvaloniaObject? MainView { get; private set; }

    /// <summary>
    /// Gets the DI container provider used to resolve services.
    /// </summary>
    public IContainerProvider Container => _containerExtension;

    // ── Initialize (called by Avalonia) ──────────────────────────────

    /// <inheritdoc />
    public override void Initialize()
    {
        base.Initialize();

        try
        {
            AvaloniaXamlLoader.Load(this);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLine($"[Prism] XAML load skipped: {ex.Message}");
        }

        // Step 1: Configure ViewModelLocator (AOT-safe)
        ConfigureViewModelLocator();

        // Step 2: Create the DI container
        ContainerLocator.SetContainerExtension(CreateContainerExtension());
        _containerExtension = ContainerLocator.Current;

        // Step 3: Module catalog
        _moduleCatalog = CreateModuleCatalog();

        // Step 4: Register Prism required types
        RegisterRequiredTypes(_containerExtension);

        // Step 5: Register user types
        RegisterTypes(_containerExtension);

        // Step 6: Configure modules
        ConfigureModuleCatalog(_moduleCatalog);

        // Step 7: Register framework exception types
        RegisterFrameworkExceptionTypes();

        // Step 8: Configure Region adapters and behaviors
        var regionAdapterMappings = _containerExtension.Resolve<RegionAdapterMappings>();
        ConfigureRegionAdapterMappings(regionAdapterMappings);

        var regionBehaviorFactory = _containerExtension.Resolve<IRegionBehaviorFactory>();
        ConfigureDefaultRegionBehaviors(regionBehaviorFactory);

        // Step 9: Create the shell
        var shell = CreateShell();
        if (shell != null)
        {
            AutowireViewModelTree(shell);
            AwaitInitialPageLifecycle(shell);
            var rm = _containerExtension.Resolve<IRegionManager>();
            RegionManager.SetRegionManager(shell, rm);
            RegionManager.UpdateRegions(rm);
            InitializeShell(shell);
        }

        // Step 10: Initialize modules
        InitializeModules();

        // Step 11: Hook user's OnInitialized
        OnInitialized();
    }

    /// <inheritdoc />
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            if (MainView is Window window)
                desktop.MainWindow = window;
            else if (MainView is Control control)
                desktop.MainWindow = new Window { Content = control };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            singleView.MainView = MainView as Control;
        }

        base.OnFrameworkInitializationCompleted();
    }

    // ── Abstract methods (must be implemented by user) ───────────────

    /// <summary>
    /// Creates the DI container extension (e.g., DryIocContainerExtension).
    /// </summary>
    protected abstract IContainerExtension CreateContainerExtension();

    /// <summary>
    /// Creates the application shell — typically a <c>NavigationPage</c>,
    /// <c>Window</c>, or other top-level control.
    /// </summary>
    protected abstract AvaloniaObject CreateShell();

    // ── Virtual methods (user can override) ──────────────────────────

    /// <summary>
    /// Configures the ViewModel locator. Default enables AOT-safe mode.
    /// Override to customize.
    /// </summary>
    protected virtual void ConfigureViewModelLocator()
    {
        PrismInitializationExtensions.ConfigureViewModelLocator();
    }

    /// <summary>
    /// Registers required Prism services. Override to add or replace defaults.
    /// </summary>
    protected virtual void RegisterRequiredTypes(IContainerRegistry containerRegistry)
    {
        containerRegistry.RegisterRequiredTypes(_moduleCatalog);
    }

    /// <summary>
    /// Registers application-specific types (pages, services, etc.).
    /// </summary>
    /// <param name="containerRegistry">The container registry.</param>
    protected virtual void RegisterTypes(IContainerRegistry containerRegistry)
    {
    }

    /// <summary>
    /// Creates the module catalog.
    /// </summary>
    protected virtual IModuleCatalog CreateModuleCatalog()
    {
        return new ModuleCatalog();
    }

    /// <summary>
    /// Configures the module catalog with application modules.
    /// </summary>
    protected virtual void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
    {
    }

    /// <summary>
    /// Registers framework-specific exception types used for unwrapping
    /// container exceptions.
    /// </summary>
    protected virtual void RegisterFrameworkExceptionTypes()
    {
    }

    /// <summary>
    /// Configures the default region adapter mappings.
    /// Override to add custom mappings for additional control types.
    /// </summary>
    protected virtual void ConfigureRegionAdapterMappings(RegionAdapterMappings mappings)
    {
    }

    /// <summary>
    /// Configures the default region behaviors. Override to add or remove behaviors.
    /// </summary>
    protected virtual void ConfigureDefaultRegionBehaviors(IRegionBehaviorFactory behaviors)
    {
        PrismInitializationExtensions.RegisterDefaultRegionBehaviors(behaviors);
    }

    /// <summary>
    /// Initializes the shell after it has been created.
    /// </summary>
    protected virtual void InitializeShell(AvaloniaObject shell)
    {
        MainView = shell;
    }

    /// <summary>
    /// Called after the application has been fully initialized.
    /// </summary>
    protected virtual void OnInitialized()
    {
        (MainView as Window)?.Show();
    }

    /// <summary>
    /// Initializes all modules in the module catalog.
    /// </summary>
    protected virtual void InitializeModules()
    {
        PrismInitializationExtensions.RunModuleManager(Container);
    }

    // ── Auto-wire helper ─────────────────────────────────────────────

    /// <summary>
    /// Recursively auto-wires ViewModels for the shell and its content tree.
    /// </summary>
    private static void AutowireViewModelTree(AvaloniaObject view)
    {
        ViewModelLocator.Autowire(view);

        // Recurse into content
        if (view is Avalonia.Controls.ContentControl cc && cc.Content is AvaloniaObject content)
            AutowireViewModelTree(content);

        // Recurse into NavigationPage content
        if (view is Avalonia.Controls.NavigationPage np && np.Content is AvaloniaObject npContent)
            AutowireViewModelTree(npContent);
    }

    /// <summary>
    /// Triggers IInitialize / INavigatedAware lifecycle on the initial page,
    /// deferred to after the visual tree attaches (so regions are ready).
    /// </summary>
    private static void AwaitInitialPageLifecycle(AvaloniaObject shell)
    {
        void FireLifecycle(object? s, Avalonia.VisualTreeAttachmentEventArgs e)
        {
            if (s is Avalonia.Visual v)
                v.AttachedToVisualTree -= FireLifecycle;

            var initialPage = shell as Page;
            if (shell is Avalonia.Controls.NavigationPage np)
                initialPage = np.Content as Page;

            if (initialPage is not null)
            {
                var p = new NavigationParameters();
                Common.MvvmHelpers.OnNavigatedTo(initialPage, p);
            }
        }

        if (shell is Avalonia.Visual visual)
            visual.AttachedToVisualTree += FireLifecycle;
    }

    /// <summary>
    /// Explicitly auto-wires the ViewModel for a view.
    /// </summary>
    protected static void AutowireViewModel(AvaloniaObject view)
    {
        ViewModelLocator.Autowire(view);
    }
}
