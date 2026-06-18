using Avalonia;
using Prism.Navigation.Regions.Adapters;

namespace Prism.Navigation.Regions.Behaviors;

public class DelayedRegionCreationBehavior
{
    private readonly string _regionName;
    private readonly AvaloniaObject _targetElement;
    private readonly RegionAdapterMappings _mappings;
    private readonly IContainerProvider _container;
    private readonly IRegionManager? _parentManager;
    private bool _created;

    public DelayedRegionCreationBehavior(
        string regionName,
        AvaloniaObject targetElement,
        RegionAdapterMappings mappings,
        IContainerProvider container,
        IRegionManager? parentManager = null)
    {
        _regionName = regionName;
        _targetElement = targetElement;
        _mappings = mappings;
        _container = container;
        _parentManager = parentManager;
    }

    public void Start()
    {
        if (_targetElement is Visual v)
            v.AttachedToVisualTree += OnAttached;
    }

    private void OnAttached(object? s, VisualTreeAttachmentEventArgs e)
    {
        if (s is Visual v)
            v.AttachedToVisualTree -= OnAttached;
        CreateRegion();
    }

    internal void CreateRegion()
    {
        if (_created) return;
        _created = true;

        var adapterType = _mappings.GetAdapterType(_targetElement.GetType());
        if (adapterType is null) return;
        var adapter = (IRegionAdapter)_container.Resolve(adapterType);
        var region = adapter.Initialize(_targetElement, _regionName);

        // Register with parent RegionManager
        var rm = _parentManager ?? RegionManager.RootRegionManager;
        if (rm is not null && !rm.Regions.ContainsRegionWithName(region.Name))
            rm.Regions.Add(region);
    }
}
