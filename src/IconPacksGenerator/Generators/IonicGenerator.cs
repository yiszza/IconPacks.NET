using CliWrap;

namespace IconPacksGenerator.Generators;

internal static class IonicGenerator
{
    private static readonly string rootPath = Path.Combine(Paths.IonicIconPath, "./src/svg/");

    private static readonly string inkscapeOutputPath = Path.Combine(
        Paths.InkscapeOutputPath,
        "Ionic"
    );

    internal static async Task RunAsync()
    {
        if (!Directory.Exists(inkscapeOutputPath))
            Directory.CreateDirectory(inkscapeOutputPath);

        var files = Directory.EnumerateFiles(rootPath, "*.svg");

        await Parallel.ForEachAsync(
            files,
            new ParallelOptions { MaxDegreeOfParallelism = 12 },
            async (file, _) =>
            {
                var filename = Path.GetFileName(file);
                var outputPath = Path.Combine(inkscapeOutputPath, filename);

                if (
                    !Path.Exists(outputPath)
                    || File.GetLastWriteTime(file) > File.GetLastWriteTime(outputPath)
                )
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

        foreach (var path in Directory.EnumerateFiles(inkscapeOutputPath, "*.svg"))
        {
            var id = Path.GetFileNameWithoutExtension(path);
            var data = Util.GetSvgData(path);
            if (!string.IsNullOrEmpty(data))
            {
                iconKinds.Add(Util.GetCamelId(id), data);
            }
        }

        Util.OutputIconKindFile(iconKinds, "Ionic", "Regular");
    }
}
