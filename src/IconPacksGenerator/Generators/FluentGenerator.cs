using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CliWrap;

namespace IconPacksGenerator.Generators;

internal static class FluentGenerator
{
    private static readonly string rootPath = Path.Combine(Paths.FluentIconPath, "./assets/");

    private static readonly string inkscapeOutputPath = Path.Combine(
        Paths.InkscapeOutputPath,
        "Fluent"
    );

    internal static async Task RunAsync()
    {
        if (!Directory.Exists(inkscapeOutputPath))
            Directory.CreateDirectory(inkscapeOutputPath);

        await RunAsync("filled");
        await RunAsync("regular");
    }

    internal static async Task RunAsync(string variant)
    {
        var variantOutputPath = Path.Combine(inkscapeOutputPath, variant);

        if (!Directory.Exists(variantOutputPath))
            Directory.CreateDirectory(variantOutputPath);

        var files = Directory.EnumerateFiles(
            rootPath,
            $"*_20_{variant}.svg",
            SearchOption.AllDirectories
        );

        await Parallel.ForEachAsync(
            files,
            new ParallelOptions { MaxDegreeOfParallelism = 12 },
            async (file, _) =>
            {
                var filename = Path.GetFileName(file)
                    .Replace("ic_fluent_", string.Empty)
                    .Replace($"_20_{variant}", string.Empty);
                var outputPath = Path.Combine(variantOutputPath, filename);

                if (
                    !File.Exists(outputPath)
                    || File.GetLastWriteTime(file) > File.GetLastWriteTime(outputPath)
                )
                {
                    await Cli.Wrap(Paths.InkscapePath)
                        .WithArguments(
                            $"\"{file}\" --actions=\"select-all;object-stroke-to-path;path-union;export-plain-svg;export-filename:{outputPath};export-do\""
                        )
                        .ExecuteAsync(_);
                }
            }
        );

        var iconKinds = new Dictionary<string, string>();

        foreach (var path in Directory.EnumerateFiles(variantOutputPath, "*.svg"))
        {
            var id = Path.GetFileNameWithoutExtension(path);
            var data = Util.GetSvgData(path);
            if (!string.IsNullOrEmpty(data))
            {
                iconKinds.Add(Util.GetCamelId(id), data);
            }
        }

        Util.OutputIconKindFile(
            iconKinds,
            "Fluent",
            $"{char.ToUpperInvariant(variant[0])}{variant[1..]}"
        );
    }
}
