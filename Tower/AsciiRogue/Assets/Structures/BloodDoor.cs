using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodDoor : Structure
{
    public bool opened = false;
    public int bloodCost = 0;
    public Vector2Int position;

    public override void Use()
    {
        if(opened)
        {
            GameManager.manager.playerStats.LossBlood(bloodCost);
            MapManager.map[position.x, position.y].structure = null;
        }

        if(!opened)
        {
            GameManager.manager.UpdateMessages($"You have to pay <color=red>{bloodCost}</color> to open this door.");
            opened = true;
        }
    }

    public override void WalkIntoTrigger()
    {
        
    }
}
