#nullable enable

namespace Prism.Navigation.Regions;

public class RegionViewRegistry : IRegionViewRegistry
{
    private readonly Dictionary<string, List<Func<IContainerProvider, object>>> _factories = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<Type>> _types = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<string>> _names = new(StringComparer.Ordinal);

    public event EventHandler<ViewRegisteredEventArgs>? ContentRegistered;

    public IEnumerable<object> GetContents(string regionName, IContainerProvider container)
    {
        var results = new List<object>();
        if (_factories.TryGetValue(regionName, out var f)) results.AddRange(f.Select(x => x(container)));
        if (_types.TryGetValue(regionName, out var t)) results.AddRange(t.Select(container.Resolve));
        if (_names.TryGetValue(regionName, out var n)) results.AddRange(n.Select(x => container.Resolve<object>(x)));
        return results;
    }

    public void RegisterViewWithRegion(string regionName, Type viewType)
    {
        if (!_types.ContainsKey(regionName)) _types[regionName] = new();
        _types[regionName].Add(viewType);
        ContentRegistered?.Invoke(this, new ViewRegisteredEventArgs(regionName,
            container => container.Resolve(viewType)));
    }

    public void RegisterViewWithRegion(string regionName, string viewName)
    {
        if (!_names.ContainsKey(regionName)) _names[regionName] = new();
        _names[regionName].Add(viewName);
        ContentRegistered?.Invoke(this, new ViewRegisteredEventArgs(regionName,
            container => container.Resolve<object>(viewName)));
    }

    public void RegisterViewWithRegion(string regionName, Func<object> viewFactory)
    {
        RegisterViewWithRegion(regionName, _ => viewFactory());
    }

    public void RegisterViewWithRegion(string regionName, Func<IContainerProvider, object> viewFactory)
    {
        if (!_factories.ContainsKey(regionName)) _factories[regionName] = new();
        _factories[regionName].Add(viewFactory);
        ContentRegistered?.Invoke(this, new ViewRegisteredEventArgs(regionName, viewFactory));
    }
}
