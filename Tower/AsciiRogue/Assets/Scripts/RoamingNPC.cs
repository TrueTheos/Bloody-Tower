using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using System;

public class RoamingNPC : MonoBehaviour, ITakeDamage, IBleeding
{
    public EnemiesScriptableObject enemySO;


    //Statistics
    public int __currentHp;
    private int maxHp;

    public int str;
    public int dex;
    public int intell;
    public int end;
    public int lvl;
    public int xpDrop;

    public int AC;


    //Variables for Cowardly enemies
    private int runCounter = 5;
    private Vector2Int runDirection;

    //Variables for Recover enemies
    private int hpRegenCooldown = 10;


    public Vector2Int __position;

    private GameObject canvas;

    [HideInInspector] public bool playerDetected;
    [HideInInspector] public bool sleeping;
    [HideInInspector] public bool attacked;

    [HideInInspector] public bool rooted;
    [HideInInspector] public int rootDuration;

    [HideInInspector] public event Action EffectTasks; //bleed, lose help because of poison etc
    private int bleedLength;
    private bool isBleeding;

    private int totalDamage; //damage that will be dealed to player

    private PlayerStats playerStats;
    private GameManager manager;

    private List<Vector2Int> path;

    public int howLongWillFololwInvisiblepLayer = 10;
    [HideInInspector] public int _x;


    public void Start()
    {
        playerStats = GameObject.Find("Player").GetComponent<PlayerStats>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        canvas = GameObject.Find("MainCanvas").gameObject;

        if(!sleeping)
        {
            sleeping = Random.Range(0, 101) <= 5 * DungeonGenerator.dungeonGenerator.currentFloor ? false : true;
        }

        _x = 0;

        if(enemySO.finishedDialogue) enemySO.finishedDialogue = false;
        maxHp = __currentHp;
    }   

    void MoveTo(int x, int y) 
    {
        if (MapManager.map[x, y] != null && !MapManager.map[x, y].hasPlayer && MapManager.map[x,y].enemy == null && !rooted)
        {
            if(MapManager.map[x, y].isWalkable || MapManager.map[x, y].type == "Door")
            {
                if (isBleeding)
                {
                    MapManager.map[__position.x, __position.y].letter = "";
                    MapManager.map[__position.x, __position.y].isWalkable = true;
                    MapManager.map[__position.x, __position.y].enemy = null;
                    MapManager.map[__position.x, __position.y].exploredColor = new Color(0.54f,0.01f,0.01f);
                    MapManager.map[__position.x, __position.y].timeColor = new Color(0, 0, 0);
                }
                else
                {
                    MapManager.map[__position.x, __position.y].letter = "";
                    MapManager.map[__position.x, __position.y].isWalkable = true;
                    MapManager.map[__position.x, __position.y].enemy = null;
                    MapManager.map[__position.x, __position.y].timeColor = new Color(0, 0, 0);
                    /*if(MapManager.map[__position.x, __position.y].exploredColor == "")
                    {
                        MapManager.map[__position.x, __position.y].exploredColor = "";
                    }     */
                }

                __position = new Vector2Int(x, y);

                MapManager.map[x, y].letter = enemySO.E_symbol;
                MapManager.map[x, y].isWalkable = false;
                MapManager.map[x, y].enemy = this.gameObject;
                MapManager.map[x, y].timeColor = enemySO.E_color;

                return;
            }
        }      
    }
    
    public void TestToWakeUp()
    {
        int dist = Mathf.Max(Mathf.Abs(MapManager.playerPos.x - __position.x), Mathf.Abs(MapManager.playerPos.y - __position.y));
        if((Random.Range(1,20) + lvl - playerStats.__dexterity - dist * 10) > 0)
        {
            WakeUp();
        }
    }

