using Avalonia.Controls;
using Prism.Common;

namespace Prism.Behaviors;

/// <summary>
/// Listens to <see cref="TabbedPage.SelectedPageChanged"/> and dispatches
/// <see cref="IActiveAware"/> notifications to the selected and deselected tabs.
/// </summary>
internal sealed class TabbedPageActiveAwareBehavior
{
    private TabbedPage? _tabbedPage;

    public void Apply(TabbedPage tabbedPage)
    {
        _tabbedPage = tabbedPage;

        // Set the initially selected page as active
        if (tabbedPage.SelectedPage is not null)
            SetPageActive(tabbedPage.SelectedPage, true);

        tabbedPage.PropertyChanged += OnTabbedPagePropertyChanged;
    }

    private void OnTabbedPagePropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != TabbedPage.SelectedPageProperty) return;

        if (e.OldValue is Page oldPage)
            SetPageActive(oldPage, false);

        if (e.NewValue is Page newPage)
            SetPageActive(newPage, true);
    }

    private static void SetPageActive(Page? page, bool isActive)
    {
        if (page is null) return;
        MvvmHelpers.InvokeViewAndViewModelAction<IActiveAware>(
            page, a => a.IsActive = isActive);
    }
}
