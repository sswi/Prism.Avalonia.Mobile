#nullable enable
using Prism.Behaviors;
using Prism.Common;
using Prism.Dialogs;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Navigation.Regions;
using Prism.Navigation.Regions.Adapters;
using Prism.Navigation.Regions.Behaviors;

namespace Prism;

public static class PrismInitializationExtensions
{
    public static void ConfigureViewModelLocator()
    {
        ViewModelLocationProvider.SetDefaultViewModelFactory(type => ContainerLocator.Container.Resolve(type));
        ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver(_ => null);
        ViewModelLocationProvider.SetDefaultViewToViewModelTypeResolver(_ => null);
    }

    public static void RegisterRequiredTypes(this IContainerRegistry containerRegistry, IModuleCatalog moduleCatalog)
    {
        containerRegistry.RegisterInstance(moduleCatalog);

        // Navigation
        containerRegistry.RegisterSingleton<INavigationService, PageNavigationService>();
        containerRegistry.RegisterScoped<IPageAccessor, PageAccessor>();
        containerRegistry.RegisterSingleton<INavigationRegistry, NavigationRegistry>();

        // Page behaviors
        containerRegistry.RegisterSingleton<IPageBehaviorFactory, DefaultPageBehaviorFactory>();

        // Region
        containerRegistry.RegisterSingleton<IRegionManager, RegionManager>();
        containerRegistry.RegisterSingleton<IRegionNavigationContentLoader, RegionNavigationContentLoader>();
        containerRegistry.RegisterSingleton<IRegionViewRegistry, RegionViewRegistry>();
        containerRegistry.RegisterSingleton<IRegionNavigationRegistry, RegionNavigationRegistry>();
        containerRegistry.RegisterSingleton<IRegionBehaviorFactory, RegionBehaviorFactory>();
        containerRegistry.RegisterSingleton<RegionAdapterMappings>();

        containerRegistry.RegisterSingleton<ContentControlRegionAdapter>();
        containerRegistry.RegisterSingleton<ItemsControlRegionAdapter>();

        var mappings = new RegionAdapterMappings();
        mappings.RegisterMapping<Avalonia.Controls.ContentControl, ContentControlRegionAdapter>();
        mappings.RegisterMapping<Avalonia.Controls.ItemsControl, ItemsControlRegionAdapter>();
        containerRegistry.RegisterInstance(mappings);

        // Dialog
        containerRegistry.RegisterSingleton<IDialogService, DialogService>();
        containerRegistry.Register<IDialogWindow, DialogWindow>();

        // Modules
        containerRegistry.RegisterSingleton<IModuleInitializer, ModuleInitializer>();
        containerRegistry.RegisterSingleton<IModuleManager, ModuleManager>();
    }

    public static void RegisterDefaultRegionBehaviors(IRegionBehaviorFactory b)
    {
        b.AddIfMissing(BindRegionContextToAvaloniaObjectBehavior.BehaviorKey, typeof(BindRegionContextToAvaloniaObjectBehavior));
        b.AddIfMissing(RegionActiveAwareBehavior.BehaviorKey, typeof(RegionActiveAwareBehavior));
        b.AddIfMissing(SyncRegionContextWithHostBehavior.BehaviorKey, typeof(SyncRegionContextWithHostBehavior));
        b.AddIfMissing(RegionManagerRegistrationBehavior.BehaviorKey, typeof(RegionManagerRegistrationBehavior));
        b.AddIfMissing(RegionMemberLifetimeBehavior.BehaviorKey, typeof(RegionMemberLifetimeBehavior));
        b.AddIfMissing(ClearChildViewsRegionBehavior.BehaviorKey, typeof(ClearChildViewsRegionBehavior));
        b.AddIfMissing(AutoPopulateRegionBehavior.BehaviorKey, typeof(AutoPopulateRegionBehavior));
        b.AddIfMissing(DestructibleRegionBehavior.BehaviorKey, typeof(DestructibleRegionBehavior));
    }

    public static void RunModuleManager(this IContainerProvider containerProvider)
        => containerProvider.Resolve<IModuleManager>().Run();
}
