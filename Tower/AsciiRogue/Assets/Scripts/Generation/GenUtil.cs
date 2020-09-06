using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Text;

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

    [System.Flags]
    public enum Axis
    {
        Horizontal = 1<<0,
        Vertical = 1<<1
    }

    public static Axis Turn(this Axis ax, int quaterClockwise)
    {
        if (ax == (Axis.Horizontal | Axis.Vertical))
        {
            return ax;
        }
        if (quaterClockwise % 2 == 0)
        {
            return ax;
        }

        if (ax.HasFlag(Axis.Vertical))
        {
            return Axis.Horizontal;
        }
        else
        {
            return Axis.Vertical;
        }

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
    public static List<T> ToList<T>(this T[,] target)
    {
        List<T> data = new List<T>();

        foreach (var item in target)
        {
            if (item!=null)
            {
                data.Add(item);
            }
        }
        return data;
    }
    public static T[,] GetFlip<T>(this T[,] target, Axis axis)
    {
        T[,] result = new T[target.GetLength(0), target.GetLength(1)];

        for (int x = 0; x < target.GetLength(0); x++)
        {
            for (int y = 0; y < target.GetLength(1); y++)
            {
                int sourceX = axis.HasFlag(Axis.Horizontal) ? target.GetLength(0) - x -1: x;
                int sourceY = axis.HasFlag(Axis.Vertical) ? target.GetLength(1) - y -1: y;
                result[x, y] = target[sourceX, sourceY];
            }

        }

        return result;
    }
    public static T[,] GetRotateClock<T>(this T[,] target, int quaterTurns)
    {
        T[,] result = new T[target.GetLength(1), target.GetLength(0)];
        quaterTurns--;
        for (int x = 0; x < target.GetLength(0); x++)
        {
            for (int y = 0; y < target.GetLength(1); y++)
            {
                result[y, target.GetLength(0) - x - 1] = target[x, y];
            }
        }
        return quaterTurns == 0 ? result : result.GetRotateClock(quaterTurns);
    }
    public static GenTile[,] GetCopy(this GenTile[,] target)
    {
        GenTile[,] result = new GenTile[target.GetLength(0), target.GetLength(1)];
        for (int x = 0; x < target.GetLength(0); x++)
        {
            for (int y = 0; y < target.GetLength(1); y++)
            {
                if (target[x,y]!=null)
                {
                    result[x, y] = GenTile.Copy(target[x, y]);
                }                
            }
        }
        return result;
    }


    public static int PrintCount = 0;
    public static string Print(this GenData target,bool simple = true)
    {

        if (simple)
        {
            PrintCount++;
            GenTile[,] tile = target.TileMap;
            StringBuilder sb = new StringBuilder();
            using (var sr = new StreamWriter("test"+PrintCount+".txt"))
            {
                for (int y = 0; y < tile.GetLength(1); y++)
                {
                    for (int x = 0; x < tile.GetLength(0); x++)
                    {
                        if (tile[x, y] == null)
                        {
                            sr.Write(' ');
                            sb.Append(' ');
                            continue;
                        }
                        int max = tile[x, y].Details.Max(d => d.Priority);
                        GenDetail toDraw = tile[x, y].Details.Where(d => d.Priority == max).First();
                        sr.Write(toDraw.Char);
                        sb.Append(toDraw.Char);
                    }
                    sr.Write('\n');
                    sb.Append('\n');
                }
            }
            return sb.ToString();
        }
        else
        {

            GenTile[,] work = new GenTile[target.Width, target.Height];

            foreach (var room in target.Rooms)
            {
                work.Place(room.PosX, room.PosY, room.Data);
            }

            target.TileMap = work;
            return Print(target);            
        }
    }

    /// <summary>
    /// checks if Position is in the corner (floor, not wall)
    /// </summary>
    public static bool IsCornerG(int gx, int gy, params GenRoom[] rooms) 
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


    /// <summary>
    /// returns a symetric version of a feature based on a room
    /// </summary>
    /// <param name="feature">the features described by tiles to be placed</param>
    /// <param name="gx">Global position X if position changes this will change</param>
    /// <param name="gy">Global position Y if position changes this will change</param>
    /// <param name="room">The room the symetry should be in</param>
    /// <param name="axis">The axis symetry should be created in</param>
    /// <returns>The complete feature</returns>
    public static GenTile[,] GetSymetry(GenTile[,] feature, ref int gx, ref int gy, GenRoom room, Axis axis)
    {
        GenRect size = new GenRect(gx, gx, gy, gy);
        GenRect checkRect = new GenRect(size);



        if (axis.HasFlag(Axis.Vertical))
        {
            do
            {
                checkRect.Transform(0, 1, 0, 0);
            } while (!TouchesWall(checkRect, room));
            checkRect.Transform(0, -1, 0, 0);

            do
            {
                checkRect.Transform(0, 0, 0, 1);
            } while (!TouchesWall(checkRect, room));
            checkRect.Transform(0, 0, 0, -1);

            GenTile[,] resize = checkRect.ResizeTiles(feature, ref gx,ref gy);

            GenTile[,] flip = resize.GetCopy().GetFlip(Axis.Vertical);
            resize = resize.Place(0, 0, flip);

            feature = resize;
        }

        if (axis.HasFlag(Axis.Horizontal))
        {
            do
            {
                checkRect.Transform(1, 0, 0, 0);
            } while (!TouchesWall(checkRect, room));
            checkRect.Transform(-1, 0, 0, 0);

            do
            {
                checkRect.Transform(0, 0, 1, 0);
            } while (!TouchesWall(checkRect, room));
            checkRect.Transform(0, 0, -1, 0);

            GenTile[,] resize = checkRect.ResizeTiles(feature, ref gx, ref gy);

            GenTile[,] flip = resize.GetCopy().GetFlip(Axis.Horizontal);
            resize = resize.Place(0, 0, flip);

            feature = resize;
        }

        return feature;
    }

    public static GenRect GrowRect(int gx, int gy, GenRoom room, int width, int height, bool withWall = false)
    {



        throw new System.NotImplementedException();
    }

    public static bool TouchesWall(GenRect rect, GenRoom room)
    {
        for (int x = rect.MinX; x <= rect.MaxX; x++)
        {
            for (int y = rect.MinY; y <= rect.MaxY; y++)
            {
                GenTile check = room.GetAtWorldspaceG(x, y);
                if (check != null)
                {
                    if (check.AnyTypes(GenDetail.DetailType.Wall))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

}
