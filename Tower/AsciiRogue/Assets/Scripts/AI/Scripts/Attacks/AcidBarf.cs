using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Attack/Acid Barf")]
public class AcidBarf : BasicAttack
{
    public BasicAttack ToCharge;

    public override void Calculate(RoamingNPC t)
    {
        if (!t.attackCharged)
        {
            t.attackCharged = true;
            GameManager.manager.UpdateMessages($"<color={t.enemySO.E_color}>Sulyvan's Beast</color> starts coughing!");
            t.nextAttack = this;
        }
        else
        {
            t.attackCharged = false;
            GameManager.manager.UpdateMessages($"<color={t.enemySO.E_color}>Sulyvan's Beast</color> barfs a puddle of <color=green>acid</color> onto the player!");
            GameManager.manager.playerStats.MeltItem();
            ToCharge.Calculate(t);
        }
    }


}
