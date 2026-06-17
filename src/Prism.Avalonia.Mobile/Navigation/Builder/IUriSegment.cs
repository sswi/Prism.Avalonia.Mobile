namespace Prism.Navigation.Builder;

/// <summary>
/// Marker interface for URI segment types in the navigation builder.
/// </summary>
public interface IUriSegment
{
    /// <summary>
    /// Gets the segment string ready for appending to a navigation URI.
    /// </summary>
    string ToSegment();
}
