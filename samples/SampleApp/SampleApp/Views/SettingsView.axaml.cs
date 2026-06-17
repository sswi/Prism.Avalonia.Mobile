using Prism;
using Prism.Mvvm;

namespace SampleApp.Views;

[PrismView("SettingsView", ViewModel = typeof(ViewModels.SettingsViewModel), ViewType = ViewType.Region)]
public partial class SettingsView : Avalonia.Controls.UserControl
{
    public SettingsView() => InitializeComponent();
}
