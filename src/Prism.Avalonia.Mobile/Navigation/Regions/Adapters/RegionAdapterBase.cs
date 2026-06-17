#nullable enable
using Avalonia;
using Prism.Ioc;

namespace Prism.Navigation.Regions.Adapters;

public abstract class RegionAdapterBase<T> : IRegionAdapter where T : AvaloniaObject
{
    private readonly IContainerExtension _container;

    protected RegionAdapterBase(IContainerExtension container) => _container = container ?? throw new ArgumentNullException(nameof(container));
    protected IContainerExtension Container => _container;

    public IRegion Initialize(object regionTarget, string regionName)
    {
        if (regionTarget is not T target)
            throw new ArgumentException($"Expected target of type {typeof(T).Name}.");

        var region = CreateRegion(Container);
        region.Name = regionName;
        if (region is ITargetAwareRegion ta) ta.TargetElement = target;

        // Set up navigation service (required by region)
        region.NavigationService = _container.Resolve<IRegionNavigationService>();

        var factory = _container.Resolve<IRegionBehaviorFactory>();
        foreach (var key in factory)
        {
            var b = factory.CreateFromKey(key);
            if (b is not null) { b.Region = region; region.Behaviors.Add(key, b); b.Attach(); }
        }

        RegisterWithParent(target, region);
        Adapt(target, region);
        return region;
    }

    protected abstract IRegion CreateRegion(IContainerExtension container);
    protected abstract void Adapt(T regionTarget, IRegion region);

    protected virtual void RegisterWithParent(AvaloniaObject target, IRegion region)
    {
        var p = (target as Visual)?.Parent;
        while (p is not null)
        {
            var rm = RegionManager.GetRegionManager(p);
            if (rm is not null) { if (!rm.Regions.ContainsRegionWithName(region.Name)) rm.Regions.Add(region); return; }
            p = (p as Visual)?.Parent;
        }
    }
}
