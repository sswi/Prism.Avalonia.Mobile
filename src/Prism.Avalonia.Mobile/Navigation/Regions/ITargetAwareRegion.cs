using Avalonia;

namespace Prism.Navigation.Regions;

/// <summary>
/// A region that is aware of its host control in the visual tree.
/// </summary>
public interface ITargetAwareRegion : IRegion
{
    /// <summary>
    /// Gets or sets the Avalonia element that hosts this region.
    /// </summary>
    AvaloniaObject? TargetElement { get; set; }
}
