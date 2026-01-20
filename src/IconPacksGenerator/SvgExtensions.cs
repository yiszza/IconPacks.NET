using Svg;

namespace IconPacksGenerator;

internal static class SvgExtensions
{
    internal static string ParsePath(this SvgElement element)
    {
        if (element is not SvgPath path)
            return string.Empty;
        return path.PathData.ToString();
    }
}
