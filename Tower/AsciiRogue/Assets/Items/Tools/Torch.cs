using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tool/Torch")]
public class Torch : ItemScriptableObject
{
    public override void Use(MonoBehaviour foo, Item itemObject)
    {
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }

    public override void onEquip(MonoBehaviour foo)
    {
        DungeonGenerator.dungeonGenerator.lightFactor -= 1;
    }

    public override void onUnequip(MonoBehaviour foo)
    { 
        DungeonGenerator.dungeonGenerator.lightFactor += 2;
    }
}
