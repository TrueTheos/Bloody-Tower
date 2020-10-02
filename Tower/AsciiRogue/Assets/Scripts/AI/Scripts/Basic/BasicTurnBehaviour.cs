using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Turn/Basic")]
public class BasicTurnBehaviour : BaseAIBehaviour<RoamingNPC>
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
        if (Attacks.Count >0)
        {
            return Attacks[Random.Range(0, Attacks.Count)];
        }
        return null;
    }

    public override void Calculate(RoamingNPC t)
    {
        if (t.rootDuration > 0) t.rootDuration--;
        else if (t.rooted) t.rooted = false;

        t.playerDetected = false;

        if (t.enemySO._Behaviour != EnemiesScriptableObject.E_behaviour.npc)
        {
            if (t.sleeping) return;

            if (!t.gameObject.transform.parent.gameObject.activeSelf) return;

            t.TriggerEffectTasks();

            foreach (Vector2Int pos in FoV.GetEnemyFoV(t.__position))
            {
                try
                {
                    if (MapManager.map[pos.x, pos.y].hasPlayer && !t.playerStats.isInvisible)
                    {
                        t.playerDetected = true;
                        t._x = t.howLongWillFololwInvisiblepLayer;
                    }
                }
                catch { }
            }

            switch (t.enemySO._Behaviour)
            {
                case EnemiesScriptableObject.E_behaviour.cowardly:
                    if (t.__currentHp <= (t.maxHp / 2) && (int)Random.Range(0, 2) == 1 && t.runCounter > 0 || (t.runCounter > 0 && t.runCounter < 5)) //RUN FROM PLAYER
                    {
                        t.runDirection = t.__position - MapManager.playerPos;

                        Vector2Int runCell = t.__position + t.runDirection;

                        t.runCounter--;

                        t.path = null;

                        t.path = AStar.CalculatePath(t.__position, runCell);

                        t.MoveTo(t.path[0].x, t.path[0].y);
                        return;
                    }
                    else t.runCounter = 5;
                    break;
                case EnemiesScriptableObject.E_behaviour.recovers:
                    if (t.__currentHp <= (t.maxHp / 2) && t.hpRegenCooldown == 0)
                    {
                        t.hpRegenCooldown--;
                        t.__currentHp += Mathf.FloorToInt(t.maxHp * .25f);
                        return;
                    }
                    else if (t.hpRegenCooldown < 10 && t.hpRegenCooldown > 0) t.hpRegenCooldown--;
                    break;
            }

            if (t.playerDetected)
            {
                if (new Vector2Int(t.__position.x - 1, t.__position.y) == MapManager.playerPos)
                {
                    GetAttack(t).Attack(t, t.playerStats);
                    return;
                }
                else if (new Vector2Int(t.__position.x + 1, t.__position.y) == MapManager.playerPos)
                {
                    GetAttack(t).Attack(t, t.playerStats);
                    return;
                }
                else if (new Vector2Int(t.__position.x, t.__position.y - 1) == MapManager.playerPos)
                {
                    GetAttack(t).Attack(t, t.playerStats);
                    return;
                }
                else if (new Vector2Int(t.__position.x, t.__position.y + 1) == MapManager.playerPos)
                {
                    GetAttack(t).Attack(t, t.playerStats);
                    return;
                }
                else if (new Vector2Int(t.__position.x - 1, t.__position.y - 1) == MapManager.playerPos)
                {
                    GetAttack(t).Attack(t, t.playerStats);
                    return;
                }
                else if (new Vector2Int(t.__position.x + 1, t.__position.y - 1) == MapManager.playerPos)
                {
                    GetAttack(t).Attack(t, t.playerStats);
                    return;
                }
                else if (new Vector2Int(t.__position.x - 1, t.__position.y + 1) == MapManager.playerPos)
                {
                    GetAttack(t).Attack(t, t.playerStats);
                    return;
                }
                else if (new Vector2Int(t.__position.x + 1, t.__position.y + 1) == MapManager.playerPos)
                {
                    GetAttack(t).Attack(t, t.playerStats);
                    return;
                }

                t.path = null;

                t.path = AStar.CalculatePath(t.__position, MapManager.playerPos);

                t.MoveTo(t.path[0].x, t.path[0].y);
            }
            else
            {
                if (t._x > 0)
                {
                    t._x--;

                    t.path = null;

                    t.path = AStar.CalculatePath(t.__position, MapManager.playerPos);

                    t.MoveTo(t.path[0].x, t.path[0].y);
                }
                else
                {
                    t.MoveTo(t.__position.x + Random.Range(-1, 2), t.__position.y + Random.Range(-1, 2)); //move to random direction
                }
            }
        }
        else
        {
            if (!t.gameObject.transform.parent.gameObject.activeSelf) return;

            if (t.enemySO.finishedDialogue)
            {
                t.MoveTo(t.__position.x + (int)Random.Range(-1, 2), t.__position.y + (int)Random.Range(-1, 2)); //move to random direction
            }
        }
    }





}
