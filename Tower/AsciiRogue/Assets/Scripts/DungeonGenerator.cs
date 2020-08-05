using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using System.Collections;
using System.IO;

public class DungeonGenerator : MonoBehaviour
{
    public FixedLevels bossRoom;
    public FixedLevels lvlTen;
    public ItemScriptableObject keyToCell;

    public string[] enemiesList;
    public string[] splitted;
    public string enemies;

    public string str;

    public List<string> enemyNames = new List<string>();
    public List<Vector2Int> enemyPositions = new List<Vector2Int>();
    public List<bool> enemySleeping = new List<bool>();

    public static DungeonGenerator dungeonGenerator;
    private FloorManager floorManager;
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
    private int countFeatures;

    public int currentFloor = 0;

    public bool isASCII;

    [SerializeField] public List<Feature> allFeatures;
    [SerializeField] public List<Feature> rooms;

    public GameObject playerPrefab;

    private int dX; //door position x
    private int dY; //door position y

    private bool playerExists;

    private GameObject floorObject = null;

    public Text screen;
    [SerializeField] public int lightFactor = 1;


    [Header("Water Colors")]
    public List<Color> waterColors;

    public void Start()
    {
        dungeonGenerator = this;
        floorManager = GetComponent<FloorManager>();
        manager = GetComponent<GameManager>();
    }

    public void InitializeDungeon()
    {
        MapManager.map = new Tile[mapWidth, mapHeight];
    }

    public void GenerateDungeon(int floorNumber)
    {
        if(floorNumber == 25 && floorManager.floorsGO.Where(obj => obj.name == $"Floor {floorNumber}").FirstOrDefault() == null)
        {            
            GenerateFixedLevel(bossRoom.fixedLevel, 25, true);            
        }
        else if(floorNumber == 10 && floorManager.floorsGO.Where(obj => obj.name == $"Floor {floorNumber}").FirstOrDefault() == null)
        {
            GenerateFixedLevel(lvlTen.fixedLevel, 10, true);  
        }
        else if (floorManager.floors.Count <= floorNumber)
        {
            //GenerateNormalDungeon(floorNumber);
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

            manager.mapName.text = "Floor " + currentFloor;
            manager.UpdateMessages($"You entered Floor {currentFloor}");

            DrawMap(true, floorManager.floors[currentFloor]);
        }
    }

