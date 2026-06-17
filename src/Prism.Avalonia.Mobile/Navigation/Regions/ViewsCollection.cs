using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Prism.Navigation.Regions;

/// <summary>
/// A filtered, sorted, observable collection of <see cref="ItemMetadata"/>
/// representing the views within a region.
/// </summary>
public class ViewsCollection : IViewsCollection, INotifyCollectionChanged
{
    private readonly ObservableCollection<ItemMetadata> _allItems;
    private readonly Predicate<ItemMetadata>? _filter;
    private readonly Comparison<ItemMetadata>? _sort;

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    public ViewsCollection(
        ObservableCollection<ItemMetadata> allItems,
        Predicate<ItemMetadata>? filter = null,
        Comparison<ItemMetadata>? sort = null)
    {
        _allItems = allItems ?? throw new ArgumentNullException(nameof(allItems));
        _filter = filter;
        _sort = sort;

        _allItems.CollectionChanged += (s, e) =>
            CollectionChanged?.Invoke(this, e);
    }

    /// <inheritdoc />
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc />
    public IEnumerator<object> GetEnumerator()
    {
        var filtered = _filter is not null
            ? _allItems.Where(i => _filter(i))
            : (IEnumerable<ItemMetadata>)_allItems;

        var sorted = _sort is not null
            ? filtered.OrderBy(i => i, Comparer<ItemMetadata>.Create(_sort))
            : filtered;

        return sorted.Select(i => i.Item).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <inheritdoc />
    public bool Contains(object view) =>
        _allItems.Any(m => m.Item == view);
}
