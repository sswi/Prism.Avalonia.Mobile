namespace Prism.Navigation.Builder;

/// <summary>
/// Implementation of <see cref="INavigationBuilder"/>.
/// Accumulates segments and parameters into a navigation URI,
/// then executes via <see cref="INavigationService"/>.
/// </summary>
public sealed class NavigationBuilder : INavigationBuilder
{
    private readonly INavigationService _navigationService;
    private readonly List<IUriSegment> _segments = new();
    private readonly NavigationParameters _parameters = new();
    private bool _useModalNavigation;
    private bool _isAnimated = true;

    /// <summary>
    /// Initializes a new instance of <see cref="NavigationBuilder"/>.
    /// </summary>
    /// <param name="navigationService">The navigation service to execute navigation through.</param>
    public NavigationBuilder(INavigationService navigationService)
    {
        _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
    }

    /// <inheritdoc />
    public INavigationBuilder AddSegment(string segmentName, Action<ISegmentBuilder>? configureSegment = null)
    {
        var builder = new SegmentBuilder(segmentName);
        configureSegment?.Invoke(builder);
        _segments.Add(builder);
        return this;
    }

    /// <inheritdoc />
    public INavigationBuilder AddTabbedSegment(Action<ITabbedSegmentBuilder> configureTabbedSegment)
    {
        var builder = new TabbedSegmentBuilder();
        configureTabbedSegment(builder);
        _segments.Add(builder);
        return this;
    }

    /// <inheritdoc />
    public INavigationBuilder AddParameter(string key, object? value)
    {
        _parameters.Add(key, value);
        return this;
    }

    /// <inheritdoc />
    public INavigationBuilder UseModalNavigation(bool useModal = true)
    {
        _useModalNavigation = useModal;
        return this;
    }

    /// <inheritdoc />
    public INavigationBuilder UseAnimatedNavigation(bool animated = true)
    {
        _isAnimated = animated;
        return this;
    }

    /// <inheritdoc />
    public Uri GetNavigationUri()
    {
        var segments = _segments.Select(s => s.ToSegment());
        var uriString = string.Join("/", segments);

        if (_useModalNavigation)
        {
            var sep = uriString.Contains('?') ? "&" : "?";
            uriString += $"{sep}{KnownNavigationParameters.UseModalNavigation}=true";
        }

        if (!_isAnimated)
        {
            var sep = uriString.Contains('?') ? "&" : "?";
            uriString += $"{sep}{KnownNavigationParameters.Animated}=false";
        }

        return new Uri(uriString, UriKind.Relative);
    }

    /// <inheritdoc />
    public INavigationParameters GetNavigationParameters()
    {
        return _parameters;
    }

    /// <inheritdoc />
    public Task<INavigationResult> NavigateAsync()
    {
        return _navigationService.NavigateAsync(GetNavigationUri(), GetNavigationParameters());
    }
}
