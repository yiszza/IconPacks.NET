using System.Drawing;
using System.Text;
using Svg;

namespace IconPacksGenerator;

internal static class Util
{
    internal static string GetCamelId(this string id)
    {
        var result = new List<string>();
        var list = id.Replace('-', '_').Split('_');
        if (int.TryParse(list[0][0..1], out var v))
        {
            list[0] = $"_{list[0]}";
        }
        foreach (var s in list)
        {
            if (s.Length > 1)
                result.Add($"{s[0..1].ToUpper()}{s[1..]}");
            else
                result.Add(s.ToUpper());
        }
        return string.Join(string.Empty, result);
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
            ? $"M{viewBox.X},{viewBox.Y} M{viewBox.Width - viewBox.X},{viewBox.Height - viewBox.Y} {pathdata}"
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
                    sb.AppendLine($"\t\tpublic static string _{kind.Key} = \"{kind.Value}\";");
                else
                    sb.AppendLine($"\t\tpublic static string {kind.Key} = \"{kind.Value}\";");
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
