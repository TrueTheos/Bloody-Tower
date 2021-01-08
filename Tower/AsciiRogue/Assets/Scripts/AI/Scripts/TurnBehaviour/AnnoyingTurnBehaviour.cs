using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Turn/Annoying")]
public class AnnoyingTurnBehaviour : BasicTurnBehaviour
{
    public override void Calculate(RoamingNPC t)
    {
        if (t.rootDuration > 0) t.rootDuration--;
        else if (t.rooted) t.rooted = false;

        if (t.sleeping) return;

        if (!t.gameObject.transform.parent.gameObject.activeSelf) return;

        t.CurrentTarget = t.playerStats;
        t.LastKnownTargetPos = t.playerStats.pos;

        t.TriggerEffectTasks();
        if (t.CurrentTarget != null && t.CurrentTarget.currHp == 0)
        {
            t.CurrentTarget = null;
        }   

        if (t.CurrentTarget != null)
        {
            if (t.runCounter > 0)
            {
                Vector2Int runCell = t.__position + t.runDirection;

                t.runCounter--;

                MoveTo(t, runCell.x, runCell.y);
            }
            else
            {
                t.path = null;

                t.path = AStar.CalculatePathNoCollisions(t.__position, t.CurrentTarget.pos);

                MoveTo(t, t.path[0].x, t.path[0].y);
            }
        }
    }

    void MoveTo(RoamingNPC t, int x, int y)
    {
        try
        {
            t.Stun(0);
            if (t.CheckStun()) return;

            if(Random.Range(1, 20) <= 3)
            {
                float randomMessage = Random.Range(0, 1f);

                if (randomMessage <= 0.25f) GameManager.manager.UpdateMessages("You hear a quiet chuckle...");
                else if(randomMessage <= .5f) GameManager.manager.UpdateMessages("You hear a soft, child's voice utter unintelligible words...");
                else if(randomMessage <= 0.75f) GameManager.manager.UpdateMessages("Hafh... ya...");
            }

            if(MapManager.map[x,y].hasPlayer)
            {
                t.runCounter = 20;

                t.runDirection = t.__position - MapManager.playerPos;

                DungeonGenerator.dungeonGenerator.DrawMap(MapManager.map);

                return;
            }
            else if ((MapManager.map[x, y].enemy == null || MapManager.map[x, y].enemy.GetComponent<RoamingNPC>().enemySO == t.enemySO) && !t.rooted)
            {
                MapManager.map[t.__position.x, t.__position.y].letter = "";
                MapManager.map[t.__position.x, t.__position.y].enemy = null;
                MapManager.map[t.__position.x, t.__position.y].timeColor = new Color(0, 0, 0);

                t.__position = new Vector2Int(x, y);

                if (!t.isInvisible) MapManager.map[x, y].letter = t.EnemySymbol;
                MapManager.map[x, y].enemy = t.gameObject;
                MapManager.map[x, y].timeColor = t.EnemyColor;            

                DungeonGenerator.dungeonGenerator.DrawMap(MapManager.map);

                return;
            }
        }
        catch { }

        DungeonGenerator.dungeonGenerator.DrawMap(MapManager.map);
    }
}
