namespace Radish.Windowing.SDL3.Utility;

internal static class LinqExtensions
{
    // FirstOrDefault but with a manually passed in extra parameter,
    // avoids closure allocations when we need the predicate to access some outside value.
    public static T? FirstOrDefault<T, TParam>(this IEnumerable<T> items, Func<T, TParam, bool> predicate,
        TParam predicateParam)
    {
        foreach (var i in items)
        {
            if (predicate(i, predicateParam))
                return i;
        }

        return default;
    }
}