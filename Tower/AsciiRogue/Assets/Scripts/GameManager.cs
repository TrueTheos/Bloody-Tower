using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    DungeonGenerator dungeonGenerator;
    public static GameManager manager;
    public EnemySpawner enemySpawner;
    public ItemSpawner itemSpawner;
    public PlayerMovement player;
    public PlayerStats playerStats;

    public GameObject enemyPrefab;

    public bool isPlayerTurn;
    public int turns; //turn counter

    public event Action playerTurnTasks; //Effects that will be executed at players turn

    [HideInInspector] public Vector2Int poisonDuration; //x = min, y = ma
    [HideInInspector] public Vector2Int fireResistanceDuration;
    [HideInInspector] public Vector2Int poisonResistanceDuration;
    [HideInInspector] public Vector2Int regenerationDuration;

    //Messages
    public Text messages;
    private string messagesText;
    private Queue m_Messages;
    public Text mapName;

    //Inventory
    public Text inventory;
    private string inventoryText;
    [HideInInspector] public Queue<string> m_Inventory;
    public GameObject invBorder; //inventory ui border  

    public GameObject mapText;
    public GameObject weaponArtIndicatorText;
    public Text mainUItext;

    [Header("Item UI stats")]
    public Text itemName;
    public Text itemType;
    public Text itemRare;
    public Text itemEffects;
    public RectTransform selector;
    [HideInInspector] public int selectedItem;
    private bool inventoryOpen = false;

    [Header("Decisions")]
    public Text decisions;
    public GameObject GOdec;
    private bool decisionMade;
    private bool deciding;
    private ItemScriptableObject _iso;
    private Item _isoGO;
    private bool equipState;
    private int decisionsCount;

    [Header("Pathfinding")]
    public List<GameObject> enemies = new List<GameObject>();

    [HideInInspector] public Tasks tasks;

    private bool cheatMenu = false;
    public string cheatString;

    private FOVNEW fv;

    void Awake()
    {
        fv = GetComponent<FOVNEW>();

        manager = this;

        m_Messages = new Queue();
        m_Inventory = new Queue<string>();

        tasks = GetComponent<Tasks>();

        dungeonGenerator = GetComponent<DungeonGenerator>();        
    }

    void Start()
    {
        dungeonGenerator.InitializeDungeon();
        dungeonGenerator.GenerateDungeon(0);
        for(int i = 1; i <= 25; i++)
        {
            dungeonGenerator.GenerateDungeon(i);
        }
        dungeonGenerator.GenerateDungeon(0);

        //Clear "Messages" Box
        m_Messages.Clear();
        messages.text = "";

        FoV.Initialize();
        fv.Initialize(fv.CanLightPass, fv.SetToVisible, fv.Distance);

        invBorder.SetActive(false);

        foreach (var item in itemSpawner.allItems)
        {
            item.identified = item.normalIdentifState;
            item.isEquipped = false;
            if(item is SpellbookSO book)
            {
                book.coolDown = 0;
            }
        }

        FirstTurn();
    }

    [Obsolete]
    public void Update()
    {
        if (isPlayerTurn)
        {
            if (Input.GetKeyDown(KeyCode.I) && !choosingWeapon)
            {
                if (!inventoryOpen)
                {
                    selector.GetComponent<Text>().enabled = true;
                    player.canMove = false;
                    inventoryOpen = true;
                    invBorder.SetActive(true);
                    mapText.SetActive(false);
                    mainUItext.enabled = false;
                    selector.GetComponent<Text>().enabled = true;
                    try {UpdateItemStats(playerStats.itemsInEq[0], playerStats.itemInEqGO[0]);}
                    catch{}
                }
                else
                {
                    selectedItem = 0;
                    player.canMove = true;
                    inventoryOpen = false;
                    invBorder.SetActive(false);
                    mapText.SetActive(true);
                    mainUItext.enabled = true;
                    selector.anchoredPosition = new Vector3(-7, -234, 0);
                    selector.GetComponent<Text>().enabled = false;
                }               
            }
            else if (Input.GetKeyDown(KeyCode.Escape) && inventoryOpen)
            {
                CloseEQ();
            }

            if (inventoryOpen)
            {
                if (Input.GetButtonDown("Use") && !choosingWeapon)
                {
                    decisionsCount = 1;
                    _iso = playerStats.itemsInEq[selectedItem];
                    _isoGO = playerStats.itemInEqGO[selectedItem];
                    equipState = playerStats.itemInEqGO[selectedItem].isEquipped;

                    GOdec.SetActive(true);

                    decisions.text = "1. Drop";
                    if (_iso.I_whereToPutIt != ItemScriptableObject.whereToPutIt.none)
                    {
                        if (equipState == true)
                        {
                            decisions.text += "\n" + "2. Unequip";
                            decisionsCount++;
                        }
                        else
                        {
                            decisions.text += "\n" + "2. Equip";
                            decisionsCount++;
                        }
                    }
                    if (_iso.I_itemType == ItemScriptableObject.itemType.Wand || _iso.I_itemType == ItemScriptableObject.itemType.Scroll || _iso.I_itemType == ItemScriptableObject.itemType.Spellbook || _iso.I_itemType == ItemScriptableObject.itemType.Gem)
                    {
                        decisionsCount++;
                        decisions.text += "\n" +  decisionsCount + ". " + "Use";
                    }
                    if (_iso.I_itemType == ItemScriptableObject.itemType.Potion)
                    {
                        decisionsCount++;
                        decisions.text += "\n" +  decisionsCount + ". " + "Drink";
                    }

                    decisionMade = false;
                    deciding = true;

                    DecisionTurn();                    
                }
                else if(Input.GetButtonDown("Use") && choosingWeapon && playerStats.itemInEqGO[selectedItem].iso is WeaponsSO) //ADD GEM TO THE SOCKET
                {
                    choosingWeapon = false;
                    choosenWeapon = playerStats.itemInEqGO[selectedItem];
                    choosenWeapon.AddGem(gemToConnect);
                    if(choosenWeapon.isEquipped) gemToConnect.Use(playerStats);
                    FinishPlayersTurn();
                }

                if (Input.GetKeyDown(KeyCode.Keypad2) && selectedItem < playerStats.itemsInEq.Count - 1)
                {
                    selectedItem++;
                    selector.anchoredPosition -= new Vector2(0, 26);
                    UpdateItemStats(playerStats.itemsInEq[selectedItem], playerStats.itemInEqGO[selectedItem]);
                }
                else if (Input.GetKeyDown(KeyCode.Keypad8) && selectedItem > 0)
                {
                    selectedItem--;
                    selector.anchoredPosition += new Vector2(0, 26);
                    UpdateItemStats(playerStats.itemsInEq[selectedItem], playerStats.itemInEqGO[selectedItem]);
                }               
            }          
        }
        
        if (deciding && !choosingWeapon)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) //drop item
            {
                Vector2Int posToDrop = new Vector2Int(1000,1000);

                if(_isoGO.cursed && _isoGO.equippedPreviously)
                {
                    UpdateMessages("You can't drop this item because it is <color=red>cursed</color!");
                    decisionMade = true;
                    FinishPlayersTurn();
                    return;
                }

                if(MapManager.map[MapManager.playerPos.x, MapManager.playerPos.y].structure == null)
                {                  
                    if(CanItemBeDroppedHere(MapManager.playerPos)) posToDrop = MapManager.playerPos;
                    else if(CanItemBeDroppedHere(new Vector2Int(MapManager.playerPos.x - 1, MapManager.playerPos.y))) posToDrop = new Vector2Int(MapManager.playerPos.x - 1, MapManager.playerPos.y);
                    else if(CanItemBeDroppedHere(new Vector2Int(MapManager.playerPos.x + 1, MapManager.playerPos.y))) posToDrop = new Vector2Int(MapManager.playerPos.x + 1, MapManager.playerPos.y);
                    else if(CanItemBeDroppedHere(new Vector2Int(MapManager.playerPos.x, MapManager.playerPos.y - 1))) posToDrop = new Vector2Int(MapManager.playerPos.x, MapManager.playerPos.y - 1);
                    else if(CanItemBeDroppedHere(new Vector2Int(MapManager.playerPos.x, MapManager.playerPos.y + 1))) posToDrop = new Vector2Int(MapManager.playerPos.x, MapManager.playerPos.y + 1);
                    else if(CanItemBeDroppedHere(new Vector2Int(MapManager.playerPos.x - 1, MapManager.playerPos.y - 1))) posToDrop = new Vector2Int(MapManager.playerPos.x - 1, MapManager.playerPos.y - 1);
                    else if(CanItemBeDroppedHere(new Vector2Int(MapManager.playerPos.x + 1, MapManager.playerPos.y + 1))) posToDrop = new Vector2Int(MapManager.playerPos.x + 1, MapManager.playerPos.y + 1);
                    else if(CanItemBeDroppedHere(new Vector2Int(MapManager.playerPos.x + 1, MapManager.playerPos.y - 1))) posToDrop = new Vector2Int(MapManager.playerPos.x + 1, MapManager.playerPos.y - 1);
                    else if(CanItemBeDroppedHere(new Vector2Int(MapManager.playerPos.x - 1, MapManager.playerPos.y + 1))) posToDrop = new Vector2Int(MapManager.playerPos.x - 1, MapManager.playerPos.y + 1);
                    else UpdateMessages("There is no space around you.");

                    if(posToDrop != new Vector2Int(1000,1000))
                    {
                        if (ColorUtility.TryParseHtmlString(_iso.I_color, out Color color))
                        {
                            MapManager.map[posToDrop.x, posToDrop.y].exploredColor = color;
                        }
                        MapManager.map[posToDrop.x, posToDrop.y].baseChar = _iso.I_symbol;

                        if(_iso is WeaponsSO bloodSword && _iso.I_name == "Bloodsword") tasks.everyTurnTasks -= bloodSword.BloodswordDecreaseDamage;

                        GameObject item = Instantiate(_isoGO.gameObject, transform.position, Quaternion.identity);
                        item.GetComponent<Item>().iso = _iso;
                        item.transform.SetParent(FloorManager.floorManager.floorsGO[DungeonGenerator.dungeonGenerator.currentFloor].transform);
                        MapManager.map[posToDrop.x, posToDrop.y].item = item.gameObject;
                    }
                }

                if (_iso.I_whereToPutIt != ItemScriptableObject.whereToPutIt.none && posToDrop != new Vector2Int(1000,1000)) //drop equipped item
                {
                    if(_isoGO.isEquipped)
                    {
                        switch (_iso.I_whereToPutIt)
                        {
                            case ItemScriptableObject.whereToPutIt.head:
                                playerStats._head = null;
                                playerStats.Head.text = "Helm:";
                                if (_isoGO.isEquipped) _isoGO.isEquipped = false;
                                break;
                            case ItemScriptableObject.whereToPutIt.body:
                                playerStats._body = null;
                                playerStats.Body.text = "Chest:";
                                if (_isoGO.isEquipped) _isoGO.isEquipped = false;
                                break;
                            case ItemScriptableObject.whereToPutIt.hand:
                                if (_iso is Torch torch)
                                {
                                    torch.RemoveTorch();
                                }

                                if (_isoGO._handSwitch == Item.hand.left)
                                {
                                    playerStats._Lhand = null;
                                    playerStats.LHand.text = "Left Hand:";
                                    if (_isoGO.isEquipped) _isoGO.isEquipped = false;
                                    if(_iso is WeaponsSO weapon)
                                    {
                                        _isoGO.UnequipWithGems();
                                    }
                                }                           
                                else if(_isoGO._handSwitch == Item.hand.right)
                                {
                                    playerStats._Rhand = null;
                                    playerStats.RHand.text = "Right Hand:";
                                    if (_isoGO.isEquipped) _isoGO.isEquipped = false;
                                    if(_iso is WeaponsSO weapon)
                                    {
                                        _isoGO.UnequipWithGems();
                                    }
                                }
                                break;
                            case ItemScriptableObject.whereToPutIt.legs:
                                playerStats._legs = null;
                                playerStats.Legs.text = "Legs:";
                                if (_isoGO.isEquipped) _isoGO.isEquipped = false;
                                break;
                            case ItemScriptableObject.whereToPutIt.ring:
                                if (playerStats._ring is RingSO ring)
                                {
                                    ring.Dequip(playerStats);
                                }
                                playerStats._ring = null;
                                playerStats.Ring.text = "Ring:";
                                if (_isoGO.isEquipped) _isoGO.isEquipped = false;
                                break;
                        }
                    }                   
                    
                    if(_iso.identified)
                    {
                        UpdateMessages($"You dropped <color={_iso.I_color}>{_iso.I_name}</color>.");
                    }
                    else
                    {
                        UpdateMessages($"You dropped <color={_iso.I_color}>{_iso.I_unInName}</color>.");
                    }
                    ApplyChangesInInventory(_iso);
                } 
                else
                {
                    if(posToDrop != new Vector2Int(1000,1000))
                    {
                        if(_iso.identified)
                        {
                            UpdateMessages($"You dropped <color={_iso.I_color}>{_iso.I_name}</color>.");
                        }
                        else
                        {
                            UpdateMessages($"You dropped <color={_iso.I_color}>{_iso.I_unInName}</color>.");
                        }
                        ApplyChangesInInventory(_iso);
                    }               
                }

                decisionMade = true;
                FinishPlayersTurn();
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha2) && decisionsCount > 1)) 
            {
                if(_iso.I_whereToPutIt != ItemScriptableObject.whereToPutIt.none)
                {
                    if (equipState) //deequip
                    {
                        if(_isoGO.cursed)
                        {
                            UpdateMessages("You can't unequipt this item becuase it's <color=red>cursed</color>.");
                        }
                        else
                        {
                            switch (_iso.I_whereToPutIt)
                            {
                                case ItemScriptableObject.whereToPutIt.head:
                                    playerStats._head = null;
                                    UpdateEquipment(playerStats.Head, "<color=#ffffff>Helm: </color>");

                                    if (_iso is ArmorSO armor)
                                    {
                                        armor.Use(playerStats);
                                    }
                                    _isoGO.isEquipped = false;
                                    break;
                                case ItemScriptableObject.whereToPutIt.body:
                                    playerStats._body = null;
                                    UpdateEquipment(playerStats.Body, "<color=#ffffff>Chest: </color>");

                                    if (_iso is ArmorSO armor1)
                                    {
                                        armor1.Use(playerStats);
                                    }
                                    _isoGO.isEquipped = false;
                                    break;
                                case ItemScriptableObject.whereToPutIt.hand:

                                    if(_iso is Torch torch)
                                    {
                                        torch.RemoveTorch();
                                    }

                                    if (_isoGO._handSwitch == Item.hand.left)
                                    {
                                        playerStats._Lhand = null;
                                        playerStats.LHand.text = "Left Hand:";
                                        if (_isoGO.isEquipped) _isoGO.isEquipped = false;
                                        if(_iso is WeaponsSO weapon)
                                        {
                                            _isoGO.UnequipWithGems();
                                       }
                                    }
                                    else if (_isoGO._handSwitch == Item.hand.right)
                                    {   
                                        playerStats._Rhand = null;
                                        playerStats.RHand.text = "Right Hand:";
                                        if (_isoGO.isEquipped) _isoGO.isEquipped = false;
                                        if(_iso is WeaponsSO weapon)
                                        {
                                            _isoGO.UnequipWithGems();
                                        }
                                    }
                                    break;
                                case ItemScriptableObject.whereToPutIt.ring:
                                    if (playerStats._ring is RingSO _ring) //deequip
                                    {
                                        _ring.Dequip(playerStats);
                                    }

                                    playerStats._ring = null;
                                    UpdateEquipment(playerStats.Ring, "<color=#ffffff>Ring: </color>");
                                    _isoGO.isEquipped = false;
                                    break;
                                case ItemScriptableObject.whereToPutIt.legs:
                                    playerStats._legs = null;
                                    UpdateEquipment(playerStats.Legs, "<color=#ffffff>Legs: </color>");

                                    if (_iso is ArmorSO armor2)
                                    {
                                        armor2.Use(playerStats);
                                    }
                                    _isoGO.isEquipped = false;
                                    break;
                            }
                            UpdateInventoryText();
                        }
                    }
                    else //equip
                    {
                        if (!_iso.identified)
                        {
                            playerStats.itemsInEq[selectedItem].identified = true; //make item identifyied
                            UpdateInventoryText(); //update item names to identifyed names (ring -> ring of fire resistance)
                            UpdateItemStats(playerStats.itemsInEq[selectedItem], playerStats.itemInEqGO[selectedItem]); //show full statistics
                            UpdateInventoryQueue(null);
                        }

                        if(_isoGO.cursed) UpdateMessages($"You wince as your grip involuntarily tightens around your {_iso.I_name}.");

                        _isoGO.equippedPreviously = true;

                        switch (_iso.I_whereToPutIt)
                        {
                            case ItemScriptableObject.whereToPutIt.head:
                                if(playerStats._head)
                                {
                                    UpdateMessages("You are already wearing something here. (<color=red>Unequip it first</color>)");
                                }
                                else
                                {
                                    playerStats._head = _iso;
                                    UpdateEquipment(playerStats.Head, "<color=#00FFFF>Helm: </color>" + "\n" + " " + playerStats._head.I_name);

                                    if (_iso is ArmorSO armor)
                                    {
                                        armor.Use(playerStats);
                                    }
                                    _isoGO.isEquipped = true;
                                }
                                
                                break;
                            case ItemScriptableObject.whereToPutIt.body:
                                if(playerStats._body)
                                {
                                    UpdateMessages("You are already wearing something here. (<color=red>Unequip it first</color>)");
                                }
                                else
                                {
                                    playerStats._body = _iso;
                                    UpdateEquipment(playerStats.Body, "<color=#00FFFF>Chest: </color>" + "\n" + " " + playerStats._body.I_name);

                                    if (_iso is ArmorSO armor1)
                                    {
                                        armor1.Use(playerStats);
                                    }
                                    _isoGO.isEquipped = true;
                                }
                                
                                break;
                            case ItemScriptableObject.whereToPutIt.hand:
                                if (_iso is Torch torch)
                                {
                                    torch.UseTorch();
                                }

                                if (playerStats._Lhand == null)
                                {
                                    playerStats._Lhand = _iso;
                                    UpdateEquipment(playerStats.LHand, "<color=#00FFFF>Left Hand: </color>" + "\n" + " " + playerStats._Lhand.I_name);
                                    _isoGO.isEquipped = true;
                                    _isoGO._handSwitch = Item.hand.left;

                                    if(_iso is WeaponsSO weapon)
                                    {
                                        _isoGO.EquipWithGems();
                                    }
                                }
                                else if (playerStats._Rhand == null)
                                {
                                    playerStats._Rhand = _iso;
                                    UpdateEquipment(playerStats.RHand, "<color=#00FFFF>Right Hand: </color>" + "\n" + " " + playerStats._Rhand.I_name);
                                    _isoGO.isEquipped = true;
                                    _isoGO._handSwitch = Item.hand.right;
                                    if(_iso is WeaponsSO weapon)
                                    {
                                        _isoGO.EquipWithGems();
                                    }
                                }
                                else
                                {
                                    UpdateMessages("You have no free hand.");
                                }
                                break;
                            case ItemScriptableObject.whereToPutIt.ring:
                                if(playerStats._ring)
                                {
                                    UpdateMessages("You are already wearing something here. (<color=red>Unequip it first</color>)");
                                }
                                else
                                {
                                    if (playerStats._ring is RingSO _ring) //deequip
                                    {
                                        _ring.Dequip(playerStats);
                                    }

                                    playerStats._ring = _iso;
                                    UpdateEquipment(playerStats.Ring, "<color=#00FFFF>Ring: </color>" + "\n" + " " + playerStats._ring.I_name);

                                    if (_iso is RingSO ring)
                                    {
                                        ring.Use(playerStats);
                                    }
                                    ApplyChangesInInventory(null);
                                    _isoGO.isEquipped = true;
                                }
                                
                                break;
                            case ItemScriptableObject.whereToPutIt.legs:
                                if(playerStats._legs)
                                {
                                    UpdateMessages("You are already wearing something here. (<color=red>Unequip it first</color>)");
                                }
                                else
                                {
                                    playerStats._legs = _iso;
                                    UpdateEquipment(playerStats.Legs, "<color=#00FFFF>Legs: </color>" + "\n" + " " + playerStats._legs.I_name);

                                    if (_iso is ArmorSO armor2)
                                    {
                                        armor2.Use(playerStats);
                                    }
                                    _isoGO.isEquipped = true;
                                }
                                
                                break;
                        }

                        UpdateInventoryText();
                    }

                    decisionMade = true;
                    FinishPlayersTurn();
                }
                else
                {
                    if(_iso is Gem gem)
                    {
                        gemToConnect = _iso;
                        UpdateMessages("Choose weapon. (ESC to cancel)");
                        choosingWeapon = true;
                        decisionMade = true;
                    }
                    else
                    {
                        _iso.Use(playerStats);
                        ApplyChangesInInventory(null);

                        decisionMade = true;
                        FinishPlayersTurn();
                    }
                }              
            }
            else if ((Input.GetKeyDown(KeyCode.Alpha3) && decisionsCount > 2))
            {
                if(_iso is Gem gem)
                {
                    gemToConnect = _iso;
                    UpdateMessages("Choose weapon.");
                    choosingWeapon = true;
                    decisionMade = true;
                }
                else
                {
                    _iso.Use(playerStats);
                    ApplyChangesInInventory(null);

                    decisionMade = true;
                    FinishPlayersTurn();
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.I))
            {
                decisionMade = true;
                FinishPlayersTurn();
            }
        }
        
        if (decisionMade)
        {
            deciding = false;
            GOdec.SetActive(false);            
        }
        
        if (playerStats.isDead)
        {
            GameObject.Find("Console").GetComponent<Text>().text = playerStats.deadText;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Application.LoadLevel(0);
            }
        }   
        
        if(Input.GetKeyDown(KeyCode.C))
        {
            cheatMenu = true;
        }
        
        if(cheatMenu)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                cheatMenu = false;

                if(cheatString.Substring(0, 2) == "fv")
                {
                    playerStats.FullVision();
                }   
                else if(cheatString.Substring(0, 5) == "spawn")
                {
                    string[] enemy = cheatString.Split(new string[] {"/"}, StringSplitOptions.None);
                    enemySpawner.SpawnAt(MapManager.playerPos.x, MapManager.playerPos.y - 1, enemySpawner.allEnemies.Where(obj => obj.name == enemy[1]).SingleOrDefault());
                }
                else if(cheatString.Substring(0, 5) == "level")
                {
                    string[] level = cheatString.Split(new string[] {"/"}, StringSplitOptions.None);
                    int _level = int.Parse(level[1]);
                    for(int i = 0; i < _level; i++)
                    {
                        playerStats.UpdateLevel(playerStats.__experienceNeededToLvlUp);
                    }
                }
            }
        }
    }

    bool CanItemBeDroppedHere(Vector2Int pos)
    {
        if(MapManager.map[pos.x, pos.y].item == null && MapManager.map[pos.x, pos.y].isWalkable && MapManager.map[pos.x, pos.y].enemy == null && MapManager.map[pos.x, pos.y].type != "Door")
        {
            return true;
        }
        else return false;
    }
    bool choosingWeapon;
    Item choosenWeapon;
    [HideInInspector] public ItemScriptableObject gemToConnect;

    void OnGUI()
    {
        if(cheatMenu)
        {
            // Make a text field that modifies stringToEdit.
            cheatString = GUI.TextField(new Rect(10, 10, 200, 20), cheatString, 25);
        }       
    }

    public void CloseEQ()
    {
        if(choosingWeapon) choosingWeapon = false;
        selectedItem = 0;
        player.canMove = true;
        inventoryOpen = false;
        invBorder.SetActive(false);
        mapText.SetActive(true);
        mainUItext.enabled = true;
        selector.anchoredPosition = new Vector3(-7, -234, 0);
        selector.GetComponent<Text>().enabled = false;
    }

    public void FirstTurn()
    {
        //FoV.GetPlayerFoV(player.position);
        fv.Compute(MapManager.playerPos, playerStats.viewRange);

        //enemySpawner.Spawn();
        //itemSpawner.Spawn();

        isPlayerTurn = true;

        GetComponent<Tasks>().EventsOnStartOfTheGame();

        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);

        //enemies = new List<GameObject>(enemySpawner.spawnedEnemies);
    }

    public void StartPlayersTurn()
    {
        isPlayerTurn = true;
        OnPlayerTurnStarts();

        foreach(var item in playerStats.itemsInEq)
        {
            if(item is SpellbookSO book)
            {
                if(book.coolDown > 0)
                {
                    book.coolDown--;
                }
            }
        }
    }

    public void OnPlayerTurnStarts()
    {
        /*
         * when player turn starts do all 
         * tasks that should be doone when player turn starts
        */
        playerTurnTasks?.Invoke();
        tasks.DoTasks();
    }

    public void DecisionTurn()
    {

    }

    public void FinishPlayersTurn()
    {
        isPlayerTurn = false;


        fv.Compute(MapManager.playerPos, playerStats.viewRange);
        //FoV.GetPlayerFoV(player.position);
        //UpdateVisibility();

        dungeonGenerator.DrawMap(true, MapManager.map);
        
        turns++;

        EnemyTurn();
    }

    public void EnemyTurn()
    {
        foreach(var enemy in enemies)
        {
            //canMoveToTheNextEnemy = false;
            if(enemy != null) enemy.GetComponent<RoamingNPC>().LookForPlayer();


            //yield return new WaitUntil(() => canMoveToTheNextEnemy);
        }

        dungeonGenerator.DrawMap(true, MapManager.map);

        StartPlayersTurn();
    }

    public void DeadTurn()
    {

    }

    void UpdateEquipment(Text _text, string _name)
    {
        _text.text = _name;
    }

    public void UpdateMessages(string newMessage)
    {
        if (m_Messages.Count >= 8)
        {
            m_Messages.Dequeue();
        }

        m_Messages.Enqueue(newMessage);

        UpdateText();
    } 

    public void UpdateText()
    {
        messagesText = "";

        foreach (System.Object obj in m_Messages)
        {
            String str = obj as String;
            messagesText = (messagesText + "\n" + str);
        }
        messages.text = messagesText;
    }

    public void UpdateInventoryQueue(string newItem)
    {
        if (m_Inventory.Count >= 17)
        {
            m_Inventory.Dequeue();
        }

        m_Inventory.Enqueue(newItem);

        UpdateInventoryText();
    }

    public void UpdateInventoryText()
    {
        inventoryText = "";

        int index = 0;

        foreach (System.Object obj in m_Inventory)
        {
            String str = obj as String;
            try
            {
                inventoryText = (inventoryText + (playerStats.itemInEqGO[index].isEquipped == true ? "(E)" : "") + str + "\n");
                index++;    
            }
            catch
            {

            }
        }
        inventory.text = inventoryText;
    }

    public void UpdateItemStats(ItemScriptableObject iso, Item item)
    {
            if(iso.identified) itemName.text = $"<color={iso.I_color}>{iso.I_name}</color>";
            else itemName.text = $"<color=purple>{iso.I_unInName}</color>";
            itemType.text = iso.I_itemType.ToString();

            if(iso.identified) itemEffects.text = $"{iso.effect}" + "\n" + (item.cursed == true && item.equippedPreviously ? "<color=red>Cursed</color> \n" : "\n") + "Weight: " + $"<color=green>{iso.I_weight}</color>" + "\n";
            else itemEffects.text = "???" + "\n" + "Weight: " + $"<color=green>{iso.I_weight}</color>";

            if(item.sockets == 1)
            {
                itemEffects.text += "\n" + "Socket: " + (item.socket1 == null ? "" : $"{item.socket1.I_name}");
            }
            else if(item.sockets == 2)
            {
                itemEffects.text += "\n" + "Socket: " + (item.socket1 == null ? "" : $"{item.socket1.I_name}");
                itemEffects.text += "\n" + "Socket: " + (item.socket2 == null ? "" : $"{item.socket2.I_name}");
            }
            else if(item.sockets == 3)
            {
                itemEffects.text += "\n" + "Socket: " + (item.socket1 == null ? "" : $"{item.socket1.I_name}");
                itemEffects.text += "\n" + "Socket: " + (item.socket2 == null ? "" : $"{item.socket2.I_name}");
                itemEffects.text += "\n" + "Socket: " + (item.socket3 == null ? "" : $"{item.socket3.I_name}");
            }

            if(iso.identified) 
            {
                switch (iso.I_rareness)
                {
                    case ItemScriptableObject.rareness.common:
                        itemRare.text = "<color=grey>Common</color>";
                        break;
                    case ItemScriptableObject.rareness.rare:
                        itemRare.text = "<color=green>Rare</color>";
                        break;
                    case ItemScriptableObject.rareness.very_rare:
                        itemRare.text = "<color=yellow>Very rare</color>";
                        break;
                    case ItemScriptableObject.rareness.mythical:
                        itemRare.text = "<color=red>Mythical</color>";
                        break;
                }
            }
            else itemRare.text = "<color=purple>???</color>";               
    }

    public void IncreaseStat(string stat)
    {
        if(playerStats.skillpoints <= 0)
        {
            playerStats._strengthButton.SetActive(false);
            playerStats._intelligenceButton.SetActive(false);
            playerStats._dexterityButton.SetActive(false);
            playerStats._enduranceButton.SetActive(false);
            return;
        }  

        if(stat == "str")
        {
            playerStats.__strength++;
            playerStats.UpdateText(PlayerStats.statType.strength);
            playerStats.skillpoints--;
        }
        else if(stat == "int")
        {
            playerStats.__intelligence++;
            playerStats.UpdateText(PlayerStats.statType.intelligence);
            playerStats.skillpoints--;
        }
        else if(stat == "dex")
        {
            playerStats.__dexterity++;
            playerStats.UpdateText(PlayerStats.statType.dexterity);
            playerStats.skillpoints--;
        }
        else if (stat == "end")
        {
            playerStats.__endurance++;
            playerStats.UpdateText(PlayerStats.statType.endurance);
            playerStats.skillpoints--;
        }      

        if(playerStats.skillpoints <= 0)
        {
            playerStats._strengthButton.SetActive(false);
            playerStats._intelligenceButton.SetActive(false);
            playerStats._dexterityButton.SetActive(false);
            playerStats._enduranceButton.SetActive(false);
            return;
        } 
    }

    public void ApplyChangesInInventory(ItemScriptableObject _item)
    {
        try
        {
            int index = System.Array.IndexOf(playerStats.itemsInEq.ToArray(), _item);
            playerStats.itemsInEq.RemoveAt(index);
            playerStats.itemInEqGO.RemoveAt(index);
            playerStats.currentItems--;
        }
        catch        
        {
            m_Inventory.Clear();

            playerStats.currentWeight = 0;

            foreach (var item in playerStats.itemsInEq)
            {
                if (item.identified)
                {
                    UpdateInventoryQueue($"<color={item.I_color}>{item.I_name}</color>");
                }
                else
                {
                    UpdateInventoryQueue($"<color=purple>{item.I_unInName}</color>");
                }

                playerStats.currentWeight += item.I_weight;
            }

            if (m_Inventory.Count == 0) inventory.text = "";

            playerStats.UpdateCapacity();
        }

        m_Inventory.Clear();

        playerStats.currentWeight = 0;

        foreach (var item in playerStats.itemsInEq)
        {
            if (item.identified)
            {
                UpdateInventoryQueue($"<color={item.I_color}>{item.I_name}</color>");
            }
            else
            {
                UpdateInventoryQueue($"<color=purple>{item.I_unInName}</color>");
            }

            playerStats.currentWeight += item.I_weight;
        }

        if (m_Inventory.Count == 0) inventory.text = "";

        playerStats.UpdateCapacity();
    }
}