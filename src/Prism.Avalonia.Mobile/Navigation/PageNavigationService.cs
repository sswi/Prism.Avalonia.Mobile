#nullable enable
using System.Web;
using Avalonia.Controls;
using Prism.Common;
using Prism.Ioc;
using Prism.Mvvm;

namespace Prism.Navigation;

/// <summary>
/// Provides page-based navigation wrapping Avalonia 12's NavigationPage.
/// </summary>
public class PageNavigationService : INavigationService
{
    private static readonly SemaphoreSlim _semaphore = new(1, 1);
    private static readonly TimeSpan _minTimeBetweenNavigations = TimeSpan.FromMilliseconds(150);
    private static DateTime _lastNavigate = DateTime.MinValue;

    private readonly IContainerProvider _container;

    public PageNavigationService(IContainerProvider container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    protected IViewRegistry Registry => _container.Resolve<INavigationRegistry>();

    // ── Helpers ────────────────────────────────────────────────────

    private static NavigationResult Ok() => new();
    private static NavigationResult Fail(Exception ex) => new(ex);

    // ── GoBackAsync ──────────────────────────────────────────────

    public virtual async Task<INavigationResult> GoBackAsync(INavigationParameters? parameters = null)
    {
        try { await WaitForPendingNavigation(); return await GoBackInternal(parameters ?? new NavigationParameters()); }
        catch (Exception ex) { return Fail(ex); }
        finally { _lastNavigate = DateTime.Now; _semaphore.Release(); }
    }

    private async Task<INavigationResult> GoBackInternal(INavigationParameters parameters)
    {
        var navPage = FindNavPage();
        if (navPage is null || navPage.NavigationStack.Count <= 1) return Fail(new NavigationException("Cannot go back."));

        var fromPage = navPage.NavigationStack[^1];
        var can = await MvvmHelpers.CanNavigateAsync(fromPage, parameters);
        if (!can) return Fail(new NavigationException("IConfirmNavigation blocked.", fromPage));

        await PopAsync(navPage);
        MvvmHelpers.OnNavigatedFrom(fromPage, parameters);
        MvvmHelpers.OnNavigatedTo(navPage.NavigationStack[^1], parameters);
        return Ok();
    }

    // ── GoBackToAsync ────────────────────────────────────────────

    public virtual async Task<INavigationResult> GoBackToAsync(string viewName, INavigationParameters? parameters = null)
    {
        try { await WaitForPendingNavigation(); return await GoBackToInternal(viewName, parameters ?? new NavigationParameters()); }
        catch (Exception ex) { return Fail(ex); }
        finally { _lastNavigate = DateTime.Now; _semaphore.Release(); }
    }

    private async Task<INavigationResult> GoBackToInternal(string viewName, INavigationParameters parameters)
    {
        var navPage = FindNavPage();
        if (navPage is null) return Fail(new NavigationException("No NavigationPage found."));

        var target = navPage.NavigationStack.FirstOrDefault(p =>
            AvaloniaViewRegistryBase.GetNavigationName(p as Avalonia.AvaloniaObject) == viewName);
        if (target is null) return Fail(new NavigationException($"Page '{viewName}' not found in stack."));

        var from = navPage.NavigationStack[^1];
        await PopToPageAsync(navPage, target);
        MvvmHelpers.OnNavigatedFrom(from, parameters);
        MvvmHelpers.OnNavigatedTo(target, parameters);
        return Ok();
    }

    // ── GoBackToRootAsync ────────────────────────────────────────

    public virtual async Task<INavigationResult> GoBackToRootAsync(INavigationParameters? parameters = null)
    {
        try { await WaitForPendingNavigation(); return await GoBackToRootInternal(parameters ?? new NavigationParameters()); }
        catch (Exception ex) { return Fail(ex); }
        finally { _lastNavigate = DateTime.Now; _semaphore.Release(); }
    }

    private async Task<INavigationResult> GoBackToRootInternal(INavigationParameters parameters)
    {
        var navPage = FindNavPage();
        if (navPage is null || navPage.NavigationStack.Count <= 1) return Fail(new NavigationException("Already at root."));

        var from = navPage.NavigationStack[^1];
        await PopToRootAsync(navPage);
        MvvmHelpers.OnNavigatedFrom(from, parameters);
        MvvmHelpers.OnNavigatedTo(navPage.NavigationStack[0], parameters);
        return Ok();
    }

    // ── NavigateAsync ────────────────────────────────────────────

    public virtual async Task<INavigationResult> NavigateAsync(Uri uri, INavigationParameters? parameters = null)
    {
        try { await WaitForPendingNavigation(); return await NavigateInternal(uri, parameters ?? new NavigationParameters()); }
        catch (Exception ex) { return Fail(ex); }
        finally { _lastNavigate = DateTime.Now; _semaphore.Release(); }
    }

    private async Task<INavigationResult> NavigateInternal(Uri uri, INavigationParameters parameters)
    {
        var segments = GetUriSegments(uri);
        if (segments.Count == 0) return Fail(new NavigationException("URI has no segments."));

        var currentPage = GetCurrentPage();
        var navPage = FindNavPage();

        if (currentPage is not null)
        {
            if (!await MvvmHelpers.CanNavigateAsync(currentPage, parameters))
                return Fail(new NavigationException("Navigation blocked.", currentPage));
        }

        if (navPage is null && currentPage is null)
            return await NavigateFirstAsync(segments, parameters);

        return await ProcessAsync(navPage, currentPage, new Queue<string>(segments), parameters);
    }

    private async Task<INavigationResult> NavigateFirstAsync(List<string> segments, INavigationParameters parameters)
    {
        var page = CreatePage(GetSegmentName(segments[0]));
        if (page is null) return Fail(new NavigationException("Cannot create first page."));

        await MvvmHelpers.OnInitializedAsync(page, GetSegmentParameters(segments[0], parameters));
        MvvmHelpers.OnNavigatedTo(page, GetSegmentParameters(segments[0], parameters));

        if (page is NavigationPage np && segments.Count > 1)
            return await ProcessAsync(np, np.Content as Page, new Queue<string>(segments.Skip(1)), parameters);

        return Ok();
    }

    private async Task<INavigationResult> ProcessAsync(NavigationPage? navPage, Page? currentPage, Queue<string> segments, INavigationParameters parameters, bool useModal = false)
    {
        if (segments.Count == 0) return Ok();

        var segment = segments.Dequeue();
        var name = GetSegmentName(segment);
        var segParams = GetSegmentParameters(segment, parameters);
        useModal = MvvmHelpers.GetUseModalNavigationParameter(segParams, navPage is not null);

        var next = CreatePage(name);
        if (next is null) return Fail(new NavigationException($"Cannot create page '{name}'."));

        await MvvmHelpers.OnInitializedAsync(next, segParams);

        if (ShouldClearStack(next, segParams) && navPage is not null)
            await PopToRootAsync(navPage);

        if (currentPage is null && navPage is not null)
            await PushAsync(navPage, next, useModal);
        else if (next is NavigationPage np)
        { if (navPage is not null) await PushAsync(navPage, np, useModal); navPage = np; }
        else if (currentPage is NavigationPage childNav)
        { await PushAsync(childNav, next, useModal); navPage = childNav; }
        else if (currentPage is TabbedPage tb)
            navPage = await TabNavAsync(tb, next, segParams, useModal);
        else if (navPage is not null)
            await PushAsync(navPage, next, useModal);

        if (currentPage is not null) MvvmHelpers.OnNavigatedFrom(currentPage, segParams);
        MvvmHelpers.OnNavigatedTo(next, segParams);

        return segments.Count > 0 ? await ProcessAsync(navPage, next, segments, parameters, useModal) : Ok();
    }

    // ── SelectTabAsync ───────────────────────────────────────────

    public virtual async Task<INavigationResult> SelectTabAsync(string name, Uri? uri = null, INavigationParameters? parameters = null)
    {
        try
        {
            await WaitForPendingNavigation();
            var tb = FindTabbedPage(GetCurrentPage());
            if (tb is null) return Fail(new NavigationException("No TabbedPage found."));

            Page? target = null;
            int idx = 0;
            foreach (var item in Avalonia.LogicalTree.LogicalExtensions.GetLogicalChildren(tb).OfType<Page>())
            {
                if (AvaloniaViewRegistryBase.GetNavigationName(item) == name || (item is ContentPage cp && cp.Header?.ToString() == name))
                { target = item; break; }
                idx++;
            }

            if (target is not null && tb is SelectingMultiPage smp)
                smp.SelectedIndex = idx;

            if (uri is not null)
                return await NavigateInternal(uri, parameters ?? new NavigationParameters());

            return Ok();
        }
        catch (Exception ex) { return Fail(ex); }
        finally { _lastNavigate = DateTime.Now; _semaphore.Release(); }
    }

    // ── Tab Navigation ───────────────────────────────────────────

    private async Task<NavigationPage?> TabNavAsync(TabbedPage tb, Page next, INavigationParameters p, bool useModal)
    {
        var childPages = Avalonia.LogicalTree.LogicalExtensions.GetLogicalChildren(tb).OfType<Page>().ToList();
        var selected = tb is SelectingMultiPage smp && smp.SelectedIndex >= 0 && smp.SelectedIndex < childPages.Count
            ? childPages[smp.SelectedIndex] : null;

        if (selected is NavigationPage tabNav)
        { await PushAsync(tabNav, next, useModal); return tabNav; }

        // If selected tab is not a NavigationPage, push modally to nearest containing NavigationPage
        var parentNav = FindNavPageUp(tb);
        if (parentNav is not null)
        { await PushAsync(parentNav, next, true); }

        return parentNav;
    }

    // ── Page Creation ────────────────────────────────────────────

    protected virtual Page? CreatePage(string segmentName)
    {
        try
        {
            var scope = _container.CreateScope();
            return Registry.CreateView(scope, segmentName) as Page;
        }
        catch (KeyNotFoundException)
        {
            throw new NavigationException($"No page registered with name '{segmentName}'.");
        }
    }

    // ── Concurrency ──────────────────────────────────────────────

    protected virtual async Task WaitForPendingNavigation()
    {
        await _semaphore.WaitAsync();
        var elapsed = DateTime.Now - _lastNavigate;
        if (elapsed < _minTimeBetweenNavigations)
            await Task.Delay(_minTimeBetweenNavigations - elapsed);
    }

    // ── URI Parsing ──────────────────────────────────────────────

    private static List<string> GetUriSegments(Uri uri) =>
        uri.OriginalString.Split('/').Where(s => !string.IsNullOrWhiteSpace(s)).Select(HttpUtility.UrlDecode).ToList();

    private static string GetSegmentName(string segment) { var i = segment.IndexOf('?'); return i >= 0 ? segment[..i] : segment; }

    private static INavigationParameters GetSegmentParameters(string segment, INavigationParameters existing)
    {
        var i = segment.IndexOf('?');
        if (i < 0) return existing;
        var q = HttpUtility.ParseQueryString(segment[(i + 1)..]);
        foreach (string? k in q.Keys) { if (k is not null) existing.Add(k, q[k]); }
        return existing;
    }

    // ── Helpers ──────────────────────────────────────────────────

    private static async Task PushAsync(NavigationPage? np, Page page, bool modal)
    {
        if (np is null) return;
        if (np is Avalonia.Controls.INavigation nav)
        {
            if (modal) await nav.PushModalAsync(page);
            else await nav.PushAsync(page);
        }
    }

    private static async Task PopAsync(NavigationPage np)
    {
        if (np is Avalonia.Controls.INavigation nav)
            await nav.PopAsync();
    }

    private static async Task PopToPageAsync(NavigationPage np, Page target)
    {
        if (np is Avalonia.Controls.INavigation nav)
            await nav.PopToPageAsync(target);
    }

    private static async Task PopToRootAsync(NavigationPage np)
    {
        if (np is Avalonia.Controls.INavigation nav)
            await nav.PopToRootAsync();
    }

    private static bool ShouldClearStack(Page next, INavigationParameters p) =>
        (next is INavigationPageOptions o && o.ClearNavigationStackOnNavigation) ||
        (p.TryGetValue<bool>(KnownNavigationParameters.ClearNavigationStack, out var c) && c);

    private Page? GetCurrentPage()
    {
        var np = FindNavPage();
        if (np is null) return null;
        if (np.ModalStack.Count > 0) return np.ModalStack[^1];
        return np.NavigationStack.Count > 0 ? np.NavigationStack[^1] : np.Content as Page;
    }

    private NavigationPage? FindNavPage()
    {
        // Search from the application's main window visual tree
        if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop
            && desktop.MainWindow is { } window)
        {
            return FindNavPageInVisual(window.Content);
        }

        if (Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.ISingleViewApplicationLifetime singleView
            && singleView.MainView is { } mainView)
        {
            return FindNavPageInVisual(mainView);
        }

        return null;
    }

    private static NavigationPage? FindNavPageInVisual(object? root)
    {
        if (root is NavigationPage np) return np;
        if (root is Avalonia.Controls.ContentControl cc && cc.Content is not null)
            return FindNavPageInVisual(cc.Content);
        if (root is Avalonia.Visual v)
        {
            foreach (var child in Avalonia.VisualTree.VisualExtensions.GetVisualChildren(v))
                { var result = FindNavPageInVisual(child); if (result is not null) return result; }
        }
        return null;
    }

    private static NavigationPage? FindNavPageUp(Avalonia.AvaloniaObject? start)
    {
        var p = start;
        while (p is not null) { if (p is NavigationPage np) return np; p = (p as Avalonia.Visual)?.Parent; }
        return null;
    }

    private static TabbedPage? FindTabbedPage(Page? page)
    {
        var p = page as Avalonia.AvaloniaObject;
        while (p is not null) { if (p is TabbedPage tb) return tb; p = (p as Avalonia.Visual)?.Parent; }
        return null;
    }
}
