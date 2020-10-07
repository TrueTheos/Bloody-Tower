using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Attack/Poison Bite")]
public class PoisonBite : BasicAttack
{

    public override void Attack(RoamingNPC t,IUnit target)
    {
        int totalDamage = 0;

        int valueRequiredToHit = 0; //value required to hit the monster

        valueRequiredToHit = Random.Range(1, 100) + t.dex - target.dex - target.ac;
        //manager.UpdateMessages($"<color=red>Value required to hit player: = d20 + {dex} - {playerStats.__dexterity} - {playerStats.armorClass} = {valueRequiredToHit}</color>");

        if (valueRequiredToHit > 50)
        {
            if (Random.Range(1, 100) < 10 - target.ac + t.dex - target.dex)
            {
                totalDamage += Mathf.FloorToInt((Random.Range(1, 4) + Mathf.FloorToInt(t.str / 5)) * 1.5f);
            }
            else
            {
                totalDamage += Random.Range(1, 4) + Mathf.FloorToInt(t.str / 5);
            }

            if (target is PlayerStats && !t.playerStats.isPoisoned)
            {
                t.playerStats.IncreasePoisonDuration(3);
                t.playerStats.Poison();
            }
            target.TakeDamage(totalDamage,ItemScriptableObject.damageType.normal);

            t.manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(t.EnemyColor)}>{t.enemySO.name}</color> used <color=green>Poison Bite</color>!");
            t.manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(t.EnemyColor)}>{t.enemySO.name}</color> attacked "+ (target is PlayerStats?"you":target.noun) +" for <color=red>{totalDamage}</color>!");
        }
        else
        {
            t.manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(t.EnemyColor)}>{t.EnemyName}</color> missed.");
        }
        if (target is PlayerStats)
        {
            t.canvas.GetComponent<Animator>().SetTrigger("Shake");
        }
    }


}
