using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    static List<Node> openList;
    static List<Node> closedList;
    static Node[,] allNodes;
    static DungeonGenerator dungeon;

    public static List<Vector2Int> CalculatePath(Vector2Int start, Vector2Int target)
    {
        if (dungeon == null) dungeon = GameObject.Find("GameManager").GetComponent<DungeonGenerator>();

        List<Vector2Int> path = new List<Vector2Int>();

        openList = new List<Node>();
        closedList = new List<Node>();
        allNodes = new Node[dungeon.mapWidth, dungeon.mapHeight];
        for (int y = 0; y < dungeon.mapHeight; y++)
        {
            for (int x = 0; x < dungeon.mapWidth; x++)
            {
                allNodes[x, y] = new Node();
            }
        }

        Node firstNode = new Node() { position = start, gCost = 0, isOnClosedList = true };
        closedList.Add(firstNode);

        foreach (Vector2Int position in GetNeighbours(firstNode.position))
        {
            Node node = new Node() { position = position, parent = firstNode, hCost = CalculateHCost(position, target), gCost = CalculateGCost(firstNode, position) };
            node.fCost = node.gCost + node.hCost;
            node.isOnOpenList = true;
            allNodes[position.x, position.y] = node;
            openList.Add(node);
        }

        Node lastNode = new Node();

        while (openList.Count > 0)
        {
            Node node = openList[0];

            foreach (Node pathNode in openList)
            {
                if (node.fCost > pathNode.fCost || (node.fCost == pathNode.fCost && node.hCost > pathNode.hCost)) node = pathNode;
            }

            if (node.position == target)
            {
                lastNode = node;
                break;
            }

            closedList.Add(node);
            allNodes[node.position.x, node.position.y].isOnClosedList = true;
            openList.Remove(node);
            allNodes[node.position.x, node.position.y].isOnOpenList = false;

            bool foundTarget = false;

            foreach (Vector2Int position in GetNeighbours(node.position))
            {
                if (position == target)
                {
                    foundTarget = true;
                    break;
                }

                Node neighbour = new Node() { position = position, parent = node, gCost = CalculateGCost(node, position), hCost = CalculateHCost(position, target), isOnOpenList = true };
                neighbour.fCost = neighbour.gCost + neighbour.hCost;
                allNodes[position.x, position.y] = neighbour;
                openList.Add(neighbour);
            }

            if (foundTarget)
            {
                openList.Clear();
                lastNode = node;
                break;
            }
        }

        path.Add(target);
        RetracePath(path, start, lastNode);

        return path;
    }

    static void RetracePath(List<Vector2Int> path, Vector2Int start, Node lastNode)
    {
        Node current = lastNode;

        try
        {
            while (current.position != start)
            {
                path.Add(current.position);
                current = current.parent;
            }
        }
        catch
        {
            path.Reverse();
        }       
        path.Reverse();
    }

    static List<Vector2Int> GetNeighbours(Vector2Int parent)
    {
        List<Vector2Int> neighbours = new List<Vector2Int>();

        for (int y = parent.y - 1; y <= parent.y + 1; y++)
        {
            for (int x = parent.x - 1; x <= parent.x + 1; x++)
            {
                if (x >= 0 && y >= 0 && x < dungeon.mapWidth && y < dungeon.mapHeight)
                {
                    if (MapManager.map[x, y] != null )
                    {                        
                        if(MapManager.map[x, y].isWalkable || MapManager.map[x, y].type == "Door")
                        {
                            if (!allNodes[x, y].isOnClosedList && !allNodes[x, y].isOnOpenList)
                            {
                                neighbours.Add(new Vector2Int(x, y));
                            }
                        }
                    }
                }
            }
        }

        return neighbours;
    }

    static int CalculateHCost(Vector2Int position, Vector2Int target)
    {
        int hCost = 0;

        int x = Mathf.Abs(position.x - target.x);
        int y = Mathf.Abs(position.y - target.y);

        hCost = x + y;

        return hCost;
    }

    static int CalculateGCost(Node parent, Vector2Int position)
    {
        int localG;

        if (position.x != parent.position.x && position.y != parent.position.y) localG = 14;
        else localG = 10;

        int gCost = parent.fCost + localG;

        return gCost;
    }
}

public class Node
{
    public Vector2Int position;
    public int fCost;
    public int gCost;
    public int hCost;
    public Node parent;
    public bool isOnClosedList = false;
    public bool isOnOpenList = false;
}
 