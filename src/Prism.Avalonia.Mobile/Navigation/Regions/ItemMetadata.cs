namespace Prism.Navigation.Regions;

/// <summary>
/// Wraps a view item within a region, tracking its name and active state.
/// </summary>
public class ItemMetadata
{
    /// <summary>
    /// Initializes a new instance of <see cref="ItemMetadata"/>.
    /// </summary>
    /// <param name="item">The view instance.</param>
    public ItemMetadata(object item)
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
    }

    /// <summary>
    /// Gets the view instance.
    /// </summary>
    public object Item { get; }

    /// <summary>
    /// Gets or sets the name of the view.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets whether this item is currently the active item in the region.
    /// </summary>
    public bool IsActive { get; set; }
}
