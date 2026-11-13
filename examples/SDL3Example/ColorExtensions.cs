using System.Drawing;
using static SDL3.SDL;

namespace Radish.Windowing.Examples;

public static class ColorExtensions
{
    public static SDL_FColor ToSdlColor(this Color c)
    {
        return new SDL_FColor
        {
            r = (float)c.R / byte.MaxValue,
            g = (float)c.G / byte.MaxValue,
            b = (float)c.B / byte.MaxValue,
            a = (float)c.A / byte.MaxValue
        };
    }
}