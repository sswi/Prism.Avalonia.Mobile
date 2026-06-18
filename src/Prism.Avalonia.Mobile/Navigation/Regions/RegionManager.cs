#nullable enable
using Avalonia;

namespace Prism.Navigation.Regions;

public class RegionManager : IRegionManager
{
    // ── Attached properties ───────────────────────────────────────────

    public static readonly AttachedProperty<IRegionManager?> RegionManagerProperty =
        AvaloniaProperty.RegisterAttached<RegionManager, AvaloniaObject, IRegionManager?>(
            "RegionManager");

    public static IRegionManager? GetRegionManager(AvaloniaObject obj)
    {
        // Walk up the visual tree to find a RegionManager
        var p = obj;
        while (p is not null)
        {
            var rm = p.GetValue(RegionManagerProperty);
            if (rm is not null) return rm;
            p = (p as Visual)?.Parent;
        }
        return null;
    }

    public static void SetRegionManager(AvaloniaObject obj, IRegionManager? value) =>
        obj.SetValue(RegionManagerProperty, value);

    public static readonly AttachedProperty<string?> RegionNameProperty =
        AvaloniaProperty.RegisterAttached<RegionManager, AvaloniaObject, string?>(
            "RegionName");

    public static string? GetRegionName(AvaloniaObject obj) =>
        obj.GetValue(RegionNameProperty);

    public static void SetRegionName(AvaloniaObject obj, string? value) =>
        obj.SetValue(RegionNameProperty, value);

    // ── Pending region creation ───────────────────────────────────────

    private static readonly List<(string name, AvaloniaObject element)> _pendingRegions = new();

    static RegionManager()
    {
        RegionNameProperty.Changed.AddClassHandler<AvaloniaObject>(
            (obj, args) =>
            {
                if (args.NewValue is string name && !string.IsNullOrEmpty(name))
                {
                    _pendingRegions.Add((name, obj));
                    UpdatingRegions?.Invoke(null, new RegionCreationEventArgs(name, obj));
                }
            });
    }

    public static event EventHandler<RegionCreationEventArgs>? UpdatingRegions;

    /// <summary>
    /// Creates all pending regions. Must be called after RegionAdapterMappings are registered.
    /// </summary>
    public static void UpdateRegions(IRegionManager? parentManager = null)
    {
        if (_pendingRegions.Count == 0) return;

        var container = ContainerLocator.Container;
        var mappings = container.Resolve<Adapters.RegionAdapterMappings>();

        foreach (var (name, element) in _pendingRegions)
        {
            var behavior = new Behaviors.DelayedRegionCreationBehavior(name, element, mappings, container, parentManager);
            behavior.Start();
        }
        _pendingRegions.Clear();
    }

    // ── Instance ──────────────────────────────────────────────────────

    private readonly RegionCollection _regions = new();

    public IRegionCollection Regions => _regions;

    public IRegionManager CreateRegionManager() => new RegionManager();

    public IRegionManager AddToRegion(string regionName, string viewName)
    {
        if (!_regions.ContainsRegionWithName(regionName))
            throw new KeyNotFoundException($"Region '{regionName}' not found.");
        _regions[regionName].Add(viewName);
        return this;
    }

    public IRegionManager AddToRegion(string regionName, object view)
    {
        if (!_regions.ContainsRegionWithName(regionName))
            throw new KeyNotFoundException($"Region '{regionName}' not found.");
        _regions[regionName].Add(view);
        return this;
    }

    public IRegionManager RegisterViewWithRegion(string regionName, Type viewType)
    {
        var viewRegistry = ContainerLocator.Container.Resolve<IRegionViewRegistry>();
        viewRegistry.RegisterViewWithRegion(regionName, viewType);
        return this;
    }

    public IRegionManager RegisterViewWithRegion(string regionName, string viewName)
    {
        var viewRegistry = ContainerLocator.Container.Resolve<IRegionViewRegistry>();
        viewRegistry.RegisterViewWithRegion(regionName, viewName);
        return this;
    }

    public IRegionManager RegisterViewWithRegion(string regionName, Func<IContainerProvider, object> viewFactory)
    {
        var viewRegistry = ContainerLocator.Container.Resolve<IRegionViewRegistry>();
        viewRegistry.RegisterViewWithRegion(regionName, viewFactory);
        return this;
    }

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
