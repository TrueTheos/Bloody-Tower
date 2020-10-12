using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrazyTurnBehaviour : BasicTurnBehaviour
{


    public override void Calculate(RoamingNPC t)
    {
        if (t.rootDuration > 0) t.rootDuration--;
        else if (t.rooted) t.rooted = false;

        if (t.enemySO._Behaviour != EnemiesScriptableObject.E_behaviour.npc)
        {
            if (t.sleeping) return;

            if (!t.gameObject.transform.parent.gameObject.activeSelf) return;

            t.TriggerEffectTasks();
            if (t.CurrentTarget != null && t.CurrentTarget.currHp == 0)
            {
                t.CurrentTarget = null;
            }


            // check if current target is still valid
            if (!(t.CurrentTarget == null))
            {
                // TODO: make it so that we loose a target when out of sight

                // we are targeting something
                // range here
                if (MapUtility.MoveDistance(t.pos, t.CurrentTarget.pos) == 1)
                {
                    // the target is in attackrange so we dont change anything

                }
                else
                {
                    // target may be running away. we are looking at other potential targets that are near us
                    if (t.RetailiationList.Count > 0)
                    {
                        // we have recently been attacked by somebody
                        var alt = t.RetailiationList.ToArray().Check((u) => MapUtility.MoveDistance(u.pos, t.pos) == 1);
                        if (alt.Length > 0)
                        {
                            t.CurrentTarget = alt.GetRandom();
                        }
                    }
                }
            }
            else
            {
                // look for possible target
                var view = FoV.GetEnemyFoV(t.__position).ToArray().Check((p) => MapManager.map[p.x, p.y].hasPlayer || MapManager.map[p.x, p.y].enemy != null);

                if (view.Length > 0)
                {
                    List<IUnit> possibleTargets = new List<IUnit>();

                    IUnit nt = null;


                    foreach (var pos in view)
                    {
                        if (MapManager.map[pos.x, pos.y].enemy != null)
                        {
                            // we found somebody to attack
                            if (nt == null)
                            {
                                nt = MapManager.map[pos.x, pos.y].enemy.GetComponent<RoamingNPC>();
                                t.LastKnownTargetPos = nt.pos;
                            }
                            else
                            {
                                if (MapUtility.MoveDistance(t.pos, nt.pos) > MapUtility.MoveDistance(t.pos, pos))
                                {
                                    nt = MapManager.map[pos.x, pos.y].enemy.GetComponent<RoamingNPC>();
                                    t.LastKnownTargetPos = nt.pos;
                                }
                            }

                        }
                        if (MapManager.map[pos.x, pos.y].hasPlayer)
                        {
                            if (nt == null)
                            {
                                nt = t.playerStats;
                                t.LastKnownTargetPos = nt.pos;
                            }
                            else
                            {
                                if (MapUtility.MoveDistance(t.pos, nt.pos) > MapUtility.MoveDistance(t.pos, pos))
                                {
                                    nt = t.playerStats;
                                    t.LastKnownTargetPos = nt.pos;
                                }
                            }
                        }
                    }
                    if (nt != null)
                    {
                        // found the closest possible target:
                        t.CurrentTarget = nt;
                        t.LastKnownTargetPos = nt.pos;
                    }
                }
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

            if (t.CurrentTarget != null)
            {

                var att = GetAttack(t);
                if (att.InRange(t.__position, t.CurrentTarget.pos))
                {
                    att.Attack(t, t.CurrentTarget);
                    return;
                }
                /*
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
                */

                t.path = null;

                t.path = AStar.CalculatePath(t.__position, t.CurrentTarget.pos);

                t.MoveTo(t.path[0].x, t.path[0].y);
            }
            else
            {
                t.MoveTo(t.__position.x + Random.Range(-1, 2), t.__position.y + Random.Range(-1, 2)); //move to random direction
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
