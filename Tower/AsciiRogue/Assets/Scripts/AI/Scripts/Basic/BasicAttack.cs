using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Attack/Basic")]
public class BasicAttack : BaseAIBehaviour<RoamingNPC>
{
    public List<BasicAttack> ToAttack;

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

        BasicAttack attack = null;
        if (t.nextAttack != null)
        {
            attack = t.nextAttack;
            attack.Calculate(t);
            t.nextAttack = null;
        }
        else
        {
            attack = ToAttack[Random.Range(0, ToAttack.Count)];
            attack.Calculate(t);
        }

    }
}
