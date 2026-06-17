using Prism;

namespace SampleApp.Views;

[PrismView("ItemsRegionPage", ViewModel = typeof(ViewModels.ItemsRegionViewModel))]
public partial class ItemsRegionPage : Avalonia.Controls.ContentPage
{
    public ItemsRegionPage() => InitializeComponent();
}
