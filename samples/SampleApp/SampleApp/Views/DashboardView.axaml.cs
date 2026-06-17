using Prism;
using Prism.Mvvm;

namespace SampleApp.Views;

[PrismView("DashboardView", ViewModel = typeof(ViewModels.DashboardViewModel), ViewType = ViewType.Region)]
public partial class DashboardView : Avalonia.Controls.UserControl
{
    public DashboardView() => InitializeComponent();
}
