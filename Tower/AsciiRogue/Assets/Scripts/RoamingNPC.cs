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

    public int howLongWillFololwInvisiblepLayer = 10;
    [HideInInspector] public int _x;
    // This can be used to 
    [NonSerialized]public Dictionary<string, object> Board = new Dictionary<string, object>();

    public void Start()
    {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        canvas = GameObject.Find("MainCanvas").gameObject;

        if(!sleepDecided)
        {
            sleeping = Random.Range(0, 101) <= 5 * DungeonGenerator.dungeonGenerator.currentFloor ? false : true;
        }

        //IF ENEMY IS BOSS MAKE HIM NOT SLEEPING
        if (enemySO.isBoss) sleeping = false;

        _x = 0;

        if(enemySO.finishedDialogue) enemySO.finishedDialogue = false;
        maxHp = __currentHp;
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
                        else
                        {
                            door.Use();
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
        int dist = Mathf.Max(Mathf.Abs(MapManager.playerPos.x - __position.x), Mathf.Abs(MapManager.playerPos.y - __position.y));
        if((Random.Range(1,20) + lvl - playerStats.__dexterity - dist * 10) > 0)
        {
            WakeUp();
        }
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
        /*WakeUp();

        __currentHp -= amount;

        canvas.GetComponent<Animator>().SetTrigger("Shake");
        if (__currentHp <= 0)
        {
            Kill();
        }*/
    }
    //called when enemy dies
    private void Kill()
    {
        enemySO.MyKill.Calculate(this);
        return;
        if (enemySO.leavesCorpse)
        {
            MapManager.map[__position.x, __position.y].enemy = null;
            MapManager.map[__position.x, __position.y].isWalkable = true;
            Corps corpse = new Corps();

            //ITEM IN CORPSE

            bool droppedItem = false;


            //CHANCE TO DROP CORPSE ITEM
            if(Random.Range(1, 100) <= 100 && enemySO.E_possileDrops != null && enemySO.E_possileDrops.Count > 0)
            {
                corpse.itemInCorpse = enemySO.E_possileDrops[Random.Range(0, enemySO.E_possileDrops.Count)];
                droppedItem = true;
            }

            if(MapManager.map[__position.x, __position.y].structure == null)
            {
                if (droppedItem)
                {
                    MapManager.map[__position.x, __position.y].timeColor = new Color(0, 0, 0);
                    MapManager.map[__position.x, __position.y].letter = "";
                    GameManager.manager.itemSpawner.SpawnAt(__position.x, __position.y, corpse.itemInCorpse);
                }
                else
                { 
                    MapManager.map[__position.x, __position.y].baseChar = EnemySymbol;
                    MapManager.map[__position.x, __position.y].exploredColor = new Color(0.2784f, 0, 0);
                    MapManager.map[__position.x, __position.y].letter = "";
                }
            }           
        }
        else
        {
            MapManager.map[__position.x, __position.y].enemy = null;
            MapManager.map[__position.x, __position.y].letter = "";
            MapManager.map[__position.x, __position.y].isWalkable = true;
        }     

        manager.UpdateMessages($"You have killed the <color={EnemyColor}>{EnemyName}</color>");
        manager.playerStats.UpdateLevel(xpDrop);

        GameObject e = null;

        foreach (var enemy in manager.enemies)
        {
            if(enemy == this.gameObject)
            {
                e = enemy;
            }
        }

        manager.StartPlayersTurn();
       
        manager.gameObject.GetComponent<Bestiary>().UpdateEnemyList(enemySO);

        manager.enemies.RemoveAt(Array.IndexOf(manager.enemies.ToArray(), e));

        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);

        Destroy(gameObject);
    }
    public void Bleed()
    {
        enemySO.MyBleed.Calculate(this);
        return;
        /*if (!isBleeding)
        {
            isBleeding = true;

            EffectTasks += Bleed;

            //bleedLength = Random.Range(manager.bleedDuration.x, manager.bleedDuration.y);

            manager.UpdateMessages($"The <color={EnemyColor}>{EnemyName}</color> starts bleeding.");
        }

        if (bleedLength <= 0)
        {
            isBleeding = false;

            EffectTasks -= Bleed;

            return;
        }

        if (bleedLength > 0)
        {
            TakeDamage(1);
            bleedLength--;
        } */       
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
        /*if (isStuned)
        {
            stuneDuration--;
        }

        if (duration > 0 && duration > stuneDuration)
        {
            stuneDuration = duration;
            isStuned = true;
        }

        if (stuneDuration <= 0)
        {
            stuneDuration = 0;
            isStuned = false;
        }*/
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
        /*if(!damageOverTurn)
        {
            damageOverTurn = true;
            EffectTasks += DamageOverTurn;
            if (dotDuration != 0) { }
            else dotDuration = 10;
            WakeUp();
        }
        else
        {
            dotDuration--;
            TakeDamage((20 + playerStats.__intelligence) / 10);
        }

        if(dotDuration <= 0)
        {
            damageOverTurn = false;
            EffectTasks -= DamageOverTurn;
        }*/
    }
    public void WakeUp()
    {
        enemySO.MyWakeUp.Calculate(this);
        return;
        if (sleeping) //wake up enemy
        {
            sleeping = false;

            if (enemySO.E_realName != string.Empty)
            {
                MapManager.map[__position.x, __position.y].timeColor = EnemyColor;
                MapManager.map[__position.x, __position.y].letter = EnemySymbol;
            }
            manager.UpdateMessages($"You woke up the <color={EnemyColor}>{EnemyName}</color>!");          
        }

        attacked = true;
        _x = howLongWillFololwInvisiblepLayer;
    }
}
