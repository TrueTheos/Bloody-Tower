using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotionCloud : Structure
{
    public PotionSO potionType;

    public override void Use()
    {
    }

    public override void WalkIntoTrigger()
    {
        potionType.Use(GameManager.manager.playerStats, null);
    }
}
