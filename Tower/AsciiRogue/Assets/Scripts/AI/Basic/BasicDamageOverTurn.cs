using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Damage Over Turn/Basic")]
public class BasicDamageOverTurn : BaseAIBehaviour<RoamingNPC>
{

    public BasicWakeUp ToWakeUp;
    public BasicTakeDamage ToTakeDamage;

    public override void Calculate(RoamingNPC t)
    {
        if (!t.damageOverTurn)
        {
            t.damageOverTurn = true;
            t.EffectTasks += ()=> AddDOT(t);
            if (t.dotDuration != 0) { }
            else t.dotDuration = 10;
            ToWakeUp.Calculate(t);
        }
        else
        {
            t.dotDuration--;
            ToTakeDamage.TakeDamage(t,(20 + t.playerStats.__intelligence)/ 10);
        }

        if (t.dotDuration <= 0)
        {
            t.damageOverTurn = false;
            t.EffectTasks -= ()=>AddDOT(t);
        }
    }

    public void AddDOT(RoamingNPC t)
    {
        Calculate(t);       
    }


}
