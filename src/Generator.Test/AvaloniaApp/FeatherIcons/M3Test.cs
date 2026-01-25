using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Media;

namespace Material3.Primitives;

public class IconSource
{
    public object? Value { get; set; }

    public static implicit operator IconSource(Geometry g)
    {
        return new IconSource { Value = g };
    }
}

[IconPacks.Generator.M3GeometryConverter(typeof(IconPacks.Feather.Regular))]
public static partial class Regular { }
