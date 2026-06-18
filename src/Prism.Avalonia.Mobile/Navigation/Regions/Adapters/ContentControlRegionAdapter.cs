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
        void UpdateContent() => regionTarget.Content = region.ActiveViews.FirstOrDefault();

        region.ActiveViews.CollectionChanged += (s, e) => UpdateContent();
        region.Views.CollectionChanged += (s, e) =>
        {
            if (regionTarget.Content is null && region.Views.Any())
            {
                var first = region.Views.First();
                if (!region.ActiveViews.Contains(first))
                    region.Activate(first);
            }
        };

        // Also listen for PropertyChanged (IsActive changes don't fire collection events)
        if (region is System.ComponentModel.INotifyPropertyChanged npc)
            npc.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(IRegion.ActiveViews))
                    UpdateContent();
            };
    }
}
