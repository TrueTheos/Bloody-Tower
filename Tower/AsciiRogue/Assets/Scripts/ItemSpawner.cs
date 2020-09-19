using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public List<ItemScriptableObject> allItems;

    public List<ItemScriptableObject> weapons;
    public List<ItemScriptableObject> armors;
    public List<ItemScriptableObject> rings;
    public List<ItemScriptableObject> potions;
    public List<ItemScriptableObject> wands;
    public List<ItemScriptableObject> bandages;


    public List<ItemScriptableObject> scrolls;
    public List<ItemScriptableObject> spellbooks;

    public List<ItemScriptableObject> tools;
    public List<ItemScriptableObject> gems;

    public List<ItemScriptableObject> artfiacts;


    private List<int> scoreTable;
    private List<Vector2Int> ranomItemTable;

    [SerializeField] public List<ItemScriptableObject> itemsToSpawn;

    public GameObject itemPrefab;

    private Vector2Int __position { get; set; }

    private void Awake()
    {
        itemsToSpawn = allItems;
    }

    public void Spawn()
    {
        foreach (var room in DungeonGenerator.dungeonGenerator.rooms)
        {
            List<Vector2Int> positions = new List<Vector2Int>();

            foreach (Vector2Int position in room.positions)
            {
                if (MapManager.map[position.x, position.y].type == "Floor" && !MapManager.map[position.x, position.y].hasPlayer && MapManager.map[position.x, position.y].structure == null)
                {
                    positions.Add(position);
                }
            }

            try
            {
                Vector2Int pos = positions[UnityEngine.Random.Range(0, positions.Count - 1)];
                SpawnAt(pos.x, pos.y);
            }
            catch { }
        }
    }

    public void SpawnAt(int x, int y, ItemScriptableObject _item = null)
    {
        ItemScriptableObject itemToSpawn = null;

        if (_item == null)
        {
            if (MapManager.map[x, y].type != "Floor" || MapManager.map[x, y].structure != null) return;

            int itemRarirty = UnityEngine.Random.Range(1, 100);

            float itemType = UnityEngine.Random.Range(1, 75);        

            string itemTypeCompariser = "";

            /*if (itemType <= 0.1) itemTypeCompariser = "Artifact";
            else if (itemType <= 10) itemTypeCompariser = "Weapon";//spawn weapon
            else if (itemType <= 20) itemTypeCompariser = "Armor";//spawn armor
            else if (itemType <= 36) itemTypeCompariser = "Potion";
            else if (itemType <= 52) itemTypeCompariser = "Scroll";
            else if (itemType <= 56) itemTypeCompariser = "Spellbook";
            else if (itemType <= 59) itemTypeCompariser = "Ring";
            else if (itemType <= 67) itemTypeCompariser = "Gem";
            else if (itemType <= 74) itemTypeCompariser = "Money";*/
            itemTypeCompariser = "Spellbook";

            List<ItemScriptableObject> validItems = new List<ItemScriptableObject>();

            scoreTable = new List<int>();

            switch (itemTypeCompariser)
            {
                case "Weapon":
                    validItems = weapons;
                    itemToSpawn = ItemToSpawn(validItems);
                    break;
                case "Armor":
                    validItems = armors;
                    itemToSpawn = ItemToSpawn(validItems);
                    break;
                case "Scroll":
                    validItems = scrolls;
                    itemToSpawn = ItemToSpawn(validItems);
                    break;
                case "Spellbook":
                    validItems = spellbooks;
                    itemToSpawn = ItemToSpawn(validItems);
                    break;
                case "Gem":
                    validItems = gems;
                    itemToSpawn = ItemToSpawn(validItems);
                    break;
                case "Wand":
                    itemToSpawn = wands[UnityEngine.Random.Range(0, wands.Count)];
                    if (itemToSpawn is WandSO wand) wand.SetCharges();
                    break;
                case "Ring":
                    itemToSpawn = rings[UnityEngine.Random.Range(0, rings.Count)];
                    break;
                case "Money":
                    break;
                case "Potion":
                    validItems = potions;
                    itemToSpawn = ItemToSpawn(validItems);
                    break;
                case "Artifact":
                    itemToSpawn = artfiacts[UnityEngine.Random.Range(0, artfiacts.Count)];
                    break;
                    /*case "Tool":
                        itemToSpawn = tools[UnityEngine.Random.Range(0, tools.Count)];
                        Debug.Log("Tool");
                        break;*/
            }     

            if (itemTypeCompariser == "Money")
            {
                __position = new Vector2Int(x, y);

                MapManager.map[__position.x, __position.y].type = "Money Pouch";
                MapManager.map[__position.x, __position.y].letter = "&";
                MapManager.map[__position.x, __position.y].exploredColor = new Color(1, 1, 0);
                MapManager.map[__position.x, __position.y].isWalkable = true;

                MoneyPouch money = new MoneyPouch
                {
                    pos = __position
                };

                MapManager.map[__position.x, __position.y].structure = money;
            }
        }
        else
        {
            itemToSpawn = _item;
        }

        try
        {
            if (itemToSpawn)
            {
                __position = new Vector2Int(x, y);

                MapManager.map[__position.x, __position.y].baseChar = itemToSpawn.I_symbol;
                if (ColorUtility.TryParseHtmlString(itemToSpawn.I_color, out Color color))
                {
                    MapManager.map[__position.x, __position.y].exploredColor = color;
                }

                DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);

                GameObject item = Instantiate(itemPrefab.gameObject, transform.position, Quaternion.identity);

                item.GetComponent<Item>().identified = itemToSpawn.normalIdentifState;

                if (itemToSpawn is WeaponsSO weapon)
                {
                    if (Enumerable.Range(0, 5).Contains(DungeonGenerator.dungeonGenerator.currentFloor))
                    {
                        item.GetComponent<Item>().sockets = 1;
                    }
                    else if (Enumerable.Range(6, 15).Contains(DungeonGenerator.dungeonGenerator.currentFloor))
                    {
                        item.GetComponent<Item>().sockets = 2;
                    }
                    else if (Enumerable.Range(16, 25).Contains(DungeonGenerator.dungeonGenerator.currentFloor))
                    {
                        item.GetComponent<Item>().sockets = 3;
                    }
                }
                else if (itemToSpawn is SpellbookSO spellbook)
                {
                    item.GetComponent<Item>().learningTurns = spellbook.learnDuration;
                    item.GetComponent<Item>().durationLeft = spellbook.duration;
                }

                item.GetComponent<Item>().iso = itemToSpawn;

                if (itemToSpawn is WeaponsSO w || itemToSpawn is ArmorSO a)
                {
                    if (UnityEngine.Random.Range(0, 100) < 10)
                    {
                        item.GetComponent<Item>().cursed = true;
                    }
                }
                else if(itemToSpawn is PotionSO p)
                {
                    int i = UnityEngine.Random.Range(1, 100);
                    if (i <= 9)
                    {
                        item.GetComponent<Item>()._BUC = Item.BUC.cursed;
                    }
                    else if(i <= 90)
                    {
                        item.GetComponent<Item>()._BUC = Item.BUC.normal;
                    }
                    else
                    {
                        item.GetComponent<Item>()._BUC = Item.BUC.blessed;
                    }
                }

                item.transform.SetParent(FloorManager.floorManager.floorsGO[DungeonGenerator.dungeonGenerator.currentFloor].transform);

                MapManager.map[__position.x, __position.y].item = item.gameObject;

                return;
            }
        }
        catch { }
    }

    ItemScriptableObject ItemToSpawn(List<ItemScriptableObject> validItemsList)
    {
        float result = UnityEngine.Random.Range(0, 1f);

        float resultMultiplier = 0;
        foreach(var item in validItemsList)
        {
            if (DungeonGenerator.dungeonGenerator.currentFloor <= 10)
            {
                resultMultiplier += item.chanceOfSpawning1to10;
            }
            else if (DungeonGenerator.dungeonGenerator.currentFloor <= 20)
            {
                resultMultiplier += item.chanceOfSpawning11to20;
            }
            else if (DungeonGenerator.dungeonGenerator.currentFloor <= 30)
            {
                resultMultiplier += item.chanceOfSpawning21to30;
            }
            else
            {
                resultMultiplier += item.chanceOfSpawning31to40;
            }
        }

        result *= resultMultiplier;

        float rolling_sum = 0;
        foreach (var item in validItemsList)
        {
            if(DungeonGenerator.dungeonGenerator.currentFloor <= 10)
            {
                rolling_sum += item.chanceOfSpawning1to10;
            }
            else if (DungeonGenerator.dungeonGenerator.currentFloor <= 10)
            {
                rolling_sum += item.chanceOfSpawning11to20;
            }
            else if (DungeonGenerator.dungeonGenerator.currentFloor <= 30)
            {
                rolling_sum += item.chanceOfSpawning21to30;
            }
            else
            {
                rolling_sum += item.chanceOfSpawning31to40;
            }

            if(rolling_sum > result)
            {
                return item;
            }
        }

        return null;
    }
}