    public void GenerateNormalDungeon(int floor)
    {
            currentFloor = floor;

            if (!GameObject.Find($"Floor {currentFloor}"))
            {
                floorObject = new GameObject($"Floor {currentFloor}");
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

            GenerateFeature("Room", new Wall(), true);

            for (int i = 0; i < 500; i++)
            {
                Feature originFeature;

                if (allFeatures.Count == 1)
                {
                    originFeature = allFeatures[0];
                }
                else
                {
                    try
                    {
                        originFeature = allFeatures[UnityEngine.Random.Range(1, allFeatures.Count - 1)];
                    }
                    catch
                    {
                        GenerateDungeon(currentFloor);
                        return;
                    }
                }

                Wall wall = null;

                wall = ChoseWall(originFeature);
                if (wall == null) continue;

                string type;

                if (originFeature.type == "Room")
                {
                    type = "Corridor";
                }
                else
                {
                    if (UnityEngine.Random.Range(0, 100) < 80)
                    {
                        type = "Room";
                    }
                    else
                    {
                        type = "Corridor";
                    }
                }

                GenerateFeature(type, wall);

                if (countFeatures == maxFeatures) break;
            }

            rooms = GetRooms();

            if(MapManager.map.Length != mapWidth * mapHeight || rooms.Count < 3)
            {
                GenerateDungeon(currentFloor);
                return;
            }

            for(int yPos = mapHeight - 1; yPos >= 0; yPos--)
            {
                for(int xPos = 0; xPos < mapWidth; xPos++)
                {
                    if(MapManager.map[xPos,yPos] == null)
                    {
                        MapManager.map[xPos, yPos] = new Tile();
                        MapManager.map[xPos,yPos].xPosition = xPos;
                        MapManager.map[xPos,yPos].yPosition = yPos;
                        MapManager.map[xPos,yPos].baseChar = "";
                        MapManager.map[xPos,yPos].type = "Darkness";
                    }
                } 
            }

            List<Tile> correctTiles = new List<Tile>();

            for(int y = mapHeight - 1; y >= 0; y--)
            {
                correctTiles = new List<Tile>();
                correctTiles = (from Tile x in MapManager.map where x?.yPosition == y select x).ToList();
                if(correctTiles.Count < mapWidth - 1)
                {
                    GenerateDungeon(currentFloor);
                    return;
                }
            }

            GenerateStairsLowerFloor(); //Generate Stairs
            GenerateStairsUpperFloor(); //Generate Stairs

            if(GameManager.manager.turns > 1)
            {
                SpawnPlayer(); //Spawn Player
            }
            
            SpawnChests(); //Spawn Chests

            CreatePillars(); //Create Pillars

            GenerateGrass(); //Generate Grass

            floorManager.floors.Add(MapManager.map);
            floorManager.floorsGO.Add(floorObject);

            if (currentFloor != 0) MovePlayerToLowerStairs();

            manager.mapName.text = "Floor " + (currentFloor);
            manager.UpdateMessages($"You entered Floor {currentFloor}");

            manager.enemySpawner.Spawn(); //spawn enemies
            manager.itemSpawner.Spawn(); //spawn items

            DrawMap(isASCII, MapManager.map);
    }

    //GENERATES LEVEL FROM STRING
    public void GenerateFixedLevel(string fixedLevel, int floor, bool spawnEnemiesFromString)
    {            
        List<Vector2Int> itemPositions = new List<Vector2Int>();

        currentFloor = floor;

        if (!GameObject.Find($"Floor {currentFloor}"))
        {
            floorObject = new GameObject($"Floor {currentFloor}");
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
            enemies = fixedLevel.Substring((mapWidth * mapHeight));

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

                if(fixedLevel[inxedString] == "#"[0])
                {
                    MapManager.map[x,y] = new Tile();
                    MapManager.map[x,y].xPosition = x;
                    MapManager.map[x,y].yPosition = y;
                    MapManager.map[x,y].baseChar = "#";
                    MapManager.map[x,y].isWalkable = false;
                    MapManager.map[x,y].isOpaque = true;
                    MapManager.map[x,y].type = "Wall";
                }
                else if(fixedLevel[inxedString] == "."[0])
                {
                    MapManager.map[x,y] = new Tile();
                    MapManager.map[x,y].xPosition = x;
                    MapManager.map[x,y].yPosition = y;
                    MapManager.map[x,y].baseChar = ".";
                    MapManager.map[x,y].isWalkable = true;
                    MapManager.map[x,y].isOpaque = false;
                    MapManager.map[x,y].type = "Floor";
                }
                else if(fixedLevel[inxedString] == "<"[0])
                {
                    MapManager.map[x,y] = new Tile();
                    MapManager.map[x, y].structure = new Stairs();
                    if (MapManager.map[x, y].structure is Stairs stairsDown)
                    {
                        stairsDown.dungeonLevelId = currentFloor + 1;
                        stairsDown.spawnPosition = new Vector2Int(x, y);
                        MapManager.map[x, y].baseChar = "<";
                        MapManager.map[x, y].isOpaque = false;
                        MapManager.lowerStairsPos = new Vector2Int(x, y);
                        floorManager.stairsUp.Add(new Vector2Int(x, y));
                    }
                }
                else if(fixedLevel[inxedString] == ">"[0])
                {
                    MapManager.map[x,y] = new Tile();
                    MapManager.map[x, y].structure = new Stairs();
                    if (MapManager.map[x, y].structure is Stairs stairsUp)
                    {
                        if (currentFloor != 0)
                        {
                            stairsUp.dungeonLevelId = currentFloor - 1;
                            stairsUp.spawnPosition = new Vector2Int(x, y);
                            MapManager.map[x, y].baseChar = ">";
                            MapManager.map[x, y].isOpaque = false;
                            MapManager.upperStairsPos = new Vector2Int(x, y);
                            floorManager.stairsDown.Add(new Vector2Int(x, y));
                        }
                    }
                }
                else if(fixedLevel[inxedString] == "+"[0])
                {
                    MapManager.map[x,y] = new Tile();
                    MapManager.map[x, y].type = "Door";
                    MapManager.map[x, y].baseChar = "+";
                    MapManager.map[x, y].exploredColor = new Color(.545f, .27f, .07f);
                    MapManager.map[x, y].isWalkable = false;
                    MapManager.map[x, y].isOpaque = true;
                }
                else if(fixedLevel[inxedString] == "_"[0]) //DOOR THAT REQUIRES KEY
                {
                    MapManager.map[x,y] = new Tile();
                    MapManager.map[x, y].type = "Door";
                    MapManager.map[x, y].baseChar = "+";
                    MapManager.map[x, y].exploredColor = new Color(.68f, .68f, .68f);
                    MapManager.map[x,y].requiresKey = true;
                    MapManager.map[x, y].isWalkable = false;
                    MapManager.map[x, y].isOpaque = true;
                }
                else if(fixedLevel[inxedString] == "0"[0])
                {
                    MapManager.map[x,y] = new Tile();
                    MapManager.map[x,y].xPosition = x;
                    MapManager.map[x,y].yPosition = y;
                    MapManager.map[x,y].baseChar = ".";
                    MapManager.map[x,y].isWalkable = true;
                    MapManager.map[x,y].isOpaque = false;
                    MapManager.map[x,y].type = "Floor";

                    itemPositions.Add(new Vector2Int(x, y));
                    //manager.itemSpawner.SpawnAt(x,y);
                }
                else if(fixedLevel[inxedString] == "-"[0])
                {
                    MapManager.map[x,y] = new Tile();
                    MapManager.map[x, y].baseChar = keyToCell.I_symbol;
                    if (ColorUtility.TryParseHtmlString(keyToCell.I_color, out Color color))
                    {
                        MapManager.map[x, y].exploredColor = color;
                    }               

                    DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);

                    GameObject item = Instantiate(manager.itemSpawner.itemPrefab.gameObject, transform.position, Quaternion.identity);

                    item.GetComponent<Item>().iso = keyToCell;

                    MapManager.map[x, y].item = item.gameObject;
                }
                else if(fixedLevel[inxedString] == "="[0])
                {
                    MapManager.map[x,y] = new Tile();
                    CreateChest(x, y);
                }
                else
                {
                    MapManager.map[x,y] = new Tile();
                    MapManager.map[x,y].xPosition = x;
                    MapManager.map[x,y].yPosition = y;
                    MapManager.map[x,y].baseChar = ".";
                    MapManager.map[x,y].isWalkable = true;
                    MapManager.map[x,y].isOpaque = false;
                    MapManager.map[x,y].type = "Floor";
                }
            }
        }
        
