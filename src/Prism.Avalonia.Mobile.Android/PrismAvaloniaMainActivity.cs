using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using AApplication = Android.App.Application;

namespace Prism.Platforms.Android;

/// <summary>
/// Base Activity for Prism.Avalonia.Mobile Android applications.
/// Intercepts system back button and edge swipe to navigate through
/// the NavigationPage stack instead of closing the Activity.
/// </summary>
/// <remarks>
/// Usage: change your MainActivity to inherit from this class:
/// <code>
/// public class MainActivity : PrismAvaloniaMainActivity { }
/// </code>
/// </remarks>
[Activity(
    Label = "PrismApp",
    Theme = "@style/MyTheme.NoActionBar",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class PrismAvaloniaMainActivity : AvaloniaMainActivity
{
    /// <summary>
    /// Intercepts system back/edge-swipe.
    /// Override to customize behavior; call base to use Prism navigation.
    /// </summary>
    public override void OnBackPressed()
    {
        if (HandleBackNavigation())
            return;
        base.OnBackPressed();
    }

    /// <summary>
    /// Pops NavigationPage or ModalStack if available.
    /// </summary>
    protected bool HandleBackNavigation()
    {
        var navPage = FindNavigationPage();
        if (navPage is null) return false;

        if (navPage.ModalStack.Count > 0 && navPage is INavigation nav)
        {
            _ = nav.PopModalAsync();
            return true;
        }

        if (navPage.NavigationStack.Count > 1 && navPage is INavigation nav2)
        {
            _ = nav2.PopAsync();
            return true;
        }

        return false;
    }

    private static NavigationPage? FindNavigationPage()
    {
        var avApp = Avalonia.Application.Current;
        if (avApp?.ApplicationLifetime is ISingleViewApplicationLifetime sv
            && sv.MainView is Visual svRoot)
            return FindNav(svRoot);

        if (avApp?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime dt
            && dt.MainWindow?.Content is Visual dtRoot)
            return FindNav(dtRoot);

        return null;
    }

    private static NavigationPage? FindNav(Visual? v)
    {
        if (v is null) return null;
        if (v is NavigationPage np) return np;
        if (v is ContentControl cc && cc.Content is Visual inner) return FindNav(inner);
        foreach (var child in Avalonia.VisualTree.VisualExtensions.GetVisualChildren(v))
        {
            if (FindNav(child as Visual) is { } result)
                return result;
        }
        return null;
    }
}
