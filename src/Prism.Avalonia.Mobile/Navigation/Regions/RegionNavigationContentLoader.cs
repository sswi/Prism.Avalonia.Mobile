#nullable enable
using Prism.Ioc;
using Prism.Mvvm;

namespace Prism.Navigation.Regions;

/// <summary>
/// Resolves content for region navigation. First checks existing region views
/// for a navigation target, then creates a new view from the container if needed.
/// </summary>
/// <remarks>
/// <para>AOT note: creates views via the container (<c>IContainerProvider.Resolve(Type)</c>
/// or <c>Resolve&lt;object&gt;(name)</c>). For full AOT safety, the Phase 4 source
/// generator will produce compile-time factory methods for each registered view type.</para>
/// </remarks>
public class RegionNavigationContentLoader : IRegionNavigationContentLoader
{
    private readonly IContainerExtension _container;

    /// <summary>
    /// Initializes a new instance of <see cref="RegionNavigationContentLoader"/>.
    /// </summary>
    public RegionNavigationContentLoader(IContainerExtension container)
    {
        _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    /// <inheritdoc />
    public object? LoadContent(IRegion region, NavigationContext navigationContext)
    {
        if (region is null)
            throw new ArgumentNullException(nameof(region));
        if (navigationContext is null)
            throw new ArgumentNullException(nameof(navigationContext));

        var candidateTargetContract = GetContractFromNavigationContext(navigationContext);

        // Step 1: Check if the view already exists in the region
        var existingView = FindExistingView(region, candidateTargetContract);
        if (existingView is not null)
        {
            // Call IRegionAware.IsNavigationTarget to see if we should reuse
            if (existingView is IRegionAware regionAware && regionAware.IsNavigationTarget(navigationContext))
                return existingView;
        }

        // Step 2: Create a new view from the container
        return CreateNewRegionView(candidateTargetContract);
    }

    /// <summary>
    /// Extracts the contract name from the navigation context URI.
    /// </summary>
    protected virtual string GetContractFromNavigationContext(NavigationContext context)
    {
        return context.Uri.OriginalString;
    }

    /// <summary>
    /// Searches existing region views for a matching target.
    /// </summary>
    private static object? FindExistingView(IRegion region, string contract)
    {
        return region.Views
            .FirstOrDefault(v => v.GetType().Name == contract
                              || v.GetType().FullName == contract);
    }

    /// <summary>
    /// Creates a new region view from the container. Tries by name first,
    /// then by type match within registered views.
    /// </summary>
    private object? CreateNewRegionView(string contract)
    {
        // Try by registered name
        try
        {
            return _container.Resolve<object>(contract);
        }
        catch
        {
            // Fall through to type-based resolution
        }

        // Try to find a matching registered type
        var regionRegistry = _container.Resolve<IRegionNavigationRegistry>();
        var viewType = regionRegistry.GetViewType(contract);
        if (viewType is not null)
        {
            return _container.Resolve(viewType);
        }

        throw new KeyNotFoundException(
            $"No view with the name '{contract}' has been registered in the container or region registry. " +
            "Use containerRegistry.RegisterForNavigation<TView>(name) or register views with the region.");
    }
}
