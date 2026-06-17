#nullable enable
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Avalonia;

namespace Prism.Navigation.Regions;

/// <summary>
/// Core implementation of <see cref="IRegion"/>. Manages a collection of views
/// and their active state.
/// </summary>
public class Region : IRegion, ITargetAwareRegion, INotifyPropertyChanged
{
    private readonly ObservableCollection<ItemMetadata> _itemMetadata = new();
    private IViewsCollection? _views;
    private IViewsCollection? _activeViews;
    private IRegionNavigationService? _navigationService;
    private IRegionBehaviorCollection? _behaviors;
    private AvaloniaObject? _targetElement;
    private IRegionManager? _regionManager;

    /// <summary>
    /// Initializes a new instance of <see cref="Region"/>.
    /// </summary>
    public Region()
    {
        _behaviors = new Behaviors.RegionBehaviorCollection(this);
        _itemMetadata.CollectionChanged += (_, _) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Views)));
    }

    /// <inheritdoc />
    public string? Name { get; set; }

    /// <inheritdoc />
    public object? Context { get; set; }

    /// <inheritdoc />
    public IViewsCollection Views =>
        _views ??= new ViewsCollection(_itemMetadata);

    /// <inheritdoc />
    public virtual IViewsCollection ActiveViews =>
        _activeViews ??= new ViewsCollection(_itemMetadata, m => m.IsActive);

    /// <inheritdoc />
    public Comparison<object>? SortComparison { get; set; }

    /// <inheritdoc />
    public IRegionManager? RegionManager
    {
        get => _regionManager;
        set => _regionManager = value;
    }

    /// <inheritdoc />
    public IRegionNavigationService NavigationService
    {
        get => _navigationService ?? throw new InvalidOperationException("NavigationService has not been set.");
        set => _navigationService = value;
    }

    /// <inheritdoc />
    public IRegionBehaviorCollection Behaviors
    {
        get => _behaviors ??= new Behaviors.RegionBehaviorCollection(this);
        set => _behaviors = value;
    }

    /// <summary>
    /// Gets or sets the Avalonia element hosting this region.
    /// </summary>
    public AvaloniaObject? TargetElement
    {
        get => _targetElement;
        set => _targetElement = value;
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public virtual IRegionManager Add(object view) => Add(view, null, false);

    /// <inheritdoc />
    public virtual IRegionManager Add(object view, string? viewName) => Add(view, viewName, false);

    /// <inheritdoc />
    public virtual IRegionManager Add(string viewName)
    {
        // Deferred: requires a registered view factory
        if (RegionManager is not null)
        {
            // Try to resolve from region view registry
            var viewRegistry = ContainerLocator.Container.Resolve<IRegionViewRegistry>();
            var contents = viewRegistry.GetContents(Name ?? string.Empty, ContainerLocator.Container);
            if (contents.Any())
            {
                foreach (var c in contents)
                    Add(c, viewName);
            }
        }
        return RegionManager!;
    }

    /// <inheritdoc />
    public virtual IRegionManager Add(object view, string? viewName, bool createRegionManagerScope)
    {
        if (view is null) throw new ArgumentNullException(nameof(view));

        var metadata = new ItemMetadata(view) { Name = viewName };
        _itemMetadata.Add(metadata);

        if (createRegionManagerScope && view is AvaloniaObject ao)
        {
            var childManager = new RegionManager();
            Prism.Navigation.Regions.RegionManager.SetRegionManager(ao, childManager);
        }

        return RegionManager!;
    }

    /// <inheritdoc />
    public virtual object? GetView(string viewName) =>
        _itemMetadata.FirstOrDefault(m => m.Name == viewName)?.Item;

    /// <inheritdoc />
    public virtual void Activate(object view)
    {
        var metadata = _itemMetadata.FirstOrDefault(m => m.Item == view)
            ?? throw new ArgumentException("The view is not part of this region.", nameof(view));
        metadata.IsActive = true;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveViews)));
    }

    /// <inheritdoc />
    public virtual void Deactivate(object view)
    {
        var metadata = _itemMetadata.FirstOrDefault(m => m.Item == view)
            ?? throw new ArgumentException("The view is not part of this region.", nameof(view));
        metadata.IsActive = false;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActiveViews)));
    }

    /// <inheritdoc />
    public virtual void Remove(object view)
    {
        var metadata = _itemMetadata.FirstOrDefault(m => m.Item == view)
            ?? throw new ArgumentException("The view is not part of this region.", nameof(view));
        metadata.IsActive = false;
        _itemMetadata.Remove(metadata);
    }

    /// <inheritdoc />
    public virtual void RemoveAll()
    {
        foreach (var item in _itemMetadata) item.IsActive = false;
        _itemMetadata.Clear();
    }

    /// <inheritdoc />
    public virtual bool Contains(object view) =>
        _itemMetadata.Any(m => m.Item == view);

    /// <inheritdoc />
    public void RequestNavigate(Uri target, Action<NavigationResult>? navigationCallback, INavigationParameters? parameters)
    {
        NavigationService.RequestNavigate(target, navigationCallback, parameters);
    }
}
