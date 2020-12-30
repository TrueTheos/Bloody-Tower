using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RNG
{
    public static int Seed = 3;

    static System.Random random;

    public static void Reset(int Seed = -1)
    {
        if (Seed == -1)
        {
            random = new System.Random();
        }
        else
        {
            random = new System.Random(Seed);
        }
    }
    // [min,max[
    public static int Range(int min, int max)
    {
        return random.Next(min, max);
    }
    public static float Range(float min, float max)
    {
        return min + (Next() * (max - min));
    }
    public static float Next()
    {
        return (float)random.NextDouble();
    }

    static RNG()
    {
        random = new System.Random(Seed);
    }
}
