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


    public List<ItemScriptableObject> scrollsLevel1;
    public List<ItemScriptableObject> scrollsLevel2;
    public List<ItemScriptableObject> scrollsLevel3;
    public List<ItemScriptableObject> spellbooks1;
    public List<ItemScriptableObject> spellbooks2;
    public List<ItemScriptableObject> spellbooks3;

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
            catch{}
        }      
    }

    public void SpawnAt(int x, int y)
    {
        int itemRarirty = UnityEngine.Random.Range(1, 100);

        float itemType = UnityEngine.Random.Range(1, 100);

        ItemScriptableObject itemToSpawn = null;

        string itemTypeCompariser = "";

        if(itemType <= 0.1) itemTypeCompariser = "Artifact";
        else if (itemType <= 10) itemTypeCompariser = "Weapon";//spawn weapon
        else if (itemType <= 20) itemTypeCompariser = "Armor";//spawn armor
        else if (itemType <= 30) itemTypeCompariser = "Scroll"; //spawn scrollOrBook
        else if (itemType <= 44) itemTypeCompariser = "Gem";//spawn gems
        else if (itemType <= 47) itemTypeCompariser = "Wand";//spawn wands
        else if (itemType <= 50) itemTypeCompariser = "Ring";//spawn rings
        else if (itemType <= 60) itemTypeCompariser = "Money";//spawn money pouch
        else if (itemType <= 100) itemTypeCompariser = "Potion";//spawn potion

        List<ItemScriptableObject> validItems = new List<ItemScriptableObject>();

        scoreTable = new List<int>();

        switch (itemTypeCompariser) 
        {
            case "Weapon":
                validItems = weapons.FindAll(m => ItemLevelCheck(m));

                itemToSpawn = ItemToSpawn(validItems);
                //itemToSpawn = weapons[UnityEngine.Random.Range(0, weapons.Count)];
                break;
            case "Armor":
                validItems = armors.FindAll(n => ItemLevelCheck(n));
                
                itemToSpawn = ItemToSpawn(validItems);
                break;         
            case "Scroll":
                itemToSpawn = SpawnScrollsSellbooks();
                //itemToSpawn = scrolls[UnityEngine.Random.Range(0, scrolls.Count)];
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
                itemToSpawn = potions[UnityEngine.Random.Range(0, potions.Count)];
                break;    
            case "Artifact":
                itemToSpawn = artfiacts[UnityEngine.Random.Range(0, artfiacts.Count)];
                break;
            /*case "Tool":
                itemToSpawn = tools[UnityEngine.Random.Range(0, tools.Count)];
                Debug.Log("Tool");
                break;*/
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

            if(itemToSpawn is SpellbookSO book) book.coolDown = 0;
            else if(itemToSpawn is WeaponsSO weapon)
            {
                if(Enumerable.Range(0, 5).Contains(DungeonGenerator.dungeonGenerator.currentFloor))
                {
                    item.GetComponent<Item>().sockets = 1;
                }
                else if(Enumerable.Range(6, 15).Contains(DungeonGenerator.dungeonGenerator.currentFloor))
                {
                    item.GetComponent<Item>().sockets = 2;
                }
                else if(Enumerable.Range(16, 25).Contains(DungeonGenerator.dungeonGenerator.currentFloor))
                {
                    item.GetComponent<Item>().sockets = 3;
                }
            } 

            item.GetComponent<Item>().iso = itemToSpawn;

            if(itemToSpawn is WeaponsSO  w || itemToSpawn is ArmorSO a)
            {
                if(UnityEngine.Random.Range(0,100) < 5)
                {
                    item.GetComponent<Item>().cursed = true;
                }
            }

            item.transform.SetParent(FloorManager.floorManager.floorsGO[DungeonGenerator.dungeonGenerator.currentFloor].transform);

            MapManager.map[__position.x, __position.y].item = item.gameObject;

            return;

            //int index = Array.IndexOf(itemsToSpawn.ToArray(), itemToSpawn);

            //itemsToSpawn.RemoveAt(index);
        }    
        }
        catch{}  

        if(itemTypeCompariser == "Money")
        {
            __position = new Vector2Int(x,y);

            MapManager.map[__position.x, __position.y].type = "Money Pouch";
            MapManager.map[__position.x, __position.y].letter = "&";
            MapManager.map[__position.x, __position.y].exploredColor = new Color(1,1,0);
            MapManager.map[__position.x, __position.y].isWalkable = true;

            MoneyPouch money = new MoneyPouch
            {
                pos = __position
            };

            MapManager.map[__position.x, __position.y].structure = money;
        } 
    }

    ItemScriptableObject ItemToSpawn(List<ItemScriptableObject> validItemsList)
    {
        scoreTable = new List<int>();
        List<ItemScriptableObject> validItems = validItemsList;
        int totalScore = 0;
        int random = 0;
        bool loopBreaker = false;

                for(int i = 0; i < validItems.Count; i++)
                {
                    scoreTable.Add(20 - Mathf.Abs(validItems[i].itemLevel - DungeonGenerator.dungeonGenerator.currentFloor));
                }

                totalScore = 0;

                ranomItemTable = new List<Vector2Int>();

                scoreTable = scoreTable.Where(i => i > 0).ToList();

                for(int i = 0; i < scoreTable.Count; i++)
                {
                        totalScore += scoreTable[i];
                        if(i != 0)
                        {
                            ranomItemTable.Add(new Vector2Int(i + 1 + ranomItemTable[i - 1].y, ranomItemTable[i - 1].y + scoreTable[i]));
                        }
                        else
                        {
                            ranomItemTable.Add(new Vector2Int(1, scoreTable[i]));
                        }
                }

                random = UnityEngine.Random.Range(1, totalScore);

                loopBreaker = false;
                foreach(var item in ranomItemTable)
                {
                    if(loopBreaker) break;

                    if(Enumerable.Range(item.x, item.y).Contains(random))
                    {   
                        loopBreaker = true;
                        return validItems[Array.IndexOf(ranomItemTable.ToArray(), item)];
                    }
                }
                return null;
    }

    ItemScriptableObject SpawnScrollsSellbooks()
    {
        string school = "";
        string item = "";

        int level = 0;

        float random = UnityEngine.Random.value;

        if(DungeonGenerator.dungeonGenerator.currentFloor <= 5)
        {
            if(random <= 0.1)
            {
                school = "Blood";
            }
            else if(random <= 0.5)
            {
                school = "chaos";
            }
            else
            {
                school = "Plague";
            }

            random = UnityEngine.Random.value;

            if(random <= 0.2)
            {
                item = "Book1";
                level = 1;
            }
            else if(random <= 0.4)
            {
                item = "Scroll2";
                level = 2;
            }
            else
            {
                item = "Scroll1";
                level = 1;
            }
        }
        else if(DungeonGenerator.dungeonGenerator.currentFloor >= 6 && DungeonGenerator.dungeonGenerator.currentFloor <= 10)
        {
            if(random <= 0.3)
            {
                school = "chaos";
            }
            else if(random <= 0.6)
            {
                school = "Plague";
            }
            else
            {
                school = "Blood";
            }

            random = UnityEngine.Random.value;

            if(random <= 0.1)
            {
                item = "Book2";
                level = 2;
            }
            else if(random <= 0.2)
            {
                item = "Book1";
                level = 1;
            }
            else if(random <= 0.3)
            {
                item = "Scroll3";
                level = 3;
            }
            else if(random <= 0.65)
            {
                item = "Scroll2";
                level = 2;
            }
            else
            {
                item = "Scroll1";
                level = 1;
            }
        }
        else if(DungeonGenerator.dungeonGenerator.currentFloor >= 11 && DungeonGenerator.dungeonGenerator.currentFloor <= 15)
        {
            if(random <= 0.4)
            {
                school = "chaos";
            }
            else if(random <= 0.9)
            {
                school = "Blood";
            }
            else
            {
                school = "Plague";
            }

            random = UnityEngine.Random.value;

            if(random <= 0.05)
            {
                item = "Book3";
                level = 3;
            }
            else if(random <= 0.15)
            {
                item = "Book2";
                level = 2;
            }
            else if(random <= 0.2)
            {
                item = "Book1";
                level = 1;
            }
            else if(random <= 0.4)
            {
                item = "Scroll3";
                level = 3;
            }
            else if(random <= 0.85)
            {
                item = "Scroll2";
                level = 2;
            }
            else
            {
                item = "Scroll1";
                level = 1;
            }
        }
        else if(DungeonGenerator.dungeonGenerator.currentFloor >= 16 && DungeonGenerator.dungeonGenerator.currentFloor <= 20)
        {
            if(random <= 0.1)
            {
                school = "Plague";
            }
            else if(random <= 0.4)
            {
                school = "chaos";
            }
            else
            {
                school = "Blood";
            }

            random = UnityEngine.Random.value;

            if(random <= 0.02)
            {
                item = "Book1";
                level = 1;
            }
            else if(random <= 0.11)
            {
                item = "Book2";
                level = 2;
            }
            else if(random <= 0.2)
            {
                item = "Book3";
                level = 3;
            }
            else if(random <= 0.6)
            {
                item = "Scroll3";
                level = 3;
            }
            else
            {
                item = "Scroll2";
                level = 2;
            }
        }        

        List<ItemScriptableObject> fittingItems = allItems.FindAll(x => ItemCheck(school, level, x));
        if(fittingItems.Count == 0)
        {
            return null;
        }
        return fittingItems[UnityEngine.Random.Range(0, fittingItems.Count)];
    }
    
    bool ItemLevelCheck(ItemScriptableObject item)
    {
        if(Enumerable.Range(item.levelMin, item.levelMax).Contains(DungeonGenerator.dungeonGenerator.currentFloor))
        {
            return true;            
        }
        else return false;
    }

    bool ItemCheck(string school, int level, ItemScriptableObject item)
    {
        if(item is ScrollSO scroll)
        {
            try
            {
                if(scroll._school == (ScrollSO.school)System.Enum.Parse(typeof(ScrollSO.school), school) && scroll.spellLevel == level)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch{return false;}
        }
        else if(item is SpellbookSO spellbook)
        {
            try
            {
                if(spellbook._school == (SpellbookSO.school)System.Enum.Parse(typeof(SpellbookSO.school), school) && spellbook.spellLevel == level)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch{return false;}
        }
        else return false;
    }
}