    public void LookForPlayer()
    {
        if(rootDuration > 0) rootDuration--;
        else if(rooted) rooted = false;

        if(enemySO._Behaviour != EnemiesScriptableObject.E_behaviour.npc)
        {
            if (sleeping) return;

            if (!gameObject.transform.parent.gameObject.activeSelf) return; 

            EffectTasks?.Invoke();      

            foreach (Vector2Int pos in FoV.GetEnemyFoV(__position))
            {
                try
                {
                    if (MapManager.map[pos.x, pos.y].hasPlayer && !playerStats.isInvisible)
                    {
                        playerDetected = true;
                    }
                }
                catch{}
            }

            switch(enemySO._Behaviour)
            {
                case EnemiesScriptableObject.E_behaviour.cowardly:
                    if(__currentHp <= (maxHp / 2) && (int)Random.Range(0,2) == 1 && runCounter > 0 || (runCounter > 0 && runCounter < 5))
                    {
                        runDirection = __position - MapManager.playerPos;

                        Vector2Int runCell = __position + runDirection;
                    
                        runCounter--;

                        MoveTo(runCell.x, runCell.y);
                        return;
                    }
                    else runCounter = 5;
                    break;
                case EnemiesScriptableObject.E_behaviour.recovers:
                    if(__currentHp <= (maxHp / 2) && hpRegenCooldown == 0)
                    {
                        hpRegenCooldown--;
                        __currentHp += Mathf.FloorToInt(maxHp * .25f);
                        return;
                    }
                    else if(hpRegenCooldown < 10 && hpRegenCooldown > 0) hpRegenCooldown--;
                    break;
            }           

            if(playerDetected) 
            {
                if (new Vector2Int(__position.x - 1, __position.y) == MapManager.playerPos)
                {
                    Attack();
                    return;
                }
                else if (new Vector2Int(__position.x + 1, __position.y) == MapManager.playerPos)
                {
                    Attack();
                    return;
                }
                else if (new Vector2Int(__position.x, __position.y - 1) == MapManager.playerPos)
                {
                   Attack();
                   return;
                }
                else if (new Vector2Int(__position.x, __position.y + 1) == MapManager.playerPos)
                {
                     Attack();
                    return;
                }
                else if (new Vector2Int(__position.x - 1, __position.y - 1) == MapManager.playerPos)
                {
                    Attack();
                    return;
                }
                else if (new Vector2Int(__position.x + 1, __position.y - 1) == MapManager.playerPos)
                {
                    Attack();
                    return;
                }
                else if (new Vector2Int(__position.x - 1, __position.y + 1) == MapManager.playerPos)
                {
                    Attack();
                    return;
                }
                else if (new Vector2Int(__position.x + 1, __position.y + 1) == MapManager.playerPos)
                {
                    Attack();
                    return;
                }

                path = null;

                path = AStar.CalculatePath(__position, MapManager.playerPos);

                MoveTo(path[0].x, path[0].y);
            }
            else
            {
                MoveTo(__position.x + Random.Range(-1, 2), __position.y + Random.Range(-1, 2)); //move to random direction
            }  
        }  
        else
        {
            if (!gameObject.transform.parent.gameObject.activeSelf) return; 

            if(enemySO.finishedDialogue)
            {
                MoveTo(__position.x + (int)Random.Range(-1, 2), __position.y + (int)Random.Range(-1, 2)); //move to random direction
            }
        }
    }

