using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListExtensions
{
    public static void Shuffle<T>(this List<T> list)
    {
        int c = list.Count;
        while (c > 1)
        {
            // get a random index to swap with c
            int randIndex = Random.Range(0, c);
            c--;

            // swap randindex with c
            T copy = list[randIndex];
            list[randIndex] = list[c];
            list[c] = copy;
        }
    }

    public static T GetRandomElementAndRemove<T>(this List<T> list)
    {
        T copy = list[UnityEngine.Random.Range(0, list.Count)];
        list.Remove(copy);
        return copy;
    }

    public static T GetRandom<T>(this List<T> list)
    {
        T copy = list[UnityEngine.Random.Range(0, list.Count)];
        return copy;
    }
}