using System.Collections.ObjectModel;

namespace Prism.Modularity;

/// <summary>
/// Holds information about the modules that can be used by the application.
/// </summary>
public class ModuleCatalog : ModuleCatalogBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ModuleCatalog"/> class.
    /// </summary>
    public ModuleCatalog() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance with an initial list of modules.
    /// </summary>
    /// <param name="modules">The initial list of modules.</param>
    public ModuleCatalog(IEnumerable<ModuleInfo> modules) : base(modules)
    {
    }

    /// <summary>
    /// Gets the items in the module catalog.
    /// </summary>
    public new Collection<IModuleCatalogItem> Items => base.Items;
}
