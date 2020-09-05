using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenData
{
    public GenTile[,] TileMap;


    public int Width => TileMap.GetLength(0);
    public int Height => TileMap.GetLength(1);

    public List<GenRoom> Rooms;

    public List<GenHallway> Hallways;


    public List<List<GenRoom>> RoomMerges; 

    public GenData(int width, int height)
    {
        TileMap = new GenTile[width, height];

        Rooms = new List<GenRoom>();
        Hallways = new List<GenHallway>();

        RoomMerges = new List<List<GenRoom>>();
    }
       
    public void PlaceRoom(int x,int y, GenRoom room)
    {
        room.PosX = x;
        room.PosY = y;
        Rooms.Add(room);
    }

    public void FixOverlap()
    {
        for (int i = 0; i < Rooms.Count-1; i++)
        {
            for (int j = i+1; j < Rooms.Count; j++)
            {
                GenRoom a = Rooms[i];
                GenRoom b = Rooms[j];

                if (a.Outer.Overlaps(b.Outer))
                {
                    RectInt rA = a.Outer;
                    RectInt rB = b.Outer;
                    // find the union of both
                    int uLeft = Mathf.Max(rA.xMin, rB.xMin);
                    int uTop = Mathf.Max(rA.yMin, rB.yMin);
                    int uRight = Mathf.Min(rA.xMax-1, rB.xMax-1);
                    int uBot = Mathf.Min(rA.yMax-1, rB.yMax-1);
                    RectInt union =
                        new RectInt(uLeft, uTop, uRight - uLeft, uBot - uTop);

                    GenRoom dominant = a.SpacePriority > b.SpacePriority ? a : b;
                    GenRoom weaker = a.SpacePriority > b.SpacePriority ? b : a;

                    HashSet<GenTile> dominantEdge = dominant.GetEdge();

                    for (int x = union.xMin; x <= union.xMax; x++)
                    {
                        for (int y = union.yMin; y <= union.yMax; y++)
                        {
                            // check if on the edge
                            if (dominantEdge.Contains(dominant.GetAtWorldspaceG(x,y)))
                            {
                                // this Tile is on the Edge and should be shared
                                weaker.SetTileAtG(x, y, dominant.GetAtWorldspaceG(x, y));
                            }
                            else
                            {
                                // remove the tile from weaker one
                                weaker.RemoveTileAtG(x, y);
                            }
                        }
                    }


                }
                

            }
        }
    }

    public void EdgeWalls(char c)
    {
        foreach (GenRoom item in Rooms)
        {
            foreach (GenTile tile in item.GetEdge())
            {
                tile.Details.Add(new GenDetail() { Char = c, Type = GenDetail.DetailType.Wall });
            }
        }
    }

}
