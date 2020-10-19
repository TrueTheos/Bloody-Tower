using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Attack/Tail Whip")]
public class TailWhip : BasicAttack
{

    public override void Calculate((RoamingNPC source,IUnit target)t)
    {
        GameManager.manager.UpdateMessages($"<color={t.source.enemySO.E_color}>The Giant Rat</color> whips it's tail into the " + (t.target is PlayerStats ? "player!" : t.target.noun));
        if (t.target is PlayerStats)
        {
            GameManager.manager.UpdateMessages("You are stunned!");
            GameManager.manager.playerStats.Stun();
        }
        if (t.target is RoamingNPC npc)
        {
            GameManager.manager.UpdateMessages(t.target.noun +" is stunned!");
            npc.Stun(1);
        }        
        ToAttack[Random.Range(0,ToAttack.Count)].Calculate((t.source, t.target));
    }

}
