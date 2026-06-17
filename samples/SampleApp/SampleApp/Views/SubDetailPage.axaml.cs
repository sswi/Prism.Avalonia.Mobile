using Prism;

namespace SampleApp.Views;

[PrismView("SubDetailPage", ViewModel = typeof(ViewModels.SubDetailViewModel))]
public partial class SubDetailPage : Avalonia.Controls.ContentPage
{
    public SubDetailPage() => InitializeComponent();
}
