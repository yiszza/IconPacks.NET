using CliWrap;
using Svg;

namespace IconPacksGenerator;

internal static class MaterialGenerator
{
    private static readonly string rootPath = Path.Combine(Paths.MaterialIconPath, "./src/");

    private static readonly string inkscapeOutputPath = Path.Combine(
        Paths.InkscapeOutputPath,
        "Material"
    );

    internal static async Task RunAsync()
    {
        await RunAsync("normal");
        await RunAsync("outlined");
        await RunAsync("round");
        await RunAsync("sharp");
        //await RunAsync("twotone");
    }

    internal static async Task RunAsync(string variant)
    {
        var variantOutputPath = Path.Combine(inkscapeOutputPath, variant);
        var cachePath = Path.Combine(variantOutputPath, "cache");

        if (!Directory.Exists(variantOutputPath))
            Directory.CreateDirectory(variantOutputPath);
        if (!Directory.Exists(cachePath))
            Directory.CreateDirectory(cachePath);

        var variantDirName = $"\\materialicons{(variant is "normal" ? string.Empty : variant)}\\";
        var files = Directory
            .EnumerateFiles(rootPath, "24px.svg", SearchOption.AllDirectories)
            .Where(file => file.Contains(variantDirName));

        await Parallel.ForEachAsync(
            files,
            new ParallelOptions { MaxDegreeOfParallelism = 12 },
            async (file, _) =>
            {
                var filename = file.Split(Path.DirectorySeparatorChar)[^3];
                var cacheFile = Path.Combine(cachePath, $"{filename}.svg");
                var outputPath = Path.Combine(variantOutputPath, $"{filename}.svg");

                if (!Path.Exists(outputPath))
                {
                    var doc = SvgDocument.Open(file);
                    var s = doc.Descendants();
                    var targets = doc.Descendants()
                        ?.Where(e =>
                        {
                            return e.TryGetAttribute("fill", out var fill) && fill == "none";
                        })
                        ?.ToList();

                    if (targets != null)
                    {
                        foreach (var node in targets)
                            node.Parent.Children.Remove(node);
                    }

                    doc.Write(cacheFile);

                    await Cli.Wrap(Paths.InkscapePath)
                        .WithArguments(
                            $"{cacheFile} --actions=\"select-all;object-stroke-to-path;path-union;export-plain-svg;export-filename:{outputPath};export-do\""
                        )
                        .ExecuteAsync(_);
                }
            }
        );

        Directory.Delete(cachePath, true);

        var iconKinds = new Dictionary<string, string>();

        foreach (var file in Directory.EnumerateFiles(variantOutputPath, "*.svg"))
        {
            var id = Path.GetFileNameWithoutExtension(file);
            var data = Util.GetSvgData(file);
            if (!string.IsNullOrEmpty(data))
            {
                iconKinds.Add(Util.GetCamelId(id), data);
            }
        }

        Util.OutputIconKindFile(
            iconKinds,
            "Material",
            $"{char.ToUpperInvariant(variant[0])}{variant[1..]}"
        );
    }
}
