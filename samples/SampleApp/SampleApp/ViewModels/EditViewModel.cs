using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;

namespace SampleApp.ViewModels;

/// <summary>
/// Demonstrates blocking back navigation via IConfirmNavigation.
/// When HasChanges is true, GoBack and device back are blocked.
/// </summary>
public class EditViewModel : BindableBase, INavigatedAware, IConfirmNavigation
{
    private readonly INavigationService _nav;
    private string _name = string.Empty;
    private bool _hasChanges;
    private string _statusMessage = "No changes yet — back navigation allowed.";

    public string Name { get => _name; set { _hasChanges = true; SetProperty(ref _name, value); } }

    public bool HasChanges
    {
        get => _hasChanges;
        set => SetProperty(ref _hasChanges, value);
    }

    public string StatusMessage { get => _statusMessage; set => SetProperty(ref _statusMessage, value); }

    public DelegateCommand GoBackCommand { get; }

    public EditViewModel(INavigationService nav)
    {
        _nav = nav;

        GoBackCommand = new DelegateCommand(async () =>
        {
            // Will be blocked by CanNavigate if HasChanges is true
            var result = await _nav.GoBackAsync();
            if (!result.Success)
                StatusMessage = "Back navigation blocked! Uncheck 'I have made changes' first.";
        });
    }

    // ── IConfirmNavigation ──────────────────────────────────────

    /// <summary>
    /// Called BEFORE every back navigation (GoBack, GoBackTo, GoBackToRoot,
    /// system back button, edge swipe). Return false to block.
    /// </summary>
    public bool CanNavigate(INavigationParameters parameters)
    {
        if (HasChanges)
        {
            StatusMessage = "Back blocked — you have unsaved changes. Uncheck the box to allow back.";
            return false;
        }

        return true;
    }

    // ── INavigatedAware ─────────────────────────────────────────

    public void OnNavigatedFrom(INavigationParameters parameters) { }

    public void OnNavigatedTo(INavigationParameters parameters)
    {
        HasChanges = false;
        _name = string.Empty;
        StatusMessage = "";
    }
}
