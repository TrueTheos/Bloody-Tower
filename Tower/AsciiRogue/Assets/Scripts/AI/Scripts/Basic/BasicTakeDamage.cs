using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Take Damage/Basic")]
public class BasicTakeDamage : BaseAIBehaviour<RoamingNPC>
{
    public BasicWakeUp ToWakeUp;


    public virtual void TakeDamage(RoamingNPC npc, int amount, ItemScriptableObject.damageType dmgType)
    {
        ToWakeUp.Calculate(npc);

        switch(dmgType)
        {
            case ItemScriptableObject.damageType.normal:
                npc.__currentHp -= amount;
                break;
            case ItemScriptableObject.damageType.light:
                if(npc.enemySO.W_light)
                {
                    npc.__currentHp -= Mathf.RoundToInt(amount * 1.5f);
                }
                else npc.__currentHp -= amount;
                break;
            case ItemScriptableObject.damageType.magic:
                if (npc.enemySO.W_magic)
                {
                    npc.__currentHp -= Mathf.RoundToInt(amount * 1.5f);
                }
                else npc.__currentHp -= amount;
                break;
        } 

        npc.canvas.GetComponent<Animator>().SetTrigger("Shake");
        if (npc.__currentHp <= 0)
        {
            npc.enemySO.MyKill.Calculate(npc);
        }
    }
}
