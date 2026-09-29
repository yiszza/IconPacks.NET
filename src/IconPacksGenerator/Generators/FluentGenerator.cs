using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CliWrap;
using Svg;

namespace IconPacksGenerator.Generators;

internal static class FluentGenerator
{
    private static readonly string rootPath = Path.Combine(Paths.FluentIconPath, "./assets/");
    private static readonly string templatePath = Path.Combine(
        Paths.InkscapeOutputPath,
        "./template/",
        "Fluent"
    );

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
        var variantDir = Path.Combine(templatePath, variant);

        if (!Directory.Exists(variantDir))
            Directory.CreateDirectory(variantDir);

        if (!Directory.Exists(variantOutputPath))
            Directory.CreateDirectory(variantOutputPath);

        {
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
                    var templateFile = Path.Combine(templatePath, variant, $"{filename}");
                    if (
                        !File.Exists(templateFile)
                        || File.GetLastWriteTime(file) > File.GetLastWriteTime(templateFile)
                    )
                    {
                        File.Copy(file, templateFile, true);
                        File.SetLastWriteTime(templateFile, DateTime.Now);
                    }
                }
            );
        }

        {
            var files = Directory.EnumerateFiles(variantDir, "*.svg", SearchOption.AllDirectories);

            await Util.StrokeToPathAsync(files, variantOutputPath);
        }

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
