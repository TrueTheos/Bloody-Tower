using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapon")]
public class WeaponsSO : ItemScriptableObject
{
    public enum additionalEffects //only when using weapon art
    {
        none,
        bleed,
        increaseDamage //for Bloodsword
    }

    public List<Vector2Int> attacks = new List<Vector2Int>();
    public List<additionalEffects> effectsOnHit;
    [Tooltip("Only relevant with ranged weapons")]
    public int MaximumProjection;
    public int BowDamage;
    //public int toHitBonus;
    public enum weaponType
    {
        sword,
        axe,
        dagger,
        katana,
        mace,
        spear,
        melee,
        bow,
        slingshot
    }
    public weaponType _weaponType;

    public override void Use(MonoBehaviour foo, Item itemObject)
    {
        
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        if(I_name == "Bloodsword") //if this item is Bloodsword
        {
            GameManager.manager.tasks.everyTurnTasks += BloodswordDecreaseDamage;
        }
    }

    [HideInInspector] public int bloodSwordCounter;

    public void BloodswordDecreaseDamage()
    {
        if(bloodSwordCounter > 0)
        {
            bloodSwordCounter--;
        }
        else
        {
            if(attacks[0].y != 1) attacks[0] = new Vector2Int(attacks[0].x, attacks[0].y - 1);
        }
    }

    public override void onEquip(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (bonusToHealth != 0) { }
            if (bonusToStrength != 0) player.__strength += bonusToStrength;
            if (bonusToIntelligence != 0) player.__intelligence += bonusToIntelligence;
            if (bonusToDexterity != 0) player.__dexterity += bonusToDexterity;
            if (bonusToEndurance != 0) player.__endurance += bonusToEndurance;
            if (bonusToNoise != 0) player.__noise += bonusToNoise;
        }
    }

    public override void onUnequip(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (bonusToHealth != 0) { }
            if (bonusToStrength != 0) player.__strength += -bonusToStrength;
            if (bonusToIntelligence != 0) player.__intelligence += -bonusToIntelligence;
            if (bonusToDexterity != 0) player.__dexterity += -bonusToDexterity;
            if (bonusToEndurance != 0) player.__endurance += -bonusToEndurance;
            if (bonusToNoise != 0) player.__noise += -bonusToNoise;
        }
    }

    public void onHit(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            //
        }
    }
}
