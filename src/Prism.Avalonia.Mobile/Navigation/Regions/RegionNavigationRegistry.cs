#nullable enable
using Prism.Mvvm;

namespace Prism.Navigation.Regions;

/// <summary>
/// Avalonia-specific registry for region-type views. Extends
/// <see cref="AvaloniaViewRegistryBase"/> with <see cref="ViewType.Region"/>.
/// </summary>
internal class RegionNavigationRegistry : AvaloniaViewRegistryBase, IRegionNavigationRegistry
{
    public RegionNavigationRegistry(IEnumerable<ViewRegistration> registrations)
        : base(ViewType.Region, registrations)
    {
    }

    /// <inheritdoc />
    protected override void ConfigureView(Avalonia.AvaloniaObject? view, Ioc.IContainerProvider container)
    {
        // Region views configure themselves via region behaviors
    }
}
