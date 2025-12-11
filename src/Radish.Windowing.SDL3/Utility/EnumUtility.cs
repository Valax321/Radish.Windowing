namespace Radish.Windowing.SDL3.Utility;

internal static class EnumUtility
{
    /// <summary>
    /// Gets a map of enum values to an index that can be used to look them up in lists, etc.
    /// Used internally to map enum values to BitArray indices for input device button states.
    /// </summary>
    /// <remarks>The actual index value of each item is not guaranteed, and may even differ between assembly versions or .NET runtimes.
    /// The value should be treated as completely transient to this application instance.</remarks>
    /// <typeparam name="T">The type of enum to map.</typeparam>
    /// <returns>The map of enum values to indices.</returns>
    public static Dictionary<T, int> GetEnumIndexMap<T>() where T : struct, Enum
    {
        var map = new Dictionary<T, int>();
        var items = Enum.GetValues<T>();
        for (var i = 0; i < items.Length; ++i)
        {
            map.Add(items[i], i);
        }
        return map;
    }
}