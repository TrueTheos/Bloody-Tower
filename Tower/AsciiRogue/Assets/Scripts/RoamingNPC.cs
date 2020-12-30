using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using System;
using System.Collections;

public class RoamingNPC : MonoBehaviour,IUnit
{
    public EnemiesScriptableObject enemySO;


    public string EnemyName
    {
        get
        {
            if (enemySO.E_realName == "")
                return enemySO.E_name;
            else            
                if (sleeping)
                    return enemySO.E_name;
                else
                    return enemySO.E_realName;            
        }        
    }
    public string EnemySymbol
    {
        get
        {
            if (enemySO.E_realName == "")
                return enemySO.E_symbol;
            else
                if (sleeping)
                return enemySO.E_symbol;
            else
                return enemySO.E_realSymbol;
        }
    }
    public Color EnemyColor
    {
        get
        {
            if (enemySO.E_realName == "")
                return enemySO.E_color;
            else
                if (sleeping)
                return enemySO.E_color;
            else
                return enemySO.E_realColor;
        }
    }

    //Statistics
    public int __currentHp { get; set; }
    public int currHp { get { return __currentHp; } set { __currentHp = value; } }
    public int maxHp { get; set; }

    public int str { get; set; }
    public int dex { get; set; }
    public int intell { get; set; }
    public int end { get; set; }
    public int lvl { get; set; }
    public int xpDrop { get; set; }

    public int AC { get; set; }
    public int ac
    {
        get
        {
            return AC;
        }
        set
        {
            AC = value;
        }
    }

    public string noun => enemySO.E_realName!=""?enemySO.E_realName:enemySO.E_name;

    //Variables for Cowardly enemies
    public int runCounter = 5;
    public Vector2Int runDirection;

    //Variables for Recover enemies
    public int hpRegenCooldown = 10;

    public bool isDead;

    public Vector2Int __position;
    public Vector2Int pos => __position;

    //[HideInInspector] public bool playerDetected;
    [HideInInspector] public bool sleeping;
    [HideInInspector] public bool sleepDecided = false; //is it is true, we don't do this  sleeping = Random.Range(0, 101) <= 5 * DungeonGenerator.dungeonGenerator.currentFloor ? false : true;
    [HideInInspector] public bool attacked;

    [HideInInspector] public bool rooted;
    [HideInInspector] public int rootDuration;

    [HideInInspector] public bool isStuned;
    [HideInInspector] public int stuneDuration;

    [HideInInspector] public bool isInvisible;

    [HideInInspector] public event Action EffectTasks; //bleed, lose help because of poison etc
    public void TriggerEffectTasks() => EffectTasks?.Invoke();
    public int bleedLength;
    public bool isBleeding;

    public int totalDamage; //damage that will be dealed to player

    [HideInInspector] public PlayerStats playerStats;
    [HideInInspector] public GameManager manager;
    [HideInInspector] public GameObject canvas;

    // Who attacked you
    [NonSerialized] public List<IUnit> RetailiationList = new List<IUnit>();
    [NonSerialized] public IUnit CurrentTarget = null;
    [NonSerialized] public Vector2Int? LastKnownTargetPos = null;

    public List<Vector2Int> path;

    public int howLongWillFololwInvisiblepLayer = 8;
    [HideInInspector] public int _x;

    // This can be used to 
    [NonSerialized]public Dictionary<string, object> Board = new Dictionary<string, object>();

    public void Start()
    {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        canvas = GameObject.Find("MainCanvas").gameObject;
        
        //IF ENEMY IS BOSS MAKE HIM NOT SLEEPING
        if (enemySO.isBoss) sleeping = false;

        _x = 0;

        if(enemySO.finishedDialogue) enemySO.finishedDialogue = false;
        maxHp = __currentHp;
    }   

    public void SpawnSleep(int floorNum)
    {
        if (!sleepDecided)
        {
            sleeping = Random.Range(0, 101) <= 5 * floorNum ? false : true;
        }
    }

