using System;
using System.Collections.Generic;
using System.Text;
using CliWrap;

namespace IconPacksGenerator.Generators;

internal static class RemixGenerator
{
    private static readonly string rootPath = Path.Combine(Paths.RemixIconPath, "./icons/");
    private static readonly string templatePath = Path.Combine(
        Paths.InkscapeOutputPath,
        "./template/",
        "Remix"
    );

    private static readonly string inkscapeOutputPath = Path.Combine(
        Paths.InkscapeOutputPath,
        "Remix"
    );

    internal static async Task RunAsync()
    {
        if (!Directory.Exists(inkscapeOutputPath))
            Directory.CreateDirectory(inkscapeOutputPath);

        await RunAsync("fill");
        await RunAsync("line");
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
                $"*-{variant}.svg",
                SearchOption.AllDirectories
            );

            await Parallel.ForEachAsync(
                files,
                new ParallelOptions { MaxDegreeOfParallelism = 12 },
                async (file, _) =>
                {
                    var filename = Path.GetFileName(file).Replace($"-{variant}", string.Empty);
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
            "Remix",
            $"{char.ToUpperInvariant(variant[0])}{variant[1..]}"
        );
    }
}
