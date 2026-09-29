using System.Diagnostics;
using System.Drawing;
using System.Text;
using Svg;

namespace IconPacksGenerator;

internal static class Util
{
    internal static async Task StrokeToPathAsync(IEnumerable<string> files, string outputDir)
    {
        var chunkSize = (int)Math.Ceiling(files.Count() / 12d);
        var buckets = files.Chunk(chunkSize).ToArray();

        await Task.WhenAll(buckets.Select((bucket) => RunInkscapeShellAsync(bucket, outputDir)));
    }

    private static async Task RunInkscapeShellAsync(IReadOnlyList<string> bucket, string outputDir)
    {
        var psi = new ProcessStartInfo
        {
            FileName = Paths.InkscapePath,
            ArgumentList = { "--shell" },
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        using var proc =
            Process.Start(psi)
            ?? throw new InvalidOperationException($"Failed to start Inkscape shell");

        var stdoutTask = Task.Run(async () =>
        {
            string? line;
            while ((line = await proc.StandardOutput.ReadLineAsync()) != null) { }
        });
        var stderrTask = Task.Run(async () =>
        {
            string? line;
            while ((line = await proc.StandardError.ReadLineAsync()) != null) { }
        });

        try
        {
            foreach (var file in bucket)
            {
                var outputPath = Path.Combine(outputDir, Path.GetFileName(file));

                if (
                    !File.Exists(outputPath)
                    || File.GetLastWriteTime(file) > File.GetLastWriteTime(outputPath)
                )
                {
                    await proc.StandardInput.WriteLineAsync(
                        $"file-open:{file};select-all;object-stroke-to-path;path-union;export-plain-svg;export-filename:{outputPath};export-do;file-close"
                    );
                }
            }
        }
        finally
        {
            proc.StandardInput.Close();
        }

        await proc.WaitForExitAsync();
        await Task.WhenAll(stdoutTask, stderrTask);

        if (proc.ExitCode != 0)
            throw new InvalidOperationException($"Inkscape shell exited with code {proc.ExitCode}");
    }

    internal static string GetCamelId(this string id)
    {
        var strings = new List<string>();
        var list = id.Replace('-', '_').Split('_');
        if (int.TryParse(list[0][0..1], out var _))
        {
            list[0] = $"_{list[0]}";
        }
        foreach (var s in list)
        {
            if (s.Length > 1)
                strings.Add($"{s[0..1].ToUpper()}{s[1..]}");
            else
                strings.Add(s.ToUpper());
        }

        var result = string.Join(string.Empty, strings);

        if (result.Equals("Equals"))
            return "_Equals";

        return result;
    }

    internal static string? GetSvgData(string path)
    {
        var result = new List<string>();
        if (File.Exists(path))
        {
            var svgDoc = SvgDocument.Open(path);
            foreach (var element in svgDoc.Children)
            {
                if (element is SvgPath p)
                {
                    result.Add(p.PathData.ToString());
                }
            }

            if (result.Count > 0)
            {
                return AddViewBox(string.Join(' ', result), svgDoc.ViewBox);
            }
        }
        return default;
    }

    internal static string AddViewBox(string pathdata, RectangleF viewBox)
    {
        return !viewBox.IsEmpty
            ? $"M{viewBox.X} {viewBox.Y} z M{viewBox.Width - viewBox.X} {viewBox.Height - viewBox.Y} z {pathdata}"
            : pathdata;
    }

    internal static void OutputIconKindFile(
        Dictionary<string, string> iconKinds,
        string type,
        string variant
    )
    {
        var sb = new StringBuilder();
        sb.AppendLine($"namespace IconPacks.{type}");
        sb.AppendLine("{");
        sb.AppendLine($"\tpublic static class {variant}");
        sb.AppendLine("\t{");

        if (iconKinds.Count > 0)
        {
            foreach (var kind in iconKinds)
            {
                if (string.Equals(kind.Key, variant))
                    sb.AppendLine($"\t\tpublic const string _{kind.Key} = \"{kind.Value}\";");
                else
                    sb.AppendLine($"\t\tpublic const string {kind.Key} = \"{kind.Value}\";");
            }
            sb.AppendLine("\t}\r\n}");

            if (!Directory.Exists(Path.Combine(Paths.RootPath, $"./IconPacks.{type}")))
            {
                Directory.CreateDirectory(Path.Combine(Paths.RootPath, $"./IconPacks.{type}"));
            }

            File.WriteAllText(
                Path.Combine(Paths.RootPath, $"./IconPacks.{type}/{variant}.cs"),
                sb.ToString()
            );
        }
    }
}
