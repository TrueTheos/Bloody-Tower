using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenData
{
    public GenTile[,] TileMap;


    public int Width => BotRight.x - TopLeft.x + 1;
    public int Height => BotRight.y - TopLeft.y + 1;

    public Vector2Int TopLeft;
    public Vector2Int BotRight;

    public List<GenRoom> Rooms;

    public List<GenHallway> Hallways;


    public List<List<GenRoom>> RoomMerges; 

    public GenData(int width, int height)
    {
        TileMap = new GenTile[width, height];
        TopLeft = new Vector2Int(0, 0);
        BotRight = new Vector2Int(width - 1, height - 1);

        Rooms = new List<GenRoom>();
        Hallways = new List<GenHallway>();

        RoomMerges = new List<List<GenRoom>>();
    }
       


    public GenTile GetTile(int gx, int gy)
    {
        foreach (var item in Rooms)
        {
            GenTile tile = item.GetAtWorldspaceG(gx, gy);
            if (tile!=null)
            {
                return tile;
            }
        }
        return null;
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
                    GenRect rA = a.Outer;
                    GenRect rB = b.Outer;
                    // find the union of both
                    int uLeft = Mathf.Max(rA.MinX, rB.MinX);
                    int uTop = Mathf.Max(rA.MinY, rB.MinY);
                    int uRight = Mathf.Min(rA.MaxX, rB.MaxX);
                    int uBot = Mathf.Min(rA.MaxY, rB.MaxY);
                    GenRect union =
                        new GenRect(uLeft, uRight,  uTop, uBot);

                    GenRoom dominant = a.SpacePriority > b.SpacePriority ? a : b;
                    GenRoom weaker = a.SpacePriority > b.SpacePriority ? b : a;

                    HashSet<GenTile> dominantEdge = dominant.GetEdge();

                    for (int x = union.MinX; x <= union.MaxX; x++)
                    {
                        for (int y = union.MinY; y <= union.MaxY; y++)
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


    public void UpdateSize()
    {
        int minX = Rooms.Min(r => r.PosX);
        int minY = Rooms.Min(r => r.PosY);
        int maxX = Rooms.Max(r => r.PosX + r.Width);
        int maxY = Rooms.Max(r => r.PosY + r.Height);

        TopLeft = new Vector2Int(minX, minY);
        BotRight = new Vector2Int(maxX, maxY);
    }

    public void MinimizeMapSize()
    {
        UpdateSize();
        TileMap = new GenTile[Width, Height];

        foreach (var room in Rooms)
        {
            room.PosX = room.PosX - TopLeft.x;
            room.PosY = room.PosY - TopLeft.y;
        }

        foreach (var room in Rooms)
        {
            TileMap.Place(room.PosX, room.PosY, room.Data);
        }
    }

    /// <summary>
    /// Check if a position is inside a room and not a wall or invalid
    /// </summary>
    /// <param name="gx"></param>
    /// <param name="gy"></param>
    /// <param name=""></param>
    /// <returns></returns>
    public bool IsInsideRoom(int gx, int gy, GenRoom room = null)
    {
        if (room == null)
        {
            GenTile tile = GetTile(gx, gy);
            if (tile!=null)
            {
                return tile.NonTypes(GenDetail.DetailType.Wall);
            }
            return false;
        }
        else
        {
            GenTile tile = room.GetAtWorldspaceG(gx, gy);
            if (tile != null)
            {
                return tile.NonTypes(GenDetail.DetailType.Wall);
            }
            return false;
        }

    }

    /// <summary>
    /// checks if Position is in the corner (floor, not wall)
    /// </summary>
    public bool IsCornerG(int gx, int gy,GenRoom room = null)
    {
        bool vertical = false;
        bool horizontal = false;
        GenTile tile;
        if (room!=null)
        {
            tile = room.GetAtWorldspaceG(gx, gy);
            if (tile!=null)
            {
                GenTile check = room.GetAtWorldspaceG(gx+1, gy);
                if (check!=null && check.AnyTypes(GenDetail.DetailType.Wall))
                {
                    horizontal = true;
                }
                check = room.GetAtWorldspaceG(gx-1, gy);
                if (check != null && check.AnyTypes(GenDetail.DetailType.Wall))
                {
                    horizontal = true;
                }
                check = room.GetAtWorldspaceG(gx, gy+1);
                if (check != null && check.AnyTypes(GenDetail.DetailType.Wall))
                {
                    vertical = true;
                }
                check = room.GetAtWorldspaceG(gx, gy-1);
                if (check != null && check.AnyTypes(GenDetail.DetailType.Wall))
                {
                    vertical = true;
                }
                return horizontal && vertical;
            }
            return false;
        }
        else
        {
            tile = GetTile(gx, gy);
            if (tile != null)
            {
                GenTile check = GetTile(gx + 1, gy);
                if (check != null && check.AnyTypes(GenDetail.DetailType.Wall))
                {
                    horizontal = true;
                }
                check = GetTile(gx - 1, gy);
                if (check != null && check.AnyTypes(GenDetail.DetailType.Wall))
                {
                    horizontal = true;
                }
                check = GetTile(gx, gy + 1);
                if (check != null && check.AnyTypes(GenDetail.DetailType.Wall))
                {
                    vertical = true;
                }
                check = GetTile(gx, gy - 1);
                if (check != null && check.AnyTypes(GenDetail.DetailType.Wall))
                {
                    vertical = true;
                }
                return horizontal && vertical;
            }
            return false;
        }


    }


}
