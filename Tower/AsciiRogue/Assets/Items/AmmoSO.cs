using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Ammo")]
public class AmmoSO : ItemScriptableObject
{

    public int Damage;

    public int Weight;
    


    public override void onEquip(MonoBehaviour foo)
    {
    }

    public override void OnPickup(MonoBehaviour foo)
    {
    }

    public override void onUnequip(MonoBehaviour foo)
    {
    }

    public override void Use(MonoBehaviour foo, Item itemObject)
    {
    }
}
