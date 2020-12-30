using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Obsolete("The FloorManager should no longer be used. Use MapManager instead")]
public class FloorManager : MonoBehaviour
{
    private List<Tile[,]> floors = new List<Tile[,]>();
    private List<GameObject> floorsGO = new List<GameObject>();
    private List<Vector2Int> stairsDown = new List<Vector2Int>();
    private List<Vector2Int> stairsUp = new List<Vector2Int>();

    private static FloorManager floorManager;

    private void Awake()
    {
        floorManager = this;
    }
}
