using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Attack/Poison Bite")]
public class PoisonBite : BasicAttack
{

    public override void Calculate(RoamingNPC t)
    {
        int totalDamage = 0;

        int valueRequiredToHit = 0; //value required to hit the monster

        valueRequiredToHit = Random.Range(1, 100) + t.dex - t.playerStats.__dexterity - t.playerStats.armorClass;
        //manager.UpdateMessages($"<color=red>Value required to hit player: = d20 + {dex} - {playerStats.__dexterity} - {playerStats.armorClass} = {valueRequiredToHit}</color>");

        if (valueRequiredToHit > 50)
        {
            if (Random.Range(1, 100) < 10 - t.playerStats.armorClass + t.dex - t.playerStats.__dexterity)
            {
                totalDamage += Mathf.FloorToInt((Random.Range(1, 4) + Mathf.FloorToInt(t.str / 5)) * 1.5f);
            }
            else
            {
                totalDamage += Random.Range(1, 4) + Mathf.FloorToInt(t.str / 5);
            }

            if (!t.playerStats.isPoisoned)
            {
                t.playerStats.IncreasePoisonDuration(3);
                t.playerStats.Poison();
            }
            t.playerStats.TakeDamage(totalDamage);

            t.manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(t.EnemyColor)}>{t.enemySO.name}</color> used <color=green>Poison Bite</color>!");
            t.manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(t.EnemyColor)}>{t.enemySO.name}</color> attacked you for <color=red>{totalDamage}</color>!");
        }
        else
        {
            t.manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(t.EnemyColor)}>{t.EnemyName}</color> missed.");
        }

        t.canvas.GetComponent<Animator>().SetTrigger("Shake");
    }


}
