using Avalonia.Controls;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Navigation.Regions;
using SampleApp.Views;
using SampleApp.ViewModels;
using SampleApp.Services;
using PrismApplication = Prism.DryIoc.PrismApplication;

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

        // ── 方式 A: [PrismView] 属性自动注册 ──────────────────────────
        cr.RegisterPrismViews();

        // ── 方式 B: 代码注册 ──────────────────────────────────────
        // cr.RegisterForNavigation<EditPage, EditViewModel>("EditPage");

        // ── 方式 B: 手动代码注册 ──────────────────────────────────────
        // 如果有不需要属性标注的 View，仍然可以用传统方式
        // cr.RegisterForNavigation<MainPage, MainViewModel>("MainPage");
        // cr.RegisterForNavigation<DetailPage, DetailViewModel>("DetailPage");

        // ── Dialog 注册 ───────────────────────────────────────────────
        cr.RegisterDialog<DialogDemoView, DialogDemoViewModel>("DemoDialog");
    }
}
