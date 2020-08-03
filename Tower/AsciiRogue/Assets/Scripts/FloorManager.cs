using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public List<Tile[,]> floors = new List<Tile[,]>();
    public List<GameObject> floorsGO = new List<GameObject>();
    public List<Vector2Int> stairsDown = new List<Vector2Int>();
    public List<Vector2Int> stairsUp = new List<Vector2Int>();

    public static FloorManager floorManager;

    private void Awake()
    {
        floorManager = this;
    }
}
