using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;


public static class GenUtil
{
    public enum Direction4
    {
        Right,
        Bottom,
        Left,
        Top
    }
    public enum Direction8
    {
        Right,
        BottomRight,
        Bottom,
        BottomLeft,
        Left,
        TopLeft,
        Top,
        TopRight
    }

    

    public static GenTile[,] Place(this GenTile[,] target, int posX, int posY, GenTile[,] data)
    {
        for (int x = 0; x < data.GetLength(0); x++)
        {
            for (int y = 0; y < data.GetLength(1); y++)
            {
                if (target.IsInbounds(posX + x,posY + y))
                {
                    if (data.Exists(x,y))
                    {
                        data[x, y].PosX = posX + x;
                        data[x, y].PosY = posY + y;
                        target[posX + x, posY + y] = data[x, y];
                    }                    
                }
            }
        }
        return target;
    }

    public static bool IsInbounds<T>(this T[,] target, int posX, int posY)
    {
        return posX >= 0 && posX < target.GetLength(0) && posY >= 0 && posY < target.GetLength(1);
    }
    public static bool Exists<T>(this T[,] target, int posX, int posY)
    {
        return posX >= 0 && posX < target.GetLength(0) && posY >= 0 && posY < target.GetLength(1) && target[posX, posY] != null;
    }


    public static void Print(this GenData target, string filename = "test.txt",bool simple = true)
    {

        if (simple)
        {
            GenTile[,] tile = target.TileMap;
            using (var sr = new StreamWriter(filename))
            {
                for (int y = 0; y < tile.GetLength(1); y++)
                {
                    for (int x = 0; x < tile.GetLength(0); x++)
                    {
                        if (tile[x, y] == null)
                        {
                            sr.Write(' ');
                            continue;
                        }
                        int max = tile[x, y].Details.Max(d => d.Priority);
                        GenDetail toDraw = tile[x, y].Details.Where(d => d.Priority == max).First();
                        sr.Write(toDraw.Char);
                    }
                    sr.Write('\n');
                }
            }
        }
        else
        {

            GenTile[,] work = new GenTile[target.Width, target.Height];

            foreach (var room in target.Rooms)
            {
                work.Place(room.PosX, room.PosY, room.Data);
            }

            target.TileMap = work;
            Print(target);
        }
    }

    /// <summary>
    /// checks if Position is in the corner (floor, not wall)
    /// </summary>
    public static bool IsCornerG(int gx, int gy, params GenRoom[] rooms) // oh no what have i done
    {
        // if it is a wall we dont need to bother as it is never a corner
        


        // how many sides are obstructed with walls
        int sidesWall = 0;

        foreach (var item in rooms)
        {

        }


        return false;

    }

    public static bool CanBeDoor(int gx, int gy, params GenTile[][,] data)
    {
        return false;
    }

    public static GenTile GetTileAtG(int gx, int gy, params GenRoom[] rooms)
    {
        return null;
    }

    public static GenTile GetTileAtG(int gx, int gy, GenData data)
    {
        return GetTileAtG(gx, gy, data.Rooms.ToArray());
    }



}