    public void MoveTo(int x, int y) 
    {
        try
        {
            Stun(0);
            if (CheckStun()) return;

            if(MapManager.map[__position.x, __position.y].type == "Cobweb")
            {
                if(Random.Range(1,100) <= 20 - (dex / 2))
                {
                    return;
                }
            }
            if (MapManager.map[x, y] != null && !MapManager.map[x, y].hasPlayer && MapManager.map[x,y].enemy == null && !rooted)
            {
                if(MapManager.map[x, y].isWalkable || MapManager.map[x, y].type == "Door")
                {
                    if (MapManager.map[x, y].type == "Door" && MapManager.map[x, y].structure is Door door)
                    {
                        if(door.opened)
                        {
                            if (isBleeding)
                            {
                                MapManager.map[__position.x, __position.y].letter = "";
                                MapManager.map[__position.x, __position.y].isWalkable = true;
                                MapManager.map[__position.x, __position.y].enemy = null;
                                MapManager.map[__position.x, __position.y].exploredColor = new Color(0.54f, 0.01f, 0.01f);
                                MapManager.map[__position.x, __position.y].timeColor = new Color(0, 0, 0);

                                MapManager.map[__position.x, __position.y].previousMonsterLetter = "";
                            }
                            else
                            {
                                MapManager.map[__position.x, __position.y].letter = "";
                                MapManager.map[__position.x, __position.y].isWalkable = true;
                                MapManager.map[__position.x, __position.y].enemy = null;
                                MapManager.map[__position.x, __position.y].timeColor = new Color(0, 0, 0);

                                MapManager.map[__position.x, __position.y].previousMonsterLetter = "";
                            }

                            __position = new Vector2Int(x, y);

                            if (!isInvisible) MapManager.map[x, y].letter = EnemySymbol;
                            MapManager.map[x, y].isWalkable = false;
                            MapManager.map[x, y].enemy = this.gameObject;
                            MapManager.map[x, y].timeColor = EnemyColor;

                            MapManager.map[__position.x, __position.y].previousMonsterLetter = EnemySymbol;
                            MapManager.map[__position.x, __position.y].previousMonsterColor = EnemyColor;

                            return;
                        }
                        else
                        {
                            if (enemySO.canOpenDoor) door.Use();
                            else {}
                        }
                    }   
                    else
                    {
                        if (isBleeding)
                        {
                            MapManager.map[__position.x, __position.y].letter = "";
                            MapManager.map[__position.x, __position.y].isWalkable = true;
                            MapManager.map[__position.x, __position.y].enemy = null;
                            MapManager.map[__position.x, __position.y].exploredColor = new Color(0.54f, 0.01f, 0.01f);
                            MapManager.map[__position.x, __position.y].timeColor = new Color(0, 0, 0);
                        }
                        else
                        {
                            MapManager.map[__position.x, __position.y].letter = "";
                            MapManager.map[__position.x, __position.y].isWalkable = true;
                            MapManager.map[__position.x, __position.y].enemy = null;
                            MapManager.map[__position.x, __position.y].timeColor = new Color(0, 0, 0);
                        }

                        __position = new Vector2Int(x, y);

                        if (!isInvisible) MapManager.map[x, y].letter = EnemySymbol;
                        MapManager.map[x, y].isWalkable = false;
                        MapManager.map[x, y].enemy = this.gameObject;
                        MapManager.map[x, y].timeColor = EnemyColor;

                        return;
                    }
                }
            }   
        } 
        catch{}

        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
    }
    public void TestToWakeUp()
    {
        enemySO.MyTestToWakeUp.Calculate(this);
        return;
    }    

    public void DoTurn()
    {
        enemySO.MyTurnAI.Calculate(this);
        RetailiationList.Clear();
    }
    public BasicAttack nextAttack = null;
    
    //=========================
    //ATTACKS
    //=========================
    
    public bool attackCharged = false;
        

    //=========================
    //+++++++++++++++++++++++++
    //=========================

    public void TakeDamage(int amount, ItemScriptableObject.damageType dmgType)
    {
        enemySO.MyTakeDamage.TakeDamage(this,amount, dmgType);
        return;
    }
    //called when enemy dies
    private void Kill()
    {
        enemySO.MyKill.Calculate(this);
        return;      
    }
    public void Bleed()
    {
        enemySO.MyBleed.Calculate(this);
        return;    
    }

    public bool CheckStun()
    {     
        if (stuneDuration <= 0)
        {
            stuneDuration = 0;
            isStuned = false;
            return false;
        }
        else
        {
            return true;
        }
    }
    public void Stun(int duration = 0)
    {
        enemySO.MyStun.StunFor(this,duration);
        return; 
    }
    
    public void MakeInvisible()
    {
        isInvisible = true;
    }
    public void RemoveInvisibility()
    {
        isInvisible = false;
    }

    //Poison Bolt
    public bool damageOverTurn;
    public int dotDuration;

    public void DamageOverTurn()
    {
        enemySO.MyDOT.Calculate(this);
        return;
    }
    public void WakeUp()
    {
        enemySO.MyWakeUp.Calculate(this);
        return;       
    }
}
