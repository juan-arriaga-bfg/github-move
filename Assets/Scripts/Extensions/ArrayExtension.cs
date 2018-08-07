using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtension
{
    public static T GetAsTwoDimensional<T>(this IList<T> source, Vector2Int size, int x, int y)
    {
        var width = size.x;
        var height = size.y;
        return source[x * height + y % width];
    }

    public static void SetAsTwoDimensional<T>(this IList<T> source, Vector2Int size, int x, int y, T value)
    {
        var width = size.x;
        var height = size.y;
        source[x * height + y % width] = value;
    }

    public static void SwapAsTwoDimensional<T>(this IList<T> source, Vector2Int size, Vector2Int first, Vector2Int second)
    {
        var width = size.x;
        var height = size.y;

        var firstIndex = first.x * height + first.y % width;
        var secondIndex = second.x * height + second.y % width;

        T tmp = source[firstIndex];
        source[firstIndex] = source[secondIndex];
        source[secondIndex] = tmp;
    }

    public static void FlipX<T>(this IList<T> source, Vector2Int size)
    {
        for (int x = 0; x < size.x / 2; x++)
            for (int y = 0; y < size.y; y++)
                source.SwapAsTwoDimensional(size, new Vector2Int(x, y), new Vector2Int(size.x - x - 1, y));
    }
    
    public static void FlipY<T>(this IList<T> source, Vector2Int size)
    {
        for (int y = 0; y < size.y / 2; y++)
            for (int x = 0; x < size.x; x++)
                source.SwapAsTwoDimensional(size, new Vector2Int(x, y), new Vector2Int(x, size.y - y - 1));
    }

    public static void FlipXY<T>(this IList<T> source)
    {
        for (int i = 0; i < source.Count/2; i++)
        {
            var tmp = source[i];
            source[i] = source[source.Count - 1 - i];
            source[source.Count - 1 - i] = tmp;
        }
    }
}