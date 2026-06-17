using Prism.Mvvm;
using Prism.Navigation.Regions;
using SampleApp.Services;
using SampleApp.Views;

namespace SampleApp.ViewModels;

public class ItemsRegionViewModel : BindableBase
{
    public ItemsRegionViewModel(IRegionViewRegistry registry, IItemService items)
    {
        foreach (var item in items.GetColorItems())
        {
            registry.RegisterViewWithRegion("ItemsRegion", () =>
                new ColorCardView { DataContext = item });
        }
    }
}
