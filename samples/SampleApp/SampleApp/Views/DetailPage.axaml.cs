using Prism;

namespace SampleApp.Views;

[PrismView("DetailPage", ViewModel = typeof(ViewModels.DetailViewModel))]
public partial class DetailPage : Avalonia.Controls.ContentPage
{
    public DetailPage() => InitializeComponent();
}
