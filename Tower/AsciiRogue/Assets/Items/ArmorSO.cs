using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Armor")]
public class ArmorSO : ItemScriptableObject
{
    public int armorClass;
    public enum a_material
    {
        iron,
        lether,
        mithril
    }
    public a_material a_Material;

    public override void Use(MonoBehaviour foo, Item itemObject)
    {
        if(foo is PlayerStats player) player.UpdateArmorClass();
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }

    public override void onEquip(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
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
}
