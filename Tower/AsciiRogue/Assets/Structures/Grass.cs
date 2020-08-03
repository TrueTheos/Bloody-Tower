using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : Structure
{
    public Vector2Int pos;

    public override void Use() { }

    public override void WalkIntoTrigger()
    {
        MapManager.map[pos.x, pos.y].isOpaque = false;
        MapManager.map[pos.x, pos.y].structure = null;
        MapManager.map[pos.x, pos.y].exploredColor = new Color(1,1,1);
        MapManager.map[pos.x, pos.y].type = "Floor";
        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
    }
}
