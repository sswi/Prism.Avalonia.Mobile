using Avalonia.Controls;
using Prism.Common;
using Prism.Navigation;

namespace Prism.Behaviors;

internal sealed class PageSystemBackBehavior
{
    private bool _isProgrammatic;

    public void Apply(NavigationPage navPage)
    {
        navPage.Popped += (s, e) =>
        {
            if (_isProgrammatic) { _isProgrammatic = false; return; }

            var p = new NavigationParameters();
            MvvmHelpers.OnNavigatedFrom(e.Page, p);
            MvvmHelpers.OnNavigatedTo(MvvmHelpers.GetCurrentPage(navPage), p);
        };
    }

    public void MarkProgrammaticPop() => _isProgrammatic = true;
}
