using Avalonia.Controls;

namespace Prism.Common;

/// <summary>
/// Default implementation of <see cref="IPageAccessor"/>.
/// </summary>
internal sealed class PageAccessor : IPageAccessor
{
    /// <inheritdoc />
    public Page? Page { get; set; }
}
