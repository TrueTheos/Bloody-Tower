using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Turn/Blob")]
public class BlobTurnBehaviour : BasicTurnBehaviour
{

    public override void Calculate(RoamingNPC t)
    {
        if (t.rootDuration > 0) t.rootDuration--;
        else if (t.rooted) t.rooted = false;

        //t.playerDetected = false;

        if (t.sleeping) return;

        if (!t.gameObject.transform.parent.gameObject.activeSelf) return;

        t.TriggerEffectTasks();
        if (t.CurrentTarget != null && t.CurrentTarget.currHp == 0)
        {
            t.CurrentTarget = null;
        }

        // check for out of sight enemy
        if (!(t.CurrentTarget == null))
        {
            if (!FoV.InLineOfSight(t.pos, t.CurrentTarget.pos))
            {
                // out of sight out of mind i guess
                t.LastKnownTargetPos = t.CurrentTarget.pos;

                t.CurrentTarget = null;
                // TODO: make enemy run to last known position
            }
        }


        // check if current target is still valid
        if (!(t.CurrentTarget == null))
        {
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
                    if (MapManager.map[pos.x, pos.y].enemy != null && MapManager.map[pos.x, pos.y].enemy.GetComponent<RoamingNPC>().enemySO.MyTurnAI is HelperTurnBehaviour)
                    {
                        // we found a doggo to attack
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

        if (t.CurrentTarget != null)
        {
            var att = GetAttack(t);
            if (att.BlobInRange(t.__position, t.CurrentTarget.pos))
            {
                att.Calculate((t, t.CurrentTarget));
                return;
            }

            t.path = null;

            t.path = AStar.CalculatePath(t.__position, t.CurrentTarget.pos);

            BlobMoveTo(t, t.path[0].x, t.path[0].y);
        }
        else
        {
            if (t._x > 0)
            {
                t._x--;

                t.path = null;

                t.path = AStar.CalculatePath(t.__position, MapManager.playerPos);

                BlobMoveTo(t, t.path[0].x, t.path[0].y);
            }
            else
            {
                //BlobMoveTo(t, t.__position.x + Random.Range(-1, 2), t.__position.y + Random.Range(-1, 2)); //move to random direction
            }
        }
    }

    void BlobMoveTo(RoamingNPC t, int x, int y)
    {
        try
        {
            t.Stun(0);
            if (t.CheckStun()) return;

            Debug.Log("1");

            if ((MapManager.map[x, y].enemy == null || MapManager.map[x,y].enemy.GetComponent<RoamingNPC>().enemySO == t.enemySO) && !t.rooted && MapManager.map[x, y].type != "Wall")
            {
                MapManager.map[t.__position.x, t.__position.y].letter = "";
                MapManager.map[t.__position.x, t.__position.y].isWalkable = true;
                MapManager.map[t.__position.x, t.__position.y].enemy = null;
                MapManager.map[t.__position.x, t.__position.y].timeColor = new Color(0, 0, 0);

                MapManager.map[t.__position.x - 1, t.__position.y].letter = "";
                MapManager.map[t.__position.x - 1, t.__position.y].isWalkable = true;
                MapManager.map[t.__position.x - 1, t.__position.y].enemy = null;
                MapManager.map[t.__position.x - 1, t.__position.y].timeColor = new Color(0, 0, 0);

                MapManager.map[t.__position.x + 1, t.__position.y].letter = "";
                MapManager.map[t.__position.x + 1, t.__position.y].isWalkable = true;
                MapManager.map[t.__position.x + 1, t.__position.y].enemy = null;
                MapManager.map[t.__position.x + 1, t.__position.y].timeColor = new Color(0, 0, 0);

                MapManager.map[t.__position.x, t.__position.y + 1].letter = "";
                MapManager.map[t.__position.x, t.__position.y + 1].isWalkable = true;
                MapManager.map[t.__position.x, t.__position.y + 1].enemy = null;
                MapManager.map[t.__position.x, t.__position.y + 1].timeColor = new Color(0, 0, 0);

                MapManager.map[t.__position.x, t.__position.y - 1].letter = "";
                MapManager.map[t.__position.x, t.__position.y - 1].isWalkable = true;
                MapManager.map[t.__position.x, t.__position.y - 1].enemy = null;
                MapManager.map[t.__position.x, t.__position.y - 1].timeColor = new Color(0, 0, 0);

                t.__position = new Vector2Int(x, y);

                if (!t.isInvisible) MapManager.map[x, y].letter = t.EnemySymbol;
                MapManager.map[x, y].isWalkable = false;
                MapManager.map[x, y].enemy = t.gameObject;
                MapManager.map[x, y].timeColor = t.EnemyColor;

                if(MapManager.map[x + 1, y].isWalkable)
                {
                    if (!t.isInvisible) MapManager.map[x + 1, y].letter = t.EnemySymbol;
                    MapManager.map[x + 1, y].isWalkable = false;
                    MapManager.map[x + 1, y].enemy = t.gameObject;
                    MapManager.map[x + 1, y].timeColor = t.EnemyColor;
                }

                if (MapManager.map[x - 1, y].isWalkable)
                {
                    if (!t.isInvisible) MapManager.map[x - 1, y].letter = t.EnemySymbol;
                    MapManager.map[x - 1, y].isWalkable = false;
                    MapManager.map[x - 1, y].enemy = t.gameObject;
                    MapManager.map[x - 1, y].timeColor = t.EnemyColor;
                }

                if (MapManager.map[x, y - 1].isWalkable)
                {
                    if (!t.isInvisible) MapManager.map[x, y - 1].letter = t.EnemySymbol;
                    MapManager.map[x, y - 1].isWalkable = false;
                    MapManager.map[x, y - 1].enemy = t.gameObject;
                    MapManager.map[x, y - 1].timeColor = t.EnemyColor;
                }

                if (MapManager.map[x, y + 1].isWalkable)
                {
                    if (!t.isInvisible) MapManager.map[x, y + 1].letter = t.EnemySymbol;
                    MapManager.map[x, y + 1].isWalkable = false;
                    MapManager.map[x, y + 1].enemy = t.gameObject;
                    MapManager.map[x, y + 1].timeColor = t.EnemyColor;
                }

                DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);

                return;
            }
        }
        catch { }

        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
    }
}
