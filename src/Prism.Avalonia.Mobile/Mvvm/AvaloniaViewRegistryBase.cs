#nullable enable
using Avalonia;

namespace Prism.Mvvm;

/// <summary>
/// Avalonia-specific base class for view registries. Extends Prism.Core's
/// <c>ViewRegistryBase&gt;AvaloniaObject&lt;</c>.
/// All View-ViewModel mappings must be registered explicitly for AOT safety.
/// </summary>
public abstract class AvaloniaViewRegistryBase : ViewRegistryBase<AvaloniaObject>
{
    // ── Attached properties ──────────────────────────────────────────

    private static readonly AttachedProperty<string?> NavigationNameProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaViewRegistryBase, AvaloniaObject, string?>(
            "NavigationName");

    private static readonly AttachedProperty<Type?> ViewModelTypeProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaViewRegistryBase, AvaloniaObject, Type?>(
            "ViewModelType");

    private static readonly AttachedProperty<IContainerProvider?> ContainerProviderProperty =
        AvaloniaProperty.RegisterAttached<AvaloniaViewRegistryBase, AvaloniaObject, IContainerProvider?>(
            "ContainerProvider");

    // ── Public accessors ──────────────────────────────────────────────

    public static string? GetNavigationName(AvaloniaObject? view) =>
        view?.GetValue(NavigationNameProperty);

    public static Type? GetViewModelType(AvaloniaObject view) =>
        view.GetValue(ViewModelTypeProperty);

    public static IContainerProvider? GetContainerProvider(AvaloniaObject view) =>
        view.GetValue(ContainerProviderProperty);

    // ── Constructor ──────────────────────────────────────────────────

    protected AvaloniaViewRegistryBase(ViewType registryType, IEnumerable<ViewRegistration> registrations)
        : base(registryType, registrations)
    {
    }

    // ── Abstract overrides ───────────────────────────────────────────

    /// <inheritdoc />
    protected override void SetNavigationNameProperty(AvaloniaObject? view, string name)
    {
        if (view is not null)
            view.SetValue(NavigationNameProperty, name);
    }

    /// <inheritdoc />
    protected override void SetViewModelProperty(AvaloniaObject? view, Type viewModelType)
    {
        if (view is not null)
            view.SetValue(ViewModelTypeProperty, viewModelType);
    }

    /// <inheritdoc />
    protected override void SetContainerProvider(AvaloniaObject? view, IContainerProvider container)
    {
        if (view is not null)
            view.SetValue(ContainerProviderProperty, container);
    }

    /// <inheritdoc />
    protected override void Autowire(AvaloniaObject? view)
    {
        if (view is not null)
            ViewModelLocator.Autowire(view);
    }

    /// <inheritdoc />
    protected override void ConfigureView(AvaloniaObject? view, IContainerProvider container)
    {
        // Subclasses override this to configure the view
    }
}
