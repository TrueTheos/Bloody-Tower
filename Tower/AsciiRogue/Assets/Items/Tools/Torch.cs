using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Tool/Torch")]
public class Torch : ItemScriptableObject
{
    public override void Use(MonoBehaviour foo)
    {
        UseTorch();
    }

    public void RemoveTorch()
    {
        isEquipped = false;
        DungeonGenerator.dungeonGenerator.lightFactor = 1;
    }

    public void UseTorch()
    {
        isEquipped = true;
        DungeonGenerator.dungeonGenerator.lightFactor = 2;
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }
}
