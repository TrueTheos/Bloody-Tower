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
            if (t.sleeping) t.sleeping = false;

            if (!t.gameObject.transform.parent.gameObject.activeSelf) return;

            t.TriggerEffectTasks();

            bool searchEnemy = true;

            if (t.Board.ContainsKey("Target"))
            {
                Debug.Log("Has Target");
                if (t.Board["Target"] != null && t.Board["Target"] is RoamingNPC)
                {
                    RoamingNPC testNPC = (RoamingNPC)t.Board["Target"];
                    if ((testNPC == null) || Vector2Int.Distance(t.__position, testNPC.__position) > 5 || testNPC.gameObject == null)
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
                Debug.Log("Has to search enemy");
                Vector2Int enemyPos = new Vector2Int(-100, -100);
                float minDis = 10000;
                bool found = false;
                foreach (Vector2Int pos in FoV.GetEnemyFoV(t.__position))
                {
                    try
                    {
                        if (MapManager.map[pos.x, pos.y].enemy != null &&
                            MapManager.map[pos.x, pos.y].enemy != t.gameObject &&
                            MapManager.map[pos.x, pos.y].enemy.GetComponent<RoamingNPC>().enemySO.MyTurnAI != t.enemySO.MyTurnAI &&
                            Vector2Int.Distance(PlayerMovement.playerMovement.position, pos) <= 3)
                        {
                            if (Vector2Int.Distance(PlayerMovement.playerMovement.position, pos) < minDis)
                            {
                                minDis = Vector2Int.Distance(PlayerMovement.playerMovement.position, pos);
                                enemyPos = pos;
                                found = true;
                            }
                        }
                    }
                    catch { }
                }

                if (found)
                {
                    Debug.Log("Found enemy");
                    t.Board["Target"] = MapManager.map[enemyPos.x, enemyPos.y].enemy.GetComponent<RoamingNPC>();
                }
            }
            if (t.Board.ContainsKey("Target"))
            {
                Debug.Log("Has enemy missing: " + t.Board["Target"] == null);
            }
            if (t.Board.ContainsKey("Target") && t.Board["Target"] != null && t.Board["Target"] is RoamingNPC)
            {
                Debug.Log("Try attack");
                RoamingNPC npc = (RoamingNPC)t.Board["Target"];
                int distance = Mathf.Max(Mathf.Abs(npc.__position.x - t.__position.x), Mathf.Abs(npc.__position.y - t.__position.y));
                // move to target and attack
                if (distance == 1)
                {
                    Debug.Log("Attack enemy");
                    GetAttack(t).Attack(t, npc);
                    return;
                }
                else
                {
                    t.path = AStar.CalculatePath(t.__position, PlayerMovement.playerMovement.position);

                    if (PlayerMovement.playerMovement.position != t.path[0] || PlayerMovement.playerMovement.position != t.path[1])
                    {
                        Debug.Log("Move to enemy");
                        t.MoveTo(t.path[0].x, t.path[0].y);
                        return;
                    }
                }
            }


            // move to player 
            // check if next to player 
            int pDistance = Mathf.Max(Mathf.Abs(PlayerMovement.playerMovement.position.x - t.__position.x), Mathf.Abs(PlayerMovement.playerMovement.position.y - t.__position.y));


            Debug.Log(pDistance);
            if (pDistance == 1)
            {
                // we are too close
                Vector2Int diff = t.__position - PlayerMovement.playerMovement.position;

                // we move away from the player

                t.MoveTo(t.__position.x + diff.x, t.__position.y + diff.y);
            }
            else
            {
                // check how far away
                // if far away follow
                if (pDistance > 2)
                {
                    Debug.Log("Follow");
                    t.path = AStar.CalculatePath(t.__position, PlayerMovement.playerMovement.position);
                    t.MoveTo(t.path[0].x, t.path[0].y);
                }
                else
                {
                    // else go random
                    t.MoveTo(t.__position.x + Random.Range(-1, 2), t.__position.y + Random.Range(-1, 2)); //move to random direction
                }
            }




        }
            
    }


}
