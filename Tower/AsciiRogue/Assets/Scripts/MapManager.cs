using System.Collections;
using System; // So the script can use the serialization commands
using System.Collections.Generic;
using UnityEngine;

public class MapManager
{
    public static bool IsValid = false;
    public static Tile[,] map; // the 2-dimensional map with the information for all the tiles
    public static Vector2Int playerPos;
    public static Vector2Int upperStairsPos, lowerStairsPos;
    public static bool NeedRepaint;
    /// <summary>
    /// Searches for the closest Position towards a goal position (excluding the target)
    /// </summary>
    /// <param name="origin">From</param>
    /// <param name="target">Tp</param>
    /// <returns></returns>
    public static bool TryFindClosestPositionTowards(Vector2Int origin, Vector2Int target, out Vector2Int endPosition)
    {
        List<Vector2Int> possible = GetValidNeighbours(target);

        if (possible.Count==0)
        {
            endPosition = origin;
            return false;
        }

        Vector2Int best = possible[0];
        float bestDistance = Vector2Int.Distance(origin, best);
        for (int i = 1; i < possible.Count; i++)
        {
            if (Vector2Int.Distance(origin,possible[i])>bestDistance)
            {
                bestDistance = Vector2Int.Distance(origin, possible[i]);
                best = possible[i];
            }
        }

        // This can be removed if we want to to allow jumping to the other side
        if (bestDistance-0.3f>Vector2Int.Distance(origin,target))
        {
            endPosition = origin;
            return false;
        }
        endPosition = best;
        return true;
    }
    public static List<Vector2Int> GetValidNeighbours(Vector2Int position)
    {
        List<Vector2Int> possible = new List<Vector2Int>();

        if (map[position.x+1,position.y+1].isWalkable && map[position.x + 1, position.y + 1].enemy==null)
        {
            possible.Add(new Vector2Int(position.x + 1, position.y + 1));
        }
        if (map[position.x + 1, position.y + 0].isWalkable && map[position.x + 1, position.y + 0].enemy == null)
        {
            possible.Add(new Vector2Int(position.x + 1, position.y + 0));
        }
        if (map[position.x + 1, position.y -1].isWalkable && map[position.x + 1, position.y - 1].enemy == null)
        {
            possible.Add(new Vector2Int(position.x + 1, position.y - 1));
        }

        if (map[position.x + 0, position.y +1].isWalkable && map[position.x + 0, position.y + 1].enemy == null)
        {
            possible.Add(new Vector2Int(position.x + 0, position.y + 1));
        }
        if (map[position.x + 0, position.y -1].isWalkable && map[position.x + 0, position.y - 1].enemy == null)
        {
            possible.Add(new Vector2Int(position.x + 0, position.y - 1));
        }

        if (map[position.x -1, position.y + 1].isWalkable && map[position.x - 1, position.y + 1].enemy == null)
        {
            possible.Add(new Vector2Int(position.x - 1, position.y + 1));
        }
        if (map[position.x -1, position.y + 0].isWalkable && map[position.x - 1, position.y + 0].enemy == null)
        {
            possible.Add(new Vector2Int(position.x - 1, position.y + 0));
        }
        if (map[position.x -1, position.y - 1].isWalkable && map[position.x - 1, position.y - 1].enemy == null)
        {
            possible.Add(new Vector2Int(position.x - 1, position.y - 1));
        }
        return possible;
    }

    public static Vector2Int FindFreeSpot()
    {
        for (int x = 0; x < map.GetLength(0); x++)
        {
            for (int y = 0; y < map.GetLength(1); y++)
            {
                if (map[x, y].isWalkable && map[x, y].type != "Cobweb" && map[x, y].hasPlayer == false && map[x, y].enemy == null)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return new Vector2Int(0, 00);
    }

}

[Serializable] // Makes the class serializable so it can be saved out to a file
public class Tile
{ // Holds all the information for each tile on the map
    public int xPosition; // the position on the x axis
    public int yPosition; // the position on the y axis
    [NonSerialized]
    //public GameObject baseObject; // the map game object attached to that position: a floor, a wall, etc.
    public string type; // The type of the tile, if it is wall, floor, etc
    public bool hasPlayer = false;
    public bool isWalkable = false;


    public bool requiresKey = false;

    public string previousMonsterLetter = "";
    public Color previousMonsterColor;

    
    public string baseChar;
    public string letter = "";
    public bool isOpaque = false; //blocks vision
    public bool isVisible = false;
    public bool isExplored = false;
    public Color exploredColor = new Color(1, 1, 1);
    public Color timeColor = new Color(0, 0, 0);
    public string color = "black";
    public GameObject enemy;
    public GameObject item;
    public Structure structure;
    public float tileLightFactor;

    public string decoy = "";

    public string specialNameOfTheCell = ""; //will be displayed if pointed, else show "type"
}

[Serializable]
public class Wall
{ // A class for saving the wall information, for the dungeon generation algorithm
    public List<Vector2Int> positions;
    public string direction;
    public int length;
    public bool hasFeature = false;
    public Feature parent;
}

[Serializable]
public class Feature
{ // A class for saving the feature (corridor or room) information, for the dungeon generation algorithm
    public List<Vector2Int> positions;
    public Wall[] walls;
    public string type;
    public int width;
    public int height;
    public int id;
    public bool hasPlayer = false;
}