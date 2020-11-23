using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections;
using System.IO;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using System.Threading;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Fixed Levels")]
    public List<FixedLevels> floor10BossRooms;
    public FixedLevels necromancerLevel;

    public string[] enemiesList;
    public string[] splitted;
    public string enemies;

    public string str;

    public List<string> enemyNames = new List<string>();
    public List<Vector2Int> enemyPositions = new List<Vector2Int>();
    public List<(Vector2Int pos, string type)> specialEnemy = new List<(Vector2Int pos, string type)>();
    public List<bool> enemySleeping = new List<bool>();

    private List<ItemScriptableObject> Prefab_itemsToSpawn = new List<ItemScriptableObject>();
    private List<Vector2Int> Prefab_itemsPositions = new List<Vector2Int>();

    public List<EnemiesScriptableObject> Prefab_enemyNames = new List<EnemiesScriptableObject>();
    public List<Vector2Int> Prefab_enemyPositions = new List<Vector2Int>();
    public List<bool> Prefab_enemySleeping = new List<bool>();

    public List<Vector2Int> mimicPositions = new List<Vector2Int>();

    private List<Vector2Int> torchPositions = new List<Vector2Int>();

    public static DungeonGenerator dungeonGenerator;
    [HideInInspector] public FloorManager floorManager;
    private GameManager manager;

    public int mapWidth;
    public int mapHeight;

    public int widthMinRoom;
    public int widthMaxRoom;
    public int heightMinRoom;
    public int heightMaxRoom;

    public int minCorridorLength;
    public int maxCorridorLength;
    public int maxFeatures;

    public int currentFloor = 0;

    public bool isASCII;

    [SerializeField] public List<Feature> allFeatures;
    [SerializeField] public List<Feature> rooms;

    public GameObject playerPrefab;

    private int dX; //door position x
    private int dY; //door position y

    private bool playerExists;

    public GameObject floorObject = null;

    public Text screen;
    public float lightFactor = 1f;


    [Header("Water Colors")]
    public List<Color> waterColors;
    [Header("Mushroom Colors")]
    public List<Color> mushroomColors;
    [Header("Mould Wall Colors")]
    public List<Color> mouldWallColors;
    [Header("Flesh Wall Colors")]
    public List<Color> fleshWallColors;
    [Header("Statue Names")]
    public List<string> statueNames = new List<string>() { "Ceramic Statue", "Obsidian Statue", "Wooden Statue", "Iron Statue", "Copper Statue", "Gold Statue" };
    [Header("Statue Colors")]
    public List<Color> statueColors = new List<Color>();
    [Header("Painting Texts")]
    public List<string> paintingTexts = new List<string>();

    public void Start()
    {
        dungeonGenerator = this;
        floorManager = GetComponent<FloorManager>();
        manager = GetComponent<GameManager>();
        prefabRooms = new List<PrefabRoom>(_prefabRooms);
    }

    public void InitializeDungeon()
    {
        MapManager.map = new Tile[mapWidth, mapHeight];
    }

    public void GenerateDungeon(int floorNumber)
    {
        try { PlayerMovement.playerMovement.StopCoroutine(PlayerMovement.playerMovement.cor); }
        catch { }

        if(floorNumber == 10 && floorManager.floorsGO.Where(obj => obj.name == $"Floor {floorNumber}").FirstOrDefault() == null)
        {            
            GenerateFixedLevel(floor10BossRooms[UnityEngine.Random.Range(0, floor10BossRooms.Count)].fixedLevel, 10, true, false);            
        }
        else if(floorNumber == 5 && floorManager.floorsGO.Where(obj => obj.name == $"Floor {floorNumber}").FirstOrDefault() == null)
        {
            GenerateFixedLevel(MapGenerator.createSewerMap().print(), floorNumber, false, false, 2);
        }
        else if (floorNumber == 13 && floorManager.floorsGO.Where(obj => obj.name == $"Floor {floorNumber}").FirstOrDefault() == null)
        {
            GenerateFixedLevel(necromancerLevel.fixedLevel, 13, true, false);
        }
        else if(floorNumber>10 && floorNumber<20 && floorManager.floorsGO.Where(obj => obj.name == $"Floor {floorNumber}").FirstOrDefault() == null)
        {
            GenerateFixedLevel(CleanerTemple.GetSimpleTemple(), floorNumber, false, false);
        }
        else if (floorManager.floors.Count <= floorNumber)
        {
            GenerateFixedLevel(MapGenerator.createSewerMap().print(), floorNumber, false);
        }
        else
        {
            bool comingFromUp = floorNumber > currentFloor ? false : true;
            
            currentFloor = floorNumber;

            MapManager.map = null;
            MapManager.map = floorManager.floors[currentFloor];

            floorObject = floorManager.floorsGO.Where(obj => obj.name == $"Floor {currentFloor}").SingleOrDefault();

            foreach (GameObject floorGO in floorManager.floorsGO)
            {
                floorGO.SetActive(false);
            }
            floorObject.SetActive(true);

            if (comingFromUp)
            {
                MovePlayerToUpperStairs(); //comming from the lower floor
            }
            else
            {
                MovePlayerToLowerStairs(); //comming from the higher floor
            }

            if(floorObject.GetComponent<FloorInfo>().viewRange != 666)
            {
                manager.playerStats.viewRange = 0;
            }

            manager.mapName.text = "Floor " + currentFloor;
            manager.UpdateMessages($"You entered Floor {currentFloor}");

            DrawMap(true, floorManager.floors[currentFloor]);
        }
    }

    //GENERATES LEVEL FROM STRING

    //# WALL
    //. FLOOR
    //< > STAIRS
    //+ DOOR
    //1 DOOR THAT REQUIRES BLOOD TO BE PAID TO OPEN THE DOOR
    //_ DOOR THAT REQUIRES KEY
    //0 ITEMS
    //- KEY
    //= CHEST
    //" MUSHROOM
    //& COBWEB
    //~ - Blood
    //2 - Pillar
    //3 - Blood Torch
    //5 - Painting
    //7 - Prefab Monster
    //9 - Prefab room item
    //{ - Fountain
    //} - Statue
    //! - Torch

    public void GenerateFixedLevel(string fixedLevel, int floor, bool spawnEnemiesFromString, bool generateWater = true, int _viewRange = 666)
    {
        torchPositions = new List<Vector2Int>();

        mimicPositions = new List<Vector2Int>();

        List<Vector2Int> itemPositions = new List<Vector2Int>();
        List<Vector2Int> cobwebPositions = new List<Vector2Int>();

        currentFloor = floor;

        if (!GameObject.Find($"Floor {currentFloor}"))
        {
            floorObject = new GameObject($"Floor {currentFloor}");
            floorObject.AddComponent<FloorInfo>();
        }

        if (_viewRange != 666)
        {
            floorObject.GetComponent<FloorInfo>().viewRange = _viewRange;
        }

        MapManager.map = new Tile[mapWidth, mapHeight];

        foreach (GameObject floorGO in floorManager.floorsGO)
        {
            floorGO.SetActive(false);
        }
        floorObject.SetActive(true);

        rooms.Clear();
        allFeatures.Clear();
        dX = 0;
        dY = 0;

        if(spawnEnemiesFromString)
        {
            string[] enemiesArray = fixedLevel.Split(new char[] { '|' });
            enemies = enemiesArray[1];

            enemiesList = enemies.Split(new string[] {";"}, StringSplitOptions.None);
            str = String.Join("/", enemiesList);
            splitted = str.Split(new string[] {"/"}, StringSplitOptions.None);

            List<string> splitted2 = new List<string>();
            splitted2 = splitted.ToList();

            for(int i = 0; i < enemiesList.Length - 1; i++)
            {
                enemyNames.Add(splitted2[0]);
                splitted2.RemoveAt(0);
                enemyPositions.Add(new Vector2Int(int.Parse(splitted2[0]), int.Parse(splitted2[1])));
                splitted2.RemoveAt(0);
                splitted2.RemoveAt(0);              
                enemySleeping.Add(splitted2[0] == "true");
                splitted2.RemoveAt(0);
            }    
        }

        int inxedString = -1;

        bool forBreaker = false;

        for(int y = mapHeight - 1; y >= 0; y--)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                inxedString++;
                try
                {
                    if(fixedLevel[inxedString] == "test"[0]) break;
                }
                catch
                {
                    forBreaker = true;
                }

                if(forBreaker) break;

                switch(fixedLevel[inxedString].ToString())
                {
                    case "#": //WALL
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = "#",
                                isWalkable = false,
                                isOpaque = true,
                                type = "Wall"
                            };
                            if (currentFloor >= 11)
                            {
                                MapManager.map[x, y].exploredColor = fleshWallColors[UnityEngine.Random.Range(0, fleshWallColors.Count)];
                                MapManager.map[x, y].specialNameOfTheCell = "Flesh Wall";
                            }
                            else if (currentFloor < 11 && UnityEngine.Random.Range(0, 100) > currentFloor * 1.5f)
                            {
                                MapManager.map[x, y].exploredColor = mouldWallColors[UnityEngine.Random.Range(0, mouldWallColors.Count)];
                                MapManager.map[x, y].specialNameOfTheCell = "Mould Wall";
                            }
                        }
                        break;
                    case ".": //FLOOR
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = ".",
                                isWalkable = true,
                                isOpaque = false,
                                type = "Floor"
                            };
                        }
                        break;
                    case "<": //STAIRS UP
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                structure = new Stairs(),
                                specialNameOfTheCell = "Stairs"
                            };
                            if (MapManager.map[x, y].structure is Stairs stairsDown)
                            {
                                stairsDown.dungeonLevelId = currentFloor + 1;
                                stairsDown.spawnPosition = new Vector2Int(x, y);
                                MapManager.map[x, y].baseChar = "<";
                                MapManager.map[x, y].isOpaque = false;
                                MapManager.map[x, y].isWalkable = true;
                                MapManager.lowerStairsPos = new Vector2Int(x, y);
                                floorManager.stairsUp.Add(new Vector2Int(x, y));
                            }
                        }
                        break;
                    case ">": //STAIRS DOWN\
                        {
                            if (currentFloor != 0)
                            {
                                MapManager.map[x, y] = new Tile
                                {
                                    structure = new Stairs(),
                                    specialNameOfTheCell = "Stairs"
                                };
                                if (MapManager.map[x, y].structure is Stairs stairsUp)
                                {
                                    stairsUp.dungeonLevelId = currentFloor - 1;
                                    stairsUp.spawnPosition = new Vector2Int(x, y);
                                    MapManager.map[x, y].baseChar = ">";
                                    MapManager.map[x, y].isOpaque = false;
                                    MapManager.map[x, y].isWalkable = true;
                                    MapManager.upperStairsPos = new Vector2Int(x, y);
                                    floorManager.stairsDown.Add(new Vector2Int(x, y));
                                }
                            }
                            else
                            {
                                MapManager.map[x, y] = new Tile
                                {
                                    xPosition = x,
                                    yPosition = y,
                                    baseChar = ".",
                                    isWalkable = true,
                                    isOpaque = false,
                                    type = "Floor"
                                };
                            }
                        }
                        break;
                    case "+": //DOOR
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                type = "Door",
                                baseChar = "+",
                                exploredColor = new Color(.545f, .27f, .07f),
                                isWalkable = false,
                                isOpaque = true
                            };
                            Door door = new Door();
                            door.position = new Vector2Int(x, y);
                            MapManager.map[x, y].structure = door;
                        }
                        break;
                    case "1": //BLOOD DOOR
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                type = "Door",
                                baseChar = "+",
                                exploredColor = new Color(1, 0, 0),
                                isWalkable = false,
                                isOpaque = true
                            };

                            BloodDoor bloodDoor = new BloodDoor
                            {
                                position = new Vector2Int(x, y)
                            };

                            int multiplier = Mathf.RoundToInt(currentFloor / 5);
                            if (multiplier < 1) multiplier = 1;
                            bloodDoor.bloodCost = 5 * multiplier;
                            MapManager.map[x, y].structure = bloodDoor;
                        }
                        break;
                    case "_": //DOOR REQUIRING KEY
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                type = "Door",
                                baseChar = "+",
                                exploredColor = new Color(.68f, .68f, .68f),
                                requiresKey = true,
                                isWalkable = false,
                                isOpaque = true
                            };
                        }
                        break;
                    case "h": //FLESH OR MOULD WALL
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = "#",
                                isWalkable = false,
                                isOpaque = true,
                                type = "Wall"
                            };
                            if (currentFloor >= 11)
                            {
                                MapManager.map[x, y].exploredColor = fleshWallColors[UnityEngine.Random.Range(0, fleshWallColors.Count)];
                                MapManager.map[x, y].specialNameOfTheCell = "Flesh Wall";
                            }
                            else if (currentFloor < 11 && UnityEngine.Random.Range(0, 100) > currentFloor * 1.5f)
                            {
                                MapManager.map[x, y].exploredColor = mouldWallColors[UnityEngine.Random.Range(0, mouldWallColors.Count)];
                                MapManager.map[x, y].specialNameOfTheCell = "Mould Wall";
                            }
                            // now add something to spawning
                            specialEnemy.Add((new Vector2Int(x, y), "h"));
                        }
                        break;
                    case "0": //ITEM
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = ".",
                                isWalkable = true,
                                isOpaque = false,
                                type = "Floor"
                            };

                            itemPositions.Add(new Vector2Int(x, y));
                        }
                        break;
                    case "=": //CHEST OR MIMIC
                        {
                            if (UnityEngine.Random.Range(0, 100) < 10)
                            {
                                MapManager.map[x, y] = new Tile
                                {
                                    xPosition = x,
                                    yPosition = y,
                                    baseChar = ".",
                                    isWalkable = true,
                                    isOpaque = false,
                                    type = "Floor",
                                    structure = null
                                };
                                mimicPositions.Add(new Vector2Int(x, y));
                            }
                            else if (UnityEngine.Random.Range(1, 100) < 7)
                            {
                                MapManager.map[x, y] = new Tile
                                {
                                    type = "Blood Anvil",
                                    baseChar = "π",
                                    exploredColor = new Color(0.7075472f, 0.123487f, 0.123487f),
                                    isWalkable = false,
                                    isOpaque = false
                                };

                                Anvil anvil = new Anvil();
                                MapManager.map[x, y].structure = anvil;
                            }
                            else
                            {
                                MapManager.map[x, y] = new Tile();

                                CreateChest(x, y);
                            }
                        }
                        break;
                    case "\"": //MUSHROOM
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = "\"",
                                isWalkable = true,
                                isOpaque = false,
                                type = "Floor",
                                specialNameOfTheCell = "Mushroom",
                                exploredColor = mushroomColors[UnityEngine.Random.Range(0, mushroomColors.Count)]
                            };
                            Mushroom mushroom = new Mushroom();
                            mushroom.pos = new Vector2Int(x, y);
                            MapManager.map[x, y].structure = mushroom;
                        }
                        break;
                    case "&": //COBWEB
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = "&",
                                isWalkable = true,
                                isOpaque = false,
                                type = "Floor"
                            };
                            cobwebPositions.Add(new Vector2Int(x, y));
                        }
                        break;
                    case "2": //PILLAR
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = "\u01C1",
                                isWalkable = false,
                                isOpaque = true,
                                type = "Pillar"
                            };
                        }
                        break;
                    case "7": //PREFAB ENEMY
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = ".",
                                isWalkable = true,
                                isOpaque = false,
                                type = "Floor"
                            };
                            Prefab_enemyPositions.Add(new Vector2Int(x, y));
                        }
                        break;
                    case "9": //PREFAB ITEM
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = ".",
                                isWalkable = true,
                                isOpaque = false,
                                type = "Floor"
                            };
                            Prefab_itemsPositions.Add(new Vector2Int(x, y));
                        }
                        break;
                    case "g": //ENEMY
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = ".",
                                isWalkable = true,
                                isOpaque = false,
                                type = "Floor"
                            };
                            if (!spawnEnemiesFromString)
                            {
                                enemyPositions.Add(new Vector2Int(x, y));
                            }
                        }
                        break;
                    case "~": //BLOOD
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                isWalkable = true,
                                isOpaque = false,
                                baseChar = "~",
                                exploredColor = waterColors[UnityEngine.Random.Range(0, waterColors.Count)],
                                type = "Water",
                                specialNameOfTheCell = "Blood"
                            };
                        }
                        break;
                    case "3": //BLOOD TORCH
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = "\u0416",
                                isWalkable = false,
                                isOpaque = true,
                                exploredColor = new Color(0.7075472f, 0.123487f, 0.123487f),
                                type = "Blood Torch"
                            };
                            BloodTorch torch = new BloodTorch();
                            torch.position = new Vector2Int(x, y);
                            MapManager.map[x, y].structure = torch;
                        }
                        break;
                    case "{": //FOUNTAIN
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                type = "Fountain",
                                baseChar = "{",
                                exploredColor = new Color(.418f, 0f, 1f),
                                isWalkable = false,
                                isOpaque = true
                            };
                            Fountain fountain = new Fountain
                            {
                                position = new Vector2Int(x, y)
                            };
                            MapManager.map[x, y].structure = fountain;
                        }
                        break;
                    case "}": //STATUE
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                type = "Floor",
                                baseChar = ".",
                                letter = "}",
                                isWalkable = false,
                                isOpaque = true,                 
                            };
                            Statue statue = new Statue
                            {
                                position = new Vector2Int(x, y)        
                            };
  
                            statue.statueName = statueNames[UnityEngine.Random.Range(0, statueNames.Count)];
                            statue.statueColor = statueColors[UnityEngine.Random.Range(0, statueColors.Count)];
                            MapManager.map[x, y].specialNameOfTheCell = statue.statueName;
                            MapManager.map[x, y].timeColor = statue.statueColor;
                            MapManager.map[x, y].structure = statue;
                        }
                        break;
                    case "5": //PAINTING
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = "#",
                                isWalkable = false,
                                isOpaque = true,
                                type = "Wall"
                            };
                            Painting painting = new Painting
                            {
                                paintingText = paintingTexts[UnityEngine.Random.Range(0, paintingTexts.Count)]
                            };
                            MapManager.map[x, y].specialNameOfTheCell = "Painting";
                            MapManager.map[x, y].structure = painting;
                            MapManager.map[x, y].exploredColor = new Color(1, 0.85f, 0);
                            break;
                        }
                    case "!":
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = "!",
                                isWalkable = false,
                                isOpaque = true,
                                type = "Torch"
                            };
                            MapManager.map[x, y].specialNameOfTheCell = "Torch";
                            MapManager.map[x, y].exploredColor = new Color(1, 1, 0);                          
                            torchPositions.Add(new Vector2Int(x, y));
                            break;
                        }
                    default:
                        {
                            MapManager.map[x, y] = new Tile
                            {
                                xPosition = x,
                                yPosition = y,
                                baseChar = ".",
                                isWalkable = true,
                                isOpaque = false,
                                type = "Floor"
                            };
                        }
                        break;

                        /*case "-": //KEY TO CELL
    {
        MapManager.map[x, y] = new Tile
        {
            baseChar = keyToCell.I_symbol
        };
        if (ColorUtility.TryParseHtmlString(keyToCell.I_color, out Color color))
        {
            MapManager.map[x, y].exploredColor = color;
        }

        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);

        GameObject item = Instantiate(manager.itemSpawner.itemPrefab.gameObject, transform.position, Quaternion.identity);

        item.GetComponent<Item>().iso = keyToCell;

        MapManager.map[x, y].item = item.gameObject;
    }
    break;*/
                }
            }
        }

        if(generateWater)
        {
            if (UnityEngine.Random.Range(1, 100) <= 30)
            {
                GenerateWaterPool();
            }
        }

        foreach (var pos in cobwebPositions)
        {
            if(MapManager.map[pos.x,pos.y].type != "Water") MapManager.map[pos.x,pos.y].type = "Cobweb";
        }
        
        floorManager.floors.Add(MapManager.map);
        floorManager.floorsGO.Add(floorObject);

        foreach(var mimic in mimicPositions)
        {     
            manager.enemySpawner.SpawnAt(mimic.x, mimic.y, manager.enemySpawner.Mimic, "true");
        }

        if(spawnEnemiesFromString)
        {
            for(int i = 0; i < enemyNames.Count; i++)
            {
                manager.enemySpawner.SpawnAt(enemyPositions[i].x, mapHeight - enemyPositions[i].y - 1, manager.enemySpawner.allEnemies.Where(obj => obj.name == enemyNames[i]).FirstOrDefault(), enemySleeping[i].ToString());
            }
        }
        else if(enemyPositions.Count > 0)
        {
            for (int i = 0; i < enemyPositions.Count; i++)
            {
                int y = mapHeight - 1 - enemyPositions[i].y;

                if(y == 0) y++;
                if(y == 21) y--;

                manager.enemySpawner.SpawnAt(enemyPositions[i].x, y);
            }
            for (int i = 0; i < specialEnemy.Count; i++)
            {
                (Vector2Int pos, string type) = specialEnemy[i];
                manager.enemySpawner.SpawnSpecial(pos.x, pos.y, type);

            }
        }

        int l = 0;
        foreach (var enemy in Prefab_enemyPositions)
        {
            manager.enemySpawner.SpawnAt(enemy.x, enemy.y, Prefab_enemyNames[l], Prefab_enemySleeping[l].ToString());
            l++;
        }

        foreach (var item in itemPositions)
        {
            manager.itemSpawner.SpawnAt(item.x, item.y);
        }

        int k = 0;
        foreach (var Prefab_item in Prefab_itemsPositions)
        {
            manager.itemSpawner.SpawnAt(Prefab_item.x, Prefab_item.y, Prefab_itemsToSpawn[k]);
            k++;
        }

        bool spawnedRandomTextEvent = false;
        if (manager.enemySpawner.textEvents.Count > 0)
        {
            int breakI = 0;
            while (!spawnedRandomTextEvent)
            {
                if (breakI > 200) break;
                int rX = UnityEngine.Random.Range(0, mapWidth);
                int rY = UnityEngine.Random.Range(0, mapHeight);

                try
                {
                    if (MapManager.map[rX, rY].type == "Floor" && MapManager.map[rX, rY].structure == null && MapManager.map[rX, rY].isWalkable)
                    {
                        int randomEventIndex = UnityEngine.Random.Range(0, manager.enemySpawner.textEvents.Count);
                        manager.enemySpawner.SpawnTextEvent(rX, rY, manager.enemySpawner.textEvents[randomEventIndex]);
                        manager.enemySpawner.textEvents.RemoveAt(randomEventIndex);
                    }
                }
                catch { }

                breakI++;
            }
        }

        //if (currentFloor != 0) MovePlayerToLowerStairs();

        manager.mapName.text = "Floor " + currentFloor;
        manager.UpdateMessages($"You entered Floor {currentFloor}");

        foreach(var torch in torchPositions)
        {
            manager.fv.ComputeTorch(new Vector2Int(torch.x, torch.y), 6);
        }

        enemyNames = new List<string>();
        enemyPositions = new List<Vector2Int>();
        enemySleeping = new List<bool>();

        Prefab_enemyNames = new List<EnemiesScriptableObject>();
        Prefab_enemyPositions = new List<Vector2Int>();
        Prefab_enemySleeping = new List<bool>();

        Prefab_itemsPositions = new List<Vector2Int>();
        Prefab_itemsToSpawn = new List<ItemScriptableObject>();
    }

    private void GenerateWaterPool()
    {
        List<Vector2Int> waterTilesToGrow = new List<Vector2Int>();

        Vector2Int startPos = new Vector2Int(100,100);

        Vector2Int vector = new Vector2Int(100,100);

        bool loopBr = false;
        for (int i = 0; i < 300; i++)
        {
            if(loopBr) break;

            int x = UnityEngine.Random.Range(1, mapWidth);
            int y = UnityEngine.Random.Range(1, mapHeight);

            try
            {
                if (MapManager.map[x, y].type == "Floor")
                {
                    startPos = new Vector2Int(x, y);
                    loopBr = true;
                }
            }
            catch { Debug.Log(x + " " + y); }
        }

        waterTilesToGrow.Add(startPos);

        for (int i = 0; i < 10; i++)
        {
            int l = waterTilesToGrow.Count;
            for (int j = 0; j < l; j++)
            {
                try
                {
                    if (UnityEngine.Random.Range(0, 100) > i * 8)
                    {
                        if (MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].type == "Wall")
                        {
                            waterTilesToGrow.RemoveAt(0);
                            /*if (UnityEngine.Random.Range(1, 100) < 30 && waterTilesToGrow[0].x > 1 && waterTilesToGrow[0].x < mapWidth && waterTilesToGrow[0].y > 1 && waterTilesToGrow[0].y < mapHeight - 1)
                            {
                                MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].isWalkable = true;
                                MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].isOpaque = false;
                                MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].baseChar = "~";
                                MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].exploredColor = waterColors[UnityEngine.Random.Range(0, waterColors.Count)];
                                MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].type = "Water";
                                MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].specialNameOfTheCell = "Blood";
                            }*/
                        }
                        else
                        {
                            MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].baseChar = "~";
                            MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].exploredColor = waterColors[UnityEngine.Random.Range(0, waterColors.Count)];
                            MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].type = "Water";
                            MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].specialNameOfTheCell = "Blood";
                            MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].isWalkable = true;
                            MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y].isOpaque = false;
                        }

                        if (MapManager.map[waterTilesToGrow[0].x + 1, waterTilesToGrow[0].y].type == "Floor" || MapManager.map[waterTilesToGrow[0].x + 1, waterTilesToGrow[0].y].type == "Wall" || MapManager.map[waterTilesToGrow[0].x + 1, waterTilesToGrow[0].y].type == "Door")
                        {
                            waterTilesToGrow.Add(new Vector2Int(waterTilesToGrow[0].x + 1, waterTilesToGrow[0].y));
                        }

                        if (MapManager.map[waterTilesToGrow[0].x - 1, waterTilesToGrow[0].y].type == "Floor" || MapManager.map[waterTilesToGrow[0].x - 1, waterTilesToGrow[0].y].type == "Wall" || MapManager.map[waterTilesToGrow[0].x - 1, waterTilesToGrow[0].y].type == "Door")
                        {
                            waterTilesToGrow.Add(new Vector2Int(waterTilesToGrow[0].x - 1, waterTilesToGrow[0].y));
                        }

                        if (MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y + 1].type == "Floor" || MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y + 1].type == "Wall" || MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y + 1].type == "Door")
                        {
                            waterTilesToGrow.Add(new Vector2Int(waterTilesToGrow[0].x, waterTilesToGrow[0].y + 1));
                        }

                        if (MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y - 1].type == "Floor" || MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y - 1].type == "Wall" || MapManager.map[waterTilesToGrow[0].x, waterTilesToGrow[0].y - 1].type == "Door")
                        {
                            waterTilesToGrow.Add(new Vector2Int(waterTilesToGrow[0].x, waterTilesToGrow[0].y - 1));
                        }
                        waterTilesToGrow.RemoveAt(0);
                    }
                    else
                    {
                        waterTilesToGrow.RemoveAt(0);                       
                    }                 
                }
                catch{waterTilesToGrow.RemoveAt(0);}
            }
        }
    }   

    public void MovePlayerToLowerStairs()
    {
        FloorManager.floorManager.floors[currentFloor - 1][MapManager.playerPos.x, MapManager.playerPos.y].hasPlayer = false;
        FloorManager.floorManager.floors[currentFloor - 1][MapManager.playerPos.x, MapManager.playerPos.y].letter = "";

        MapManager.map[floorManager.stairsDown[currentFloor - 1].x, floorManager.stairsDown[currentFloor - 1].y].hasPlayer = true;
        MapManager.map[floorManager.stairsDown[currentFloor - 1].x, floorManager.stairsDown[currentFloor - 1].y].letter = "<color=green>@</color>";

        MapManager.playerPos = new Vector2Int(floorManager.stairsDown[currentFloor - 1].x, floorManager.stairsDown[currentFloor - 1].y);
        manager.player.position = new Vector2Int(floorManager.stairsDown[currentFloor - 1].x, floorManager.stairsDown[currentFloor - 1].y);

        floorManager.floors[currentFloor] = MapManager.map;

        manager.playerStats.fullLevelVision = false;

        DrawMap(true, floorManager.floors[currentFloor]);
    }

    public void MovePlayerToUpperStairs()
    {
        FloorManager.floorManager.floors[currentFloor + 1][MapManager.playerPos.x, MapManager.playerPos.y].hasPlayer = false;
        FloorManager.floorManager.floors[currentFloor + 1][MapManager.playerPos.x, MapManager.playerPos.y].letter = "";

        MapManager.map[floorManager.stairsUp[currentFloor].x, floorManager.stairsUp[currentFloor].y].hasPlayer = true;
        MapManager.map[floorManager.stairsUp[currentFloor].x, floorManager.stairsUp[currentFloor].y].letter = "<color=green>@</color>";

        MapManager.playerPos = new Vector2Int(floorManager.stairsUp[currentFloor].x, floorManager.stairsUp[currentFloor].y);

        manager.player.position = new Vector2Int(floorManager.stairsUp[currentFloor].x, floorManager.stairsUp[currentFloor].y);

        floorManager.floors[currentFloor] = MapManager.map;

        manager.playerStats.fullLevelVision = false;

        DrawMap(true, floorManager.floors[currentFloor]);
    }
    
    private void SpawnChests()
    {
        var room = rooms[UnityEngine.Random.Range(0, rooms.Count)];

        List<Vector2Int> corners = GetCornersPositions(room);

        try
        {
            Vector2Int chestPos = corners[UnityEngine.Random.Range(0, corners.Count)];
            if (UnityEngine.Random.Range(0, 10) < 8) //TODO: Make this dependant on something
            {
                manager.enemySpawner.SpawnAt(chestPos.x, chestPos.y, manager.enemySpawner.Mimic, "true");
            }
            else
            {
                CreateChest(chestPos.x, chestPos.y);
            }
        }
        catch{ SpawnChests(); }
    }

    public void CreateChest(int x, int y)
    {
        MapManager.map[x, y].baseChar = "=";
        MapManager.map[x, y].exploredColor = new Color(.4f, .2f, 0);
        MapManager.map[x, y].isWalkable = false;
        MapManager.map[x, y].type = "Chest";

        Chest chest = new Chest();

        MapManager.map[x, y].structure = chest;

        int itemRarirty = UnityEngine.Random.Range(1, 100);

        string rarity;

        if (itemRarirty <= 2)
        {
            rarity = "very_rare";
        }
        else if (itemRarirty <= 7)
        {
            rarity = "rare";
        }
        else
        {
            rarity = "common";
        }

        bool loopBreaker = false;

        MapManager.map[x, y].structure = chest;

        int emptyChest = UnityEngine.Random.Range(0, 100); //if < 40 then chest is empty

        for (int i = 0; i < manager.itemSpawner.allItems.Count; i++)
        {
            if (loopBreaker) return;

            if (emptyChest <= 40) 
            {
                chest.itemInChest = null;
                loopBreaker = true;
            }
            else
            {
                int randomItem = UnityEngine.Random.Range(0, manager.itemSpawner.allItems.Count);

                if (manager.itemSpawner.allItems[randomItem].I_rareness.ToString() == rarity)
                {
                    chest.itemInChest = manager.itemSpawner.allItems[randomItem];

                    loopBreaker = true;
                }

            }
        }
    }

    private void CreatePillars()
    {
        foreach (var room in rooms) //Generate Fillars
        {
            if (UnityEngine.Random.Range(0, 50) < 10)
            {
                foreach (var corner in GetCornersPositions(room))
                {
                    MapManager.map[corner.x, corner.y].type = "Pillar";
                    MapManager.map[corner.x, corner.y].isWalkable = false;
                    MapManager.map[corner.x, corner.y].baseChar = "\u01C1";
                    MapManager.map[corner.x, corner.y].exploredColor = new Color(0.85f, 0.85f, 0.85f);
                }
            }
        }
    }

    public List<Vector2Int> GetCornersPositions(Feature room)
    {
        List<Vector2Int> cornerPositions = new List<Vector2Int>();

        List<Vector2Int> positions = new List<Vector2Int>();

        foreach (Vector2Int position in room.positions)
        {
            if (MapManager.map[position.x, position.y].type == "Floor" && !MapManager.map[position.x, position.y].hasPlayer && MapManager.map[position.x, position.y].structure == null)
            {
                positions.Add(position);
            }
        }

        foreach (var pos in positions)
        {
            try
            {
                if (MapManager.map[pos.x - 1, pos.y].type == "Wall" && MapManager.map[pos.x - 1, pos.y + 1].type == "Wall" && MapManager.map[pos.x, pos.y + 1].type == "Wall")
                {
                    cornerPositions.Add(pos);
                }
                else if (MapManager.map[pos.x - 1, pos.y].type == "Wall" && MapManager.map[pos.x - 1, pos.y - 1].type == "Wall" && MapManager.map[pos.x, pos.y - 1].type == "Wall")
                {
                    cornerPositions.Add(pos);
                }
                else if (MapManager.map[pos.x + 1, pos.y].type == "Wall" && MapManager.map[pos.x + 1, pos.y - 1].type == "Wall" && MapManager.map[pos.x, pos.y - 1].type == "Wall")
                {
                    cornerPositions.Add(pos);
                }
                else if (MapManager.map[pos.x + 1, pos.y].type == "Wall" && MapManager.map[pos.x + 1, pos.y + 1].type == "Wall" && MapManager.map[pos.x, pos.y + 1].type == "Wall")
                {
                    cornerPositions.Add(pos);
                }

            }
            catch
            {}          
        }

        return cornerPositions;
    }

    public void DrawMap(bool isASCII, Tile[,] map)
    {
        if (isASCII)
        {
            string asciiMap = "";

            for (int y = (mapHeight - 1); y >= 0; y--)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (map[x, y] != null)
                    {
                        if(map[x,y].decoy != "")
                        {
                            asciiMap += map[x, y].decoy;
                            map[x, y].decoy = "";
                        }
                        else if (map[x, y].letter == "")
                        {
                            //TIME COLOR;
                            if (map[x, y].isExplored)
                            {
                                if(map[x,y].timeColor != new Color(0,0,0))
                                    asciiMap += $"<color=#{CalculateFade(map[x,y].timeColor, x, y, MapManager.playerPos, lightFactor)}>{MapManager.map[x, y].baseChar}</color>";
                                else
                                    asciiMap += $"<color=#{CalculateFade(map[x, y].exploredColor, x, y, MapManager.playerPos, lightFactor)}>{MapManager.map[x, y].baseChar}</color>";
                            }
                            else
                            {
                                asciiMap += " ";
                            }
                        }
                        else if (map[x, y].isExplored)
                        {
                            if (map[x, y].timeColor != new Color(0, 0, 0))
                            {
                                asciiMap += $"<color=#{CalculateFade(map[x, y].timeColor, x, y, MapManager.playerPos, lightFactor)}>{MapManager.map[x, y].letter}</color>";
                            }
                            else
                            {
                                asciiMap += $"<color=#{CalculateFade(map[x, y].exploredColor, x, y, MapManager.playerPos, lightFactor)}>{MapManager.map[x, y].letter}</color>";                                
                            }
                        }
                        else
                        {
                            asciiMap += " ";
                        }
                    }
                    else
                    {
                        asciiMap += " ";
                        MapManager.map[x, y] = new Tile();
                    }
                    if (x == (mapWidth - 1))
                    {
                        asciiMap += "\n";
                    }
                }
            }

            screen.text = asciiMap;
        }
    }

    string CalculateFade(Color rgb_value, int x, int y, Vector2Int playerPos, float refadeFactor = 1)
    {
        int dst = (int)(Mathf.Abs(x - playerPos.x) + Mathf.Abs(y - playerPos.y)) + 1;
        float R = rgb_value.r;
        float G = rgb_value.g;
        float B = rgb_value.b;

        Color color;

        float tileLight = MapManager.map[x, y].tileLightFactor;
        if (tileLight != 0) Debug.Log(tileLight);
       
        if (MapManager.map[x, y].isVisible)
        {
            R = rgb_value.r / dst;
            G = rgb_value.g / dst;
            B = rgb_value.b / dst;

            if (MapManager.map[x, y].item != null)
            {
                R *= 3;
                G *= 3;
                B *= 3;
                color = new Color(R * (refadeFactor + tileLight), G * (refadeFactor + tileLight), B * (refadeFactor + tileLight));
                return ColorUtility.ToHtmlStringRGBA(color);
            }
            else if(MapManager.map[x, y].enemy != null)
            {
                R *= 3;
                G *= 3;
                B *= 3;
                color = new Color(R * (refadeFactor + tileLight), G * (refadeFactor + tileLight), B * (refadeFactor + tileLight));
                return ColorUtility.ToHtmlStringRGBA(color);
            }
            else
            {
                color = new Color(R * (refadeFactor + tileLight), G * (refadeFactor + tileLight), B * (refadeFactor + tileLight));
                return ColorUtility.ToHtmlStringRGBA(color);
            }
        }
        else
            return ColorUtility.ToHtmlStringRGBA(new Color(R / 9, G / 9, B / 9));
    }

    //-----------------------------------------------------------------------------------

    public List<PrefabRoom> _prefabRooms = new List<PrefabRoom>();
    private List<PrefabRoom> prefabRooms = new List<PrefabRoom>();

    class Map
    {
        public static int WIDTH = 59;
        public static int HEIGHT = 22;
        private char[,] cells = new char[WIDTH, HEIGHT]; // x,y

        public Map()
        {
            fillWithWalls();
        }

        public Map(Map m)
        {
            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                    cells[x, y] = m.get(x, y);
        }

        public void fillWithWalls()
        {
            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                    cells[x, y] = '#';
        }

        public void set(int x, int y, char c)
        {
            if (x < 1 || x > WIDTH - 2) // can't modify outer walls.
                return;
            if (y < 1 || y > HEIGHT - 2)
                return; 

            cells[x, y] = c;
        }

        public char get(int x, int y)
        {
            try
            {
                return cells[x, y];
            }
            catch{return '.';}
        }   

        public void carve(Structure s, char c)
        {
            for (int y = 0; y < s.height; y++)
                for (int x = 0; x < s.width; x++)
                    set(s.x + x, s.y + y, c);
        }

        public string print()
        {
            String s = "";
            for (int y = 0; y < HEIGHT; y++)
            {               
                for (int x = 0; x < WIDTH; x++)
                    s = s + get(x, y);
            }

            return s;
        }

        public int countAdjacentWalls(Location l)
        {
            int cnt = 0;
            for (int i = 0; i < 8; i++)
            {
                Location l2 = l.step((Direction)i);
                if (get(l2.x, l2.y) == '#')
                    cnt++;
            }

            return cnt;
        }

    }

    class Structure // These are rectangles with a marker that is either Room or Corridor. 
    {
        public int width;
        public int height;
        public int x;
        public int y;
        private Purpose purpose;
        public List<Vector2Int> doors = new List<Vector2Int>();
        public bool isPrefabRoom;
        public PrefabRoom prefabValue;

        public enum Purpose
        {
            Room,
            Corridor
        }

        public Structure(int x, int y, int w, int h, Purpose p)
        {
            this.x = x;
            this.y = y;
            this.width = w;
            this.height = h;
            this.purpose = p;
        }

        public Structure(Location a, Location b, Purpose p)
        {
            x = Math.Min(a.x, b.x);
            y = Math.Min(a.y, b.y);
            width = Math.Max(a.x, b.x) - x + 1;
            height = Math.Max(a.y, b.y) - y + 1;
            this.purpose = p;
        }

        public Structure getLineAlongTheBoundary(Direction d)
        {
            switch (d)
            {
                case Direction.NORTH: return new Structure(x, y - 2, width, 1, Structure.Purpose.Room);
                case Direction.SOUTH: return new Structure(x, y + height + 1, width, 1, Structure.Purpose.Room);
                case Direction.WEST: return new Structure(x - 2, y, 1, height, Structure.Purpose.Room);
                case Direction.EAST: return new Structure(x + width + 1, y, 1, height, Structure.Purpose.Room);
            }
            return null;
        }

        public bool isRoom()
        {
            return this.purpose == Purpose.Room;
        }

        public bool isCorridor()
        {
            return this.purpose == Purpose.Corridor;
        }

        public bool touches(Structure other) { // see if two structures overlap or are adjacent to each other
            if (this.x > other.x + other.width)
                return false;
            if (other.x > this.x + this.width)
                return false;
            if (this.y > other.y + other.height)
                return false;
            if (other.y > this.y + this.height)
                return false;
            return true;
        }


        public Location[] getRandom(int nb) // get x random Locations in this room (avoids overwriting loot and monsters)
        {
            if (nb > width * height)
                nb = width * height; // avoid infinite loops
            
            Location[] li = new Location[nb];
            int i = 0;
            Location l;
            bool found;
            while (i < nb)
            {
                found = false;
                l = getRandom();
                for (int j = 0; j < i; j++)
                    if (l.Equals(li[j]))
                        found = true;
                if (!found)
                    li[i++] = l;
            }
            return li;
        }

        public Location getRandom()
        {
            return new Location(Rand.range(x, x + width), Rand.range(y, y + height));
        }

        public Location[] getAllLocations()
        {
            Location[] list = new Location[width * height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    list[x * height + y] = new Location(this.x+x, this.y+y);
            return list;
        }

        public bool touches(Location n) // same as contains, but uses the outer wall around this structure as a boundary
        {
            return x - 1 <= n.x && n.x <= x + width && y - 1 <= n.y && n.y <= y + height;
        }

        public bool contains(Location n)
        {
            return x <= n.x && n.x < x + width && y <= n.y && n.y < y + height; 
        }

        // grow the room in a given direction (N S E W)
        public Structure grow(Direction dr)
        {
            switch (dr)
            {
                case Direction.NORTH: return new Structure(x, y - 1, width, height + 1, purpose);
                case Direction.SOUTH: return new Structure(x, y, width, height + 1, purpose);
                case Direction.WEST: return new Structure(x-1, y, width + 1, height, purpose);
                case Direction.EAST: return new Structure(x, y, width + 1, height, purpose);
            }
            return null;
        }

        public String toString()
        {
            return purpose.ToString() + " at " + x + "," + y + " w:" + width + " h:" + height;
        }

        public bool isInBounds()
        {
            return new Location(x, y).isInBounds() && new Location(x + width - 1, y + height - 1).isInBounds();
        }

        private bool isOrtogonallyAdjacent(Location l)
        {
            for (int i = 0; i < 4; i++)
            {
                Location l2 = l.step((Direction)(i * 2));
                if (this.contains(l2))
                    return true;
            }
            return false;
        }
        
        
        public List<Location> findCommonBorder(Structure p)
        {
            List<Location> li = new List<Location>();
            for (int cx = x; cx < x + width; cx++)
            {
                Location l = new Location(cx, y - 1);
                if (p.isOrtogonallyAdjacent(l))
                    li.Add(l);
                l = new Location(cx, y + height);
                if (p.isOrtogonallyAdjacent(l))
                    li.Add(l);
            }
            for (int cy = y; cy < y + height; cy++)
            {
                Location l = new Location(x-1, cy);
                if (p.isOrtogonallyAdjacent(l))
                    li.Add(l);
                l = new Location(x+width, cy);
                if (p.isOrtogonallyAdjacent(l))
                    li.Add(l);
            }

            return li;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            Location other = (Location)obj;
            return x == other.x && y == other.y;
        }

        public override int GetHashCode()
        {
            return 13 * x + 37 * y;
        }
    }

    class Rand
    {
        private static int Seed = new System.Random().Next(0, 200000); // use a seed so we can recreate the level if we need to debug it
        private static readonly System.Random _random = new System.Random(Seed);

        public static int range(int min, int max) 
        {
            return _random.Next(min, max); // 
        }

    }

    public enum Direction : int
    {
        NORTH,
        NORTHEAST,
        EAST,
        SOUTHEAST,
        SOUTH,
        SOUTHWEST,
        WEST,
        NORTHWEST
    }


    class Location
    {
        public int x;
        public int y;

        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Boolean isInBounds()
        {
            return x >= 1 && x < Map.WIDTH - 1 && y >= 1 && y < Map.HEIGHT - 1;
        }

        public Location step(Direction d)
        {
            switch (d)
            {
                case Direction.NORTH: return new Location(x, y - 1);
                case Direction.SOUTH: return new Location(x, y + 1);
                case Direction.EAST: return new Location(x + 1, y);
                case Direction.WEST: return new Location(x - 1, y);
                case Direction.NORTHWEST: return this.step(Direction.NORTH).step(Direction.WEST);
                case Direction.NORTHEAST: return this.step(Direction.NORTH).step(Direction.EAST);
                case Direction.SOUTHWEST: return this.step(Direction.SOUTH).step(Direction.WEST);
                case Direction.SOUTHEAST: return this.step(Direction.SOUTH).step(Direction.EAST);
            }
            return null;
        }

    }

    abstract class MapGenerator
    {
        public static Map createSewerMap()
        {
            try
            {
                Map m = new Map();

                // start with basic architecture

                // carve a central room, then attach stuff to it
                Structure centralRoom;

                int x = Map.WIDTH / 2 + Rand.range(-5, 6);
                int y = Map.HEIGHT / 2 + Rand.range(-5, 6);

                int w = 0;
                int h = 0;
                if (dungeonGenerator.prefabRooms.Count > 0)
                {
                    PrefabRoom prefab = dungeonGenerator.prefabRooms[UnityEngine.Random.Range(0, dungeonGenerator.prefabRooms.Count)];
                    w = prefab.height;
                    h = prefab.width;
                    centralRoom = new Structure(x, y, w, h, Structure.Purpose.Room);
                    centralRoom.prefabValue = prefab;
                    centralRoom.isPrefabRoom = true;
                    dungeonGenerator.prefabRooms.RemoveAt(dungeonGenerator.prefabRooms.IndexOf(prefab));
                }
                else
                {
                    w = Rand.range(3, 11);
                    h = Rand.range(3, 11);
                    centralRoom = new Structure(x, y, w, h, Structure.Purpose.Room);
                }

                m.carve(centralRoom, '.');

                List<Structure> structs = new List<Structure>
            {
                centralRoom
            };


                for (int i = 0; i < 10; i++)
                {
                    // pick a structure to attach to. 
                    Structure anchor = null;

                    // generate a new structure to attach to it
                    Structure newStruct = null;

                    // number of attempts to try
                    int attempts = 0;
                    int maxAttempts = 20;

                    while (newStruct == null && attempts < maxAttempts)
                    {
                        anchor = structs[Rand.range(0, structs.Count)];
                        newStruct = GenerateNewStructure(m, structs, anchor);
                        attempts++;
                    }
                    if (newStruct == null)
                        break; // we couldn't find one. Avoid endless loops

                    m.carve(newStruct, '.');
                    //Console.WriteLine("New {0}", newStruct.toString());


                    // add doors
                    List<Location> candidates = anchor.findCommonBorder(newStruct);
                    Location l = candidates[Rand.range(0, candidates.Count)];
                    if (anchor.isRoom() || newStruct.isRoom())
                    {
                        newStruct.doors.Add(new Vector2Int(l.x, l.y));
                        m.set(l.x, l.y, '+'); // at least one of them is a room. Connect them with a door
                    }
                    else
                    {
                        m.set(l.x, l.y, '.'); // both corridors. connect with open space
                    }

                    // now add more doors to other nearby structures
                    foreach (Structure other in structs)
                    {
                        if (other == anchor)
                            continue;
                        candidates = other.findCommonBorder(newStruct);
                        if (candidates.Count == 0)
                            continue;
                        if (Rand.range(0, 100) < 33)
                            continue;
                        l = candidates[Rand.range(0, candidates.Count)];
                        m.set(l.x, l.y, (other.isCorridor() && anchor.isCorridor()) ? '.' : '+');
                    }

                    structs.Add(newStruct);
                }

                FloorGenData gen = new FloorGenData();
                if (centralRoom.isPrefabRoom)
                {
                    gen.AddRoom(centralRoom.prefabValue);
                }

                int MaxPrefabRoom = 4;

                // furnish the dungeon
                foreach (Structure s in structs.EnumerateRandom())
                {
                    if (s.isRoom())
                    {
                        if (!s.isPrefabRoom && gen.PrefabRoomCount<MaxPrefabRoom)
                        {
                            // check if we can add one
                            if (PrefabProvider.Rooms.TryGetRoom(s.width,s.height,gen,out PrefabRoom prefab,(TagSetCheck)"Room" | "All"))
                            {                                
                                gen.AddRoom(prefab);
                                s.prefabValue = prefab;
                                s.isPrefabRoom = true;
                            }                            
                        }

                        furnishRoom(m, s);
                    }                        
                    else
                        furnishCorridor(m, s);

                }
                Debug.Log("Furnish rooms complete: " + gen.PrefabRoomCount);
                foreach (var placed in gen.PlacedRooms)
                {
                    Debug.Log(placed.name);
                }
                Debug.Log("------------");

                // spawn the stairs
                Structure r = SpawnStairsPlaceHolder('<', m, structs, null);
                SpawnStairsPlaceHolder('>', m, structs, r);
                return m;
            }
            catch
            {
                return createSewerMap();
            }         
        }

        private static Structure SpawnStairsPlaceHolder(char c, Map m, List<Structure> structs, Structure notHere)
        {
            int attempts = 0; // there might only be one room, avoid infinite loops
            while (true)
            {
                Structure r = getRandomRoomHelper(structs);
                if (r == notHere && attempts++ < 100)
                    continue;
                Location l = r.getRandom();
                if (m.get(l.x, l.y) == '.')
                {
                    m.set(l.x, l.y, c);
                    return r;
                }
            }
        }

        private static Structure getRandomRoomHelper(List<Structure> structs)
        {
            int offset = Rand.range(0, structs.Count);
            for (int i = 0; i < structs.Count; i++)
            {
                Structure s = structs[(i + offset) % structs.Count];
                if (s.isRoom())
                    return s;
            }
            return null;
        }

        public static void furnishRoom(Map m, Structure s)
        {
            if (!s.isPrefabRoom)
            {
                int size = s.width * s.height;

                Location[] location = s.getAllLocations();

                if (UnityEngine.Random.Range(0, 100) < 7)
                {
                    float randomN = UnityEngine.Random.Range(0, 1f);
                    if (randomN > .75f)
                    {
                        int randomTile = UnityEngine.Random.Range(0, s.width - 1);
                        if (m.get(location[randomTile].x, location[0].y - 1) != '+' && m.get(location[randomTile].x, location[0].y - 1) != '1')
                        {
                            m.set(location[randomTile].x, location[0].y - 1, '5');
                        }
                    }
                    else if(randomN > .5f)
                    {
                        int randomTile = UnityEngine.Random.Range(0, s.height - 1);
                        if (m.get(location[0].x - 1, location[randomTile].y) != '+' && m.get(location[0].x - 1, location[randomTile].y) != '1')
                        {
                            m.set(location[0].x - 1, location[randomTile].y, '5');
                        }
                    }
                    else if (randomN > .25f)
                    {
                        int randomTile = UnityEngine.Random.Range(0, s.height - 1);
                        if (m.get(location[s.width].x + 1, location[randomTile].y) != '+' && m.get(location[s.width].x + 1, location[randomTile].y) != '1')
                        {
                            m.set(location[s.width].x + 1, location[randomTile].y, '5');
                        }
                    }
                    else
                    {
                        int randomTile = UnityEngine.Random.Range(0, s.width - 1);
                        if (m.get(location[randomTile].x, location[s.height].y + 1) != '+' && m.get(location[randomTile].x, location[s.height].y + 1) != '1')
                        {
                            m.set(location[randomTile].x, location[s.height].y + 1, '5');
                        }
                    }
                }

                if (UnityEngine.Random.Range(0, 100) < 15)
                {
                    foreach (var door in s.doors)
                    {
                        m.set(door.x, door.y, '1');
                    }
                }

                //GENERATE MUSHROOMS
                if (UnityEngine.Random.Range(0, 100) < 50)
                {
                    foreach (var loc in location)
                    {
                        if (UnityEngine.Random.Range(1, 15) < 3 && m.get(loc.x, loc.y) != '<' && m.get(loc.x, loc.y) != '>')
                        {
                            m.set(loc.x, loc.y, '"');
                        }
                    }
                }
                //GENERATE COBWEB
                if (UnityEngine.Random.Range(0, 100) > 50 + dungeonGenerator.currentFloor)
                {
                    for (int i = 0; i < UnityEngine.Random.Range(location.Length * 0.25f, location.Length * 0.75f); i++)
                    {
                        int x = s.getRandom().x;
                        int y = s.getRandom().y;
                        if (m.get(x, y) != '<' && m.get(x, y) != '>')
                        {
                            m.set(x, y, '&');
                        }
                    }
                }

                //GENERATE STATUE
                if(UnityEngine.Random.Range(0, 100) < 50)
                {
                    int x = s.getRandom().x;
                    int y = s.getRandom().y;

                    if (m.get(x, y) != '<' && m.get(x, y) != '>')
                    {
                        m.set(x, y, '}');
                    }
                }

                int randomRoomDesign = UnityEngine.Random.Range(0, 20);

                if (randomRoomDesign <= 2)
                {
                    if (m.get(location[0].x, location[0].y) != '<' && m.get(location[0].x, location[0].y) != '>') m.set(location[0].x, location[0].y, '}');
                    if (m.get(location[size - 1].x, location[size - 1].y) != '<' && m.get(location[size - 1].x, location[size - 1].y) != '>') m.set(location[size - 1].x, location[size - 1].y, '}');
                    if (m.get(location[s.width - 1].x, location[0].y) != '<' && m.get(location[s.width - 1].x, location[0].y) != '>') m.set(location[s.width - 1].x, location[0].y, '}');
                    if (m.get(location[0].x, location[s.height - 1].y) != '<' && m.get(location[0].x, location[s.height - 1].y) != '>') m.set(location[0].x, location[s.height - 1].y, '}');
                }

                // loot
                String loot = "0";
                int monsterCount = 1;

                // minimum room size is currently 3x3 so this is at least 9
                Location[] l = s.getRandom(loot.Length + monsterCount);
                int j = 0;
                char[] lootArray = loot.ToCharArray();
                for (int i = 0; i < lootArray.Length; i++)
                    spawnLootPlaceHolder(m, l[j++], lootArray[i]);
                for (int i = 0; i < monsterCount; i++)
                    spawnMonsterPlaceHolder(m, l[j++]);
            }
            else
            {
                Location[] location = s.getAllLocations();
                int i = 0;
                int itemsSpawned = 0;
                int enemiesSpawned = 0;
                foreach (var loc in location)
                {
                    if (s.prefabValue.room[i] == '9')
                    {
                        m.set(loc.x, loc.y, '9');
                        dungeonGenerator.Prefab_itemsToSpawn.Add(s.prefabValue.itemsToSpawn[itemsSpawned]);
                        itemsSpawned++;
                    }
                    else if(s.prefabValue.room[i] == '7')
                    {
                        m.set(loc.x, loc.y, '7');
                        dungeonGenerator.Prefab_enemyNames.Add(s.prefabValue.enemyNames[enemiesSpawned]);
                        dungeonGenerator.Prefab_enemySleeping.Add(s.prefabValue.enemySleeping[enemiesSpawned]);
                        enemiesSpawned++;
                    }
                    else
                    {
                        m.set(loc.x, loc.y, s.prefabValue.room[i]);
                    }
                    i++;
                }

                foreach (var omni in s.prefabValue.OmniAIs)
                {
                    GameObject go = new GameObject(omni.Value.name, typeof(OmniBehaviour));
                    var b = go.GetComponent<OmniBehaviour>();
                    go.transform.SetParent(FloorManager.floorManager.floorsGO[DungeonGenerator.dungeonGenerator.currentFloor].transform);
                    b.AI = omni.Value;
                    b.Position = new Vector2Int(s.x + omni.Key.x,s.y+omni.Key.y);
                    GameManager.manager.enemies.Add(go);
                }
             
            }
        }


        public static void furnishCorridor(Map m, Structure s)
        {
            if (Rand.range(0, 100) < s.width * s.height * 3)
                spawnMonsterPlaceHolder(m, s.getRandom());

            // check top left and bottom right to see if this is a dead end
            Location tl = new Location(s.x, s.y);
            if (m.countAdjacentWalls(tl) == 7) // dead end, add a chest
                spawnChestPlaceHolder(m, tl);
            Location br = new Location(s.x + s.width - 1, s.y + s.height - 1);
            if (m.countAdjacentWalls(br) == 7) // dead end, add a chest
                spawnChestPlaceHolder(m, br);
        }

        public static void spawnMonsterPlaceHolder(Map m, Location l)
        {
            m.set(l.x, l.y, '.');
            dungeonGenerator.enemyPositions.Add(new Vector2Int(l.x, l.y));
        }

        public static void spawnChestPlaceHolder(Map m, Location l)
        {
            m.set(l.x, l.y, '=');
        }

        public static void spawnLootPlaceHolder(Map m, Location l, char c)
        {
            m.set(l.x, l.y, c);
        }

        public static Structure GenerateNewStructure(Map m, List<Structure> existing, Structure connectHere)
        {
            // things to we try to do to generate the new structure:

            // if room: attach a corridor along it
            // if either room or corridor: attach another room along it
            // attach a corridor perpendicular to it


            // try to attach a corridor along the NWSE boundaries of the anchor. 
            if (connectHere.isRoom() && Rand.range(0, 100) < 50)
            {
                foreach (Direction d in getRandomDirections())
                {
                    Location n = connectHere.getLineAlongTheBoundary(d).getRandom(); // get a random spot
                    if (n.isInBounds() && !isNear(existing, n)) // found a spot. try to grow the corridor along the room
                    {
                        Direction d2 = (Direction)(((int)d + 2) % 8); // rotate 90 degrees
                        Direction d3 = (Direction)(((int)d + 6) % 8); // rotate -90 degrees
                        Location cursord2 = new Location(n.x, n.y);
                        Location cursord3 = new Location(n.x, n.y);

                        int minDist = 5;
                        int maxDist = Rand.range(10, 16);
                        int dist = 1;
                        Structure sn = new Structure(n.x, n.y, 1, 1, Structure.Purpose.Corridor);

                        bool blocked2 = false;
                        bool blocked3 = false;
                        while (dist < maxDist && !(blocked2 && blocked3))
                        {
                            if (!blocked2)
                            {
                                Location c = cursord2.step(d2);
                                if (!c.isInBounds() || isNear(existing, c))
                                {
                                    blocked2 = true;
                                }
                                else
                                {
                                    dist++;
                                    cursord2 = c;
                                }
                            }

                            if (!blocked3)
                            {
                                Location c = cursord3.step(d3);
                                if (!c.isInBounds() || isNear(existing, c))
                                {
                                    blocked3 = true;
                                }
                                else
                                {
                                    dist++;
                                    cursord3 = c;
                                }
                            }
                        }
                        if (dist >= minDist)
                            return new Structure(cursord2, cursord3, Structure.Purpose.Corridor);
                        // fall through here
                    }
                }

            }

            // try to attach another room along the NSEW boundaries of the anchor
            if (Rand.range(0, 100) < 50)
            {
                foreach (Direction d in getRandomDirections())
                {
                    Location n = connectHere.getLineAlongTheBoundary(d).getRandom(); // get a random spot
                    if (!isNear(existing, n)) // found a spot. try to grow a room out of this
                    {
                        Structure ns = new Structure(n.x, n.y, 1, 1, Structure.Purpose.Room);
                        // grow the room in every direction possible until max dimension is reached or we can't grow any more

                        Direction dr = (Direction)(Rand.range(0, 4) * 2);
                        int tryCount = 0;

                        int maxDRand = Rand.range(1, 11);

                        int maxDim = 9;

                        switch (maxDRand)
                        {
                            case 1:
                            case 2:
                            case 3:
                            case 4:
                            case 5:
                            case 6:
                                maxDim -= 0;
                                break;
                            case 7:
                            case 8:
                                maxDim -= 1;
                                break;
                            case 9:
                            case 10:
                                maxDim -= 2;
                                break;
                            default:
                                break;
                        }
                        
                        
                        int minDim = 3;
                        
                        
                        while (tryCount < 4)
                        {
                            Structure ns2 = ns.grow(dr);
                            if (ns2.isInBounds() && !isIn(existing, ns2) && ns2.width <= maxDim && ns2.height <= maxDim)
                            {
                                ns = ns2;
                                tryCount = 0;
                            } 
                            else
                            {
                                tryCount++;
                            }
                            dr = (Direction)(((int)dr + 2) % 8);
                        }

                        if (ns.width >= minDim && ns.height >= minDim)
                            return ns;
                    }
                }
            }

            // finally, attach a corridor perpendicular to the anchor
            foreach (Direction d in getRandomDirections())
            {
                Location n = connectHere.getLineAlongTheBoundary(d).getRandom(); // get a random spot
                if (!isNear(existing, n)) // found a spot. try to grow a corridor out of this
                {
                    Structure ns = new Structure(n.x, n.y, 1, 1, Structure.Purpose.Corridor);
                    // grow the corridor in the same direction

                    int maxDim = Rand.range(8,15);
                    int minDim = 3;

                    bool blocked = false;

                    while (!blocked)
                    {
                        Structure ns2 = ns.grow(d);
                        if (ns2.isInBounds() && !isIn(existing, ns2) && ns2.width <= maxDim && ns2.height <= maxDim)
                            ns = ns2;
                        else
                            blocked = true;
                    }

                    if (ns.width >= minDim || ns.height >= minDim)
                        return ns;
                }
            }
            return null;
        }

        private static bool isNear(List<Structure> structs, Location n)
        {
            foreach (Structure s in structs)
                if (s.touches(n))
                    return true;
            return false;
        }

        private static bool isIn(List<Structure> structs, Structure st)
        {
            foreach (Structure s in structs)
                if (s.touches(st))
                    return true;
            return false;
        }


        private static bool isIn(List<Structure> structs, Location n)
        {
            foreach (Structure s in structs)
                if (s.contains(n))
                    return true;
            return false;
        }

        private static List<Direction> getRandomDirections()
        {
            List<Direction> l = new List<Direction>();
            Direction[] dirs = { Direction.NORTH, Direction.SOUTH, Direction.EAST, Direction.WEST };
            foreach (Direction d in dirs)
                l.Insert(Rand.range(0, l.Count), d);
            return l;
        }
    }
} 