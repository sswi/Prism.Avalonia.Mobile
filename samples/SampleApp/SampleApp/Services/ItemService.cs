using SampleApp.Models;
using System.Collections.Generic;

namespace SampleApp.Services;

public class ItemService : IItemService
{
    public List<ColorItem> GetColorItems() =>
    [
        new() { Name = "Coral",    Color = "#FF6F61", Description = "Warm and inviting" },
        new() { Name = "Teal",     Color = "#008080", Description = "Calm and balanced" },
        new() { Name = "Lavender", Color = "#967BB6", Description = "Soft and elegant" },
        new() { Name = "Amber",    Color = "#FFC107", Description = "Bright and energetic" },
        new() { Name = "Indigo",   Color = "#3F51B5", Description = "Deep and focused" },
        new() { Name = "Mint",     Color = "#4CAF50", Description = "Fresh and clean" },
        new() { Name = "Crimson",  Color = "#DC143C", Description = "Bold and passionate" },
        new() { Name = "Slate",    Color = "#607D8B", Description = "Neutral and solid" },
    ];
}
