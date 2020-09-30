using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperTurnBehaviour : BasicTurnBehaviour
{

    public List<BasicAttack> Attacks;


    public BasicAttack GetAttack(RoamingNPC t)
    {
        if (t.nextAttack != null)
        {
            BasicAttack att = t.nextAttack;
            t.nextAttack = null;
            return att;
        }
        if (Attacks.Count > 0)
        {
            return Attacks[Random.Range(0, Attacks.Count)];
        }
        return null;
    }

    public override void Calculate(RoamingNPC t)
    {
        if (t.rootDuration > 0) t.rootDuration--;
        else if (t.rooted) t.rooted = false;

        if (t.enemySO._Behaviour != EnemiesScriptableObject.E_behaviour.npc)
        {
            if (t.sleeping) return;

            if (!t.gameObject.transform.parent.gameObject.activeSelf) return;

            t.TriggerEffectTasks();

            Vector2Int enemyPos = new Vector2Int(-100, -100);
            float minDis = 10000;

            foreach (Vector2Int pos in FoV.GetEnemyFoV(t.__position))
            {
                try
                {
                    if (MapManager.map[pos.x, pos.y].enemy != null && Vector2Int.Distance(MapManager.playerPos,pos) <= 3 )
                    {
                        if (Vector2Int.Distance(MapManager.playerPos, pos)<minDis)
                        {
                            minDis = Vector2Int.Distance(MapManager.playerPos, pos);
                            enemyPos = pos;
                        }
                    }
                }
                catch { }
            }

            if (enemyPos != new Vector2Int(-100,-100))
            {

            }


        }
            
    }


}
