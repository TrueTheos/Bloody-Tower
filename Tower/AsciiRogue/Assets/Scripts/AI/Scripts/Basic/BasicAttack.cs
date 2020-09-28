using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Attack/Basic")]
public class BasicAttack : BaseAIBehaviour<RoamingNPC>
{


    public override void Calculate(RoamingNPC t)
    {
        if (t.isInvisible) t.RemoveInvisibility();
        try
        {
            GameManager.manager.StopCoroutine(GameManager.manager.waitingCoroutine);
            GameManager.manager.waitingCoroutine = null;
            GameManager.manager.waiting = false;
            GameManager.manager.readingBook = null;
        }
        catch { }

        string attack = "";
        if (t.nextAttack != "")
        {
            attack = t.nextAttack;
            t.SendMessage(attack);
            t.nextAttack = "";
        }
        else
        {
            attack = t.enemySO.attacks[Random.Range(0, t.enemySO.attacks.Count)].ToString();
            t.SendMessage(attack);
        }

    }
}
