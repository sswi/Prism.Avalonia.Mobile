#nullable enable
using Avalonia.Controls;
using Prism.Ioc;

namespace Prism.Navigation.Regions.Adapters;

public class ItemsControlRegionAdapter : RegionAdapterBase<ItemsControl>
{
    public ItemsControlRegionAdapter(IContainerExtension container) : base(container) { }

    protected override IRegion CreateRegion(IContainerExtension container) => new AllActiveRegion();

    protected override void Adapt(ItemsControl regionTarget, IRegion region)
    {
        region.Views.CollectionChanged += (s, e) =>
        {
            regionTarget.ItemsSource = region.Views.ToList();
        };
    }
}
