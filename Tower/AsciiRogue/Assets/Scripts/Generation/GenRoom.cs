using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenRoom
{
    public GenTile[,] Data;

    public int PosX;
    public int PosY;

    public int Width;
    public int Height;

    public int SpacePriority = 1;
    /// <summary>
    /// <see cref="GenTile"/> in global space
    /// </summary>
    public GenTile GetAtWorldspaceG(int gx, int gy)
    {
        if (Data.Exists(gx - PosX, gy - PosY))
        {
            return Data[gx - PosX, gy - PosY];
        }
        return null;
    }
    /// <summary>
    /// Check if tile exists in Global Space
    /// </summary>
    public bool TileExistsG(int gx,int gy)
    {
        return Data.IsInbounds(gx - PosX, gy - PosY);
    }
    public void SetTileAtG(int gx, int gy, GenTile tile)
    {
        if (Data.Exists(gx - PosX, gy - PosY))
        {
            Data[gx - PosX, gy - PosY] = tile;
        }
    }

    public void RemoveTileAtG(int gx, int gy)
    {
        if (Data.Exists(gx - PosX, gy - PosY))
        {
            Data[gx - PosX, gy - PosY] = null;
        }
    }



    public RectInt Outer => new RectInt(PosX, PosY, Width, Height);
    public RectInt Inner => new RectInt(PosX + 1, PosX + 1,Width - 2, Height - 2);

    private GenRoom()
    {
    }
    
    public static GenRoom Sized(int width, int height, bool defaultFill = false)
    {
        GenRoom room = new GenRoom();
        room.Data = new GenTile[width, height];
        room.PosX = 0;
        room.PosY = 0;

        room.Width = width;
        room.Height = height;

        if (defaultFill)
        {
            room.FillFloor();
        }

        return room;
    }
    

    public void FillFloor(char c = '.')
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                GenTile t = GenTile.GetAt(x, y);
                t.Details.Add(new GenDetail() { Char = c, Type = GenDetail.DetailType.Floor, });
                Data[x, y] = t;
            }
        }
    }

    public HashSet<GenTile> GetEdge()
    {
        HashSet<GenTile> Edge = new HashSet<GenTile>();

        HashSet<GenTile> Potential = new HashSet<GenTile>();

        void MoveUp(GenTile tile)
        {
            if (!Edge.Contains(tile))
            {
                if (Potential.Contains(tile))
                {
                    // we add it to edge
                    Edge.Add(tile);
                    Potential.Remove(tile);
                }
                else
                {
                    Potential.Add(tile);
                }
            }
        }
        void MoveUpDown(int x, int y)
        {            
            if (Data.Exists(x, y+1))
            {
                MoveUp(Data[x, y+1]);
            }
            if (Data.Exists(x, y-1))
            {
                MoveUp(Data[x, y-1]);
            }
        }
        void MoveLeftRight(int x, int y)
        {
            if (Data.Exists(x + 1, y))
            {
                MoveUp(Data[x + 1, y]);
            }
            if (Data.Exists(x - 1, y))
            {
                MoveUp(Data[x - 1, y]);
            }
        }

        // top-left -> top-right 
        // and moving down
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (Data.Exists(x,y))
                {
                    // found our Tile
                    GenTile t = Data[x, y];
                    MoveUp(t);
                    MoveLeftRight(x, y);
                    break;
                }
            }
        }

        // bottom-left -> bottom-right 
        // and moving up
        for (int x = 0; x < Width; x++)
        {
            for (int y = Height-1; y >= 0; y--)
            {
                if (Data.Exists(x, y))
                {
                    // found our Tile
                    GenTile t = Data[x, y];
                    MoveUp(t);
                    MoveLeftRight(x, y);
                    break;
                }
            }
        }

        // top-left -> bottom-left
        // and moving right
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (Data.Exists(x, y))
                {
                    // found our Tile
                    GenTile t = Data[x, y];
                    MoveUp(t);
                    MoveUpDown(x, y);
                    break;
                }
            }
        }
        
        // top-right -> bottom-right
        // and moving left
        for (int y = 0; y < Height; y++)
        {
            for (int x = Width-1; x >= 0; x--)
            {
                if (Data.Exists(x, y))
                {
                    // found our Tile
                    GenTile t = Data[x, y];
                    MoveUp(t);
                    MoveUpDown(x, y);
                    break;
                }
            }
        }

        return Edge;
    }



}
