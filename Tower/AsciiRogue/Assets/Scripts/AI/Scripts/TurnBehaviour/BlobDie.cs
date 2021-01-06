using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Die/Blob")]
public class BlobDie : BasicDie
{
    public override void Calculate(RoamingNPC t)
    {       
        Corps corpse = new Corps();

        bool droppedItem = false;

        if (Random.Range(1, 100) <= 100 && t.enemySO.E_possileDrops != null && t.enemySO.E_possileDrops.Count > 0)
        {
            corpse.itemInCorpse = t.enemySO.E_possileDrops[Random.Range(0, t.enemySO.E_possileDrops.Count)];
            droppedItem = true;
        }

        if (MapManager.map[t.__position.x, t.__position.y].structure == null)
        {
            Debug.Log("blob kill 1");

            if (droppedItem)
            {
                GameManager.manager.itemSpawner.SpawnAt(MapManager.CurrentFloor, t.__position.x, t.__position.y, corpse.itemInCorpse);
            }

            MapManager.map[t.__position.x, t.__position.y].baseChar = t.EnemySymbol;
            MapManager.map[t.__position.x, t.__position.y].exploredColor = new Color(0.2784f, 0, 0);
            MapManager.map[t.__position.x, t.__position.y].letter = "";

            MapManager.map[t.__position.x, t.__position.y].enemy = null;
            MapManager.map[t.__position.x, t.__position.y].isWalkable = true;

            corpse.enemyBody = t.enemySO;
            MapManager.map[t.__position.x, t.__position.y].structure = corpse;
        }

        if (MapManager.map[t.__position.x + 1, t.__position.y].structure == null && MapManager.map[t.__position.x + 1, t.__position.y].enemy?.GetComponent<RoamingNPC>().enemySO == t.enemySO)
        {
            Debug.Log("blob kill 2");
            MapManager.map[t.__position.x + 1, t.__position.y].baseChar = t.EnemySymbol;
            MapManager.map[t.__position.x + 1, t.__position.y].exploredColor = new Color(0.2784f, 0, 0);
            MapManager.map[t.__position.x + 1, t.__position.y].letter = "";

            MapManager.map[t.__position.x + 1, t.__position.y].enemy = null;
            MapManager.map[t.__position.x + 1, t.__position.y].isWalkable = true;

            corpse = new Corps();

            corpse.enemyBody = t.enemySO;
            MapManager.map[t.__position.x + 1, t.__position.y].structure = corpse;
        }
        if (MapManager.map[t.__position.x - 1, t.__position.y].structure == null && MapManager.map[t.__position.x - 1, t.__position.y].enemy?.GetComponent<RoamingNPC>().enemySO == t.enemySO)
        {
            Debug.Log("blob kill 3");
            MapManager.map[t.__position.x - 1, t.__position.y].baseChar = t.EnemySymbol;
            MapManager.map[t.__position.x - 1, t.__position.y].exploredColor = new Color(0.2784f, 0, 0);
            MapManager.map[t.__position.x - 1, t.__position.y].letter = "";

            MapManager.map[t.__position.x - 1, t.__position.y].enemy = null;
            MapManager.map[t.__position.x - 1, t.__position.y].isWalkable = true;

            corpse = new Corps();

            corpse.enemyBody = t.enemySO;
            MapManager.map[t.__position.x - 1, t.__position.y].structure = corpse;
        }
        if (MapManager.map[t.__position.x, t.__position.y + 1].structure == null && MapManager.map[t.__position.x, t.__position.y + 1].enemy?.GetComponent<RoamingNPC>().enemySO == t.enemySO)
        {
            MapManager.map[t.__position.x, t.__position.y + 1].baseChar = t.EnemySymbol;
            MapManager.map[t.__position.x, t.__position.y + 1].exploredColor = new Color(0.2784f, 0, 0);
            MapManager.map[t.__position.x, t.__position.y + 1].letter = "";

            MapManager.map[t.__position.x, t.__position.y + 1].enemy = null;
            MapManager.map[t.__position.x, t.__position.y + 1].isWalkable = true;

            corpse = new Corps();

            corpse.enemyBody = t.enemySO;
            MapManager.map[t.__position.x, t.__position.y + 1].structure = corpse;
        }
        if (MapManager.map[t.__position.x, t.__position.y - 1].structure == null && MapManager.map[t.__position.x, t.__position.y - 1].enemy?.GetComponent<RoamingNPC>().enemySO == t.enemySO)
        {
            Debug.Log("5");
            MapManager.map[t.__position.x, t.__position.y - 1].baseChar = t.EnemySymbol;
            MapManager.map[t.__position.x, t.__position.y - 1].exploredColor = new Color(0.2784f, 0, 0);
            MapManager.map[t.__position.x, t.__position.y - 1].letter = "";

            MapManager.map[t.__position.x, t.__position.y - 1].enemy = null;
            MapManager.map[t.__position.x, t.__position.y - 1].isWalkable = true;

            corpse = new Corps();

            corpse.enemyBody = t.enemySO;
            MapManager.map[t.__position.x, t.__position.y - 1].structure = corpse;
        }

        t.manager.playerStats.__sanity += 15;

        t.manager.UpdateMessages($"You have killed the <color={t.EnemyColor}>{t.EnemyName}</color>");
        t.manager.playerStats.UpdateLevel(t.xpDrop);

        t.manager.StartPlayersTurn();

        t.manager.gameObject.GetComponent<Bestiary>().UpdateEnemyList(t.enemySO);

        t.manager.enemies.Remove(t.gameObject);

        DungeonGenerator.dungeonGenerator.DrawMap(MapManager.map);


        Destroy(t.gameObject);
    }
}
