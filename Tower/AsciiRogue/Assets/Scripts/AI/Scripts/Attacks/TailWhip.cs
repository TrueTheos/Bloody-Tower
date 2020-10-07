using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Attack/Tail Whip")]
public class TailWhip : BasicAttack
{

    public override void Attack(RoamingNPC t,IUnit target)
    {
        GameManager.manager.UpdateMessages($"<color={t.enemySO.E_color}>The Giant Rat</color> whips it's tail into the " + (target is PlayerStats ? "player!" : target.noun));
        if (target is PlayerStats)
        {
            GameManager.manager.UpdateMessages("You are stunned!");
            GameManager.manager.playerStats.Stun();
        }
        if (target is RoamingNPC npc)
        {
            GameManager.manager.UpdateMessages(target.noun +" is stunned!");
            npc.Stun(1);
        }        
        ToAttack[Random.Range(0,ToAttack.Count)].Attack(t,target);
    }

}
