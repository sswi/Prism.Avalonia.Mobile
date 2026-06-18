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
        {
            void Handler(object? s, VisualTreeAttachmentEventArgs e)
            { v.AttachedToVisualTree -= Handler; CreateRegion(); }
            v.AttachedToVisualTree += Handler;
        }
    }

    private void CreateRegion()
    {
        var adapterType = _mappings.GetAdapterType(_targetElement.GetType());
        if (adapterType is null) return;
        var adapter = (IRegionAdapter)_container.Resolve(adapterType);
        var region = adapter.Initialize(_targetElement, _regionName);

        // Ensure region is registered with a RegionManager
        if (_parentManager is not null && !_parentManager.Regions.ContainsRegionWithName(region.Name))
            _parentManager.Regions.Add(region);
        else if (region is ITargetAwareRegion ta)
        {
            // Fallback: walk visual tree
            var p = (ta.TargetElement as Visual)?.Parent;
            while (p is not null)
            {
                var rm = RegionManager.GetRegionManager(p);
                if (rm is not null)
                {
                    if (!rm.Regions.ContainsRegionWithName(region.Name))
                        rm.Regions.Add(region);
                    return;
                }
                p = (p as Visual)?.Parent;
            }
        }
    }
}
