using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace IconPacks.Showcase;

public partial class IconItem(string key, string? data) : ObservableObject
{
    public string Key { get; init; } = key;
    public string? Data { get; init; } = data;

    [RelayCommand]
    void CopyKey()
    {
        if (
            Application.Current!.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop
        )
            desktop.MainWindow?.Clipboard!.SetTextAsync(Key);
    }

    [RelayCommand]
    void CopyData()
    {
        if (
            Application.Current!.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop
        )
            desktop.MainWindow?.Clipboard!.SetTextAsync(Data);
    }
}

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    public ObservableCollection<IconItem>? currentItems;

    [ObservableProperty]
    public ListBoxItem? selectedItem;

    private readonly Dictionary<string, Type> types;

    public MainViewModel()
    {
        this.types = new()
        {
            { "Feather", typeof(Feather.Regular) },
            { "Fluent.Filled", typeof(Fluent.Filled) },
            { "Fluent.Regular", typeof(Fluent.Regular) },
            { "FontAwesome.Brands", typeof(FontAwesome.Brands) },
            { "FontAwesome.Regular", typeof(FontAwesome.Regular) },
            { "FontAwesome.Solid", typeof(FontAwesome.Solid) },
            { "Hero.Outline", typeof(Hero.Outline) },
            { "Hero.Solid", typeof(Hero.Solid) },
            { "Ionic", typeof(Ionic.Regular) },
            { "Lucide", typeof(Lucide.Regular) },
            { "Material.Regular", typeof(Material.Regular) },
            { "Material.Outlined", typeof(Material.Outlined) },
            { "Material.Round", typeof(Material.Round) },
            { "Material.Sharp", typeof(Material.Sharp) },
            { "MaterialCommunity", typeof(MaterialCommunity.Regular) },
            { "Remix.Fill", typeof(Remix.Fill) },
            { "Remix.Line", typeof(Remix.Line) },
            { "Tabler.Filled", typeof(Tabler.Filled) },
            { "Tabler.Outline", typeof(Tabler.Outline) },
        };

        this.CurrentItems = new ObservableCollection<IconItem>(
            typeof(IconPacks.Feather.Regular)
                .GetFields(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
                )
                .Select(f => new IconItem(f.Name, (string?)f.GetValue(null)))
        );
    }

    partial void OnSelectedItemChanged(ListBoxItem value)
    {
        this.types.TryGetValue(value.Content!.ToString()!, out var type);

        if (type is null)
            return;

        this.CurrentItems?.Clear();
        this.CurrentItems = new ObservableCollection<IconItem>(
            type.GetFields(
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static
                )
                .Select(f => new IconItem(f.Name, (string?)f.GetValue(null)))
        );
    }
}
