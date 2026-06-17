using Prism;
using Prism.Navigation.Regions;
using System;

namespace SampleApp.Views;

[PrismView("RegionDemoPage", ViewModel = typeof(ViewModels.RegionDemoViewModel))]
public partial class RegionDemoPage : Avalonia.Controls.ContentPage
{
    public RegionDemoPage() => InitializeComponent();

    private void OnDashboardClick(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        => NavigateRegion("DashboardView");
    private void OnProfileClick(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        => NavigateRegion("ProfileView");
    private void OnSettingsClick(object? s, Avalonia.Interactivity.RoutedEventArgs e)
        => NavigateRegion("SettingsView");

    private void NavigateRegion(string name)
    {
        var rm = RegionManager.GetRegionManager(this);
        rm?.RequestNavigate("DemoRegion", new Uri(name, UriKind.Relative));
    }
}
