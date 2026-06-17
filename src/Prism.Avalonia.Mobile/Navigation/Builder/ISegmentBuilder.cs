namespace Prism.Navigation.Builder;

/// <summary>
/// Builder for a single navigation URI segment like "ViewA?useModalNavigation=true".
/// </summary>
public interface ISegmentBuilder
{
    /// <summary>
    /// Adds a parameter to this segment.
    /// </summary>
    ISegmentBuilder AddParameter(string key, object? value);

    /// <summary>
    /// Sets whether this segment should use modal navigation.
    /// </summary>
    ISegmentBuilder UseModalNavigation(bool useModal = true);

    /// <summary>
    /// Sets whether this segment should be animated.
    /// </summary>
    ISegmentBuilder UseAnimatedNavigation(bool animated = true);

    /// <summary>
    /// Builds the URI segment string.
    /// </summary>
    string ToSegment();
}
