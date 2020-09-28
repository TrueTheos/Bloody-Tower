using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Attack/Fading Bite")]
public class FadingBite : BasicAttack
{


    public override void Calculate(RoamingNPC t)
    {
        t.StartCoroutine(_FadingBite(t));
    }
    private IEnumerator _FadingBite(RoamingNPC t)
    {
        t.manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(t.EnemyColor)}>{t.enemySO.name}</color> used <color=red>Fading Bite</color>!");
        ToAttack[Random.Range(0,ToAttack.Count)].Calculate(t);
        yield return new WaitForSeconds(.2f);

        //run away
        t.runDirection = t.__position - MapManager.playerPos;

        Vector2Int runCell = t.__position + t.runDirection;

        if (MapManager.map[runCell.x, runCell.y].type == "Wall")
        {
            if (Random.Range(0, 100) > 78)
            {
                t.manager.UpdateMessages($"<color=#{ColorUtility.ToHtmlStringRGB(t.EnemyColor)}>{t.enemySO.name}</color> used <color=lightblue>Jump</color>!");
                runCell = new Vector2Int(MapManager.playerPos.x + (MapManager.playerPos.x - t.__position.x), MapManager.playerPos.y + (MapManager.playerPos.y - t.__position.y));
                t.MoveTo(runCell.x, runCell.y);
            }
        }
        else
        {
            t.runCounter--;

            t.path = null;

            t.path = AStar.CalculatePath(t.__position, runCell);

            t.MoveTo(t.path[0].x, t.path[0].y);
        }

        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
    }

}
