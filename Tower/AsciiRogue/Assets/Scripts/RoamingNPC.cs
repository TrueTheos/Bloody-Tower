using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using System;
using System.Collections;

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

        //IF ENEMY IS BOSS MAKE HIM NOT SLEEPING
        if (sleeping && enemySO._Difficulty == EnemiesScriptableObject.E_difficulty.boss) sleeping = false;

        _x = 0;

        if(enemySO.finishedDialogue) enemySO.finishedDialogue = false;
        maxHp = __currentHp;
    }   

    void MoveTo(int x, int y) 
    {
        try
        {
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
        catch{}

        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
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
                    if(__currentHp <= (maxHp / 2) && (int)Random.Range(0,2) == 1 && runCounter > 0 || (runCounter > 0 && runCounter < 5)) //RUN FROM PLAYER
                    {
                        runDirection = __position - MapManager.playerPos;

                        Vector2Int runCell = __position + runDirection;
                    
                        runCounter--;

                        path = null;

                        path = AStar.CalculatePath(__position, runCell);

                        MoveTo(path[0].x, path[0].y);
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
        try 
        { 
            GameManager.manager.StopCoroutine(GameManager.manager.waitingCoroutine);
            GameManager.manager.waitingCoroutine = null;
            GameManager.manager.waiting = false;
            GameManager.manager.readingBook = null;
        }
        catch { }

        EnemiesScriptableObject.E_Attacks attack = enemySO.attacks[Random.Range(0, enemySO.attacks.Count)];

        switch(attack)
        {
            case EnemiesScriptableObject.E_Attacks.normal:
                NormalAttack();
                break;
            case EnemiesScriptableObject.E_Attacks.poisonBite:
                PoisonBite();
                break;
            case EnemiesScriptableObject.E_Attacks.fadingBite:
                StartCoroutine(FadingBite());
                break;
        }
    }

    private void NormalAttack()
    {
        totalDamage = 0;

        int valueRequiredToHit = 0; //value required to hit the monster

        valueRequiredToHit = Random.Range(1, 100) + dex - playerStats.__dexterity - playerStats.armorClass;
        //manager.UpdateMessages($"<color=red>Value required to hit player: = d20 + {dex} - {playerStats.__dexterity} - {playerStats.armorClass} = {valueRequiredToHit}</color>");

        if (valueRequiredToHit > 50)
        {
            if (Random.Range(1, 100) < 10 - playerStats.armorClass + dex - playerStats.__dexterity)
            {
                totalDamage += Mathf.FloorToInt((Random.Range(1, 4) + Mathf.FloorToInt(str / 5)) * 1.5f);
            }
            else
            {
                totalDamage += Random.Range(1, 4) + Mathf.FloorToInt(str / 5);
            }


            manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(enemySO.E_color)}>{enemySO.name}</color> attacked you for <color=red>{totalDamage} ({playerStats.__currentHp - totalDamage}/{playerStats.__maxHp})</color>!");
            playerStats.TakeDamage(totalDamage);
        }
        else
        {
            manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(enemySO.E_color)}>{enemySO.E_name}</color> missed.");
        }

        canvas.GetComponent<Animator>().SetTrigger("Shake");
    }

    private void PoisonBite()
    {
        totalDamage = 0;

        int valueRequiredToHit = 0; //value required to hit the monster

        valueRequiredToHit = Random.Range(1, 100) + dex - playerStats.__dexterity - playerStats.armorClass;
        //manager.UpdateMessages($"<color=red>Value required to hit player: = d20 + {dex} - {playerStats.__dexterity} - {playerStats.armorClass} = {valueRequiredToHit}</color>");

        if (valueRequiredToHit > 50)
        {
            if (Random.Range(1, 100) < 10 - playerStats.armorClass + dex - playerStats.__dexterity)
            {
                totalDamage += Mathf.FloorToInt((Random.Range(1, 4) + Mathf.FloorToInt(str / 5)) * 1.5f);
            }
            else
            {
                totalDamage += Random.Range(1, 4) + Mathf.FloorToInt(str / 5);
            }

            if (!playerStats.isPoisoned)
            {
                playerStats.poisonDuration = 3;
                playerStats.Poison();
            }
            playerStats.TakeDamage(totalDamage);

            manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(enemySO.E_color)}>{enemySO.name}</color> used <color=green>Poison Bite</color>!");
            manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(enemySO.E_color)}>{enemySO.name}</color> attacked you for <color=red>{totalDamage}</color>!");
        }
        else
        {
            manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(enemySO.E_color)}>{enemySO.E_name}</color> missed.");
        }

        canvas.GetComponent<Animator>().SetTrigger("Shake");
    }

    private IEnumerator FadingBite()
    {
        manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(enemySO.E_color)}>{enemySO.name}</color> used <color=red>Fading Bite</color>!");
        NormalAttack();
        yield return new WaitForSeconds(.2f);

        //run away
        runDirection = __position - MapManager.playerPos;

        Vector2Int runCell = __position + runDirection;

        if(MapManager.map[runCell.x, runCell.y].type == "Wall")
        {
            if(Random.Range(0,100) > 78)
            {
                manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(enemySO.E_color)}>{enemySO.name}</color> used <color=lightblue>Jump</color>!");
                runCell = new Vector2Int(MapManager.playerPos.x + (MapManager.playerPos.x - __position.x), MapManager.playerPos.y + (MapManager.playerPos.y - __position.y));
                MoveTo(runCell.x, runCell.y);
            }
        }
        else
        {
            runCounter--;

            path = null;

            path = AStar.CalculatePath(__position, runCell);

            MoveTo(path[0].x, path[0].y);
        }

        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
        StopCoroutine(FadingBite());
    }
    
    public void TakeDamage(int amount)
    {
        WakeUp();

        __currentHp -= amount;

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

            bool droppedItem = false;

            if(Random.Range(1, 100) <= 19 && enemySO.E_possileDrops != null && enemySO.E_possileDrops.Count > 0)
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
                    MapManager.map[__position.x, __position.y].baseChar = enemySO.E_symbol;
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

        manager.StartPlayersTurn();
       
        manager.gameObject.GetComponent<Bestiary>().UpdateEnemyList(enemySO);

        manager.enemies.RemoveAt(Array.IndexOf(manager.enemies.ToArray(), e));

        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);

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
