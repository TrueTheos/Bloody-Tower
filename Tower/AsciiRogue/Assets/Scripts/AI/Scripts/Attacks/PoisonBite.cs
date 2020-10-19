using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Attack/Poison Bite")]
public class PoisonBite : BasicAttack
{

    public override void Calculate((RoamingNPC source,IUnit target) t)
    {
        int totalDamage = 0;

        int valueRequiredToHit = 0; //value required to hit the monster

        valueRequiredToHit = Random.Range(1, 100) + t.source.dex - t.target.dex - t.target.ac;
        //manager.UpdateMessages($"<color=red>Value required to hit player: = d20 + {dex} - {playerStats.__dexterity} - {playerStats.armorClass} = {valueRequiredToHit}</color>");

        if (valueRequiredToHit > 50)
        {
            if (Random.Range(1, 100) < 10 - t.target.ac + t.source.dex - t.target.dex)
            {
                totalDamage += Mathf.FloorToInt((Random.Range(1, 4) + Mathf.FloorToInt(t.source.str / 5)) * 1.5f);
            }
            else
            {
                totalDamage += Random.Range(1, 4) + Mathf.FloorToInt(t.source.str / 5);
            }

            if (t.target is PlayerStats && !t.source.playerStats.isPoisoned)
            {
                t.source.playerStats.IncreasePoisonDuration(3);
                t.source.playerStats.Poison();
            }
            t.target.TakeDamage(totalDamage,ItemScriptableObject.damageType.normal);

            t.source.manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(t.source.EnemyColor)}>{t.source.enemySO.name}</color> used <color=green>Poison Bite</color>!");
            t.source.manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(t.source.EnemyColor)}>{t.source.enemySO.name}</color> attacked "+ (t.target is PlayerStats?"you": t.target.noun) +" for <color=red>{totalDamage}</color>!");
        }
        else
        {
            t.source.manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(t.source.EnemyColor)}>{t.source.EnemyName}</color> missed.");
        }
        if (t.target is PlayerStats)
        {
            t.source.canvas.GetComponent<Animator>().SetTrigger("Shake");
        }
    }


}
