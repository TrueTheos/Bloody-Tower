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

    public override void Use(MonoBehaviour foo)
    {
        if(foo is PlayerStats player) player.UpdateArmorClass();
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }
}
