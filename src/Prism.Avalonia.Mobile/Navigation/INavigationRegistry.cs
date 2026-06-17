using Prism.Mvvm;

namespace Prism.Navigation;

/// <summary>
/// Registry for page-type views. Extends <see cref="IViewRegistry"/> from Prism.Core
/// without adding extra members — the interface exists so the DI container can
/// distinguish the page registry from other registries (region, dialog).
/// </summary>
public interface INavigationRegistry : IViewRegistry
{
}
