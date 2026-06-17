using Prism;

namespace SampleApp.Views;

[PrismView("MainPage", ViewModel = typeof(ViewModels.MainViewModel))]
public partial class MainPage : Avalonia.Controls.ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }
}
