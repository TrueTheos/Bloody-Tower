﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePointer : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;

    private Vector2Int mousePos = new Vector2Int();

    public List<Vector2Int> path;

    private Rect mapRect;

    private bool selected;

    private PlayerMovement playerMovement;

    private bool _selected
    {
        get
        {
            return selected;
        }
        set
        {
            if (value != selected)
            {
                selected = value;
                DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
            }
        }
    }

    void Start()
    {
        mapRect = new Rect(pointA.transform.position.x, pointA.transform.position.y, pointB.transform.position.x - pointA.transform.position.x, pointB.transform.position.y - pointA.transform.position.y);
        playerMovement = GameManager.manager.player;
    }

    // Update is called once per frame
    void Update()
    {
        if(mapRect.Contains(Input.mousePosition))
        {
            mousePos = new Vector2Int(Mathf.FloorToInt((Input.mousePosition.x - 19) / 14), Mathf.FloorToInt((Input.mousePosition.y - 254) / 20));
            if (MapManager.map[mousePos.x, mousePos.y].isExplored && MapManager.map[mousePos.x, mousePos.y].isWalkable || MapManager.map[mousePos.x, mousePos.y].type == "Door")
            {
                _selected = true;
                path = null;

                path = AStar.CalculatePath(MapManager.playerPos, mousePos);

                foreach (var pathTile in path)
                {
                    MapManager.map[pathTile.x, pathTile.y].decoy = $"<color=yellow>\u205C</color>";
                }

                DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
            }         
            else
            {
                _selected = false;
            }
        }

        if(_selected && Input.GetMouseButtonDown(0))
        {
            StartCoroutine(WalkPath(path));
        }
    }

    IEnumerator WalkPath(List<Vector2Int> _path)
    {
        for (int i = 0; i < _path.Count; i++)
        {
            playerMovement.Move(_path[i]);
            yield return new WaitForSeconds(.06f);
        }      
        yield return null;
    }
}
