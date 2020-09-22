using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemThrowHelper 
{

    private static ThrowInfo Info;

    public static int MaxRange => Info.MaxRange;

    public static void PrepareItem(PlayerStats player, Item item)
    {
        LoadItem(player, item);
    }

    private static void LoadItem(PlayerStats player,Item item)
    {
        Info = new ThrowInfo();
        Info.CurrentItem = item;

        Info.MaxRange = player.__strength / 10 + 1;
        switch (item.iso)
        {
            case WeaponsSO weapon:
                switch (weapon._weaponType)
                {
                    case WeaponsSO.weaponType.spear:
                    case WeaponsSO.weaponType.sword:                      
                    case WeaponsSO.weaponType.axe:
                        Info.GetDamage = () => Random.Range(2, 5);
                        Info.MaxRange -= 2;
                        Info.MaxRange = Mathf.Max(0, Info.MaxRange);
                        break;
                    case WeaponsSO.weaponType.katana:
                    case WeaponsSO.weaponType.dagger:
                    case WeaponsSO.weaponType.mace:
                        Info.GetDamage = () => Random.Range(1, 4);
                        Info.MaxRange -= 2;
                        Info.MaxRange = Mathf.Max(0, Info.MaxRange);
                        break;
                    case WeaponsSO.weaponType.melee:
                    case WeaponsSO.weaponType.bow:
                    case WeaponsSO.weaponType.slingshot:
                    default:
                        Info.GetDamage = () => 1;
                        Info.MaxRange -= 1;
                        Info.MaxRange = Mathf.Max(0, Info.MaxRange);
                        break;
                }
                Info.ValidTarget = () => MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
                Info.TargetMove = () => MapManager.map[Targeting.Position.x, Targeting.Position.y].isVisible;
                break;
            case PotionSO potion:
                Info.GetDamage = () => 0;
                Info.ValidTarget = () => true;
                Info.TargetMove = () => MapManager.map[Targeting.Position.x, Targeting.Position.y].isVisible;
                break;
            case ArmorSO armor:
                // dont know what you did but hey, i dont judge
                Info.MaxRange -= 2;
                Info.MaxRange = Mathf.Max(0, Info.MaxRange);
                Info.ValidTarget = () => MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
                Info.TargetMove = () => MapManager.map[Targeting.Position.x, Targeting.Position.y].isVisible;
                Info.GetDamage = () => 1;
                break;
            default:
                // simple 1 damage
                Info.MaxRange -= 0;
                Info.MaxRange = Mathf.Max(0, Info.MaxRange);
                Info.ValidTarget = () => MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
                Info.TargetMove = () => MapManager.map[Targeting.Position.x, Targeting.Position.y].isVisible;
                Info.GetDamage = () => 1;
                break;
        }
    }

    public static void Activate(PlayerStats player)
    {
        switch (Info.CurrentItem.iso)
        {
            case PotionSO potion:
                ThrowPotion(player, potion);
                break;
            case ArmorSO armor:
            case WeaponsSO weapon:
            default:
                ThrowDamageThing(player);
                break;
        }
        GameManager.manager.ApplyChangesInInventory(Info.CurrentItem.iso);
    }

    public static bool CanBeThrown(Item item)
    {
        switch (item.iso)
        {
            case ScrollSO scroll:
            case SpellbookSO book:
                return false;
            default:
                break;
        }
        return true;
    }

    private static void ThrowPotion(PlayerStats player,PotionSO potion)
    {
        Vector2Int TargetPos = Targeting.Position;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                try
                {
                    Tile t = MapManager.map[TargetPos.x + x, TargetPos.y + y];

                    potion.Use(player, Info.CurrentItem);
                }
                catch (System.Exception)
                {
                }
            }
        }
    }

    private static void ThrowDamageThing(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = (roll + player.__dexterity - npc.dex - npc.AC) -
            Mathf.Max(Mathf.Abs(PlayerMovement.playerMovement.position.x - Targeting.Position.x),
                Mathf.Abs(PlayerMovement.playerMovement.position.x - Targeting.Position.x));


        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 50 || roll >= 80) //Do we hit?
        {
            damage = Info.GetDamage();
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }

    }

    private static void MissEnemyWakeUp(RoamingNPC npc)
    {
        GameManager.manager.UpdateMessages("You missed!");
        PlayerMovement.playerMovement.WakeUpEnemy(npc);
    }

    private static void DealDamageToEnemy(RoamingNPC npc, int damage)
    {
        if (npc.sleeping)
        {
            PlayerMovement.playerMovement.WakeUpEnemy(npc);
            npc.TakeDamage(Mathf.FloorToInt(damage * PlayerMovement.playerMovement.sleepingDamage));
            GameManager.manager.UpdateMessages($"You dealt <color=red>{damage}</color> damage to <color=#{ColorUtility.ToHtmlStringRGB(npc.EnemyColor)}>{npc.EnemyName}</color>");
        }
        else
        {
            npc._x = npc.howLongWillFololwInvisiblepLayer;
            npc.TakeDamage(damage);
            GameManager.manager.UpdateMessages($"You dealt <color=red>{damage}</color> damage to <color=#{ColorUtility.ToHtmlStringRGB(npc.EnemyColor)}>{npc.EnemyName}</color>");
        }
    }

    public static bool IsValidTarget()
    {
        return Info.ValidTarget();
    }

    public static bool AllowTargetingMove()
    {
        return Info.TargetMove();
    }

    private delegate bool Check();
    private delegate int Numbercheck();   

    private class ThrowInfo
    {
        public Item CurrentItem;

        public Check ValidTarget;
        public Check TargetMove;
        public Numbercheck GetDamage;

        public int MaxRange;

        

    }

}
