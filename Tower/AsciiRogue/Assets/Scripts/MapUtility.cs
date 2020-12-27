using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MapUtility
{

    public static Vector2Int[] Horizontal1 = new Vector2Int[]
    {
        new Vector2Int(1,0),
        new Vector2Int(-1,0)
    };
    public static Vector2Int[] Vertical1 = new Vector2Int[]
    {
        new Vector2Int(0,1),
        new Vector2Int(0,-1)
    };
    public static Vector2Int[] Cross1 = Horizontal1.Copy().Combine(Vertical1.Copy());
    public static Vector2Int[] Diagonal1 = new Vector2Int[]
    {
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,-1)
    };
    public static Vector2Int[] Box1 = Cross1.Copy().Combine(Diagonal1.Copy());
    public static Vector2Int[] Origin = new Vector2Int[] { new Vector2Int(0, 0) };

    public static int MoveDistance(Vector2Int a, Vector2Int b) => Mathf.Max(Mathf.Abs(a.x - b.x), Mathf.Abs(a.y - b.y));

    public static bool IsInbounds(int gx, int gy) => !(gx < 0 || gy < 0 || gx >= MapManager.map.GetLength(0) || gy >= MapManager.map.GetLength(1));
    public static bool IsInbounds(Vector2Int pos) => !(pos.x < 0 || pos.y < 0 || pos.x >= MapManager.map.GetLength(0) || pos.y >= MapManager.map.GetLength(1));
    /// <summary>
    /// Check if a position can be moved to
    /// </summary>
    public static bool IsMoveable(int gx, int gy) => 
        IsInbounds(gx,gy) && 
        MapManager.map[gx,gy].isWalkable && 
        MapManager.map[gx, gy].enemy == null && 
        !MapManager.map[gx, gy].hasPlayer;

    public static bool IsMoveable(Vector2Int pos) => IsMoveable(pos.x, pos.y);

    public static Vector2Int[] Combine(this Vector2Int[] me, Vector2Int[] other)
    {
        Vector2Int[] res = new Vector2Int[me.Length + other.Length];
        int ci = 0;
        for (int i = 0; i < me.Length; i++)
        {
            res[ci] = me[i];
            ci++;
        }
        for (int i = 0; i < other.Length; i++)
        {
            res[ci] = other[i];
            ci++;
        }
        return res;
    }
    public static Vector2Int[] MoveCenter(this Vector2Int[] target, Vector2Int newPivit)
    {
        Vector2Int[] res = new Vector2Int[target.Length];
        for (int i = 0; i < target.Length; i++)
        {
            res[i] = newPivit + target[i];
        }
        return res;
    }

    public static Vector2Int[] RemoveOutOfBounds(this Vector2Int[] target)
    {
        int n = 0;
        for (int i = 0; i < target.Length; i++)
        {
            if (IsInbounds(target[i]))
            {
                n++;
            }
        }
        Vector2Int[] res = new Vector2Int[n];
        int c = 0;
        for (int i = 0; i < target.Length; i++)
        {
            if (IsInbounds(target[i]))
            {
                res[c] = target[i];
                c++;
            }
        }
        return res;
    }

    public static T[] Check<T>(this T[] target, System.Func<T, bool> valid)
    {
        bool[] v = new bool[target.Length];
        int vC = 0;
        for (int i = 0; i < target.Length; i++)
        {
            if (valid(target[i]))
            {
                vC++;
                v[i] = true;
            }
        }
        int c = 0;
        T[] res = new T[vC];
        for (int i = 0; i < target.Length; i++)
        {
            if (v[i])
            {
                res[c] = target[i];
                c++;
            }
        }
        return res;
    }
    public static T[] Copy<T>(this T[] target)
    {
        T[] res = new T[target.Length];
        for (int i = 0; i < target.Length; i++)
        {
            res[i] = target[i];
        }
        return res;
    }
    public static T[] Randomize<T>(this T[] target)
    {
        int n = target.Length;
        while (n > 1)
        {
            int k = RNG.Range(0,n--);
            T temp = target[n];
            target[n] = target[k];
            target[k] = temp;
        }
        return target;
    }


    public static T[] AsArray<T>(this HashSet<T> me)
    {
        T[] arr = new T[me.Count];
        int i = 0;
        foreach (var item in me)
        {
            arr[i] = item;
            i++;
        }
        return arr;
    }
    public static IEnumerable<Vector2Int> GetSimpleNeighbours(Vector2Int position,System.Func<Vector2Int,bool> valid = null, bool repeat = false)
    {

        Vector2Int[] nOrder = Box1.Copy().MoveCenter(position).Check((vec) => valid != null ? (valid(vec) && IsInbounds(vec)):IsInbounds(vec)).Randomize();

        int n = nOrder.Length;
        int i = 0;

        if (n==0)
        {
            yield break;
        }
        do
        {
            yield return nOrder[i % n];
            i++;
        } while (repeat);
    }
    public static IEnumerable<T> MakeEnumerator<T>(this T[] data,bool repeat = false)
    {
        int n = data.Length;
        int i = 0;

        if (n == 0)
        {
            yield break;
        }
        do
        {
            yield return data[i % n];
            i++;
        } while (repeat);
    }

    public static IEnumerable<T> EnumerateRandom<T>(this IList<T> data)
    {
        int[] index = new int[data.Count];
        for (int i = 0; i < data.Count; i++)
        {
            index[i] = i;
        }
        index = index.Randomize();

        for (int i = 0; i < data.Count; i++)
        {
            yield return data[index[i]];
        }
    }
}