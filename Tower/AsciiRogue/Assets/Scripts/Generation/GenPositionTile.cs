using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenPositionTile
{
    public GenTile Tile;
    public Vector2Int PositionG;

    public GenPositionTile(GenTile tile, Vector2Int positionG)
    {
        Tile = tile;
        PositionG = positionG;
    }

    public override bool Equals(object obj)
    {
        var tile = obj as GenPositionTile;
        if (tile == null)
        {
            return false;
        }
        return tile.Tile == Tile;
    }

    public override int GetHashCode()
    {
        var hashCode = -1891638552;
        hashCode = hashCode * -1521134295 + EqualityComparer<GenTile>.Default.GetHashCode(Tile);
        hashCode = hashCode * -1521134295 + EqualityComparer<Vector2Int>.Default.GetHashCode(PositionG);
        return hashCode;
    }
}
