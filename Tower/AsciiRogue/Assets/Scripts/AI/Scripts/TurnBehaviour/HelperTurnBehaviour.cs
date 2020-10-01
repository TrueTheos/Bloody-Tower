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
                if (t.Board["Target"] != null && t.Board["Target"] is RoamingNPC)
                {
                    if (Vector2Int.Distance(t.__position,((RoamingNPC)t.Board["Target"]).__position)>4||((RoamingNPC)t.Board["Target"]).gameObject != null)
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
                Vector2Int enemyPos = new Vector2Int(-100, -100);
                float minDis = 10000;
                bool found = false;
                foreach (Vector2Int pos in FoV.GetEnemyFoV(t.__position))
                {
                    try
                    {
                        if (MapManager.map[pos.x, pos.y].enemy != null && 
                            MapManager.map[pos.x, pos.y].enemy !=t.gameObject &&
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
                    t.Board["Target"] = MapManager.map[enemyPos.x, enemyPos.y].enemy.GetComponent<RoamingNPC>();
                }
            }

            if (t.Board.ContainsKey("Target") && t.Board["Target"]!=null && t.Board["Target"] is RoamingNPC)
            {
                RoamingNPC npc = (RoamingNPC)t.Board["Target"];
                int distance = Mathf.Max(Mathf.Abs(npc.__position.x - t.__position.x), Mathf.Abs(npc.__position.y - t.__position.y));
                // move to target and attack
                if (distance == 1)
                {
                    GetAttack(t).Attack(t,npc);
                }
                else
                {
                    t.path = AStar.CalculatePath(t.__position, PlayerMovement.playerMovement.position);

                    t.MoveTo(t.path[0].x, t.path[0].y);
                }
            }
            else
            {
                // move to player 
                // check if next to player 
                int distance = Mathf.Max(Mathf.Abs(PlayerMovement.playerMovement.position.x - t.__position.x), Mathf.Abs(PlayerMovement.playerMovement.position.y - t.__position.y));


                Debug.Log(distance);
                if ( distance == 1)
                {
                    // we are too close
                    Vector2Int diff =   t.__position - PlayerMovement.playerMovement.position ;

                    // we move away from the player

                    t.MoveTo(t.__position.x + diff.x, t.__position.y + diff.y);
                }
                else
                {
                    // check how far away
                    // if far away follow
                    if (distance > 2)
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


}
