using SampleApp.Models;
using System.Collections.Generic;

namespace SampleApp.Services;

public interface IItemService
{
    List<ColorItem> GetColorItems();
}