        bool loopBreaker2 = false;
        
        /*for(int i = 0; i < mapWidth * mapHeight; i++)
        {
            if(loopBreaker2) break;
            Vector2Int pos2 = new Vector2Int(UnityEngine.Random.Range(1, mapWidth), UnityEngine.Random.Range(1, mapHeight));
            if(MapManager.map[pos2.x, pos2.y].isWalkable)
            {
                loopBreaker2 = true;

                GameObject player = GameObject.Find("Player");

                player.GetComponent<PlayerMovement>().position = pos2;
                MapManager.map[pos2.x, pos2.y].hasPlayer = true;
                MapManager.map[pos2.x, pos2.y].timeColor = new Color(0.5f, 1, 0);
                MapManager.map[pos2.x, pos2.y].letter = "@";
                MapManager.playerPos = new Vector2Int(pos2.x, pos2.y);
                GetComponent<GameManager>().player = player.GetComponent<PlayerMovement>();
                GetComponent<GameManager>().playerStats = player.GetComponent<PlayerStats>();
            }
        }*/

        if(UnityEngine.Random.Range(1,100) <= 30)
        {
            GenerateWaterPool();
        }
        
        floorManager.floors.Add(MapManager.map);
        floorManager.floorsGO.Add(floorObject);  

        if(spawnEnemiesFromString)
        {
            for(int i = 0; i < enemyNames.Count; i++)
            {
                manager.enemySpawner.SpawnAt(enemyPositions[i].x, mapHeight - enemyPositions[i].y - 1, manager.enemySpawner.allEnemies.Where(obj => obj.name == enemyNames[i]).SingleOrDefault(), enemySleeping[i]);
            }
        }
        else if(enemyPositions.Count > 1)
        {
            for (int i = 0; i < enemyPositions.Count; i++)
            {
                int y = mapHeight - 1 - enemyPositions[i].y;

                if(y == 0) y++;
                if(y == 21) y--;

                manager.enemySpawner.SpawnAt(enemyPositions[i].x, y);
            }
        }

        foreach (var item in itemPositions)
        {
            manager.itemSpawner.SpawnAt(item.x, item.y);
        }

