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
            if (GameManager.manager.playerStats.__sanity <= 75)
            {
                if (GameManager.manager.playerStats.__sanity <= 20)
                {
                    if (UnityEngine.Random.Range(1, 100) <= 30)
                    {
                        GameManager.manager.UpdateMessages("You want to open the door, but you know you'll find something terrible behind it.");
                        return;
                    }
                }
                else if (GameManager.manager.playerStats.__sanity <= 50)
                {
                    if (UnityEngine.Random.Range(1, 100) <= 10)
                    {
                        GameManager.manager.UpdateMessages("You try to open the door, but you are to afraid of what you might see behind it.");
                        return;
                    }
                }
                else
                {
                    if (UnityEngine.Random.Range(1, 100) <= 5)
                    {
                        GameManager.manager.UpdateMessages("You try to open the door, but you are to afraid of what you might see behind it.");
                        return;
                    }
                }
            }

            opened = true;
            MapManager.map[position.x, position.y].isWalkable = true;
            MapManager.map[position.x, position.y].isOpaque = false;
            MapManager.map[position.x, position.y].baseChar = "/";
            DungeonGenerator.dungeonGenerator.DrawMap(MapManager.map);
        }
    }

    public override void WalkIntoTrigger()
    {

    }
}
