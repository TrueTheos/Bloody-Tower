using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Ring")]
public class RingSO : ItemScriptableObject
{
    public enum ringEffect
    {
        fireResistance,
        invisible
    }
    public ringEffect _ringEffect;

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }

    public override void Use(MonoBehaviour foo)
    {
        if (foo is PlayerStats player) Equip(foo);      
    }

    private void Equip(MonoBehaviour foo)
    {
        switch (_ringEffect)
        {
            case ringEffect.fireResistance:
                FireResistance(foo);
                break;
            case ringEffect.invisible:
                Invisible(foo);
                break;
        }
    }

    public void Dequip(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            switch (_ringEffect)
            {
                case ringEffect.fireResistance:
                    player.fireResistanceDuration = 0;
                    break;
                case ringEffect.invisible:
                    Invisible(foo);
                    break;
            }
        }      
    }

    private void FireResistance(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            player.FireResistance();
            player.fireResistanceDuration = int.MaxValue;
        }
    }

    private void Invisible(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.Invisible();
        }
    } 
}    

