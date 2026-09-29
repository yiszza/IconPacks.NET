using CliWrap;
using Svg;

namespace IconPacksGenerator.Generators;

internal static class MaterialGenerator
{
    private static readonly string rootPath = Path.Combine(Paths.MaterialIconPath, "./src/");
    private static readonly string templatePath = Path.Combine(
        Paths.InkscapeOutputPath,
        "./template/",
        "Material"
    );

    private static readonly string inkscapeOutputPath = Path.Combine(
        Paths.InkscapeOutputPath,
        "Material"
    );

    internal static async Task RunAsync()
    {
        await RunAsync("regular");
        await RunAsync("outlined");
        await RunAsync("round");
        await RunAsync("sharp");
        await RunAsync("twotone");
    }

    internal static async Task RunAsync(string variant)
    {
        var variantOutputPath = Path.Combine(inkscapeOutputPath, variant);

        var variantDir = Path.Combine(templatePath, variant);

        if (!Directory.Exists(variantDir))
            Directory.CreateDirectory(variantDir);

        if (!Directory.Exists(variantOutputPath))
            Directory.CreateDirectory(variantOutputPath);

        {
            var variantDirName =
                $"\\materialicons{(variant is "regular" ? string.Empty : variant)}\\";

            var files = Directory
                .EnumerateFiles(rootPath, "24px.svg", SearchOption.AllDirectories)
                .Where(file => file.Contains(variantDirName));

            await Parallel.ForEachAsync(
                files,
                new ParallelOptions { MaxDegreeOfParallelism = 12 },
                async (file, _) =>
                {
                    var filename = file.Split(Path.DirectorySeparatorChar)[^3];
                    var templateFile = Path.Combine(templatePath, variant, $"{filename}.svg");

                    if (
                        !File.Exists(templateFile)
                        || File.GetLastWriteTime(file) > File.GetLastWriteTime(templateFile)
                    )
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

                        doc.Write(templateFile);
                    }
                }
            );
        }

        {
            var files = Directory.EnumerateFiles(variantDir, "*.svg", SearchOption.AllDirectories);

            await Util.StrokeToPathAsync(files, variantOutputPath);
        }

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
