using Avalonia;
using Prism.Navigation.Regions.Adapters;

namespace Prism.Navigation.Regions.Behaviors;

public class DelayedRegionCreationBehavior
{
    private readonly string _regionName;
    private readonly AvaloniaObject _targetElement;
    private readonly RegionAdapterMappings _mappings;
    private readonly IContainerProvider _container;

    public DelayedRegionCreationBehavior(string regionName, AvaloniaObject targetElement, RegionAdapterMappings mappings, IContainerProvider container)
    {
        _regionName = regionName; _targetElement = targetElement; _mappings = mappings; _container = container;
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
        adapter.Initialize(_targetElement, _regionName);
    }
}
