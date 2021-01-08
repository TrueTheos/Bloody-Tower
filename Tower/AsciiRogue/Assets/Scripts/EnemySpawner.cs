using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemiesScriptableObject[] allEnemies;
    [Header("Preset monsters")]
    public EnemiesScriptableObject Mimic;
    public EnemiesScriptableObject Zombie;
    public EnemiesScriptableObject Hamp;

    [Header("Random Text Events")]
    public List<TriggerTextAI> textEvents = new List<TriggerTextAI>();

    public BaseOmniAI SecretDoorAI;

    [HideInInspector]public List<GameObject> spawnedEnemies = new List<GameObject>();
    public List<int> enemiesPerRoom;
    
    public GameObject enemyPrefab;

    public Vector2Int __position { get; set; }

    public GameManager manager;

    public void Spawn(Floor floor)
    {
        foreach(var room in DungeonGenerator.dungeonGenerator.rooms)
        {
            int howManyMobs = enemiesPerRoom[UnityEngine.Random.Range(0, enemiesPerRoom.Count)];

            if(howManyMobs != 0)
            {
                for (int i = 1; i < howManyMobs; i++)
                {
                    GetRandomEnemy(floor);
                }
            }           
        }  
    }

    public void SpawnAt(Floor floor,int x, int y, EnemiesScriptableObject enemySO = null, string sleep = "")
    {
        try{if(floor.Tiles[x,y].type != "Floor" || floor.Tiles[x,y].structure != null || floor.Tiles[x,y].hasPlayer) return;}
        catch{};

        __position = new Vector2Int(x, y);

        if(enemySO == null) enemySO = GetRandomEnemy(floor);
        try
        {
            floor.Tiles[__position.x, __position.y].timeColor = enemySO.E_color;
        }
        catch
        {
            return;
        }
        floor.Tiles[__position.x, __position.y].letter = enemySO.E_symbol;
        floor.Tiles[__position.x, __position.y].isWalkable = false;

        //DungeonGenerator.dungeonGenerator.DrawMap(true, floor.Tiles);

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
        else
        {
            so.SpawnSleep(MapManager.GetIndexOfFloor(floor));
        }

        so.__currentHp = enemySO.maxHealth;
        so.str = enemySO.strength;
        so.dex = enemySO.dexterity;
        so.intell = enemySO.intelligence;
        so.end = enemySO.endurance;

        //so.__currentHp = Mathf.FloorToInt(so.str + (so.end * 3) - 5);

        so.xpDrop = Mathf.RoundToInt((so.str + so.dex + so.intell + so.end) / 3);

        manager.enemies.Add(enemy.gameObject);
        enemy.transform.SetParent(floor.GO.transform);
        floor.Tiles[__position.x, __position.y].enemy = enemy.gameObject;

        spawnedEnemies.Add(enemy.gameObject);
    }

    private EnemiesScriptableObject GetRandomEnemy(Floor floor)
    {
        foreach (var enemy in allEnemies)
        {
            int randomEnemy = RNG.Range(0, allEnemies.Length - 1);
            if (Enumerable.Range(allEnemies[randomEnemy].E_lvlMin, allEnemies[randomEnemy].E_lvlMax - allEnemies[randomEnemy].E_lvlMin).Contains(MapManager.GetIndexOfFloor(floor)))
            {
                Debug.Log($"<color=red>{allEnemies[randomEnemy].E_name} {MapManager.GetIndexOfFloor(floor)}</color>");
                return allEnemies[randomEnemy];
            }
        }

        return null;
    }

    internal void SpawnSpecial(Floor floor,int x, int y, string type)
    {
        switch (type)
        {
            case "h":
                // hidden Door:
                GameObject go = new GameObject("Hidden door",typeof(OmniBehaviour));
                var b = go.GetComponent<OmniBehaviour>();
                go.transform.SetParent(floor.GO.transform);
                b.AI = SecretDoorAI;
                b.Position = new Vector2Int(x, y);
                
                manager.enemies.Add(go);
                spawnedEnemies.Add(go);
                break;
            default:
                break;
        }
    }

    public void SpawnTextEvent(Floor floor,int x, int y, TriggerTextAI eve)
    {
        GameObject go = new GameObject("Random Text Event" , typeof(OmniBehaviour));
        var b = go.GetComponent<OmniBehaviour>();
        go.transform.SetParent(floor.GO.transform);
        b.AI = eve; // what AI you want to use has to be the SO
        b.Position = new Vector2Int(x,y); // the position of the SO if relevant for what it should do
        GameManager.manager.enemies.Add(go);
    }

    /*private void Spawn(int x, int y)
    {
        __position = new Vector2Int(x, y);

        MapManager.map[__position.x, __position.y].letter = enemiesToSpawn[enemiesToSpawn.Length - 1].E_symbol;
        MapManager.map[__position.x, __position.y].isWalkable = false;
        MapManager.map[__position.x, __position.y].exploredColor = enemiesToSpawn[enemiesToSpawn.Length - 1].E_color;

        DungeonGenerator.dungeonGenerator.DrawMap(MapManager.map);

        GameObject enemy = Instantiate(enemyPrefab.gameObject, transform.position, Quaternion.identity);
        enemy.GetComponent<RoamingNPC>().__position = __position;
        enemy.GetComponent<RoamingNPC>().enemySO = enemiesToSpawn[enemiesToSpawn.Length - 1];
        enemy.transform.SetParent(FloorManager.floorManager.floorsGO[DungeonGenerator.dungeonGenerator.currentFloor].transform);
        MapManager.map[__position.x, __position.y].enemy = enemy.gameObject;

        spawnedEnemies.Add(enemy.gameObject);

        Array.Resize(ref enemiesToSpawn, enemiesToSpawn.Length - 1);
    }*/
}
