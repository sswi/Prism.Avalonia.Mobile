namespace Prism.Navigation.Builder;

/// <summary>
/// Builder for a tabbed page navigation segment.
/// Creates a URI segment that configures tab creation and selection.
/// </summary>
public interface ITabbedSegmentBuilder
{
    /// <summary>
    /// Adds a tab to the segment.
    /// </summary>
    /// <param name="segmentName">The navigation name of the tab page.</param>
    /// <param name="configureTab">Optional configuration for the tab.</param>
    ITabbedSegmentBuilder CreateTab(string segmentName, Action<ICreateTabBuilder>? configureTab = null);

    /// <summary>
    /// Sets which tab should be selected after navigation.
    /// </summary>
    ITabbedSegmentBuilder SelectedTab(string tabName);

    /// <summary>
    /// Builds the tabbed URI segment string.
    /// </summary>
    string ToSegment();
}
