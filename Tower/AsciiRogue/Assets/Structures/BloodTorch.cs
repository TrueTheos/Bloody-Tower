using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodTorch : Structure
{
    public bool opened = false;
    public Vector2Int position;

    public override void Use()
    {
        if (opened)
        {
            GameManager.manager.UpdateMessages($"<color=red>You destroyed torch!</color>");
            GameManager.manager.UpdateMessages($"<color=red><i>You hear a terrifying scream!</i></color>");         
            GameManager.manager.StartCoroutine(GameManager.manager.DestroyTorchEffect(position));
            for (int i = 0; i < 6; i++)
            {
                GameManager.manager.enemySpawner.SpawnAt(MapManager.CurrentFloor, Random.Range(position.x - 1, position.x + 2), Random.Range(position.y - 1, position.y + 2), GameManager.manager.enemySpawner.Zombie);
            }
            MapManager.map[position.x, position.y].isWalkable = true;
            MapManager.map[position.x, position.y].isOpaque = false;
            MapManager.map[position.x, position.y].baseChar = ".";
            MapManager.map[position.x, position.y].structure = null;
            DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
            GameObject.Find("MainCanvas").gameObject.GetComponent<Animator>().SetTrigger("Shake");
        }

        if (!opened)
        {
            GameManager.manager.UpdateMessages($"Do you want to destroy this torch?");
            opened = true;
        }
    }

    public override void WalkIntoTrigger()
    {

    }
}
