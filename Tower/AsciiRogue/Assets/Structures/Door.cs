using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Structure
{
    public bool opened = false;
    public Vector2Int position;


    public override void Use()
    {
        if (opened)
        {

        }

        if (!opened)
        {
            opened = true;
            MapManager.map[position.x, position.y].isWalkable = true;
            MapManager.map[position.x, position.y].isOpaque = false;
            MapManager.map[position.x, position.y].baseChar = "/";
            DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
        }
    }

    public override void WalkIntoTrigger()
    {

    }
}
