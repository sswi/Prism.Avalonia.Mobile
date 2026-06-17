using Avalonia.Controls;

namespace Prism.Common;

/// <summary>
/// A scoped service that tracks the <see cref="Page"/> for the current navigation scope.
/// Each page created during navigation receives its own scope and its own
/// <see cref="IPageAccessor"/>.
/// </summary>
public interface IPageAccessor
{
    /// <summary>
    /// Gets or sets the <see cref="Page"/> associated with the current navigation scope.
    /// </summary>
    Page? Page { get; set; }
}
