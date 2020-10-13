using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Turn/Helper")]
public class HelperTurnBehaviour : BasicTurnBehaviour
{


    public override void Calculate(RoamingNPC t)
    {
        if (t.rootDuration > 0) t.rootDuration--;
        else if (t.rooted) t.rooted = false;

        if (t.enemySO._Behaviour != EnemiesScriptableObject.E_behaviour.npc)
        {
            if (t.sleeping)
            {
                int distance = Mathf.Max(Mathf.Abs(PlayerMovement.playerMovement.position.x - t.__position.x), Mathf.Abs(PlayerMovement.playerMovement.position.y - t.__position.y));
                if (distance < 7)
                {
                    t.sleeping = false;
                }
                else
                {
                    //Debug.Log("I am sleeping");
                    return;
                }
            }
            

            if (!t.gameObject.transform.parent.gameObject.activeSelf) return;

            t.TriggerEffectTasks();

            bool searchEnemy = true;

            if (t.Board.ContainsKey("Target"))
            {                
                if (t.Board["Target"] != null && t.Board["Target"] is RoamingNPC)
                {
                    //Debug.Log("Has Target");
                    RoamingNPC testNPC = (RoamingNPC)t.Board["Target"];
                    if ((testNPC == null) || Vector2Int.Distance(PlayerMovement.playerMovement.position, testNPC.__position) > 5 || testNPC.gameObject == null)
                    {
                        t.Board["Target"] = null;
                    }
                    else
                    {
                        searchEnemy = false;
                    }
                }
            }
            if (searchEnemy)
            {
                //Debug.Log("Has to search enemy");
                Vector2Int enemyPos = new Vector2Int(-100, -100);
                float minDis = 10000;
                bool found = false;
                foreach (Vector2Int pos in FoV.GetEnemyFoV(t.__position))
                {
                    try
                    {
                        int dis = Mathf.Max(Mathf.Abs(PlayerMovement.playerMovement.position.x - pos.x), Mathf.Abs(PlayerMovement.playerMovement.position.y - pos.y));
                        if (MapManager.map[pos.x, pos.y].enemy != null &&
                            MapManager.map[pos.x, pos.y].enemy != t.gameObject &&
                            MapManager.map[pos.x, pos.y].enemy.GetComponent<RoamingNPC>().enemySO.MyTurnAI != t.enemySO.MyTurnAI &&
                            dis <= 4)
                        {
                            if (dis < minDis)
                            {
                                minDis = dis;
                                enemyPos = pos;
                                found = true;
                            }
                        }
                    }
                    catch { }
                }

                if (found)
                {
                    //Debug.Log("Found enemy");
                    t.Board["Target"] = MapManager.map[enemyPos.x, enemyPos.y].enemy.GetComponent<RoamingNPC>();
                }
            }
            if (t.Board.ContainsKey("Target"))
            {
                //Debug.Log("Has enemy missing: " + (t.Board["Target"] == null));
            }
            if (t.Board.ContainsKey("Target") && t.Board["Target"] != null && t.Board["Target"] is RoamingNPC npc)
            {
                //Debug.Log("Target aquired");

                int distance = Mathf.Max(Mathf.Abs(npc.__position.x - t.__position.x), Mathf.Abs(npc.__position.y - t.__position.y));
                // move to target and attack

                var att = GetAttack(t);
                if (att.InRange(t.__position, npc.pos))
                {
                    att.Attack(t, npc);
                    return;
                }
                else
                {
                    t.path = null;
                    t.path = AStar.CalculatePath(t.__position, npc.__position);
                    Debug.Log(t.path.Count);
                    if (!(PlayerMovement.playerMovement.position == t.path[0] || PlayerMovement.playerMovement.position == t.path[1]))
                    {
                       // Debug.Log("Move to enemy");
                        t.MoveTo(t.path[0].x, t.path[0].y);
                        return;
                    }
                }
            }


            // move to player 
            // check if next to player 
            int pDistance = Mathf.Max(Mathf.Abs(PlayerMovement.playerMovement.position.x - t.__position.x), Mathf.Abs(PlayerMovement.playerMovement.position.y - t.__position.y));


            //Debug.Log(pDistance);
            if (pDistance == 1)
            {
                // we are too close
                Vector2Int diff = t.__position - PlayerMovement.playerMovement.position;

                // we move away from the player
                if (MapUtility.IsMoveable(t.__position.x + diff.x, t.__position.y + diff.y))
                {
                    t.MoveTo(t.__position.x + diff.x, t.__position.y + diff.y);
                }
                else
                {
                    // find another way
                    Vector2Int[] alternatives = MapUtility.Box1
                        .Copy()
                        .Check((pos) => MapUtility.IsMoveable(pos) && MapUtility.MoveDistance(pos, PlayerMovement.playerMovement.position) == 2);
                    if (alternatives.Length>0)
                    {
                        Vector2Int target = alternatives.GetRandom();
                        t.MoveTo(target.x, target.y);
                    }
                }
            }
            else
            {
                // check how far away
                // if far away follow
                if (pDistance > 2)
                {
                    t.path = null;
                    //Debug.Log("Follow");
                    t.path = AStar.CalculatePath(t.__position, PlayerMovement.playerMovement.position);
                    //Debug.Log(t.path.Count);
                    t.MoveTo(t.path[0].x, t.path[0].y);
                }
                else
                {
                    // else go random
                    Vector2Int[] alternatives = MapUtility.Box1
                        .Copy().Combine(MapUtility.Origin.Copy())
                        .MoveCenter(t.__position)
                        .Check((pos) => MapUtility.IsMoveable(pos) && MapUtility.MoveDistance(pos, PlayerMovement.playerMovement.position) > 1);
                    if (alternatives.Length > 0)
                    {
                        Vector2Int target = alternatives.GetRandom();
                        t.MoveTo(target.x, target.y);
                    }
                }
            }




        }
            
    }


}
