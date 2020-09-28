using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Test To Wake Up/Basic")]
public class BasicTestToWakeUp : BaseAIBehaviour<RoamingNPC>
{
    public BasicWakeUp ToWakeup;

    public override void Calculate(RoamingNPC t)
    {
        int dist = Mathf.Max(Mathf.Abs(MapManager.playerPos.x - t.__position.x), Mathf.Abs(MapManager.playerPos.y - t.__position.y));
        if ((Random.Range(1, 20) + t.lvl - t.playerStats.__dexterity - dist * 10) > 0)
        {
            ToWakeup.Calculate(t);
        }
    }



}
