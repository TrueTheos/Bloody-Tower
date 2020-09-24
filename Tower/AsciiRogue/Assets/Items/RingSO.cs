using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Ring")]
public class RingSO : ItemScriptableObject
{
    public enum ringEffect
    {
        fireResistance,
        invisible,
        basedOnName,
        none
    }
    public ringEffect _ringEffect;

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }

    public override void Use(MonoBehaviour foo, Item itemObject)
    {
        if (foo is PlayerStats player) Equip(foo);      
    }

    private void Equip(MonoBehaviour foo)
    {
    }

    public void Dequip(MonoBehaviour foo)
    {    
    }

    private void FireResistance(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            player.IncreaseFireResistanceDuration(int.MaxValue);
            player.FireResistance();
        }
    }

    private void Invisible(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.Invisible();
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

            switch (_ringEffect)
            {
                case ringEffect.fireResistance:
                    FireResistance(foo);
                    break;
                case ringEffect.invisible:
                    Invisible(foo);
                    break;
                default:
                    break;
            }
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

            switch (_ringEffect)
            {
                case ringEffect.fireResistance:
                    player.RemoveFireResistance();
                    break;
                case ringEffect.invisible:
                    player.RemoveInvisibility();
                    break;
                default:
                    break;
            }
        }
    }
}    

