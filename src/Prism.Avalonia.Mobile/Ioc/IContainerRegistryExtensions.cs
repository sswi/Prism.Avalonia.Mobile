#nullable enable
using System.Diagnostics.CodeAnalysis;
using Prism.Mvvm;
using Prism.Navigation;

namespace Prism.Ioc;

public static class AvaloniaContainerRegistryExtensions
{
    /// <summary>
    /// Internal static list that accumulates ViewRegistration objects during
    /// RegisterTypes(). Consumed by NavigationRegistry when it is first resolved.
    /// </summary>
    internal static readonly List<ViewRegistration> PendingRegistrations = new();

    public static void RegisterForNavigation<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        TView
    >(this IContainerRegistry containerRegistry, string? name = null)
    {
        var viewType = typeof(TView);
        var viewName = string.IsNullOrWhiteSpace(name) ? viewType.Name : name;

        containerRegistry.Register(typeof(object), viewType, viewName);
        containerRegistry.Register(viewType, viewType);

        PendingRegistrations.Add(new ViewRegistration
        {
            Type = ViewType.Page,
            View = viewType,
            ViewModel = null,
            Name = viewName
        });
    }

    public static void RegisterForNavigation<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        TViewModel
    >(this IContainerRegistry containerRegistry, string? name = null)
        where TView : Avalonia.AvaloniaObject
        where TViewModel : class
    {
        var viewType = typeof(TView);
        var viewModelType = typeof(TViewModel);
        var viewName = string.IsNullOrWhiteSpace(name) ? viewType.Name : name;

        containerRegistry.Register(typeof(object), viewType, viewName);
        containerRegistry.Register(viewType, viewType);
        containerRegistry.Register(viewModelType, viewModelType);

        ViewModelLocationProvider.Register(viewType.ToString(), viewModelType);

        PendingRegistrations.Add(new ViewRegistration
        {
            Type = ViewType.Page,
            View = viewType,
            ViewModel = viewModelType,
            Name = viewName
        });
    }

    public static void RegisterDialog<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        TView
    >(this IContainerRegistry containerRegistry, string? name = null)
    {
        containerRegistry.RegisterForNavigation<TView>(name);
    }

    public static void RegisterDialog<
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        TView,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        TViewModel
    >(this IContainerRegistry containerRegistry, string? name = null)
        where TView : Avalonia.AvaloniaObject
        where TViewModel : class
    {
        containerRegistry.RegisterForNavigation<TView, TViewModel>(name);
    }
}
