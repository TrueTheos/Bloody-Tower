using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivingMonster : MonoBehaviour
{
    public ItemScriptableObject candle;
    public ItemScriptableObject fingerOfDeath;
    public Canvas shake;

    public bool TestRevive(Vector2Int pos, Item item)
    {
        try
        {
            if (MapManager.map[pos.x + 1, pos.y + 1].item.GetComponent<Item>().iso.I_name == candle.I_name
               && MapManager.map[pos.x - 1, pos.y - 1].item.GetComponent<Item>().iso.I_name == candle.I_name
               && MapManager.map[pos.x + 1, pos.y - 1].item.GetComponent<Item>().iso.I_name == candle.I_name
               && MapManager.map[pos.x - 1, pos.y + 1].item.GetComponent<Item>().iso.I_name == candle.I_name)
            {
                if (item.iso.I_name == fingerOfDeath.I_name)
                {
                    if (MapManager.map[pos.x, pos.y].structure is Corps corpse)
                    {
                        GameManager.manager.enemySpawner.SpawnAt(MapManager.CurrentFloor, pos.x, pos.y + 1, corpse.enemyBody, "false");
                        StartCoroutine(SummonMonster());
                        return true;
                    }
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    IEnumerator SummonMonster()
    {
        GameManager.manager.UpdateMessages("<i>You hear a frightening sound, the lights around you grow intense, then you see a creature appearing out of nowhere.</i>");
        GameManager.manager.CloseEQ();
        yield return new WaitForSeconds(.1f);
        shake.GetComponent<Animator>().SetTrigger("Shake");
        yield return new WaitForSeconds(.1f);
        shake.GetComponent<Animator>().SetTrigger("Shake");
        yield return new WaitForSeconds(.1f);
        shake.GetComponent<Animator>().SetTrigger("Shake");

        Vector2Int playerPos = MapManager.playerPos;

        int x = 0;

        for (int i = 1; i < 60; i++)
        {
            x = playerPos.x;
            for (int i1 = playerPos.y + i; i1 >= playerPos.y; i1--)
            {
                try { MapManager.map[x, i1].decoy = "<color=purple>*</color>"; }
                catch { }

                try { MapManager.map[x, playerPos.y + (playerPos.y - i1)].decoy = "<color=purple>*</color>"; }
                catch { }

                try { MapManager.map[x, i1].decoy = "<color=purple>*</color>"; }
                catch { }

                try { MapManager.map[x, playerPos.y + (playerPos.y - i1)].decoy = "<color=purple>*</color>"; }
                catch { }
                x++;
            }

            x = playerPos.x;

            for (int i1 = playerPos.y + i; i1 >= playerPos.y; i1--)
            {
                try { MapManager.map[x, i1].decoy = "<color=purple>*</color>"; }
                catch { }

                try { MapManager.map[x, playerPos.y + (playerPos.y - i1)].decoy = "<color=purple>*</color>"; }
                catch { }

                try { MapManager.map[x, i1].decoy = "<color=purple>*</color>"; }
                catch { }

                try { MapManager.map[x, playerPos.y + (playerPos.y - i1)].decoy = "<color=purple>*</color>"; }
                catch { }
                x--;
            }
            yield return new WaitForSeconds(0.001f);
            DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
        }

        yield return null;
    }
}
