using System.Web;

namespace Prism.Navigation.Builder;

/// <summary>
/// Implementation of <see cref="ISegmentBuilder"/>.
/// </summary>
internal sealed class SegmentBuilder : ISegmentBuilder, IUriSegment
{
    private readonly string _segmentName;
    private readonly Dictionary<string, object?> _parameters = new();

    public SegmentBuilder(string segmentName)
    {
        _segmentName = segmentName ?? throw new ArgumentNullException(nameof(segmentName));
    }

    /// <inheritdoc />
    public ISegmentBuilder AddParameter(string key, object? value)
    {
        _parameters[key] = value;
        return this;
    }

    /// <inheritdoc />
    public ISegmentBuilder UseModalNavigation(bool useModal = true)
    {
        _parameters[KnownNavigationParameters.UseModalNavigation] = useModal;
        return this;
    }

    /// <inheritdoc />
    public ISegmentBuilder UseAnimatedNavigation(bool animated = true)
    {
        _parameters[KnownNavigationParameters.Animated] = animated;
        return this;
    }

    /// <inheritdoc />
    public string ToSegment()
    {
        if (_parameters.Count == 0)
            return _segmentName;

        var queryParts = _parameters
            .Select(p =>
            {
                var value = p.Value?.ToString() ?? string.Empty;
                return $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(value)}";
            });

        return $"{_segmentName}?{string.Join("&", queryParts)}";
    }
}
