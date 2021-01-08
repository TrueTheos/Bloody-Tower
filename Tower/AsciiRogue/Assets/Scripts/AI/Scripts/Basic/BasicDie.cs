using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Die/Basic")]
public class BasicDie : BaseAIBehaviour<RoamingNPC>
{

    public override void Calculate(RoamingNPC t)
    {
        if (!t.isDead) t.isDead = true;
        else return;

        if (t.enemySO.leavesCorpse)
        {
            MapManager.map[t.__position.x, t.__position.y].enemy = null;
            MapManager.map[t.__position.x, t.__position.y].isWalkable = true;
            Corps corpse = new Corps();     

            //ITEM IN CORPSE

            bool droppedItem = false;


            //CHANCE TO DROP CORPSE ITEM
            if (t.enemySO.E_possileDrops != null && t.enemySO.E_possileDrops.Count > 0)
            {
                corpse.itemInCorpse = t.enemySO.E_possileDrops[Random.Range(0, t.enemySO.E_possileDrops.Count)];
                if(corpse.itemInCorpse != null)
                {
                    droppedItem = true;
                }
            }

            if (MapManager.map[t.__position.x, t.__position.y].structure == null)
            {
                /*if (droppedItem)
                {
                    /*MapManager.map[t.__position.x, t.__position.y].timeColor = new Color(0, 0, 0);
                    MapManager.map[t.__position.x, t.__position.y].letter = "";
                    GameManager.manager.itemSpawner.SpawnAt(t.__position.x, t.__position.y, corpse.itemInCorpse);
                }
                else
                {
                    MapManager.map[t.__position.x, t.__position.y].baseChar = t.EnemySymbol;
                    MapManager.map[t.__position.x, t.__position.y].exploredColor = new Color(0.2784f, 0, 0);
                    MapManager.map[t.__position.x, t.__position.y].letter = "";
                }*/

                if(droppedItem)
                {
                    GameManager.manager.itemSpawner.SpawnAt(MapManager.CurrentFloor, t.__position.x, t.__position.y, corpse.itemInCorpse);
                }

                MapManager.map[t.__position.x, t.__position.y].baseChar = t.EnemySymbol;
                MapManager.map[t.__position.x, t.__position.y].exploredColor = new Color(0.2784f, 0, 0);
                MapManager.map[t.__position.x, t.__position.y].letter = "";

                corpse.enemyBody = t.enemySO;
                MapManager.map[t.__position.x, t.__position.y].structure = corpse;
            }      
        }
        else
        {
            MapManager.map[t.__position.x, t.__position.y].enemy = null;
            MapManager.map[t.__position.x, t.__position.y].letter = "";
            MapManager.map[t.__position.x, t.__position.y].isWalkable = true;
        }

        t.manager.playerStats.__sanity += 10;

        t.manager.UpdateMessages($"You have killed the <color=#{t.EnemyColor}>{t.EnemyName}</color>");
        RunManager.CurrentRun.Set(RunManager.Names.EnemiesKilled,
            RunManager.CurrentRun.Get<int>(RunManager.Names.EnemiesKilled)+1);
        t.manager.playerStats.UpdateLevel(t.xpDrop);
        /*
        GameObject e = null;

        foreach (var enemy in manager.enemies)
        {
            if(enemy.GetComponent<RoamingNPC>().__position == __position)
            {
                e = enemy;
            }
        }

        foreach (var enemy in t.manager.enemies)
        {
            if (enemy == t.gameObject)
            {
                e = enemy;
            }
        }
        */

        t.manager.StartPlayersTurn();

        t.manager.gameObject.GetComponent<Bestiary>().UpdateEnemyList(t.enemySO);

        t.manager.enemies.Remove(t.gameObject);

        DungeonGenerator.dungeonGenerator.DrawMap(MapManager.map);


        Destroy(t.gameObject);

    }

}
