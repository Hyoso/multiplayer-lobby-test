using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ArrayExtensions
{
    public static int Find<T>(this T[] array, T searchType)
    {
        for (int i = 0; i < array.Length; ++i)
        {
            if (array[i].Equals(searchType))
            {
                return i;
            }
        }

        return -1;
    }

    public static void ForEach<T>(this T[] array, Action<T> lambda)
    {
        foreach (var v in array)
        {
            lambda(v);
        }
    }
}