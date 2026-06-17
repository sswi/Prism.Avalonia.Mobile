#nullable enable
using System.Collections;

namespace Prism.Navigation.Regions.Behaviors;

/// <summary>
/// Implementation of <see cref="IRegionBehaviorCollection"/>.
/// </summary>
internal class RegionBehaviorCollection : IRegionBehaviorCollection
{
    private readonly IRegion _region;
    private readonly Dictionary<string, IRegionBehavior> _behaviors = new(StringComparer.Ordinal);

    public RegionBehaviorCollection(IRegion region)
    {
        _region = region ?? throw new ArgumentNullException(nameof(region));
    }

    /// <inheritdoc />
    public IRegionBehavior this[string key] =>
        _behaviors.TryGetValue(key, out var behavior)
            ? behavior
            : throw new KeyNotFoundException($"Behavior '{key}' not found.");

    /// <inheritdoc />
    public void Add(string key, IRegionBehavior behavior)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentNullException(nameof(key));
        behavior.Region = _region;
        _behaviors[key] = behavior;
    }

    /// <inheritdoc />
    public bool ContainsKey(string key) => _behaviors.ContainsKey(key);

    /// <inheritdoc />
    public IEnumerator<KeyValuePair<string, IRegionBehavior>> GetEnumerator() =>
        _behaviors.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
