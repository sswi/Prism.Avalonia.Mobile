using System.Web;

namespace Prism.Navigation.Builder;

/// <summary>
/// Implementation of <see cref="ITabbedSegmentBuilder"/>.
/// </summary>
internal sealed class TabbedSegmentBuilder : ITabbedSegmentBuilder, IUriSegment
{
    private readonly List<string> _tabs = new();
    private string? _selectedTab;

    /// <inheritdoc />
    public ITabbedSegmentBuilder CreateTab(string segmentName, Action<ICreateTabBuilder>? configureTab = null)
    {
        var tabBuilder = new CreateTabBuilder(segmentName);
        configureTab?.Invoke(tabBuilder);
        _tabs.Add(tabBuilder.ToSegment());
        return this;
    }

    /// <inheritdoc />
    public ITabbedSegmentBuilder SelectedTab(string tabName)
    {
        _selectedTab = tabName;
        return this;
    }

    /// <inheritdoc />
    public string ToSegment()
    {
        var baseName = "__TabbedPage";

        if (_tabs.Count > 0)
        {
            var tabsParam = $"{KnownNavigationParameters.CreateTab}={string.Join(";", _tabs)}";
            return string.IsNullOrEmpty(_selectedTab)
                ? $"{baseName}?{tabsParam}"
                : $"{baseName}?{tabsParam}&{KnownNavigationParameters.SelectedTab}={HttpUtility.UrlEncode(_selectedTab)}";
        }

        return baseName;
    }
}
