#nullable enable
using Avalonia;

namespace Prism.Navigation.Regions;

/// <summary>
/// Implementation of <see cref="IRegionManager"/>.
/// </summary>
public class RegionManager : IRegionManager
{
    // ── Attached properties ───────────────────────────────────────────

    public static readonly AttachedProperty<IRegionManager?> RegionManagerProperty =
        AvaloniaProperty.RegisterAttached<RegionManager, AvaloniaObject, IRegionManager?>(
            "RegionManager");

    public static IRegionManager? GetRegionManager(AvaloniaObject obj) =>
        obj.GetValue(RegionManagerProperty);

    public static void SetRegionManager(AvaloniaObject obj, IRegionManager? value) =>
        obj.SetValue(RegionManagerProperty, value);

    public static readonly AttachedProperty<string?> RegionNameProperty =
        AvaloniaProperty.RegisterAttached<RegionManager, AvaloniaObject, string?>(
            "RegionName");

    public static string? GetRegionName(AvaloniaObject obj) =>
        obj.GetValue(RegionNameProperty);

    public static void SetRegionName(AvaloniaObject obj, string? value) =>
        obj.SetValue(RegionNameProperty, value);

    static RegionManager()
    {
        RegionNameProperty.Changed.AddClassHandler<AvaloniaObject>(
            (obj, args) =>
            {
                if (args.NewValue is string name && !string.IsNullOrEmpty(name))
                    UpdatingRegions?.Invoke(null, new RegionCreationEventArgs(name, obj));
            });
    }

    public static event EventHandler<RegionCreationEventArgs>? UpdatingRegions;
    public static void UpdateRegions() { /* DelayedRegionCreationBehavior handles this */ }

    // ── Instance ──────────────────────────────────────────────────────

    private readonly RegionCollection _regions = new();

    /// <inheritdoc />
    public IRegionCollection Regions => _regions;

    /// <inheritdoc />
    public IRegionManager CreateRegionManager() => new RegionManager();

    /// <inheritdoc />
    public IRegionManager AddToRegion(string regionName, string viewName)
    {
        if (!_regions.ContainsRegionWithName(regionName))
            throw new KeyNotFoundException($"Region '{regionName}' not found.");

        _regions[regionName].Add(viewName);
        return this;
    }

    /// <inheritdoc />
    public IRegionManager AddToRegion(string regionName, object view)
    {
        if (!_regions.ContainsRegionWithName(regionName))
            throw new KeyNotFoundException($"Region '{regionName}' not found.");

        _regions[regionName].Add(view);
        return this;
    }

    /// <inheritdoc />
    public IRegionManager RegisterViewWithRegion(string regionName, Type viewType)
    {
        var viewRegistry = ContainerLocator.Container.Resolve<IRegionViewRegistry>();
        viewRegistry.RegisterViewWithRegion(regionName, viewType);
        return this;
    }

    /// <inheritdoc />
    public IRegionManager RegisterViewWithRegion(string regionName, string viewName)
    {
        var viewRegistry = ContainerLocator.Container.Resolve<IRegionViewRegistry>();
        viewRegistry.RegisterViewWithRegion(regionName, viewName);
        return this;
    }

    /// <inheritdoc />
    public IRegionManager RegisterViewWithRegion(string regionName, Func<IContainerProvider, object> viewFactory)
    {
        var viewRegistry = ContainerLocator.Container.Resolve<IRegionViewRegistry>();
        viewRegistry.RegisterViewWithRegion(regionName, viewFactory);
        return this;
    }

    /// <inheritdoc />
    public void RequestNavigate(
        string regionName,
        Uri target,
        Action<NavigationResult>? navigationCallback,
        INavigationParameters? parameters = null)
    {
        if (!_regions.ContainsRegionWithName(regionName))
            throw new KeyNotFoundException($"Region '{regionName}' not found.");

        _regions[regionName].RequestNavigate(target, navigationCallback, parameters);
    }
}

/// <summary>
/// Event args for region creation.
/// </summary>
public class RegionCreationEventArgs : EventArgs
{
    public string RegionName { get; }
    public AvaloniaObject TargetElement { get; }

    public RegionCreationEventArgs(string regionName, AvaloniaObject targetElement)
    {
        RegionName = regionName;
        TargetElement = targetElement;
    }
}
