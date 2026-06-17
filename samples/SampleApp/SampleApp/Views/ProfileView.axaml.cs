using Prism;
using Prism.Mvvm;

namespace SampleApp.Views;

[PrismView("ProfileView", ViewModel = typeof(ViewModels.ProfileViewModel), ViewType = ViewType.Region)]
public partial class ProfileView : Avalonia.Controls.UserControl
{
    public ProfileView() => InitializeComponent();
}
