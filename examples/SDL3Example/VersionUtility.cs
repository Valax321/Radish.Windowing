namespace Radish.Windowing.Examples;

internal static class VersionUtility
{
    public static Version ParseSdlVersion(int version)
    {
        return new Version(version / 1000000, (version / 1000) % 1000, version % 1000);
    }
}