using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPos
{
    int PosX { get; }
    int PosY { get; }
}
public static class PosExtension
{
    public static Vector2Int GetPos(this IPos target)
    {
        return new Vector2Int(target.PosX, target.PosY);
    }
}
