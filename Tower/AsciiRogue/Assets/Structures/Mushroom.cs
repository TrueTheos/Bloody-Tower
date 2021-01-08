using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : Structure
{
    public Vector2Int pos;

    public override void Use() 
    {
        if(GameManager.manager.playerStats.hasPoisonResistance)
        {
            GameManager.manager.UpdateMessages("You ate mushroom but it was poisonous. You lose <color=red>1 health</color>.");
            GameManager.manager.playerStats.TakeDamage(1,ItemScriptableObject.damageType.normal);
        }
        else if(Random.Range(0,100) > 40 + GameManager.manager.playerStats.__endurance)
        {
            if(Random.Range(0,3) > 1)
            {
                GameManager.manager.UpdateMessages("You ate mushroom but it was poisonous. You lose <color=red>1 health</color>.");
                GameManager.manager.playerStats.TakeDamage(1, ItemScriptableObject.damageType.normal);
            }
            else
            {
                GameManager.manager.UpdateMessages("You feel pain in your chest, suddenly <color=red>blood</color> is pouring out of your mouth.");
                GameManager.manager.UpdateMessages("You lose <color=red>1 health</color>.");
                GameManager.manager.playerStats.LossBlood(2);
                GameManager.manager.playerStats.TakeDamage(1, ItemScriptableObject.damageType.normal);
            }       
        }
        else
        {
            GameManager.manager.UpdateMessages("You ate mushroom, you restore <color=red>1 health</color>.");
            if (GameManager.manager.playerStats.__currentHp < GameManager.manager.playerStats.__maxHp) GameManager.manager.playerStats.__currentHp++;
        }

        MapManager.map[pos.x, pos.y].isOpaque = false;
        MapManager.map[pos.x, pos.y].baseChar = ".";
        MapManager.map[pos.x, pos.y].structure = null;
        MapManager.map[pos.x, pos.y].exploredColor = new Color(1, 1, 1);
        MapManager.map[pos.x, pos.y].type = "Floor";
        DungeonGenerator.dungeonGenerator.DrawMap(MapManager.map);
    }

    public override void WalkIntoTrigger()
    {
        GameManager.manager.UpdateMessages("Press <color=yellow>'space'</color> to eat mushroom.");
    }
}
