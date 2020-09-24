using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector2Int position;
    private Vector2Int target;

    public static PlayerMovement playerMovement;

    GameManager manager;
    private PlayerStats playerStats;

    [SerializeField] public bool canMove = true;

    private int damage; //damage that will be dealed to enemy;  
    public readonly float sleepingDamage = 1.3f; 

    [HideInInspector] public Coroutine cor;

    public Vector2Int waterPoisonDuraiton;

    public float MoveCooldown = 0;
    public float MoveDelay = 0.16f;


    private void Start()
    {
        playerMovement = this;
        playerStats = GetComponent<PlayerStats>();
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        MoveCooldown = 0;
    }

    private void Update()
    {
        // advance time
        MoveCooldown = Mathf.Max(MoveCooldown - Time.deltaTime, 0);

        if (manager.isPlayerTurn && canMove)
        {
            if (MoveCooldown==0)
            {
                Vector2Int move = new Vector2Int();

                if (Input.GetKeyDown(KeyCode.Keypad8) || Input.GetKey(KeyCode.Keypad8))
                {
                    move.y = 1;
                }
                if (Input.GetKeyDown(KeyCode.Keypad2) || Input.GetKey(KeyCode.Keypad2))
                {
                    move.y=-1;
                }
                if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKey(KeyCode.Keypad4))
                {
                    move.x = -1;
                }
                if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKey(KeyCode.Keypad6))
                {
                    move.x = 1;
                }
                if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKey(KeyCode.Keypad7))
                {
                    move.x = -1;
                    move.y = 1;
                }
                if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKey(KeyCode.Keypad9))
                {
                    move.x = 1;
                    move.y = 1;
                }
                if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKey(KeyCode.Keypad3))
                {
                    move.x = 1;
                    move.y = -1;
                }
                if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKey(KeyCode.Keypad1))
                {
                    move.x = -1;
                    move.y = -1;
                }

                if (move.sqrMagnitude > 0)
                {
                    Move(InputToVector(move.x,move.y));
                    MoveCooldown = MoveDelay;
                }
            }
            
            
            Selector.Current.SelectedTile(position.x, position.y);

            if (Input.GetButtonUp("Use"))
            {
                if(MapManager.map[position.x, position.y].structure != null)
                {
                    MapManager.map[position.x, position.y].structure.Use();

                    //FOVNEW.fv.Initialize(FOVNEW.fv.CanLightPass, FOVNEW.fv.SetToVisible, FOVNEW.fv.Distance);
                    FoV.Initialize();
                    //manager.UpdateVisibility();
                    manager.StartPlayersTurn();
                }
                else if(MapManager.map[position.x, position.y].item != null)
                {
                    if (playerStats.maximumInventorySpace > playerStats.currentItems && playerStats.currentWeight + MapManager.map[position.x, position.y].item.GetComponent<Item>().iso.I_weight <= playerStats.maxWeight)
                    {
                        playerStats.currentWeight += MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_weight;
                        playerStats.Pickup(MapManager.map[target.x, target.y].item, MapManager.map[target.x, target.y].item.GetComponent<Item>().iso, target);
                        MapManager.map[position.x, position.y].letter = "";
                        MapManager.map[target.x, target.y].baseChar = ".";
                        MapManager.map[target.x, target.y].exploredColor = new Color(1, 1, 1);
                        MapManager.map[position.x, position.y].hasPlayer = false;
                        MapManager.map[position.x, position.y].timeColor = new Color(0, 0, 0);
                        position = target;
                        MapManager.map[position.x, position.y].hasPlayer = true;
                        MapManager.map[position.x, position.y].baseChar = ".";
                        MapManager.map[position.x, position.y].timeColor = new Color(0.5f, 1, 0);
                        MapManager.map[position.x, position.y].letter = "@";
                        MapManager.playerPos = new Vector2Int(position.x, position.y);
                    }
                    else if (playerStats.maximumInventorySpace < playerStats.currentItems)
                    {
                        manager.UpdateMessages("Your backpack can't hold any more items!");
                        MapManager.map[position.x, position.y].letter = "";
                        MapManager.map[position.x, position.y].hasPlayer = false;
                        MapManager.map[position.x, position.y].timeColor = new Color(0, 0, 0);
                        position = target;

                        MapManager.map[position.x, position.y].hasPlayer = true;
                        MapManager.map[position.x, position.y].timeColor = new Color(0.5f, 1, 0);
                        MapManager.map[position.x, position.y].letter = "@";
                        MapManager.playerPos = new Vector2Int(position.x, position.y);
                    }
                    else if (playerStats.currentWeight + MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_weight > playerStats.maxWeight)
                    {
                        if (!MapManager.map[target.x, target.y].item.GetComponent<Item>().identified)
                        {
                            manager.UpdateMessages($"The {MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_unInName} is too heavy. It weighs {MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_weight}");
                        }
                        else
                        {
                            manager.UpdateMessages($"The {MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_name} is too heavy. It weighs {MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_weight}");
                        }
                        MapManager.map[position.x, position.y].letter = "";
                        MapManager.map[position.x, position.y].hasPlayer = false;
                        MapManager.map[position.x, position.y].timeColor = new Color(0, 0, 0);
                        position = target;

                        MapManager.map[position.x, position.y].hasPlayer = true;
                        MapManager.map[position.x, position.y].timeColor = new Color(0.5f, 1, 0);
                        MapManager.map[position.x, position.y].letter = "@";
                        MapManager.playerPos = new Vector2Int(position.x, position.y);
                    }
                }
            }
        }
    }

    Vector2Int InputToVector(int x, int y)
    {
        Vector2Int target = new Vector2Int(position.x + x, position.y + y);

        return target;
    }

    IEnumerator MoveTR()
    {
        while(true)
        {           
            Move(InputToVector(1, 1));
            yield return new WaitForSeconds(0.16f);
        }
    }

    IEnumerator MoveT()
    {
        while (true)
        {
            Move(InputToVector(0, 1));
            yield return new WaitForSeconds(0.16f);
        }
    }

    IEnumerator MoveTL()
    {
        while (true)
        {
            Move(InputToVector(-1, 1));
            yield return new WaitForSeconds(0.16f);
        }
    }

    IEnumerator MoveL()
    {
        while (true)
        {
            Move(InputToVector(-1, 0));
            yield return new WaitForSeconds(0.16f);
        }
    }

    IEnumerator MoveBL()
    {
        while (true)
        {
            Move(InputToVector(-1, -1));
            yield return new WaitForSeconds(0.16f);
        }
    }

    IEnumerator MoveB()
    {
        while (true)
        {
            Move(InputToVector(0, -1));
            yield return new WaitForSeconds(0.16f);
        }
    }

    IEnumerator MoveBR()
    {
        while (true)
        {
            Move(InputToVector(1, -1));
            yield return new WaitForSeconds(0.16f);
        }
    }

    IEnumerator MoveR()
    {
        while (true)
        {
            Move(InputToVector(1, 0));
            yield return new WaitForSeconds(0.16f);
        }
    }

    public void Move(Vector2Int _target)
    {
        target = _target;

        for (int y = position.y - playerStats.__noise; y < position.y + playerStats.__noise; y++)
        {
            for (int x = position.x - playerStats.__noise; x < position.x + playerStats.__noise; x++)
            {
                try
                {
                    if (MapManager.map[x, y].enemy != null)
                    {
                        MapManager.map[x, y].enemy.GetComponent<RoamingNPC>().TestToWakeUp();
                    }
                }
                catch { }

            }
        }

        if (MapManager.map[position.x, position.y].type == "Cobweb")
        {
            if(Random.Range(1,100) <= 20 - (playerStats.__dexterity / 2))
            {
                //dont move
                manager.UpdateMessages("You are stuck in the cobweb.");
                manager.FinishPlayersTurn();
                return;              
            }
        }

        if(MapManager.map[target.x, target.y].item != null)
        {
            GameManager.manager.UpdateMessages("Press <color=yellow>'space'</color> to pick up item.");
        }
        
        if (MapManager.map[target.x, target.y].isWalkable && MapManager.map[target.x, target.y].enemy == null) // && MapManager.map[target.x, target.y].item == null
        {
            if(MapManager.map[target.x, target.y].structure != null)
            {
                MapManager.map[target.x, target.y].structure.WalkIntoTrigger();
            }

            MapManager.map[position.x, position.y].hasPlayer = false;
            MapManager.map[position.x, position.y].letter = "";
            MapManager.map[position.x, position.y].timeColor = new Color(0, 0, 0);
            position = target;
            MapManager.map[position.x, position.y].hasPlayer = true;
            MapManager.map[position.x, position.y].timeColor = new Color(0.5f, 1, 0);
            MapManager.map[position.x, position.y].letter = "@";
            MapManager.playerPos = new Vector2Int(position.x, position.y);
        }
        else if(MapManager.map[target.x, target.y].structure != null && !MapManager.map[target.x, target.y].isWalkable)
        {
            MapManager.map[target.x, target.y].structure.Use();
            target = position;
        }
        else if (MapManager.map[target.x, target.y].enemy != null)
        {
            if(MapManager.map[target.x, target.y].enemy.GetComponent<RoamingNPC>().enemySO._Behaviour == EnemiesScriptableObject.E_behaviour.npc && !MapManager.map[target.x, target.y].enemy.GetComponent<RoamingNPC>().enemySO.finishedDialogue)
            {
                playerStats.dialogue = true;
                playerStats.npcDialogue = MapManager.map[target.x, target.y].enemy.GetComponent<RoamingNPC>();
                return;
            }
            else Attack(MapManager.map[target.x, target.y].enemy, target.x, target.y);
        }
        else if (MapManager.map[target.x, target.y].type == "Door") //if in the direction we want to go is door
        {
            /*targetDoor = target;
            touchedDoor = true;*/
            if(MapManager.map[target.x, target.y].requiresKey)
            {
                bool lb = false;
                foreach(Item item in playerStats.itemsInEqGO)
                {
                    if(lb) break;

                    if(item.iso is Key key)
                    {
                        lb = true;

                        manager.UpdateMessages($"You use <color={key.I_color}>{key.I_name}</color> to open the door.");

                        MapManager.map[position.x, position.y].hasPlayer = false;
                        MapManager.map[position.x, position.y].letter = "";
                        MapManager.map[position.x, position.y].timeColor = new Color(0, 0, 0);
                        position = target;
                        MapManager.map[position.x, position.y].hasPlayer = true;
                        MapManager.map[position.x, position.y].timeColor = new Color(0.5f, 1, 0);
                        MapManager.map[position.x, position.y].letter = "@";
                        MapManager.playerPos = new Vector2Int(position.x, position.y);

                        manager.FinishPlayersTurn();

                        return;
                    }                   
                }

                manager.UpdateMessages("You don't have any fitting key.");
            }
            else
            {
                MapManager.map[position.x, position.y].hasPlayer = false;
                MapManager.map[position.x, position.y].letter = "";
                MapManager.map[position.x, position.y].timeColor = new Color(0, 0, 0);
                position = target;
                MapManager.map[position.x, position.y].hasPlayer = true;
                MapManager.map[position.x, position.y].timeColor = new Color(0.5f, 1, 0);
                MapManager.map[position.x, position.y].letter = "@";
                MapManager.playerPos = new Vector2Int(position.x, position.y);
            }
        }
        /*else if (MapManager.map[target.x, target.y].item != null)
        {
            //to do: add key to pickup item
            if (playerStats.maximumInventorySpace > playerStats.currentItems && playerStats.currentWeight + MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_weight <= playerStats.maxWeight)
            {
                playerStats.currentWeight += MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_weight;
                playerStats.Pickup(MapManager.map[target.x, target.y].item, MapManager.map[target.x, target.y].item.GetComponent<Item>().iso, target);
                MapManager.map[position.x, position.y].letter = "";
                MapManager.map[target.x, target.y].baseChar = ".";
                MapManager.map[target.x, target.y].exploredColor = new Color(1,1,1);
                MapManager.map[position.x, position.y].hasPlayer = false;
                MapManager.map[position.x, position.y].timeColor = new Color(0, 0, 0);
                position = target;
                MapManager.map[position.x, position.y].hasPlayer = true;
                MapManager.map[position.x, position.y].baseChar = ".";
                //MapManager.map[position.x, position.y].exploredColor = new Color(0, 0, 0);
                MapManager.map[position.x, position.y].timeColor = new Color(0.5f, 1, 0);
                MapManager.map[position.x, position.y].letter = "@";
                MapManager.playerPos = new Vector2Int(position.x, position.y);
            }
            else if (playerStats.maximumInventorySpace < playerStats.currentItems)
            {
                manager.UpdateMessages("Your backpack can't hold any more items!");
                MapManager.map[position.x, position.y].letter = "";
                MapManager.map[position.x, position.y].hasPlayer = false;
                MapManager.map[position.x, position.y].timeColor = new Color(0, 0, 0);
                position = target;

                MapManager.map[position.x, position.y].hasPlayer = true;
                MapManager.map[position.x, position.y].timeColor = new Color(0.5f, 1, 0);
                MapManager.map[position.x, position.y].letter = "@";
                MapManager.playerPos = new Vector2Int(position.x, position.y);
            }
            else if (playerStats.currentWeight + MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_weight > playerStats.maxWeight)
            {
                if(!MapManager.map[target.x, target.y].item.GetComponent<Item>().identified)
                {
                    manager.UpdateMessages($"The {MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_unInName} is too heavy. It weighs {MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_weight}");
                }
                else
                {
                    manager.UpdateMessages($"The {MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_name} is too heavy. It weighs {MapManager.map[target.x, target.y].item.GetComponent<Item>().iso.I_weight}");
                }
                MapManager.map[position.x, position.y].letter = "";
                MapManager.map[position.x, position.y].hasPlayer = false;
                MapManager.map[position.x, position.y].timeColor = new Color(0, 0, 0);
                position = target;

                MapManager.map[position.x, position.y].hasPlayer = true;
                MapManager.map[position.x, position.y].timeColor = new Color(0.5f, 1, 0);
                MapManager.map[position.x, position.y].letter = "@";
                MapManager.playerPos = new Vector2Int(position.x, position.y);
            }
        }*/

        manager.FinishPlayersTurn();
    }

    private void Attack(GameObject e, int x, int y) //float damageMultiplier, WeaponsSO.additionalEffects effect
    {        
        for(int y2 = position.y - 2; y2 < position.y + 2; y2++)
        {
            for(int x2 = position.x - 2; x2 < position.x + 2; x2++)
            {
                try
                {
                    if(MapManager.map[x2,y2].enemy != null)
                    {
                        MapManager.map[x2,y2].enemy.GetComponent<RoamingNPC>().TestToWakeUp();
                    }
                }
                catch{}
                
            }  
        }

        if(playerStats._Lhand?.iso is WeaponsSO w)
        {
            w.onHit(playerStats);
        }
        if(playerStats._Rhand?.iso is WeaponsSO w2)
        {
            w2.onHit(playerStats);
        }

        if (playerStats.isInvisible) 
        {
            playerStats.RemoveInvisibility();
        }     

        RoamingNPC roamingNpcScript = e.GetComponent<RoamingNPC>();

        damage = 0; //total damage

        int r = Random.Range(1,100);

        int valueRequiredToHit = 0; //value required to hit the monster

        if(roamingNpcScript.sleeping)
        {
            valueRequiredToHit = r + playerStats.__dexterity - roamingNpcScript.dex / 4 - roamingNpcScript.AC;         
        }
        else
        {
            valueRequiredToHit = r + playerStats.__dexterity - roamingNpcScript.dex - roamingNpcScript.AC;       
        }

        if(r <= 0)
        {
            manager.UpdateMessages("You missed!");
            WakeUpEnemy(roamingNpcScript);
            return;
        }

        if(valueRequiredToHit > 50 || r >= 80) //Do we hit?
        {
            if(playerStats._Lhand?.iso is WeaponsSO weaponL)
            {
                for(x = 0; x < weaponL.attacks.Count; x++)
                {
                    damage += Random.Range(1, weaponL.attacks[x].y);

                    //IF WEAPON IS BLOOD SWORD WE INCREASE ITS DAMAGE
                    if(weaponL.I_name == "Bloodsword") 
                    {
                        weaponL.attacks[0] = new Vector2Int(1, weaponL.attacks[0].y + 1);
                        weaponL.bloodSwordCounter = manager.tasks.bloodSwordCooldown;
                    }
                }
            }
            if(playerStats._Rhand?.iso is WeaponsSO weaponR)
            {
                for(x = 0; x < weaponR.attacks.Count; x++)
                {
                    damage += Random.Range(1, weaponR.attacks[x].y);

                    //IF WEAPON IS BLOOD SWORD WE INCREASE ITS DAMAGE
                    if(weaponR.I_name == "Bloodsword") 
                    {
                        weaponR.attacks[0] = new Vector2Int(1, weaponR.attacks[0].y + 1); 
                        weaponR.bloodSwordCounter = manager.tasks.bloodSwordCooldown;
                    }
                }
            }

            //CRIT?
            if(Random.Range(1,100) < 10 - roamingNpcScript.AC + roamingNpcScript.dex -  playerStats.__dexterity)
            {
                //manager.UpdateMessages($"<color=green>We crit, chance = 5 + {roamingNpcScript.dex} - {playerStats.__dexterity}</color>");
                damage += Mathf.FloorToInt((Random.Range(1,4) + Mathf.FloorToInt(playerStats.__strength / 5)) * 1.5f);
                //manager.UpdateMessages($"<color=green>You attacked for {damage} (d4 + ({playerStats.__strength} / 5) * 1.5)</color>");

            }
            else
            {
                damage += Random.Range(1,4) + Mathf.FloorToInt(playerStats.__strength / 5);
                //manager.UpdateMessages($"<color=green>You attacked for {damage} (d4 + {playerStats.__strength} / 5)</color>");
            }

            if(roamingNpcScript.sleeping)
            {
                WakeUpEnemy(roamingNpcScript);
                roamingNpcScript.TakeDamage(Mathf.FloorToInt(damage * sleepingDamage));
                manager.UpdateMessages($"You dealt <color=red>{damage}</color> damage to <color=#{ColorUtility.ToHtmlStringRGB(roamingNpcScript.EnemyColor)}>{roamingNpcScript.EnemyName}</color>");
            }
            else
            {
                roamingNpcScript._x = roamingNpcScript.howLongWillFololwInvisiblepLayer;
                roamingNpcScript.TakeDamage(damage);
                manager.UpdateMessages($"You dealt <color=red>{damage}</color> damage to <color=#{ColorUtility.ToHtmlStringRGB(roamingNpcScript.EnemyColor)}>{roamingNpcScript.EnemyName}</color>");
            }
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            manager.UpdateMessages("You missed!");
            WakeUpEnemy(roamingNpcScript);
        }        
    }

    public void WakeUpEnemy(RoamingNPC roamingNpc)
    {
        if (roamingNpc.sleeping) //wake up enemy
        {
            //roamingNpc.sleeping = false;
            //manager.UpdateMessages($"You woke up the <color={roamingNpc.EnemyColor}>{roamingNpc.EnemyName}</color>!");
            roamingNpc.WakeUp(); // code already exists in the the enemy
        }

        roamingNpc.attacked = true;
        roamingNpc._x = roamingNpc.howLongWillFololwInvisiblepLayer;
    }
}