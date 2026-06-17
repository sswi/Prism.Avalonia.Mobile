#nullable enable
using Avalonia;
using Avalonia.Controls;
using Prism;
using Prism.Navigation;

namespace Prism.Common;

/// <summary>
/// Helper methods for navigation lifecycle dispatch.
/// </summary>
internal static class MvvmHelpers
{
    internal static void InvokeViewAndViewModelAction<T>(object? view, Action<T> action)
    {
        if (view is null) return;
        if (view is T viewAsT) action(viewAsT);
        if (view is StyledElement styled && styled.DataContext is T vmAsT) action(vmAsT);
    }

    internal static async Task<bool> CanNavigateAsync(object? page, INavigationParameters parameters)
    {
        if (page is null) return true;

        if (page is Navigation.IConfirmNavigation confirm && !confirm.CanNavigate(parameters)) return false;
        if (page is Navigation.IConfirmNavigationAsync confirmAsync && !await confirmAsync.CanNavigateAsync(parameters)) return false;

        if (page is StyledElement styled && styled.DataContext is not null)
        {
            var vm = styled.DataContext;
            if (vm is Navigation.IConfirmNavigation vmConfirm && !vmConfirm.CanNavigate(parameters)) return false;
            if (vm is Navigation.IConfirmNavigationAsync vmConfirmAsync && !await vmConfirmAsync.CanNavigateAsync(parameters)) return false;
        }
        return true;
    }

    internal static void OnNavigatedFrom(object? page, INavigationParameters p)
        => InvokeViewAndViewModelAction<Navigation.INavigatedAware>(page, v => v.OnNavigatedFrom(p));

    internal static void OnNavigatedTo(object? page, INavigationParameters p)
        => InvokeViewAndViewModelAction<Navigation.INavigatedAware>(page, v => v.OnNavigatedTo(p));

    internal static async Task OnInitializedAsync(object? page, INavigationParameters p)
    {
        if (page is null) return;
        if (page is Navigation.IInitialize init) init.Initialize(p);
        if (page is StyledElement s1 && s1.DataContext is Navigation.IInitialize vmInit) vmInit.Initialize(p);
        if (page is Navigation.IInitializeAsync initAsync) await initAsync.InitializeAsync(p);
        if (page is StyledElement s2 && s2.DataContext is Navigation.IInitializeAsync vmInitAsync) await vmInitAsync.InitializeAsync(p);
    }

    internal static NavigationPage? FindNavigationPageFromCurrent(Page? current)
    {
        var p = current as StyledElement;
        while (p is not null) { if (p is NavigationPage np) return np; p = p.Parent; }
        return null;
    }

    internal static Page? GetCurrentPage(NavigationPage? np)
    {
        if (np is null) return null;
        if (np.ModalStack.Count > 0) return np.ModalStack[^1];
        if (np.NavigationStack.Count > 0) return np.NavigationStack[^1];
        return np.Content as Page;
    }

    internal static bool GetAnimatedParameter(INavigationParameters p)
    {
        if (p.TryGetValue<bool>(Navigation.KnownNavigationParameters.Animated, out var a)) return a;
        return true;
    }

    internal static bool GetUseModalNavigationParameter(INavigationParameters p, bool hasNavPage)
    {
        if (p.TryGetValue<bool>(Navigation.KnownNavigationParameters.UseModalNavigation, out var m)) return m;
        return !hasNavPage;
    }
}
