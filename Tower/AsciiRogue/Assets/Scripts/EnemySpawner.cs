using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemiesScriptableObject[] allEnemies;
    public EnemiesScriptableObject Mimic;
    public EnemiesScriptableObject Zombie;
    [HideInInspector]public List<GameObject> spawnedEnemies = new List<GameObject>();
    public List<int> enemiesPerRoom;
    
    public GameObject enemyPrefab;

    public Vector2Int __position { get; set; }

    public GameManager manager;

    private bool loopBreaker;

    public void Spawn()
    {
        foreach(var room in DungeonGenerator.dungeonGenerator.rooms)
        {
            int howManyMobs = enemiesPerRoom[UnityEngine.Random.Range(0, enemiesPerRoom.Count)];

            if(howManyMobs != 0)
            {
                for (int i = 1; i < howManyMobs; i++)
                {
                    GetRandomEnemy();
                }
            }           
        }  
    }

    public void SpawnAt(int x, int y, EnemiesScriptableObject enemySO = null, string sleep = "")
    {        
        try{if(MapManager.map[x,y].type != "Floor" || MapManager.map[x,y].structure != null) return;}
        catch{};

        __position = new Vector2Int(x, y);

        if(enemySO == null) enemySO = GetRandomEnemy();

        try
        {
            MapManager.map[__position.x, __position.y].timeColor = enemySO.E_color;
        }
        catch
        {
            return;
        }
        MapManager.map[__position.x, __position.y].letter = enemySO.E_symbol;
        MapManager.map[__position.x, __position.y].isWalkable = false;

        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);

        GameObject enemy;

        try
        {
            enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
        catch
        {
            enemy = Instantiate(GameManager.manager.enemyPrefab, transform.position, Quaternion.identity);
        }

        RoamingNPC so = enemy.GetComponent<RoamingNPC>();

        so.__position = __position;

        so.enemySO = enemySO;

        if(sleep != "")
        {
            so.sleepDecided = true;
            if(sleep == "true")
            {
                so.sleeping = true;
            }
            else if(sleep == "false")
            {
                so.sleeping = false;
            }
        }

        so.__currentHp = enemySO.maxHealth;
        so.str = enemySO.strength;
        so.dex = enemySO.dexterity;
        so.intell = enemySO.intelligence;
        so.end = enemySO.endurance;

        //so.__currentHp = Mathf.FloorToInt(so.str + (so.end * 3) - 5);

        so.xpDrop = Mathf.RoundToInt((so.str + so.dex + so.intell + so.end) / 3);

        manager.enemies.Add(enemy.gameObject);
        enemy.transform.SetParent(FloorManager.floorManager.floorsGO[DungeonGenerator.dungeonGenerator.currentFloor].transform);
        MapManager.map[__position.x, __position.y].enemy = enemy.gameObject;

        spawnedEnemies.Add(enemy.gameObject);
    }

    private EnemiesScriptableObject GetRandomEnemy()
    {
        loopBreaker = false;

        foreach (var enemy in allEnemies)
        {
            int randomEnemy = UnityEngine.Random.Range(0, allEnemies.Length - 1);
            if (Enumerable.Range(allEnemies[randomEnemy].E_lvlMin, allEnemies[randomEnemy].E_lvlMax - allEnemies[randomEnemy].E_lvlMin).Contains(DungeonGenerator.dungeonGenerator.currentFloor))
            {
                Debug.Log($"<color=red>{allEnemies[randomEnemy].E_name} {DungeonGenerator.dungeonGenerator.currentFloor}</color>");
                return allEnemies[randomEnemy];
            }
        }

        return null;
    }

    /*private void Spawn(int x, int y)
    {
        __position = new Vector2Int(x, y);

        MapManager.map[__position.x, __position.y].letter = enemiesToSpawn[enemiesToSpawn.Length - 1].E_symbol;
        MapManager.map[__position.x, __position.y].isWalkable = false;
        MapManager.map[__position.x, __position.y].exploredColor = enemiesToSpawn[enemiesToSpawn.Length - 1].E_color;

        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);

        GameObject enemy = Instantiate(enemyPrefab.gameObject, transform.position, Quaternion.identity);
        enemy.GetComponent<RoamingNPC>().__position = __position;
        enemy.GetComponent<RoamingNPC>().enemySO = enemiesToSpawn[enemiesToSpawn.Length - 1];
        enemy.transform.SetParent(FloorManager.floorManager.floorsGO[DungeonGenerator.dungeonGenerator.currentFloor].transform);
        MapManager.map[__position.x, __position.y].enemy = enemy.gameObject;

        spawnedEnemies.Add(enemy.gameObject);

        Array.Resize(ref enemiesToSpawn, enemiesToSpawn.Length - 1);
    }*/
}
