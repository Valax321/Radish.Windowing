namespace Radish.Windowing.SDL3.Utility;

internal static class EnumUtility
{
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