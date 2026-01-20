using CliWrap;

namespace IconPacksGenerator;

internal class FontAwesomeGenerator
{
    private static readonly string rootPath = Path.Combine(
        Paths.FontAwesomeIconPath,
        "./svgs-full/"
    );

    private static readonly string inkscapeOutputPath = Path.Combine(
        Paths.InkscapeOutputPath,
        "FontAwesome"
    );

    internal static async Task RunAsync()
    {
        if (!Directory.Exists(inkscapeOutputPath))
            Directory.CreateDirectory(inkscapeOutputPath);

        foreach (var path in Directory.EnumerateDirectories(rootPath))
        {
            await RunAsync(Path.GetFileName(path));
        }
    }

    private static async Task RunAsync(string variant)
    {
        var variantOutputPath = Path.Combine(inkscapeOutputPath, variant);
        if (!Directory.Exists(variantOutputPath))
            Directory.CreateDirectory(variantOutputPath);

        var files = Directory.EnumerateFiles(Path.Combine(rootPath, variant), "*.svg");

        await Parallel.ForEachAsync(
            files,
            new ParallelOptions { MaxDegreeOfParallelism = 12 },
            async (file, _) =>
            {
                var filename = Path.GetFileName(file);
                var outputPath = Path.Combine(variantOutputPath, filename);

                if (!Path.Exists(outputPath))
                {
                    await Cli.Wrap(Paths.InkscapePath)
                        .WithArguments(
                            $"{file} --actions=\"select-all;object-stroke-to-path;path-union;export-plain-svg;export-filename:{outputPath};export-do\""
                        )
                        .ExecuteAsync(_);
                }
            }
        );

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
            "FontAwesome",
            $"{char.ToUpperInvariant(variant[0])}{variant[1..]}"
        );
    }
}
