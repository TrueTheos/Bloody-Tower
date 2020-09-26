using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Attack/Basic")]
public class BasicAttack : BaseAIBehaviour<RoamingNPC>
{


    public override void Calculate(RoamingNPC t)
    {
        try
        {
            GameManager.manager.StopCoroutine(GameManager.manager.waitingCoroutine);
            GameManager.manager.waitingCoroutine = null;
            GameManager.manager.waiting = false;
            GameManager.manager.readingBook = null;
        }
        catch { }

        string attack = t.enemySO.attacks[Random.Range(0, t.enemySO.attacks.Count)].ToString();
        t.SendMessage(attack);

    }
}
