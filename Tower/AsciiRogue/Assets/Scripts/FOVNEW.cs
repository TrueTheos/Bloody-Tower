using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVNEW : MonoBehaviour
{
    public static FOVNEW fv;

    private List<Vector2Int> tilesToSet = new List<Vector2Int>(); //tiles that have to be false;

    public List<Vector2Int> visibleTiles = new List<Vector2Int>();

    public void Start()
    {
        fv = this;
    }

    public bool CanLightPass(int x, int y)
    {
        try
        {
            return MapManager.map[x, y].isOpaque ? true : false;
        }
        catch { return false; }
    }

    public void SetToVisible(int x, int y, bool visible)
    {
        try
        {
            if(!MapManager.map[x, y].isExplored)
                MapManager.map[x, y].isExplored = true;
            MapManager.map[x, y].isVisible = visible;
            visibleTiles.Add(new Vector2Int(x, y));
        }
        catch { }
    }

    public int Distance(int x, int y)
    {
        return (int)(Mathf.Abs(x - MapManager.playerPos.x) + Mathf.Abs(y - MapManager.playerPos.y)) + 1;
    }

    public void Initialize(Func<int, int, bool> blocksLight, Action<int, int, bool> setVisible, Func<int, int, int> getDistance)
    {
        _blocksLight = blocksLight;
        GetDistance = getDistance;
        SetVisible = setVisible;
    }

    public void Compute(Vector2Int origin, int rangeLimit)
    {
        SetVisible(origin.x, origin.y, true);
        visibleTiles.Clear();

        if (GameManager.manager.playerStats.fullLevelVision == false)
        {
            try
            {
                foreach (var tile in tilesToSet)
                {
                    MapManager.map[tile.x, tile.y].isVisible = false;
                }
            }
            catch{}
        }       

        for (int octant = 0; octant < 8; octant++) Compute(octant, origin, rangeLimit, 1, new Slope(1, 1), new Slope(0, 1));
    }

    struct Slope // represents the slope Y/X as a rational number
    {
        public Slope(int y, int x) { Y = y; X = x; }

        public bool Greater(int y, int x) { return Y * x > X * y; } // this > y/x
        public bool GreaterOrEqual(int y, int x) { return Y * x >= X * y; } // this >= y/x
        public bool LessOrEqual(int y, int x) { return Y * x <= X * y; } // this < y/x
                                                                   //public bool LessOrEqual(int y, int x) { return Y*x <= X*y; } // this <= y/x

        public readonly int X, Y;
    }

    void Compute(int octant, Vector2Int origin, int rangeLimit, int x, Slope top, Slope bottom)
    {
        /*foreach (var tile in tilesToSet)
        {
            MapManager.map[tile.x, tile.y].isVisible = false;
        }
        tilesToSet.Clear();*/

        for (; (int)x <= (int)rangeLimit; x++) // rangeLimit < 0 || x <= rangeLimit
        {
            int topY;
            if (top.X == 1)
            {
                topY = x;
            }
            else
            {
                topY = ((x * 2 - 1) * top.Y + top.X) / (top.X * 2); // get the tile that the top vector enters from the left
                int ay = (topY * 2 + 1) * top.X;
                if (BlocksLight(x, topY, octant, origin)) // if the top tile is a wall...
                {
                    if (top.GreaterOrEqual(ay, x * 2)) topY++; // but the top vector misses the wall and passes into the tile above, move up
                }
                else // the top tile is not a wall
                {
                    if (top.Greater(ay, x * 2 + 1)) topY++; // so if the top vector passes into the tile above, move up
                }
            }

            int bottomY = bottom.Y == 0 ? 0 : ((x * 2 - 1) * bottom.Y + bottom.X) / (bottom.X * 2);
            int wasOpaque = -1; // 0:false, 1:true, -1:not applicable
            for (int y = topY; y >= bottomY; y--)
            {
                int tx = origin.x, ty = origin.y;
                switch (octant) // translate local coordinates to map coordinates
                {
                    case 0: tx += x; ty -= y; break;
                    case 1: tx += y; ty -= x; break;
                    case 2: tx -= y; ty -= x; break;
                    case 3: tx -= x; ty -= y; break;
                    case 4: tx -= x; ty += y; break;
                    case 5: tx -= y; ty += x; break;
                    case 6: tx += y; ty += x; break;
                    case 7: tx += x; ty += y; break;
                }

                bool inRange = rangeLimit < 0 || GetDistance(tx, ty) <= rangeLimit;
                // NOTE: use the following line instead to make the algorithm symmetrical
                // if(inRange && (y != topY || top.GreaterOrEqual(y, x)) && (y != bottomY || bottom.LessOrEqual(y, x))) SetVisible(tx, ty);
                //if (inRange) SetVisible(tx, ty);
                if (inRange && (y != topY || top.GreaterOrEqual(y, x)) && (y != bottomY || bottom.LessOrEqual(y, x)))
                {
                    SetVisible(tx, ty, true); 
                    tilesToSet.Add(new Vector2Int(tx, ty));
                }
                //else SetVisible(tx, ty, false);

                bool isOpaque = !inRange || _blocksLight(tx, ty);
                // if y == topY or y == bottomY, make sure the sector actually intersects the wall tile. if not, don't consider
                // it opaque to prevent the code below from moving the top vector up or the bottom vector down
                if (isOpaque &&
                   (y == topY && top.LessOrEqual(y * 2 - 1, x * 2) && !BlocksLight(x, y - 1, octant, origin) ||
                    y == bottomY && bottom.GreaterOrEqual(y * 2 + 1, x * 2) && !BlocksLight(x, y + 1, octant, origin)))
                {
                    isOpaque = false;
                }

                if (x != rangeLimit)
                {
                    if (isOpaque)
                    {
                        if (wasOpaque == 0) // if we found a transition from clear to opaque, this sector is done in this column, so
                        {                  // adjust the bottom vector upwards and continue processing it in the next column.
                                           // (x*2-1, y*2+1) is a vector to the top-left corner of the opaque block
                            if (!inRange || y == bottomY) { bottom = new Slope(y * 2 + 1, x * 2); break; } // don't recurse unless necessary
                            else Compute(octant, origin, rangeLimit, x + 1, top, new Slope(y * 2 + 1, x * 2));
                        }
                        wasOpaque = 1;
                    }
                    else // adjust the top vector downwards and continue if we found a transition from opaque to clear
                    {    // (x*2+1, y*2+1) is the top-right corner of the clear tile (i.e. the bottom-right of the opaque tile)
                        if (wasOpaque > 0) top = new Slope(y * 2 + 1, x * 2);
                        wasOpaque = 0;
                    }
                }
            }

            if (wasOpaque != 0) break; // if the column ended in a clear tile, continue processing the current sector
        }
    }

    bool BlocksLight(int x, int y, int octant, Vector2Int origin)
    {
        int nx = origin.x, ny = origin.y;
        switch (octant)
        {
            case 0: nx += x; ny -= y; break;
            case 1: nx += y; ny -= x; break;
            case 2: nx -= y; ny -= x; break;
            case 3: nx -= x; ny -= y; break;
            case 4: nx -= x; ny += y; break;
            case 5: nx -= y; ny += x; break;
            case 6: nx += y; ny += x; break;
            case 7: nx += x; ny += y; break;
        }
        return _blocksLight(nx, ny);
    }

    public Func<int, int, bool> _blocksLight;
    public Func<int, int, int> GetDistance;
    public Action<int, int, bool> SetVisible;
}
