namespace Prism.Navigation;

/// <summary>
/// When implemented by a page, controls its behavior inside a
/// <see cref="Avalonia.Controls.TabbedPage"/>.
/// </summary>
public interface ITabbedPageOptions
{
    /// <summary>
    /// Gets the title to display on the tab.
    /// </summary>
    string? TabTitle { get; }

    /// <summary>
    /// Gets the icon to display on the tab.
    /// </summary>
    object? TabIcon { get; }
}
