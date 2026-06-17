using System.Collections;
using System.Collections.Specialized;

namespace Prism.Navigation.Regions;

/// <summary>
/// Internal collection of <see cref="IRegion"/> instances.
/// </summary>
internal class RegionCollection : IRegionCollection, INotifyCollectionChanged
{
    private readonly Dictionary<string, IRegion> _regions = new(StringComparer.Ordinal);

    /// <inheritdoc />
    public IRegion this[string regionName] =>
        _regions.TryGetValue(regionName, out var region)
            ? region
            : throw new KeyNotFoundException($"Region '{regionName}' not found.");

    /// <inheritdoc />
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc />
    public IEnumerator<IRegion> GetEnumerator() => _regions.Values.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public void Add(IRegion region)
    {
        if (region is null) throw new ArgumentNullException(nameof(region));
        if (string.IsNullOrEmpty(region.Name))
            throw new ArgumentException("Region must have a name.");
        _regions[region.Name] = region;
        CollectionChanged?.Invoke(this,
            new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, region));
    }

    /// <inheritdoc />
    public void Add(string regionName, IRegion region)
    {
        region.Name = regionName;
        Add(region);
    }

    /// <inheritdoc />
    public bool Remove(string regionName)
    {
        if (_regions.TryGetValue(regionName, out var region))
        {
            _regions.Remove(regionName);
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, region));
            return true;
        }
        return false;
    }

    /// <inheritdoc />
    public bool ContainsRegionWithName(string regionName) =>
        _regions.ContainsKey(regionName);
}
