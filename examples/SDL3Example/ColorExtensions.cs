using System.Drawing;
using SDL3;

namespace Radish.Windowing.Examples;

public static class ColorExtensions
{
    public static SDL.FColor ToSdlColor(this Color c)
    {
        return new SDL.FColor(
            (float)c.R / byte.MaxValue,
            (float)c.G / byte.MaxValue,
            (float)c.B / byte.MaxValue,
            (float)c.A / byte.MaxValue
        );
    }
}