#nullable enable
using Avalonia.Controls;
using Prism.Ioc;

namespace Prism.Navigation.Regions.Adapters;

public class ContentControlRegionAdapter : RegionAdapterBase<ContentControl>
{
    public ContentControlRegionAdapter(IContainerExtension container) : base(container) { }

    protected override IRegion CreateRegion(IContainerExtension container) => new SingleActiveRegion();

    protected override void Adapt(ContentControl regionTarget, IRegion region)
    {
        region.ActiveViews.CollectionChanged += (s, e) =>
        {
            regionTarget.Content = region.ActiveViews.FirstOrDefault();
        };

        // Auto-activate first view added
        region.Views.CollectionChanged += (s, e) =>
        {
            if (regionTarget.Content is null && region.Views.Any())
            {
                var first = region.Views.First();
                if (!region.ActiveViews.Contains(first))
                    region.Activate(first);
            }
        };
    }
}
