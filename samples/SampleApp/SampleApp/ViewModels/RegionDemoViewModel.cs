using Avalonia.Threading;
using Prism.Mvvm;
using Prism.Navigation.Regions;
using System;

namespace SampleApp.ViewModels;

public class RegionDemoViewModel(IRegionManager rm) : BindableBase, IRegionAware
{
    public void OnNavigatedTo(NavigationContext ctx)
    {
        rm.RequestNavigate("DemoRegion", new Uri("DashboardView", UriKind.Relative));
    }

    public bool IsNavigationTarget(NavigationContext ctx) => true;
    public void OnNavigatedFrom(NavigationContext ctx) { }
}
