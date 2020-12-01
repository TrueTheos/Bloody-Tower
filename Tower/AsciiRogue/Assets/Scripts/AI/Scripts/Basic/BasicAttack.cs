using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Attack/Basic")]
public class BasicAttack : BaseAIBehaviour<(RoamingNPC source,IUnit target)>
{
    public List<BasicAttack> ToAttack;

    public int MaxRange = 1;
    public int MinRange = 1;

    // Override this of you want some other calculation process in determining ranges
    public virtual bool InRange(Vector2Int a, Vector2Int b)
    {
        int dis = MapUtility.MoveDistance(a, b);
        return dis >= MinRange && dis <= MaxRange;
    }

    public virtual bool BlobInRange(Vector2Int a, Vector2Int b)
    {
        if (MapUtility.MoveDistance(a, b) >= MinRange && MapUtility.MoveDistance(a, b) <= MinRange)
        {
            return true;
        }
        else if (MapUtility.MoveDistance(new Vector2Int(a.x, a.y + 1), b) >= MinRange && MapUtility.MoveDistance(new Vector2Int(a.x, a.y + 1), b) <= MinRange)
        {
            return true;
        }
        else if (MapUtility.MoveDistance(new Vector2Int(a.x, a.y - 1), b) >= MinRange && MapUtility.MoveDistance(new Vector2Int(a.x, a.y - 1), b) <= MinRange)
        {
            return true;
        }
        else if (MapUtility.MoveDistance(new Vector2Int(a.x + 1, a.y), b) >= MinRange && MapUtility.MoveDistance(new Vector2Int(a.x + 1, a.y), b) <= MinRange)
        {
            return true;
        }
        else if (MapUtility.MoveDistance(new Vector2Int(a.x - 1, a.y), b) >= MinRange && MapUtility.MoveDistance(new Vector2Int(a.x - 1, a.y), b) <= MinRange)
        {
            return true;
        }
        else return false;
    }

    public override void Calculate((RoamingNPC source, IUnit target) t)
    {
        if (t.source.isInvisible) t.source.RemoveInvisibility();
        try
        {
            GameManager.manager.StopCoroutine(GameManager.manager.waitingCoroutine);
            GameManager.manager.waitingCoroutine = null;
            GameManager.manager.waiting = false;
            //GameManager.manager.readingBook = null;
        }
        catch { }

        BasicAttack attack = null;
        if (t.source.nextAttack != null)
        {
            attack = t.source.nextAttack;
            attack.Calculate((t.source, t.target));
            t.source.nextAttack = null;
        }
        else
        {
            attack = ToAttack[Random.Range(0, ToAttack.Count)];
            attack.Calculate((t.source, t.target));
        }
    }
}
