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

        /*so.lvl = DungeonGenerator.dungeonGenerator.currentFloor;
        so.str = 10 + Mathf.FloorToInt(so.lvl * 0.5f);
        so.dex = 10 + Mathf.FloorToInt(so.lvl * 0.5f);
        so.intell = 10 + Mathf.FloorToInt(so.lvl * 0.3f);
        so.end = 10 + Mathf.FloorToInt(so.lvl * 0.5f); */

        /*if (so.enemySO.__modifier.HasFlag(EnemiesScriptableObject.E_modifier.strong))
        {
            so.str += Mathf.FloorToInt(so.str * .75f);
        }
        if (so.enemySO.__modifier.HasFlag(EnemiesScriptableObject.E_modifier.nimble))
        {
            so.dex += Mathf.FloorToInt(so.dex * .75f);
        }
        if (so.enemySO.__modifier.HasFlag(EnemiesScriptableObject.E_modifier.smart))
        {
            so.intell += Mathf.FloorToInt(so.intell * .75f);
        }
        if (so.enemySO.__modifier.HasFlag(EnemiesScriptableObject.E_modifier.tough))
        {
            so.end += Mathf.FloorToInt(so.end * .75f);
        }
        if (so.enemySO.__modifier.HasFlag(EnemiesScriptableObject.E_modifier.weak))
        {
            so.str -= Mathf.FloorToInt(so.str * .5f);
        }
        if (so.enemySO.__modifier.HasFlag(EnemiesScriptableObject.E_modifier.clumsy))
        {
            so.dex -= Mathf.FloorToInt(so.dex * .5f);
        }
        if (so.enemySO.__modifier.HasFlag(EnemiesScriptableObject.E_modifier.stupid))
        {
            so.intell -= Mathf.FloorToInt(so.intell * .5f);
        }
        if (so.enemySO.__modifier.HasFlag(EnemiesScriptableObject.E_modifier.frail))
        {
            so.end -= Mathf.FloorToInt(so.end * .5f);
        }
        if (so.enemySO.__modifier.HasFlag(EnemiesScriptableObject.E_modifier.giant))
        {
            so.str += Mathf.FloorToInt(so.str * .4f);
            so.dex += Mathf.FloorToInt(so.dex * .4f);
            so.intell += Mathf.FloorToInt(so.intell * .4f);
            so.end += Mathf.FloorToInt(so.end * .4f);
        }
        if (so.enemySO.__modifier.HasFlag(EnemiesScriptableObject.E_modifier.small))
        {
            so.str -= Mathf.FloorToInt(so.str * .2f);
            so.dex -= Mathf.FloorToInt(so.dex * .2f);
            so.intell -= Mathf.FloorToInt(so.intell * .2f);
            so.end -= Mathf.FloorToInt(so.end * .2f);
        }
        if (so.enemySO.__modifier.HasFlag(EnemiesScriptableObject.E_modifier.caster))
        {
            so.str -= Mathf.FloorToInt(so.str * .1f);
            so.dex -= Mathf.FloorToInt(so.dex * .1f);
            so.intell += Mathf.FloorToInt(so.intell * .5f);
            so.end -= Mathf.FloorToInt(so.end * .1f);
        }



        switch (so.enemySO._Difficulty)
        {
            case EnemiesScriptableObject.E_difficulty.easy:
                so.str -= Mathf.FloorToInt(so.str * .2f);
                so.dex -= Mathf.FloorToInt(so.dex * .2f);
                so.intell -= Mathf.FloorToInt(so.intell * .2f);
                so.end -= Mathf.FloorToInt(so.end * .2f);
                break;
            case EnemiesScriptableObject.E_difficulty.normal:
                break;
            case EnemiesScriptableObject.E_difficulty.hard:
                so.str += Mathf.FloorToInt(so.str * .2f);
                so.dex += Mathf.FloorToInt(so.dex * .2f);
                so.intell += Mathf.FloorToInt(so.intell * .2f);
                so.end += Mathf.FloorToInt(so.end * .2f);
                break;
            case EnemiesScriptableObject.E_difficulty.boss:
                so.str += Mathf.FloorToInt(so.str * .4f);
                so.dex += Mathf.FloorToInt(so.dex * .4f);
                so.intell += Mathf.FloorToInt(so.intell * .4f);
                so.end += Mathf.FloorToInt(so.end * .4f);
                break;
        }*/

        so.str = enemySO.strength;
        so.dex = enemySO.dexterity;
        so.intell = enemySO.intelligence;
        so.end = enemySO.endurance;

        so.__currentHp = Mathf.FloorToInt(so.str + (so.end * 2) - 5);

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
