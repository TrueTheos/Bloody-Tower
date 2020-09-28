using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Attack/Tail Whip")]
public class TailWhip : BasicAttack
{

    public override void Calculate(RoamingNPC t)
    {
        GameManager.manager.UpdateMessages($"<color={t.enemySO.E_color}>The Giant Rat</color> whips it's tail into the player!");
        GameManager.manager.UpdateMessages("You are stunned!");
        GameManager.manager.playerStats.Stun();
        ToAttack[Random.Range(0,ToAttack.Count)].Calculate(t);
    }

}
