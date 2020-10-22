using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Attack/Normal Attack")]
public class NormalAttack : BasicAttack
{

    public override void Calculate((RoamingNPC source,IUnit target) t)
    {
        int totalDamage = 0;

        int valueRequiredToHit = 0; //value required to hit the monster

        valueRequiredToHit = Random.Range(1, 100) + t.source.dex - t.source.playerStats.__dexterity - t.source.playerStats.armorClass;
        //manager.UpdateMessages($"<color=red>Value required to hit player: = d20 + {dex} - {playerStats.__dexterity} - {playerStats.armorClass} = {valueRequiredToHit}</color>");

        if (valueRequiredToHit > 50)
        {
            if (Random.Range(1, 100) < 10 - t.source.playerStats.armorClass + t.source.dex - t.source.playerStats.__dexterity)
            {
                totalDamage += Mathf.FloorToInt((Random.Range(1, 4) + Mathf.FloorToInt(t.source.str / 5)) * 1.5f);
            }
            else
            {
                totalDamage += Random.Range(1, 4) + Mathf.FloorToInt(t.source.str / 5);
            }

            if (t.source.enemySO.attackText.Count > 0)
            {
                t.source.manager.UpdateMessages($"The <color=#{ColorUtility.ToHtmlStringRGB(t.source.EnemyColor)}>{t.source.enemySO.name}</color> {t.source.enemySO.attackText[Random.Range(0, t.source.enemySO.attackText.Count)]} <color=red>{totalDamage} ({t.target.currHp - totalDamage}/{t.target.maxHp}) damage</color>!");
            }
            else
            {
                t.source.manager.UpdateMessages($"The <color=#{ColorUtility.ToHtmlStringRGB(t.source.EnemyColor)}>{t.source.enemySO.name}</color> attacks "+ (t.target is PlayerStats ? "you" : t.target.noun) + $". <color=red>{totalDamage} ({t.target.currHp - totalDamage}/{t.target.maxHp}) damage</color>!");
            }
            t.target.TakeDamage(totalDamage, ItemScriptableObject.damageType.normal);
        }
        else
        {
            t.source.manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(t.source.EnemyColor)}>{t.source.EnemyName}</color> missed.");
        }

        t.source.canvas.GetComponent<Animator>().SetTrigger("Shake");
    }


}
