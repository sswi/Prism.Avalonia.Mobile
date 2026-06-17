using Avalonia.Controls;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Navigation.Regions;
using SampleApp.Views;
using SampleApp.ViewModels;
using SampleApp.Services;
using PrismApplication = Prism.DryIoc.PrismApplication;
using Avalonia;

namespace SampleApp;

public partial class App : PrismApplication
{
    protected override AvaloniaObject CreateShell()
    {
        return new NavigationPage
        {
            Content = Container.Resolve<MainPage>()
        };
    }

    protected override void RegisterTypes(IContainerRegistry cr)
    {
        cr.RegisterSingleton<IItemService, ItemService>();

        // Pages
        cr.RegisterForNavigation<MainPage, MainViewModel>("MainPage");
        cr.RegisterForNavigation<DetailPage, DetailViewModel>("DetailPage");
        cr.RegisterForNavigation<SubDetailPage, SubDetailViewModel>("SubDetailPage");
        cr.RegisterForNavigation<RegionDemoPage, RegionDemoViewModel>("RegionDemoPage");
        cr.RegisterForNavigation<ItemsRegionPage, ItemsRegionViewModel>("ItemsRegionPage");
        cr.RegisterForNavigation<EditPage, EditViewModel>("EditPage");

        // Region views
        cr.RegisterForNavigation<DashboardView, DashboardViewModel>("DashboardView");
        cr.RegisterForNavigation<ProfileView, ProfileViewModel>("ProfileView");
        cr.RegisterForNavigation<SettingsView, SettingsViewModel>("SettingsView");

        // Dialogs
        cr.RegisterDialog<DialogDemoView, DialogDemoViewModel>("DemoDialog");
    }
}
