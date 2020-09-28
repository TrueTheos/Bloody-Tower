using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Wake Up/Basic")]
public class BasicWakeUp : BaseAIBehaviour<RoamingNPC>
{


    public override void Calculate(RoamingNPC t)
    {
        if (t.sleeping) //wake up enemy
        {
            t.sleeping = false;

            if (t.enemySO.E_realName != string.Empty)
            {
                MapManager.map[t.__position.x, t.__position.y].timeColor = t.EnemyColor;
                MapManager.map[t.__position.x, t.__position.y].letter = t.EnemySymbol;
            }
            t.manager.UpdateMessages($"You woke up the <color={t.EnemyColor}>{t.EnemyName}</color>!");
        }

        t.attacked = true;
        t._x = t.howLongWillFololwInvisiblepLayer;
    }
}
