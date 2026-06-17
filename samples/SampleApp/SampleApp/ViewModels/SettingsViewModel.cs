using Prism.Mvvm;

namespace SampleApp.ViewModels;

public class SettingsViewModel : BindableBase
{
    private bool _notifications = true;
    private bool _darkMode;

    public bool NotificationsEnabled { get => _notifications; set => SetProperty(ref _notifications, value); }
    public bool DarkModeEnabled { get => _darkMode; set => SetProperty(ref _darkMode, value); }
}
