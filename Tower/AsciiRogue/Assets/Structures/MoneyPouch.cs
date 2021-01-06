using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPouch : Structure
{
    
    public Vector2Int pos;

    public override void Use()
    {

    }

    public override void WalkIntoTrigger()
    {
        int cash = MapManager.CurrentFloorIndex + Random.Range(1,6);
        GameManager.manager.playerStats.__coins += cash;

        GameManager.manager.UpdateMessages($"You picked up <color=yellow>{cash}</color> coins!");

        MapManager.map[pos.x, pos.y].structure = null;
        MapManager.map[pos.x, pos.y].exploredColor = new Color(1,1,1);
        MapManager.map[pos.x, pos.y].type = "Floor";
        DungeonGenerator.dungeonGenerator.DrawMap(MapManager.map);
    }
}
