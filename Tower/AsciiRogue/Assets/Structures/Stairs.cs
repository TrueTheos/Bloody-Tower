using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Stairs : Structure
{
    public int dungeonLevelId; //id of the floor where it will teloport you
    public Vector2Int spawnPosition;

    public override void Use()
    {
        if(DungeonGenerator.dungeonGenerator.currentFloor <= dungeonLevelId)
        {
            MapManager.map[MapManager.playerPos.x, MapManager.playerPos.y].hasPlayer = false;
            MapManager.map[MapManager.playerPos.x, MapManager.playerPos.y].letter = "";

            DungeonGenerator.dungeonGenerator.GenerateDungeon(dungeonLevelId);

            //DungeonGenerator.dungeonGenerator.MovePlayerToUpperStairs();
            //DungeonGenerator.dungeonGenerator.MovePlayerToLowerStairs();
        }
        else
        {
            MapManager.map[MapManager.playerPos.x, MapManager.playerPos.y].hasPlayer = false;
            MapManager.map[MapManager.playerPos.x, MapManager.playerPos.y].letter = "";

            DungeonGenerator.dungeonGenerator.GenerateDungeon(dungeonLevelId);

            //DungeonGenerator.dungeonGenerator.MovePlayerToLowerStairs();
            //DungeonGenerator.dungeonGenerator.MovePlayerToUpperStairs();
        }
    }

    public override void WalkIntoTrigger()
    {
        
    }
}