        //if (currentFloor != 0) MovePlayerToLowerStairs();

        manager.mapName.text = "Floor " + currentFloor;
        manager.UpdateMessages($"You entered Floor {currentFloor}");

        enemyPositions.Clear();
        enemyNames.Clear();
        enemySleeping.Clear();
        
        //DrawMap(true, MapManager.map);
    }

    private void GenerateWaterPool()
    {
        List<Vector2Int> waterTilesToGrow = new List<Vector2Int>();

        Vector2Int startPos = new Vector2Int(100,100);

        Vector2Int vector = new Vector2Int(100,100);

        bool loopBr = false;
        for (int i = 0; i < 200; i++)
        {
            if(loopBr) break;

            int x = UnityEngine.Random.Range(1, mapWidth);
            int y = UnityEngine.Random.Range(1, mapHeight);

            if(MapManager.map[x,y].type == "Floor")
            {
                startPos = new Vector2Int(x,y);
                loopBr = true;
            }
        }

        waterTilesToGrow.Add(startPos);

        for (int i = 0; i < 10; i++)
        {
            int l = waterTilesToGrow.Count;
            for (int j = 0; j < l; j++)
            {
                MapManager.map[waterTilesToGrow[0].x,waterTilesToGrow[0].y].baseChar = "~";
                MapManager.map[waterTilesToGrow[0].x,waterTilesToGrow[0].y].exploredColor = waterColors[UnityEngine.Random.Range(0, waterColors.Count)];
                MapManager.map[waterTilesToGrow[0].x,waterTilesToGrow[0].y].type = "Water";

                if(MapManager.map[waterTilesToGrow[0].x + 1,waterTilesToGrow[0].y].type == "Floor")
                {
                    waterTilesToGrow.Add(new Vector2Int(waterTilesToGrow[0].x + 1,waterTilesToGrow[0].y));
                }
                if(MapManager.map[waterTilesToGrow[0].x - 1,waterTilesToGrow[0].y].type == "Floor")
                {
                    waterTilesToGrow.Add(new Vector2Int(waterTilesToGrow[0].x - 1,waterTilesToGrow[0].y));
                }
                if(MapManager.map[waterTilesToGrow[0].x,waterTilesToGrow[0].y + 1].type == "Floor")
                {
                    waterTilesToGrow.Add(new Vector2Int(waterTilesToGrow[0].x,waterTilesToGrow[0].y + 1));
                }
                if(MapManager.map[waterTilesToGrow[0].x,waterTilesToGrow[0].y - 1].type == "Floor")
                {
                    waterTilesToGrow.Add(new Vector2Int(waterTilesToGrow[0].x,waterTilesToGrow[0].y - 1));
                }
                waterTilesToGrow.RemoveAt(0);
            }
        }
    }


    bool TileCheck(int y, Tile tile)
    {
        if(tile.yPosition == y)
        {
            return true;
        }
        else return false;
    }
    private void GenerateStairsUpperFloor()
    {
        Feature room = rooms[rooms.Count - 1];

        List<Vector2Int> positions = new List<Vector2Int>();

        positions = GetCornersPositions(room);

        int random = UnityEngine.Random.Range(0, positions.Count - 1);

        MapManager.map[positions[random].x, positions[random].y].structure = new Stairs();
        if (MapManager.map[positions[random].x, positions[random].y].structure is Stairs stairsDown)
        {
            stairsDown.dungeonLevelId = currentFloor + 1;
            stairsDown.spawnPosition = new Vector2Int(positions[random].x, positions[random].y);
            MapManager.map[positions[random].x, positions[random].y].baseChar = "<";
            MapManager.map[positions[random].x, positions[random].y].isOpaque = false;
            MapManager.lowerStairsPos = new Vector2Int(positions[random].x, positions[random].y);
            floorManager.stairsUp.Add(new Vector2Int(positions[random].x, positions[random].y));
        }
    }

    private void GenerateStairsLowerFloor()
    {
        Feature room = rooms[0];

        List<Vector2Int> positions = new List<Vector2Int>();

        positions = GetCornersPositions(room);

        int random = UnityEngine.Random.Range(0, positions.Count - 1);       

        MapManager.map[positions[random].x, positions[random].y].structure = new Stairs();
        if (MapManager.map[positions[random].x, positions[random].y].structure is Stairs stairsUp)
        {
            if (currentFloor != 0)
            {
                stairsUp.dungeonLevelId = currentFloor - 1;
                stairsUp.spawnPosition = new Vector2Int(positions[random].x, positions[random].y);
                MapManager.map[positions[random].x, positions[random].y].baseChar = ">";
                MapManager.map[positions[random].x, positions[random].y].isOpaque = false;
                MapManager.upperStairsPos = new Vector2Int(positions[random].x, positions[random].y);
                floorManager.stairsDown.Add(new Vector2Int(positions[random].x, positions[random].y));
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

    private void GenerateFeature(string type, Wall wall, bool isFirst = false)
    {
        Feature room = new Feature
        {
            positions = new List<Vector2Int>()
        };

        int roomWidth = 0;
        int roomHeight = 0;

        if (type == "Room")
        {
            roomWidth = UnityEngine.Random.Range(widthMinRoom, widthMaxRoom);
            roomHeight = UnityEngine.Random.Range(heightMinRoom, heightMaxRoom);
        }
        else
        {
            switch (wall.direction)
            {
                case "South":
                    roomWidth = 3;
                    roomHeight = UnityEngine.Random.Range(minCorridorLength, maxCorridorLength);
                    break;
                case "North":
                    roomWidth = 3;
                    roomHeight = UnityEngine.Random.Range(minCorridorLength, maxCorridorLength);
                    break;
                case "West":
                    roomWidth = UnityEngine.Random.Range(minCorridorLength, maxCorridorLength);
                    roomHeight = 3;
                    break;
                case "East":
                    roomWidth = UnityEngine.Random.Range(minCorridorLength, maxCorridorLength);
                    roomHeight = 3;
                    break;
            }
        }

        int xStartingPoint;
        int yStartingPoint;

        if (isFirst)
        {
            xStartingPoint = UnityEngine.Random.Range(0, mapWidth);
            yStartingPoint = UnityEngine.Random.Range(0, mapHeight);
        }
        else
        {
            int id;
            if (wall.positions.Count == 3) id = 1;
            else id = UnityEngine.Random.Range(1, wall.positions.Count - 2);
            if (id < 1)
            {
                id = 1;
            }

            if (id > wall.positions.Count - 2) id = wall.positions.Count - 2;
            xStartingPoint = wall.positions[id].x;
            yStartingPoint = wall.positions[id].y;
        }

        Vector2Int lastWallPosition = new Vector2Int(xStartingPoint, yStartingPoint);

        if (isFirst)
        {
            xStartingPoint -= UnityEngine.Random.Range(0, roomWidth - 1);
            yStartingPoint -= UnityEngine.Random.Range(0, roomHeight - 1);
        }
        else
        {
            dX = lastWallPosition.x;
            dY = lastWallPosition.y;

            switch (wall.direction)
            {
                case "South":
                    if (type == "Room") xStartingPoint -= UnityEngine.Random.Range(1, roomWidth - 2);
                    else xStartingPoint--;
                    yStartingPoint -= roomHeight;
                    break;
                case "North":
                    if (type == "Room") xStartingPoint -= UnityEngine.Random.Range(1, roomWidth - 2);
                    else xStartingPoint--;
                    yStartingPoint++;
                    break;
                case "West":
                    xStartingPoint -= roomWidth;
                    if (type == "Room") yStartingPoint -= UnityEngine.Random.Range(1, roomHeight - 2);
                    else yStartingPoint--;
                    break;
                case "East":
                    xStartingPoint++;
                    if (type == "Room") yStartingPoint -= UnityEngine.Random.Range(1, roomHeight - 2);
                    else yStartingPoint--;
                    break;
            }
        }

        if (!CheckIfHasSpace(new Vector2Int(xStartingPoint, yStartingPoint), new Vector2Int(xStartingPoint + roomWidth - 1, yStartingPoint + roomHeight - 1))) return;

        room.walls = new Wall[4];

        for (int i = 0; i < room.walls.Length; i++)
        {
            room.walls[i] = new Wall();
            room.walls[i].positions = new List<Vector2Int>();
            room.walls[i].length = 0;
            room.walls[i].parent = room;

            switch (i)
            {
                case 0:
                    room.walls[i].direction = "South";
                    break;
                case 1:
                    room.walls[i].direction = "North";
                    break;
                case 2:
                    room.walls[i].direction = "West";
                    break;
                case 3:
                    room.walls[i].direction = "East";
                    break;
            }
        }

        for (int y = 0; y < roomHeight; y++)
        {
            for (int x = 0; x < roomWidth; x++)
            {
                Vector2Int position = new Vector2Int();
                position.x = xStartingPoint + x;
                position.y = yStartingPoint + y;

                room.positions.Add(position);

                MapManager.map[position.x, position.y] = new Tile();
                MapManager.map[position.x, position.y].xPosition = position.x;
                MapManager.map[position.x, position.y].yPosition = position.y;

                if (y == 0)
                {
                    room.walls[0].positions.Add(position);
                    room.walls[0].length++;
                    MapManager.map[position.x, position.y].type = "Wall";
                    MapManager.map[position.x, position.y].baseChar = "#";
                    MapManager.map[position.x, position.y].isWalkable = false;
                    MapManager.map[position.x, position.y].isOpaque = true;
                }
                if (y == (roomHeight - 1))
                {
                    room.walls[1].positions.Add(position);
                    room.walls[1].length++;
                    MapManager.map[position.x, position.y].type = "Wall";
                    MapManager.map[position.x, position.y].baseChar = "#";
                    MapManager.map[position.x, position.y].isWalkable = false;
                    MapManager.map[position.x, position.y].isOpaque = true;
                }
                if (x == 0)
                {
                    room.walls[2].positions.Add(position);
                    room.walls[2].length++;
                    MapManager.map[position.x, position.y].type = "Wall";
                    MapManager.map[position.x, position.y].baseChar = "#";
                    MapManager.map[position.x, position.y].isWalkable = false;
                    MapManager.map[position.x, position.y].isOpaque = true;
                }
                if (x == (roomWidth - 1))
                {
                    room.walls[3].positions.Add(position);
                    room.walls[3].length++;
                    MapManager.map[position.x, position.y].type = "Wall";
                    MapManager.map[position.x, position.y].baseChar = "#";
                    MapManager.map[position.x, position.y].isWalkable = false;
                    MapManager.map[position.x, position.y].isOpaque = true;
                }
                if (MapManager.map[position.x, position.y].type != "Wall")
                {
                    MapManager.map[position.x, position.y].type = "Floor";
                    MapManager.map[position.x, position.y].baseChar = ".";
                    MapManager.map[position.x, position.y].isWalkable = true;
                }
            }
        }

        if (!isFirst)
        {
            MapManager.map[lastWallPosition.x, lastWallPosition.y].type = "Floor";
            MapManager.map[lastWallPosition.x, lastWallPosition.y].isWalkable = true;
            MapManager.map[lastWallPosition.x, lastWallPosition.y].baseChar = ".";
            MapManager.map[lastWallPosition.x, lastWallPosition.y].isOpaque = false;

            switch (wall.direction)
            {
                case "South":
                    MapManager.map[lastWallPosition.x, lastWallPosition.y - 1].type = "Floor";
                    MapManager.map[lastWallPosition.x, lastWallPosition.y - 1].baseChar = ".";
                    MapManager.map[lastWallPosition.x, lastWallPosition.y - 1].isWalkable = true;
                    MapManager.map[lastWallPosition.x, lastWallPosition.y - 1].isOpaque = false;

                    break;
                case "North":
                    MapManager.map[lastWallPosition.x, lastWallPosition.y + 1].type = "Floor";
                    MapManager.map[lastWallPosition.x, lastWallPosition.y + 1].baseChar = ".";
                    MapManager.map[lastWallPosition.x, lastWallPosition.y + 1].isWalkable = true;
                    MapManager.map[lastWallPosition.x, lastWallPosition.y + 1].isOpaque = false;

                    break;
                case "West":
                    MapManager.map[lastWallPosition.x - 1, lastWallPosition.y].type = "Floor";
                    MapManager.map[lastWallPosition.x - 1, lastWallPosition.y].baseChar = ".";
                    MapManager.map[lastWallPosition.x - 1, lastWallPosition.y].isWalkable = true;
                    MapManager.map[lastWallPosition.x - 1, lastWallPosition.y].isOpaque = false;

                    break;
                case "East":
                    MapManager.map[lastWallPosition.x + 1, lastWallPosition.y].type = "Floor";
                    MapManager.map[lastWallPosition.x + 1, lastWallPosition.y].baseChar = ".";
                    MapManager.map[lastWallPosition.x + 1, lastWallPosition.y].isWalkable = true;
                    MapManager.map[lastWallPosition.x + 1, lastWallPosition.y].isOpaque = false;

                    break;
            }
        }

        int chanceToSpawnADoor = UnityEngine.Random.Range(1, 5);

        if (!isFirst && chanceToSpawnADoor == 1)
        {
            MapManager.map[dX, dY].type = "Door";
            MapManager.map[dX, dY].baseChar = "+";
            MapManager.map[dX, dY].exploredColor = new Color(.545f, .27f, .07f);
            MapManager.map[dX, dY].isWalkable = false;
            MapManager.map[dX, dY].isOpaque = true;
        }

        room.width = roomWidth;
        room.height = roomHeight;
        room.type = type;
        room.id = countFeatures;
        allFeatures.Add(room);
        countFeatures++;
    }

    private bool CheckIfHasSpace(Vector2Int start, Vector2Int end)
    {
        bool hasSpace = true;

        for (int y = start.y; y <= end.y; y++)
        {
            for (int x = start.x; x <= end.x; x++)
            {
                if (x < 0 || y < 0 || x >= mapWidth || y >= mapHeight) return false;
                if (MapManager.map[x, y] != null)
                {
                    return false;
                }
            }
        }

        return hasSpace;
    }

    private Wall ChoseWall(Feature feature)
    {

        for (int i = 0; i < 10; i++)
        {
            int id = UnityEngine.Random.Range(0, 100) / 25;
            if (!feature.walls[id].hasFeature)
            {
                return feature.walls[id];
            }
        }
        return null;
    }

    private List<Feature> GetRooms()
    {
        List<Feature> newRooms = new List<Feature>();

        foreach (Feature feature in allFeatures)
        {
            if (feature.type == "Room")
            {
                newRooms.Add(feature);
            }
        }

        return newRooms;
    }

    private void SpawnChests()
    {
        var room = rooms[UnityEngine.Random.Range(0, rooms.Count)];

        List<Vector2Int> corners = GetCornersPositions(room);

        try
        {
            Vector2Int chestPos = corners[UnityEngine.Random.Range(0, corners.Count)];

            CreateChest(chestPos.x, chestPos.y);
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

        bool loopBreaker = false;

        MapManager.map[x, y].structure = chest;

        int randomItem = UnityEngine.Random.Range(0, manager.itemSpawner.allItems.Count);
        chest.itemInChest = manager.itemSpawner.allItems[randomItem];
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

    private void GenerateGrass()
    {
        for (int i = 0; i < rooms.Count; i++)
        {
            if (UnityEngine.Random.Range(1, 100) < 35)
            {
                Feature room = rooms[i];

                List<Vector2Int> positions = new List<Vector2Int>();

                foreach (Vector2Int position in room.positions)
                {
                    if (MapManager.map[position.x, position.y].type == "Floor" && !MapManager.map[position.x, position.y].hasPlayer && MapManager.map[position.x, position.y].structure == null)
                    {
                        positions.Add(position);
                    }
                }

                int grasstiles = UnityEngine.Random.Range(1, positions.Count / 4); //how many grass

                for (int z = 0; z < grasstiles; z++)
                {
                    int randomTile = UnityEngine.Random.Range(0, positions.Count);

                    if (MapManager.map[positions[randomTile].x, positions[randomTile].y].isWalkable && MapManager.map[positions[randomTile].x, positions[randomTile].y].structure == null)
                    {
                        MapManager.map[positions[randomTile].x, positions[randomTile].y].type = "Grass";
                        MapManager.map[positions[randomTile].x, positions[randomTile].y].isWalkable = true;
                        MapManager.map[positions[randomTile].x, positions[randomTile].y].letter = "\u0239";
                        MapManager.map[positions[randomTile].x, positions[randomTile].y].exploredColor = new Color(0.53f, 0.76f, 0.47f);
                        MapManager.map[positions[randomTile].x, positions[randomTile].y].isOpaque = true;

                        Grass grass = new Grass
                        {
                            pos = new Vector2Int(positions[randomTile].x, positions[randomTile].y)
                        };

                        MapManager.map[positions[randomTile].x, positions[randomTile].y].structure = grass;
                    }

                    positions.RemoveAt(randomTile);
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

    private void SpawnPlayer()
    {
        if (!playerExists)
        {
            playerExists = true;

            Feature room = rooms[0];

            List<Vector2Int> positions = new List<Vector2Int>();

            foreach (Vector2Int position in room.positions)
            {
                if (MapManager.map[position.x, position.y].type == "Floor")
                {
                    positions.Add(position);
                }
            }

            Vector2Int pos = positions[UnityEngine.Random.Range(0, positions.Count - 1)];

            GameObject player = GameObject.Find("Player");

            player.GetComponent<PlayerMovement>().position = pos;
            MapManager.map[pos.x, pos.y].hasPlayer = true;
            MapManager.map[pos.x, pos.y].timeColor = new Color(0.5f, 1, 0);
            MapManager.map[pos.x, pos.y].letter = "@";
            MapManager.playerPos = new Vector2Int(pos.x, pos.y);
            room.hasPlayer = true;
        }
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

    string CalculateFade(Color rgb_value, int x, int y, Vector2Int playerPos, int refadeFactor = 1)
    {
        int dst = (int)(Mathf.Abs(x - playerPos.x) + Mathf.Abs(y - playerPos.y)) + 1;
        float R = rgb_value.r / dst;
        float G = rgb_value.g / dst;
        float B = rgb_value.b / dst;

        Color color;
       
        if (MapManager.map[x, y].isVisible)
            if (MapManager.map[x, y].item != null)
            {
                R *= 2;
                G *= 2;
                B *= 2;
                color = new Color(R * refadeFactor, G * refadeFactor, B * refadeFactor);
                return ColorUtility.ToHtmlStringRGBA(color);
            }
            else if(MapManager.map[x, y].enemy != null)
            {
                R *= 2;
                G *= 2;
                B *= 2;
                color = new Color(R * refadeFactor, G * refadeFactor, B * refadeFactor);
                return ColorUtility.ToHtmlStringRGBA(color);
            }
            else
            {
                color = new Color(R * refadeFactor, G * refadeFactor, B * refadeFactor);
                return ColorUtility.ToHtmlStringRGBA(color);
            }
        else
            return ColorUtility.ToHtmlStringRGBA(new Color(R / 2, G / 2, B / 2));
    }

    //-----------------------------------------------------------------------------------

    class Map
    {
        public static int WIDTH = 56;
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

        internal string print()
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
                    list[x * height + y] = new Location(this.x+x, this.y);
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
            Map m = new Map();

            // start with basic architecture

            // carve a central room, then attach stuff to it
            int x = Map.WIDTH / 2 + Rand.range(-5, 6);
            int y = Map.HEIGHT / 2 + Rand.range(-5, 6);
            int w = Rand.range(3, 11);
            int h = Rand.range(3, 11);
            Structure centralRoom = new Structure(x, y, w, h, Structure.Purpose.Room);

            m.carve(centralRoom, '.');

            List<Structure> structs = new List<Structure>();
            structs.Add(centralRoom);


            for (int i = 0; i < 20; i++)
            {
                // pick a structure to attach to. 
                Structure anchor = null; ;

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
                    m.set(l.x, l.y, '+'); // at least one of them is a room. Connect them with a door
                } else
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

            // furnish the dungeon
            foreach (Structure s in structs)
            {
                if (s.isRoom())
                    furnishRoom(m, s);
                else
                    furnishCorridor(m, s);

            }

            // spawn the stairs
            Structure r = SpawnStairsPlaceHolder('<', m, structs, null);
            SpawnStairsPlaceHolder('>', m, structs, r);
            return m;
        }

        private static Structure SpawnStairsPlaceHolder(char c, Map m, List<Structure> structs, Structure notHere)
        {
            int attempts = 5; // there might only be one room, avoid infinite loops
            while (true)
            {
                Structure r = getRandomRoomHelper(structs);
                if (r == notHere && attempts++ < 5)
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
            int size = s.width * s.height;

            // loot
            String loot = "0";
            int monsterCount = 1;

            if (size > 100)
            {
                loot = "000";
                monsterCount = 3;
            }
            else if (size > 50)
            {
                loot = "00";
                monsterCount = 2;
            }

            // minimum room size is currently 3x3 so this is at least 9
            Location[] l = s.getRandom(loot.Length + monsterCount);
            int j = 0;
            char[] lootArray = loot.ToCharArray();
            for (int i = 0; i < lootArray.Length; i++)
                spawnLootPlaceHolder(m, l[j++], lootArray[i]);  
            for (int i = 0; i < monsterCount; i++)
                spawnMonsterPlaceHolder(m, l[j++]);
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
            m.set(l.x, l.y, 'g');
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
                        int maxDim = 10;
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

