using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Bleed/Basic")]
public class BasicBleed : BaseAIBehaviour<RoamingNPC>
{
    public BasicTakeDamage ToTakeDamage;

    public override void Calculate(RoamingNPC t)
    {
        if (!t.isBleeding)
        {
            t.isBleeding = true;

            t.EffectTasks += t.Bleed;

            //bleedLength = Random.Range(manager.bleedDuration.x, manager.bleedDuration.y);

            t.manager.UpdateMessages($"The <color={t.EnemyColor}>{t.EnemyName}</color> starts bleeding.");
        }

        if (t.bleedLength <= 0)
        {
            t.isBleeding = false;

            t.EffectTasks -= t.Bleed;

            return;
        }

        if (t.bleedLength > 0)
        {
            ToTakeDamage.TakeDamage(t,1);
            t.bleedLength--;
        }
    }


}
