using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tool/Torch")]
public class Torch : ItemScriptableObject
{
    public override void Use(MonoBehaviour foo, Item itemObject)
    {
        UseTorch();
    }

    public void RemoveTorch()
    {
        DungeonGenerator.dungeonGenerator.lightFactor = 1;
    }

    public void UseTorch()
    {
        DungeonGenerator.dungeonGenerator.lightFactor = 2;
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }

    public override void onEquip(MonoBehaviour foo)
    {
    }

    public override void onUnequip(MonoBehaviour foo)
    {
    }
}
