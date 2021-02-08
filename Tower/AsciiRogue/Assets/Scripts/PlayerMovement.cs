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

    public bool isStunned;

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

                if (Controls.GetKeyDown(Controls.Inputs.MoveUp) || Controls.GetKey(Controls.Inputs.MoveUp))
                {
                    move.y = 1;
                }
                if (Controls.GetKeyDown(Controls.Inputs.MoveDown) || Controls.GetKey(Controls.Inputs.MoveDown))
                {
                    move.y=-1;
                }
                if (Controls.GetKeyDown(Controls.Inputs.MoveLeft) || Controls.GetKey(Controls.Inputs.MoveLeft))
                {
                    move.x = -1;
                }
                if (Controls.GetKeyDown(Controls.Inputs.MoveRight) || Controls.GetKey(Controls.Inputs.MoveRight))
                {
                    move.x = 1;
                }
                if (Controls.GetKeyDown(Controls.Inputs.MoveUpLeft) || Controls.GetKey(Controls.Inputs.MoveUpLeft))
                {
                    move.x = -1;
                    move.y = 1;
                }
                if (Controls.GetKeyDown(Controls.Inputs.MoveUpRight) || Controls.GetKey(Controls.Inputs.MoveUpRight))
                {
                    move.x = 1;
                    move.y = 1;
                }
                if (Controls.GetKeyDown(Controls.Inputs.MoveDownRight) || Controls.GetKey(Controls.Inputs.MoveDownRight))
                {
                    move.x = 1;
                    move.y = -1;
                }
                if (Controls.GetKeyDown(Controls.Inputs.MoveDownLeft) || Controls.GetKey(Controls.Inputs.MoveDownLeft))
                {
                    move.x = -1;
                    move.y = -1;
                }

                if(Controls.GetKeyDown(Controls.Inputs.CloseDoors) || Controls.GetKey(Controls.Inputs.CloseDoors))
                {
                    if(MapManager.map[position.x - 1, position.y + 1]?.structure is Door door)
                    {
                        door.CloseDoor();
                    }
                    if (MapManager.map[position.x, position.y + 1]?.structure is Door door1)
                    {
                        door1.CloseDoor();
                    }
                    if (MapManager.map[position.x + 1, position.y + 1]?.structure is Door door2)
                    {
                        door2.CloseDoor();
                    }
                    if (MapManager.map[position.x + 1, position.y]?.structure is Door door4)
                    {
                        door4.CloseDoor();
                    }
                    if (MapManager.map[position.x + 1, position.y - 1]?.structure is Door door5)
                    {
                        door5.CloseDoor();
                    }
                    if (MapManager.map[position.x, position.y - 1]?.structure is Door door6)
                    {
                        door6.CloseDoor();
                    }
                    if (MapManager.map[position.x - 1, position.y - 1]?.structure is Door door7)
                    {
                        door7.CloseDoor();
                    }
                    if (MapManager.map[position.x - 1, position.y]?.structure is Door door8)
                    {
                        door8.CloseDoor();
                    }
                    manager.FinishPlayersTurn();
                }

                if (move.sqrMagnitude > 0)
                {
                    Move(InputToVector(move.x,move.y));
                    MoveCooldown = MoveDelay;
                }
            }
            
            
            Selector.Current.SelectedTile(position.x, position.y);

            if (Controls.GetKeyUp(Controls.Inputs.Use))
            {
                if (MapManager.map[position.x, position.y].item != null)
                {
                    if (playerStats.maximumInventorySpace > playerStats.currentItems && playerStats.currentWeight + MapManager.map[position.x, position.y].item.GetComponent<Item>().iso.I_weight <= playerStats.maxWeight)
                    {
                        playerStats.currentWeight += MapManager.map[position.x, position.y].item.GetComponent<Item>().iso.I_weight;
                        playerStats.Pickup(MapManager.map[position.x, position.y].item, MapManager.map[position.x, position.y].item.GetComponent<Item>().iso, target);
                        MapManager.map[position.x, position.y].letter = "";
                        MapManager.map[position.x, position.y].baseChar = ".";
                        MapManager.map[position.x, position.y].exploredColor = new Color(1, 1, 1);
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
                else if (MapManager.map[position.x, position.y].structure != null)
                {
                    MapManager.map[position.x, position.y].structure.Use();

                    //FOVNEW.fv.Initialize(FOVNEW.fv.CanLightPass, FOVNEW.fv.SetToVisible, FOVNEW.fv.Distance);
                    FoV.Initialize();
                    //manager.UpdateVisibility();
                    manager.StartPlayersTurn();
                    manager.fv.Compute(position, playerStats.viewRange);
                    DungeonGenerator.dungeonGenerator.DrawMap(MapManager.map);
                }
            }
        }
        else if(isStunned)
        {
            isStunned = false;
            canMove = true;
            manager.FinishPlayersTurn();
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
                        //MapManager.map[x, y].enemy.GetComponent<RoamingNPC>().TestToWakeUp(); //TEST TO WAKE 
                        MapManager.map[x, y].enemy.GetComponent<RoamingNPC>().WakeUp(); //WAKE UP 100%
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

        if (MapManager.map[target.x, target.y].enemy != null)
        {
            if (MapManager.map[target.x, target.y].enemy.GetComponent<RoamingNPC>().enemySO._Behaviour == EnemiesScriptableObject.E_behaviour.npc && !MapManager.map[target.x, target.y].enemy.GetComponent<RoamingNPC>().enemySO.finishedDialogue)
            {
                playerStats.dialogue = true;
                playerStats.npcDialogue = MapManager.map[target.x, target.y].enemy.GetComponent<RoamingNPC>();
                return;
            }
            else Attack(MapManager.map[target.x, target.y].enemy, target.x, target.y);
        }
        else if (MapManager.map[target.x, target.y].isWalkable && MapManager.map[target.x, target.y].enemy == null) // && MapManager.map[target.x, target.y].item == null
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
            RunManager.CurrentRun.Set(RunManager.Names.TilesMoved, RunManager.CurrentRun.Get<int>(RunManager.Names.TilesMoved) + 1);
        }
        else if(MapManager.map[target.x, target.y].structure != null && !MapManager.map[target.x, target.y].isWalkable)
        {
            MapManager.map[target.x, target.y].structure.Use();
            target = position;
        }
        /*else if(MapManager.map[target.x, target.y].type == "Wall")
        {
            if(inputX == 1 && inputY == 1) //move top right
            {
                
            }
            else if (inputX == 1 && inputY == 0) //move right
            {
                if(MapManager.map[target.x, target.y + 1]?.structure is Door door)
                {
                    door.Use();
                }
                if (MapManager.map[target.x, target.y - 1]?.structure is Door door1)
                {
                    door1.Use();
                }
            }
            else if (inputX == 1 && inputY == -1) //move bottom right
            {

            }
            else if (inputX == 0 && inputY == -1) //move bottom
            {
                if (MapManager.map[target.x - 1, target.y]?.structure is Door door)
                {
                    door.Use();
                }
                if (MapManager.map[target.x + 1, target.y]?.structure is Door door1)
                {
                    door1.Use();
                }
            }
            else if (inputX == -1 && inputY == -1) //move bottom left
            {

            }
            else if (inputX == -1 && inputY == 0) //move left
            {
                if (MapManager.map[target.x, target.y + 1]?.structure is Door door)
                {
                    door.Use();
                }
                if (MapManager.map[target.x, target.y - 1]?.structure is Door door1)
                {
                    door1.Use();
                }
            }
            else if (inputX == -1 && inputY == 1) //move left top
            {

            }
            else if (inputX == 0 && inputY == 1) //move top
            {
                if (MapManager.map[target.x - 1, target.y]?.structure is Door door)
                {
                    door.Use();
                }
                if (MapManager.map[target.x + 1, target.y]?.structure is Door door1)
                {
                    door1.Use();
                }
            }
        }*/
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

                        RunManager.CurrentRun.Set(RunManager.Names.DoorsOpen, RunManager.CurrentRun.Get<int>(RunManager.Names.DoorsOpen)+1);
                        RunManager.CurrentRun.Set(RunManager.Names.DoorsUnlocked, RunManager.CurrentRun.Get<int>(RunManager.Names.DoorsUnlocked) + 1);
                        RunManager.CurrentRun.Set(RunManager.Names.TilesMoved, RunManager.CurrentRun.Get<int>(RunManager.Names.TilesMoved ) + 1);
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
                RunManager.CurrentRun.Set(RunManager.Names.TilesMoved, RunManager.CurrentRun.Get<int>(RunManager.Names.TilesMoved) + 1);
            }
        }   

        manager.FinishPlayersTurn();
    }

    private void Attack(GameObject e, int x, int y) //float damageMultiplier, WeaponsSO.additionalEffects effect
    {
        for (int y2 = position.y - 2; y2 < position.y + 2; y2++)
        {
            for (int x2 = position.x - 2; x2 < position.x + 2; x2++)
            {
                try
                {
                    if (MapManager.map[x2, y2].enemy != null)
                    {
                        MapManager.map[x2, y2].enemy.GetComponent<RoamingNPC>().TestToWakeUp();
                    }
                }
                catch { }

            }
        }

        if (playerStats._Lhand?.iso is WeaponsSO w)
        {
            w.onHit(playerStats);
        }
        if (playerStats._Rhand?.iso is WeaponsSO w2)
        {
            w2.onHit(playerStats);
        }

        if (playerStats.isInvisible)
        {
            playerStats.RemoveInvisibility();
        }

        RoamingNPC roamingNpcScript = e.GetComponent<RoamingNPC>();

        int damageLeftHand = 0;
        int damageRightHand = 0;

        ItemScriptableObject.damageType leftHandDamageType = ItemScriptableObject.damageType.normal;
        ItemScriptableObject.damageType rightHandDamageType = ItemScriptableObject.damageType.normal;

        int r = Random.Range(1, 100);

        int valueRequiredToHit = 0; //value required to hit the monster

        if (roamingNpcScript.sleeping)
        {
            valueRequiredToHit = r + playerStats.__dexterity - roamingNpcScript.dex / 4 - roamingNpcScript.AC;
        }
        else
        {
            valueRequiredToHit = r + playerStats.__dexterity - roamingNpcScript.dex - roamingNpcScript.AC;
        }

        if (r <= 0)
        {
            manager.UpdateMessages("You missed!");
            WakeUpEnemy(roamingNpcScript);
            return;
        }

        if (valueRequiredToHit > 50 || r >= 80) //Do we hit?
        {
            if (playerStats._Lhand?.iso is WeaponsSO weaponL)
            {
                leftHandDamageType = weaponL.I_damageType;

                for (x = 0; x < weaponL.attacks.Count; x++)
                {
                    damageLeftHand += Random.Range(1, weaponL.attacks[x].y + 1);

                    //IF WEAPON IS BLOOD SWORD WE INCREASE ITS DAMAGE
                    if (weaponL.I_name == "Bloodsword")
                    {
                        weaponL.attacks[0] = new Vector2Int(1, weaponL.attacks[0].y + 1);
                        weaponL.bloodSwordCounter = manager.tasks.bloodSwordCooldown;
                    }
                }
            }
            if (playerStats._Rhand?.iso is WeaponsSO weaponR)
            {
                rightHandDamageType = weaponR.I_damageType;

                for (x = 0; x < weaponR.attacks.Count; x++)
                {
                    damageRightHand += Random.Range(1, weaponR.attacks[x].y + 1);

                    //IF WEAPON IS BLOOD SWORD WE INCREASE ITS DAMAGE
                    if (weaponR.I_name == "Bloodsword")
                    {
                        weaponR.attacks[0] = new Vector2Int(1, weaponR.attacks[0].y + 1);
                        weaponR.bloodSwordCounter = manager.tasks.bloodSwordCooldown;
                    }
                }
            }

            //CRIT?
            if (Random.Range(1, 100) < 10 - roamingNpcScript.AC + roamingNpcScript.dex - playerStats.__dexterity)
            {
                if (playerStats._Lhand?.iso is WeaponsSO)
                {
                    damageLeftHand += Mathf.FloorToInt((Random.Range(1, 4) + Mathf.FloorToInt(playerStats.__strength / 5)) * 1.5f);
                }
            }
            else
            {
                //damageLeftHand += Random.Range(1, 4) + Mathf.FloorToInt(playerStats.__strength / 5);
            }

            if (Random.Range(1, 100) < 10 - roamingNpcScript.AC + roamingNpcScript.dex - playerStats.__dexterity)
            {
                if ((playerStats._Lhand?.iso is WeaponsSO))
                {
                    damageRightHand += Mathf.FloorToInt((Random.Range(1, 4) + Mathf.FloorToInt(playerStats.__strength / 5)) * 1.5f);
                }
            }
            else
            {
               // damageRightHand += Random.Range(1, 4) + Mathf.FloorToInt(playerStats.__strength / 5);
            }

            if (damageLeftHand == 0) damageLeftHand = Random.Range(1, 4);
            if (damageRightHand == 0) damageRightHand = Random.Range(1, 4);

            if (roamingNpcScript.sleeping)
            {
                damage = Mathf.FloorToInt(damageLeftHand * sleepingDamage) + Mathf.FloorToInt(damageRightHand * sleepingDamage);
                manager.UpdateMessages($"You dealt <color=red>{damage}</color> damage to <color=#{ColorUtility.ToHtmlStringRGB(roamingNpcScript.EnemyColor)}>{roamingNpcScript.EnemyName}</color>");
                WakeUpEnemy(roamingNpcScript);
                roamingNpcScript.TakeDamage(Mathf.FloorToInt(damageLeftHand * sleepingDamage), leftHandDamageType);
                roamingNpcScript.TakeDamage(Mathf.FloorToInt(damageRightHand * sleepingDamage), rightHandDamageType);               
            }
            else
            {
                damage = damageLeftHand + damageRightHand;
                manager.UpdateMessages($"You dealt <color=red>{damage}</color> damage to <color=#{ColorUtility.ToHtmlStringRGB(roamingNpcScript.EnemyColor)}>{roamingNpcScript.EnemyName}</color>");
                roamingNpcScript._x = roamingNpcScript.howLongWillFololwInvisiblepLayer;
                roamingNpcScript.TakeDamage(damageLeftHand, leftHandDamageType);
                roamingNpcScript.TakeDamage(damageRightHand, rightHandDamageType);            
            }
            RunManager.CurrentRun.Set(RunManager.Names.EnemiesAttacked, RunManager.CurrentRun.Get<int>(RunManager.Names.EnemiesAttacked) + 1);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            manager.UpdateMessages("You missed!");
            WakeUpEnemy(roamingNpcScript);
        }
    }

    private float GetRoll()
    {
        float result = (Random.Range(0, 1f) + Random.Range(0, 1f)) * 3;
        if (result > 5.75f) result += GetRoll();
        return result;
    }

    private bool AttackHits(RoamingNPC npc)
    {
        float roll = GetRoll();
        if (roll < .25f) return false;
        else return (playerStats.__dexterity - npc.dex) >= 3;
    }

    public void WakeUpEnemy(RoamingNPC roamingNpc)
    {
        if (roamingNpc.sleeping) //wake up enemy
        {
            roamingNpc.WakeUp(); // code already exists in the the enemy
        }

        roamingNpc.attacked = true;
        roamingNpc._x = roamingNpc.howLongWillFololwInvisiblepLayer;
    }
}