using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Attack/Basic")]
public class BasicAttack : BaseAIBehaviour<RoamingNPC>
{
    public List<BasicAttack> ToAttack;



    public virtual void Attack(RoamingNPC source, IUnit target)
    {
        if (source.isInvisible) source.RemoveInvisibility();
        try
        {
            GameManager.manager.StopCoroutine(GameManager.manager.waitingCoroutine);
            GameManager.manager.waitingCoroutine = null;
            GameManager.manager.waiting = false;
            GameManager.manager.readingBook = null;
        }
        catch { }

        BasicAttack attack = null;
        if (source.nextAttack != null)
        {
            attack = source.nextAttack;
            attack.Attack(source,target);
            source.nextAttack = null;
        }
        else
        {
            attack = ToAttack[Random.Range(0, ToAttack.Count)];
            attack.Attack(source,target);
        }
    }
}
