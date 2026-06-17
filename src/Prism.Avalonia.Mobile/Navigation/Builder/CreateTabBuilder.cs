using System.Web;

namespace Prism.Navigation.Builder;

/// <summary>
/// Implementation of <see cref="ICreateTabBuilder"/>.
/// </summary>
internal sealed class CreateTabBuilder : ICreateTabBuilder
{
    private readonly string _segmentName;
    private string? _title;
    private readonly Dictionary<string, object?> _parameters = new();

    public CreateTabBuilder(string segmentName)
    {
        _segmentName = segmentName ?? throw new ArgumentNullException(nameof(segmentName));
    }

    /// <inheritdoc />
    public ICreateTabBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    /// <inheritdoc />
    public ICreateTabBuilder AddParameter(string key, object? value)
    {
        _parameters[key] = value;
        return this;
    }

    /// <inheritdoc />
    public string ToSegment()
    {
        var name = _segmentName;

        // Optionally prepend NavigationPage wrapper
        // Format: "NavigationPage|ViewA"
        if (_parameters.Count > 0)
        {
            var queryParts = _parameters
                .Select(p => $"{HttpUtility.UrlEncode(p.Key)}={HttpUtility.UrlEncode(p.Value?.ToString() ?? string.Empty)}");
            name += $"?{string.Join("&", queryParts)}";
        }

        if (!string.IsNullOrEmpty(_title))
        {
            name += $"&{KnownNavigationParameters.Title}={HttpUtility.UrlEncode(_title)}";
        }

        return name;
    }
}
