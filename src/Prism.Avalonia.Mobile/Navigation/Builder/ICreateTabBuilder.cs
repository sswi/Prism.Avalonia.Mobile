namespace Prism.Navigation.Builder;

/// <summary>
/// Builder for creating a tab configuration within a <see cref="TabbedSegmentBuilder"/>.
/// </summary>
public interface ICreateTabBuilder
{
    /// <summary>
    /// Sets the title for this tab.
    /// </summary>
    ICreateTabBuilder Title(string title);

    /// <summary>
    /// Adds a parameter to this tab.
    /// </summary>
    ICreateTabBuilder AddParameter(string key, object? value);

    /// <summary>
    /// Builds the tab segment string.
    /// </summary>
    string ToSegment();
}
