using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Stun/Basic")]
public class BasicStun : BaseAIBehaviour<RoamingNPC>
{


    public override void Calculate(RoamingNPC t)
    {
        StunFor(t, 1);
    }

    public virtual void StunFor(RoamingNPC npc, int duration)
    {
        if (npc.isStuned)
        {
            npc.stuneDuration--;
        }

        if (duration > 0 && duration > npc.stuneDuration)
        {
            npc.stuneDuration = duration;
            npc.isStuned = true;
        }

        if (npc.stuneDuration <= 0)
        {
            npc.stuneDuration = 0;
            npc.isStuned = false;
        }
    }



}
