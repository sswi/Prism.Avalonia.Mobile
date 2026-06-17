#nullable enable
using Prism.Common;

namespace Prism.Navigation.Regions;

public class RegionNavigationService : IRegionNavigationService
{
    private readonly IRegionNavigationContentLoader _contentLoader;
    private IRegion? _region;

    public RegionNavigationService(IRegionNavigationContentLoader contentLoader)
        => _contentLoader = contentLoader ?? throw new ArgumentNullException(nameof(contentLoader));

    public IRegion Region { get => _region!; set => _region = value; }
    public IRegionNavigationJournal? Journal { get; set; }

    public event EventHandler<RegionNavigationEventArgs>? Navigating;
    public event EventHandler<RegionNavigationEventArgs>? Navigated;
    public event EventHandler<RegionNavigationFailedEventArgs>? NavigationFailed;

    public void RequestNavigate(Uri target, Action<NavigationResult>? callback, INavigationParameters? parameters = null)
    {
        parameters ??= new NavigationParameters();
        var ctx = new NavigationContext(this, target, parameters);

        Navigating?.Invoke(this, new RegionNavigationEventArgs(ctx));

        try
        {
            if (!ConfirmFromActive(ctx))
            {
                var fail = new RegionNavigationFailedEventArgs(ctx, new NavigationException("IConfirmNavigationRequest returned false."));
                NavigationFailed?.Invoke(this, fail);
                callback?.Invoke(new NavigationResult(fail.Error));
                return;
            }

            foreach (var av in Region.ActiveViews)
                MvvmHelpers.InvokeViewAndViewModelAction<IRegionAware>(av, a => a.OnNavigatedFrom(ctx));

            var view = _contentLoader.LoadContent(Region, ctx);
            if (view is null) { callback?.Invoke(new NavigationResult(new NavigationException("Content null."))); return; }

            Region.Activate(view);
            MvvmHelpers.InvokeViewAndViewModelAction<IRegionAware>(view, a => a.OnNavigatedTo(ctx));
            Navigated?.Invoke(this, new RegionNavigationEventArgs(ctx));
            Journal?.RecordNavigation(new RegionNavigationJournalEntry { Uri = target, Parameters = parameters }, false);
            callback?.Invoke(new NavigationResult());
        }
        catch (Exception ex)
        {
            NavigationFailed?.Invoke(this, new RegionNavigationFailedEventArgs(ctx, ex));
            callback?.Invoke(new NavigationResult(ex));
        }
    }

    private static bool ConfirmFromActive(NavigationContext ctx)
    {
        foreach (var v in ctx.NavigationService.Region.ActiveViews)
        {
            if (v is IConfirmNavigationRequest c) { bool? r = null; c.ConfirmNavigationRequest(ctx, res => r = res); if (r == false) return false; }
        }
        return true;
    }
}
