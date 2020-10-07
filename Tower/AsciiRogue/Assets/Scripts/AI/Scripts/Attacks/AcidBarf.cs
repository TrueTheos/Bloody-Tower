using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Attack/Acid Barf")]
public class AcidBarf : BasicAttack
{
    public BasicAttack ToCharge;


    public override void Attack(RoamingNPC source, IUnit target)
    {
        if (!source.attackCharged)
        {
            source.attackCharged = true;
            GameManager.manager.UpdateMessages($"<color={source.enemySO.E_color}>Sulyvan's Beast</color> starts coughing!");
            source.nextAttack = this;
        }
        else
        {
            source.attackCharged = false;
            GameManager.manager.UpdateMessages($"<color={source.enemySO.E_color}>Sulyvan's Beast</color> barfs a puddle of <color=green>acid</color> onto" +(target is PlayerStats ? "the player!" : target.noun));
            GameManager.manager.playerStats.MeltItem();
            ToCharge.Attack(source,target);
        }
    }

}
