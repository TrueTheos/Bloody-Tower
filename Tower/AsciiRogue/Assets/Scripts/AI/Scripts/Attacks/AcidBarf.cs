using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Attack/Acid Barf")]
public class AcidBarf : BasicAttack
{
    public BasicAttack ToCharge;


    public override void Calculate((RoamingNPC source, IUnit target)t)
    {
        if (!t.source.attackCharged)
        {
            t.source.attackCharged = true;
            GameManager.manager.UpdateMessages($"<color={t.source.enemySO.E_color}>Sulyvan's Beast</color> starts coughing!");
            t.source.nextAttack = this;
        }
        else
        {
            t.source.attackCharged = false;
            GameManager.manager.UpdateMessages($"<color={t.source.enemySO.E_color}>Sulyvan's Beast</color> barfs a puddle of <color=green>acid</color> onto" +(t.target is PlayerStats ? "the player!" : t.target.noun));
            GameManager.manager.playerStats.MeltItem();
            ToCharge.Calculate((t.source, t.target));
        }
    }

}
