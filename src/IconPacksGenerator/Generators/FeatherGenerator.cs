using CliWrap;

namespace IconPacksGenerator.Generators;

internal static class FeatherGenerator
{
    private static readonly string rootPath = Path.Combine(Paths.FeatherIconPath, "./icons/");
    private static readonly string inkscapeOutputPath = Path.Combine(
        Paths.InkscapeOutputPath,
        "Feather"
    );

    internal static async Task RunAsync()
    {
        if (!Directory.Exists(inkscapeOutputPath))
            Directory.CreateDirectory(inkscapeOutputPath);

        var files = Directory.EnumerateFiles(rootPath, "*.svg");

        await Util.StrokeToPathAsync(files, inkscapeOutputPath);

        var iconKinds = new Dictionary<string, string>();

        foreach (var path in Directory.EnumerateFiles(inkscapeOutputPath, "*.svg"))
        {
            var id = Path.GetFileNameWithoutExtension(path);
            var data = Util.GetSvgData(path);
            if (!string.IsNullOrEmpty(data))
            {
                iconKinds.Add(Util.GetCamelId(id), data);
            }
        }

        Util.OutputIconKindFile(iconKinds, "Feather", "Regular");
    }
}
