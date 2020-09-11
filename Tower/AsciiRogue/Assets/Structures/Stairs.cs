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
        MapManager.map[MapManager.playerPos.x, MapManager.playerPos.y].hasPlayer = false;
        MapManager.map[MapManager.playerPos.x, MapManager.playerPos.y].letter = "";

        DungeonGenerator.dungeonGenerator.GenerateDungeon(dungeonLevelId);
    }

    public override void WalkIntoTrigger()
    {
        
    }
}
