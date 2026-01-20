using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace IconPacks.Showcase;

public record IconItem(string Description, string? Data);

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<IconItem>? currentItems;

    [ObservableProperty]
    public int selectedIndex;

    public MainViewModel()
    {
        this.CurrentItems = new ObservableCollection<IconItem>(
            typeof(IconPacks.Feather.Feather)
                .GetFields(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
                )
                .Select(f => new IconItem(f.Name, (string?)f.GetValue(null)))
        );
    }

    partial void OnSelectedIndexChanged(int value)
    {
        var type = value switch
        {
            0 => typeof(IconPacks.Feather.Feather),
            1 => typeof(IconPacks.FontAwesome.Brands),
            2 => typeof(IconPacks.FontAwesome.Regular),
            3 => typeof(IconPacks.FontAwesome.Solid),
            4 => typeof(IconPacks.Ionic.Ionic),
            5 => typeof(IconPacks.Material.Normal),
            6 => typeof(IconPacks.Material.Outlined),
            7 => typeof(IconPacks.Material.Round),
            8 => typeof(IconPacks.Material.Sharp),
            9 => typeof(IconPacks.MaterialCommunity.Material),
            10 => typeof(IconPacks.Tabler.Filled),
            11 => typeof(IconPacks.Tabler.Outline),
            _ => throw new NotSupportedException(),
        };

        this.CurrentItems?.Clear();
        this.CurrentItems = new ObservableCollection<IconItem>(
            type.GetFields(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
                )
                .Select(f => new IconItem(f.Name, (string?)f.GetValue(null)))
        );
    }
}
