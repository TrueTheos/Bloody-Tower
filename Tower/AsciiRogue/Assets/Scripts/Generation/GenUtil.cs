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



    public static bool IsInDirection4(GenRect rect, int gx, int gy, params Direction4[] dir)
    {
        Direction8[] other = new Direction8[dir.Length];
        for (int i = 0; i < dir.Length; i++)
        {
            Direction8 target = Direction8.Bottom;
            switch (dir[i])
            {
                case Direction4.Right:
                    target = Direction8.Right;
                    break;
                case Direction4.Bottom:
                    target = Direction8.Bottom;
                    break;
                case Direction4.Left:
                    target = Direction8.Left;
                    break;
                case Direction4.Top:
                    target = Direction8.Top;
                    break;
                default:
                    break;
            }
            other[i] = target;
        }
        return IsInOctant(rect, gx, gy, other);
    }

    public static bool IsInOctant(GenRect rect, int gx, int gy, params Direction8[] dir)
    {


        //NOTE: does not check if in rect
        float tx = (rect.MinX + rect.MaxX) / 2f;
        float ty = (rect.MinY + rect.MaxY) / 2f;

        // to a range of -1 to 1

        // relativ position
        float cx = gx - tx;
        float cy = gy - ty;

        cx = cx / (rect.WidthT / 2);
        cy = cy / (rect.WidthT / 2);



        // regular directions
        if (dir.Contains(Direction8.Top))
        {
            if (cy <= 0 && Mathf.Abs(cx) <= Mathf.Abs(cy))
            {
                return true;
            }
        }
        if (dir.Contains(Direction8.Right))
        {
            if (cx >= 0 && Mathf.Abs(cy) <= Mathf.Abs(cx))
            {
                return true;
            }
        }
        if (dir.Contains(Direction8.Bottom))
        {
            if (cy >= 0 && Mathf.Abs(cx)<= Mathf.Abs(cy))
            {
                return true;
            }
        }
        if (dir.Contains(Direction8.Left))
        {
            if (cx <= 0 && Mathf.Abs(cy) <= Mathf.Abs(cx))
            {
                return true;
            }
        }

        // quadrants
        if (dir.Contains(Direction8.TopLeft))
        {
            if (cy <= 0 && cx <= 0)
            {
                return true;
            }
        }
        if (dir.Contains(Direction8.TopRight))
        {
            if (cy <= 0 && cx >= 0)
            {
                return true;
            }
        }
        if (dir.Contains(Direction8.BottomRight))
        {
            if (cy >=0 && cx >= 0)
            {
                return true;
            }
        }
        if (dir.Contains(Direction8.BottomLeft))
        {
            if (cy >=0 && cx <= 0)
            {
                return true;
            }
        }

        return false;
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
    
    public static Direction4 Rotate(this Direction4 dir, int quaterClockwise)
    {
        while (quaterClockwise<0)
        {
            quaterClockwise += 4;
        }
        return (Direction4)(((int)dir + quaterClockwise) % 4);
    }
    public static Direction8 Rotate(this Direction8 dir, int octaClockwise)
    {
        while (octaClockwise < 0)
        {
            octaClockwise += 8;
        }
        return (Direction8)(((int)dir + octaClockwise) % 8);
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
    public static T GetRandom<T>(this T[] target)
    {
        return target[Random.Range(0, target.Length)];
    }
    public static T GetRandom<T>(this List<T> target)
    {
        return target[Random.Range(0, target.Count)];
    }
    public static List<T> GetRandom<T>(this T[] target, int count)
    {
        List<T> result = new List<T>();
        List<T> w = target.ToList();
        for (int i = 0; i < count; i++)
        {
            if (w.Count == 0)
            {
                break;
            }
            T one = w[Random.Range(0, w.Count)];
            w.Remove(one);
            result.Add(one);            
        }
        return result;
    }
    public static List<T> GetRandom<T>(this List<T> target, int count)
    {
        return GetRandom(target.ToArray(),count);
    }
    public static GenTile[,] Fill(int width, int height, GenTile tile)
    {
        GenTile[,] result = new GenTile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                result[x, y] = GenTile.Copy(tile);
            }
        }
        return result;
    }
    public static T[,] Fill<T>(this T[,] target,T with)
    {
        for (int x = 0; x < target.GetLength(0); x++)
        {
            for (int y = 0; y < target.GetLength(1); y++)
            {
                target[x, y] = with;
            }
        }
        return target;
    }
    public static Vector2Int GetCenter<T>(this T[,] target)
    {
        float xC = (0 + target.GetLength(0)-1) / 2f;
        float yC = (0 + target.GetLength(1)-1) / 2f;

        float xA = xC + (Random.Range(0, 2) == 0 ? 0.2f :-0.2f);
        float yA = yC + (Random.Range(0, 2) == 0 ? 0.2f : -0.2f);

        return new Vector2Int(Mathf.RoundToInt(xA), Mathf.RoundToInt(yA));
    }
    public static T[,] Place<T>(this T[,] target, int x, int y, T[,] other)
    {
        for (int ox = 0; ox < other.GetLength(0); ox++)
        {
            for (int oy = 0; oy < other.GetLength(1); oy++)
            {
                if (target.IsInbounds(x+ox,y+oy))
                {
                    target[x + ox, y + oy] = other[ox, oy];
                }
                else
                {
                    Debug.Log("out of bounds "+ ox + " " + oy);
                }
            }
        }
        return target;
    }
    public static char[,] AsPlaceable(this string target)
    {
        char[,] res = new char[target.Length, 1];
        for (int i = 0; i < target.Length; i++)
        {
            res[i, 0] = target[i];
        }
        return res;
    }

    public static int PrintCount = 0;
    public static string Print(this GenData target,bool simple = true)
    {

        if (simple)
        {
            PrintCount++;
            GenTile[,] tile = target.TileMap;
            StringBuilder sb = new StringBuilder();

            for (int y = 0; y < tile.GetLength(1); y++)
            {
                for (int x = 0; x < tile.GetLength(0); x++)
                {
                    if (tile[x, y] == null)
                    {
                        sb.Append(' ');
                        continue;
                    }
                    int max = tile[x, y].Details.Max(d => d.Priority);
                    GenDetail toDraw = tile[x, y].Details.Where(d => d.Priority == max).First();

                    sb.Append(toDraw.Char);
                }
                //sb.Append('\n');
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

    
    public static GenTile GetTileAtG(int gx, int gy, params GenRoom[] rooms)
    {
        for (int i = 0; i < rooms.Length; i++)
        {
            var room = rooms[i];
            GenTile t = room.GetAtWorldspaceG(gx, gy);
            if (t!=null)
            {
                return t;
            }
        }
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
        GenRect size = new GenRect(gx, gx+feature.GetLength(0)-1, gy, gy+feature.GetLength(1)-1);
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