    public void Attack()
    {       
        totalDamage = 0;

        int valueRequiredToHit = 0; //value required to hit the monster

        valueRequiredToHit = Random.Range(1,100) + dex - playerStats.__dexterity - playerStats.armorClass;
        //manager.UpdateMessages($"<color=red>Value required to hit player: = d20 + {dex} - {playerStats.__dexterity} - {playerStats.armorClass} = {valueRequiredToHit}</color>");

        if(valueRequiredToHit > 50)
        {
            if(Random.Range(1,100) < 10 - playerStats.armorClass + dex - playerStats.__dexterity)
            {
                totalDamage += Mathf.FloorToInt((Random.Range(1,4) + Mathf.FloorToInt(str / 5)) * 1.5f);
            }
            else
            {
                totalDamage += Random.Range(1,4) + Mathf.FloorToInt(str / 5);
            }

            
            manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(enemySO.E_color)}>{enemySO.name}</color> attacked you for <color=red>{totalDamage}</color>!");
            playerStats.TakeDamage(totalDamage);
        }
        else
        {
            manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(enemySO.E_color)}>{enemySO.E_name}</color> missed.");
        }

        /*if (Random.Range(1, 200) < 5 && !playerStats.damagedEyes)
            {
                playerStats.damagedEyes = true; 
                playerStats.viewRange = 4;

                foreach (var item in MapManager.map)
                {
                    item.isVisible = false;
                }

                manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(enemySO.E_color)}>{enemySO.name}</color> hit you in the eye.");
            }
        }*/
        canvas.GetComponent<Animator>().SetTrigger("Shake");
    }

    public void TakeDamage(int amount)
    {
        WakeUp();

        __currentHp -= amount;

        string str = amount.ToString();

        MapManager.map[__position.x, __position.y].decoy = $"<color=white>{str[0].ToString()}</color>";
        try {MapManager.map[__position.x + 1, __position.y].decoy = $"<color=white>{str[1].ToString()}</color>";}
        catch{}

        canvas.GetComponent<Animator>().SetTrigger("Shake");
        if (__currentHp <= 0)
        {
            Kill();
        }
    }

    //called when enemy dies
    private void Kill()
    {
        if (enemySO.leavesCorpse)
        {
            MapManager.map[__position.x, __position.y].enemy = null;
            MapManager.map[__position.x, __position.y].isWalkable = true;
            Corps corpse = new Corps();
            
            //ITEM IN CORPSE

            /*if(Random.Range(1, 100) <= 16 && enemySO.E_itemDroppedAfterBeingKilled)
            {
                corpse.itemInCorpse = enemySO.E_itemDroppedAfterBeingKilled;
            }*/

            if(MapManager.map[__position.x, __position.y].structure == null)
            {
                corpse.enemyBody = enemySO;

                MapManager.map[__position.x, __position.y].structure = corpse;
                MapManager.map[__position.x, __position.y].baseChar = enemySO.E_symbol;
                MapManager.map[__position.x, __position.y].exploredColor = new Color(0.2784f, 0, 0);
                MapManager.map[__position.x, __position.y].letter = "";

                DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
            }           
        }
        else
        {
            MapManager.map[__position.x, __position.y].enemy = null;
            MapManager.map[__position.x, __position.y].letter = "";
            MapManager.map[__position.x, __position.y].isWalkable = true;
            //MapManager.map[__position.x, __position.y].exploredColor = "";
        }     

        manager.UpdateMessages($"You have killed the <color={enemySO.E_color}>{enemySO.name}</color>");
        manager.playerStats.UpdateLevel(xpDrop);

        GameObject e = null;

        /*foreach (var enemy in manager.enemies)
        {
            if(enemy.GetComponent<RoamingNPC>().__position == __position)
            {
                e = enemy;
            }
        }*/

        foreach (var enemy in manager.enemies)
        {
            if(enemy == this.gameObject)
            {
                e = enemy;
            }
        }

        /*try
        {
            e = manager.enemies.Where(obj => obj.GetComponent<RoamingNPC>().__position == this.__position).FirstOrDefault(); //SingleOrDefault()   
        }
        catch
        {
            e = manager.enemies.Where(obj => obj.GetComponent<RoamingNPC>().__position == this.__position).SingleOrDefault();
        }*/

        /*if(Random.Range(1, 100) < 5)
        {
            manager.UpdateMessages($"The dying <color={enemySO.E_color}>{enemySO.E_name}</color> gave the last cry that awakened the nearby monsters.");

            for (int x = __position.x - 7; x < __position.x + 7; x++)
            {
                for (int y = __position.y - 7; y < __position.y + 7; y++)
                {

                    try
                    {
                        if (MapManager.map[x, y].enemy != null)
                        {
                            MapManager.map[x, y].enemy.GetComponent<RoamingNPC>().sleeping = false;
                            MapManager.map[x, y].enemy.GetComponent<RoamingNPC>().moveToCorpseTimer = 10;
                            MapManager.map[x, y].enemy.GetComponent<RoamingNPC>().target = __position;
                        }
                    }
                    catch { }
                    
                }
            }

        }*/
       
        manager.gameObject.GetComponent<Bestiary>().UpdateEnemyList(enemySO);

        manager.enemies.RemoveAt(Array.IndexOf(manager.enemies.ToArray(), e));

        Destroy(gameObject);
    }

    public void Bleed()
    {
        if (!isBleeding)
        {
            isBleeding = true;

            EffectTasks += Bleed;

            //bleedLength = Random.Range(manager.bleedDuration.x, manager.bleedDuration.y);

            manager.UpdateMessages($"The <color={enemySO.E_color}>{enemySO.E_name}</color> starts bleeding.");
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
        }        
    }

    //Poison Bolt

    public bool damageOverTurn;
    public int dotDuration;

    public void DamageOverTurn()
    {
        if(!damageOverTurn)
        {
            damageOverTurn = true;
            EffectTasks += DamageOverTurn;
            dotDuration = 10;
            WakeUp();
        }
        else
        {
            dotDuration--;
            TakeDamage((40 + playerStats.__intelligence) / 10);
        }

        if(dotDuration <= 0)
        {
            damageOverTurn = false;
            EffectTasks -= DamageOverTurn;
        }
    }

    public void WakeUp()
    {
        if (sleeping) //wake up enemy
        {
            sleeping = false;
            manager.UpdateMessages($"You woke up the <color={enemySO.E_color}>{enemySO.E_name}</color>!");          
        }

        attacked = true;
        _x = howLongWillFololwInvisiblepLayer;
    }
}
