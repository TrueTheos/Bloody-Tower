using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Take Damage/Basic")]
public class BasicTakeDamage : BaseAIBehaviour<RoamingNPC>
{
    public BasicWakeUp ToWakeUp;
    public BasicDie ToDie;


    public virtual void TakeDamage(RoamingNPC npc, int amount)
    {
        ToWakeUp.Calculate(npc);

        npc.__currentHp -= amount;

        npc.canvas.GetComponent<Animator>().SetTrigger("Shake");
        if (npc.__currentHp <= 0)
        {
            ToDie.Calculate(npc);
        }
    }







}
