using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Bell")]
public class Bell : Tool
{
    public override void Use(MonoBehaviour foo, Item itemObject)
    {
        GameManager.manager.UpdateMessages("You ring the bell.");

        for (int x = MapManager.playerPos.x - 5; x < MapManager.playerPos.x + 5; x++)
        {
            for (int y = MapManager.playerPos.y - 5; y < MapManager.playerPos.y + 5; y++)
            {
                MapManager.map[x, y].enemy?.GetComponent<RoamingNPC>().WakeUp();
            }
        }
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
