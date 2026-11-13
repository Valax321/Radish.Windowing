using System.Drawing;

namespace Radish.Windowing.SDL3;

internal sealed unsafe class SdlDisplay(uint displayId) : IDisplay
{
    public uint DisplayIndex => displayId;

    public VideoMode NativeVideoMode
    {
        get
        {
            var dm = SDL_GetDesktopDisplayMode(displayId);
            if (dm == null)
                throw new NativeWindowException(SDL_GetError());
            return new VideoMode(new Size(dm->w, dm->h), dm->refresh_rate);
        }
    }

    public VideoMode CurrentVideoMode
    {
        get
        {
            var dm = SDL_GetCurrentDisplayMode(displayId);
            if (dm == null)
                throw new NativeWindowException(SDL_GetError());
            return new VideoMode(new Size(dm->w, dm->h), dm->refresh_rate);
        }
    }

    public IEnumerable<VideoMode> FullscreenVideoModes
    {
        get
        {
            var modes = (SDL_DisplayMode**)SDL_GetFullscreenDisplayModes(displayId, out var cnt);
            if (modes == null)
                throw new NativeWindowException(SDL_GetError());

            var mArr = new VideoMode[cnt];

            for (var i = 0; i < cnt; ++i)
            {
                var dm = modes[i];
                mArr[i] = new VideoMode(new Size(dm->w, dm->h), dm->refresh_rate);
            }

            SDL_free((nint)modes);

            return mArr;
        }
    }

    public float ContentScale => SDL_GetDisplayContentScale(displayId);

    public bool SupportsHDR
    {
        get
        {
            var props = SDL_GetDisplayProperties(displayId);
            if (props == 0)
                throw new NativeWindowException(SDL_GetError());
            
            return SDL_GetBooleanProperty(props, SDL_PROP_DISPLAY_HDR_ENABLED_BOOLEAN, false);
        }
    }

    public IntPtr NativeHandle => (IntPtr)displayId;

    public override string ToString()
    {
        var n = SDL_GetDisplayName(displayId);
        if (n == null)
            throw new NativeWindowException(SDL_GetError());
        return n;
    }

    private bool Equals(SdlDisplay other)
    {
        return DisplayIndex == other.DisplayIndex;
    }

    public bool Equals(IDisplay? other)
    {
        return other is SdlDisplay sd && Equals(sd);
    }

    public override bool Equals(object? obj)
    {
        return ReferenceEquals(this, obj) || obj is SdlDisplay other && Equals(other);
    }

    public override int GetHashCode()
    {
        return DisplayIndex.GetHashCode();
    }

    public static bool operator ==(SdlDisplay? left, SdlDisplay? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(SdlDisplay? left, SdlDisplay? right)
    {
        return !Equals(left, right);
    }
}