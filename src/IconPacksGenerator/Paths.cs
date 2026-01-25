namespace IconPacksGenerator;

internal static class Paths
{
    internal static readonly string RootPath = Path.GetFullPath(
        "../../../../",
        AppDomain.CurrentDomain.BaseDirectory
    );

    internal static readonly string PacksPath = Path.Combine(RootPath, "./packs");

    internal static readonly string InkscapePath = "C:\\Program Files\\Inkscape\\bin\\inkscape.exe";

    internal static readonly string InkscapeOutputPath = Path.Combine(
        RootPath,
        "./inkscapeOutput/"
    );

    internal static readonly string FeatherIconPath = Path.Combine(RootPath, "3rdparty/Feather/");

    internal static readonly string FontAwesomeIconPath = Path.Combine(
        RootPath,
        "3rdparty/FontAwesome/"
    );

    internal static readonly string IonicIconPath = Path.Combine(RootPath, "3rdparty/Ionic/");

    internal static readonly string MaterialIconPath = Path.Combine(RootPath, "3rdparty/Material/");

    internal static readonly string MaterialCommunityIconPath = Path.Combine(
        RootPath,
        "3rdparty/MaterialCommunity/"
    );

    internal static readonly string SimpleIconPath = Path.Combine(RootPath, "3rdparty/Simple/");

    internal static readonly string TablerIconPath = Path.Combine(RootPath, "3rdparty/Tabler/");

    internal static readonly string FluentIconPath = Path.Combine(RootPath, "3rdparty/Fluent/");

    internal static readonly string RemixIconPath = Path.Combine(RootPath, "3rdparty/Remix/");

    internal static readonly string HeroIconPath = Path.Combine(RootPath, "3rdparty/Hero/");

    internal static readonly string LucideIconPath = Path.Combine(RootPath, "3rdparty/Lucide/");
}
