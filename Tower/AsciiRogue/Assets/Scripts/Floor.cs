using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor 
{
    public bool Valid;
    public bool Active
    {
        get => GO?.activeInHierarchy ?? false;
        set => GO?.SetActive(value);
    }
    public bool EnteredBefore = false;
    public Tile[,] Tiles;
    public GameObject GO;
    public Vector2Int StairsDown = new Vector2Int(-1, -1);
    public Vector2Int StairsUp = new Vector2Int(-1, -1);
    public RandomFloorEventsSO randomEvent;

    public Tile this[int x, int y]
    {
        get => Tiles[x, y];
        set => Tiles[x, y] = value;
    }

    public static implicit operator Tile[,](Floor f)
    {
        return f.Tiles;
    }
}
