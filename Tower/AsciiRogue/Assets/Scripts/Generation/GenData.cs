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
       

    public List<GenTile> GetAllTiles(int gx, int gy)
    {
        List<GenTile> tiles = new List<GenTile>();
        foreach (var room in Rooms)
        {
            GenTile tile = room.GetAtWorldspaceG(gx, gy);
            if (tile!=null)
            {
                tiles.Add(tile);
            }
        }
        return tiles;
    }
    public List<GenRoom> GetOccupyingRooms(int gx, int gy)
    {
        List<GenRoom> tiles = new List<GenRoom>();
        foreach (var room in Rooms)
        {
            GenTile tile = room.GetAtWorldspaceG(gx, gy);
            if (tile != null)
            {
                tiles.Add(room);
            }
        }
        return tiles;
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
    public GenTile[,] GetTiles(GenRect rect)
    {
        GenTile[,] tiles = new GenTile[rect.WidthT, rect.HeightT];
        for (int x = 0; x < tiles.GetLength(0); x++)
        {
            for (int y = 0; y < tiles.GetLength(1); y++)
            {
                tiles[x, y] = GetTile(rect.MinX + x, rect.MinY + y);
            }
        }
        return tiles;
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

                    HashSet<GenPositionTile> dominantEdge = dominant.GetEdge();

                    for (int x = union.MinX; x <= union.MaxX; x++)
                    {
                        for (int y = union.MinY; y <= union.MaxY; y++)
                        {
                            // check if on the edge
                            if (dominantEdge.Any( e => e.Tile == dominant.GetAtWorldspaceG(x,y)))
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

    public void EdgeWalls(char c, GenRoom specific = null)
    {
        if (specific != null)
        {
            foreach (var tile in specific.GetEdge())
            {
                tile.Tile.Details.Add(new GenDetail() { Char = c, Type = GenDetail.DetailType.Wall });
            }
        }
        else
        {
            foreach (GenRoom item in Rooms)
            {
                foreach (var tile in item.GetEdge())
                {
                    tile.Tile.Details.Add(new GenDetail() { Char = c, Type = GenDetail.DetailType.Wall });
                }
            }
        }
        
    }
    public List<GenPositionTile> GetConnectingTiles(GenRoom a, GenRoom b)
    {
        List<GenPositionTile> at = a.GetEdge().ToList();
        List<GenPositionTile> aa = a.GetAllTiles();
        List<GenPositionTile> bt = b.GetEdge().ToList();
        List<GenPositionTile> ba = b.GetAllTiles();

        List<GenPositionTile> result = new List<GenPositionTile>();

        foreach (var item in at)
        {
            if (ba.Any(x=>x.Tile==item.Tile))
            {
                result.Add(item);
            }
        }
        foreach (var item in bt)
        {
            if (aa.Any(x=>x.Tile == item.Tile))
            {
                result.Add(item);
            }
        }
        result = result.Distinct().ToList();
        return result;
    }

    public List<GenPositionTile> GetDoorableTiles(List<GenPositionTile> tiles)
    {
        return tiles.Where(t => CanBeDoor(t.PositionG.x, t.PositionG.y)).ToList();
    }

    public Dictionary<GenRoom,List<GenRoom>> GetAdjacentRoomMap()
    {
        Dictionary<GenRoom, List<GenRoom>> result = new Dictionary<GenRoom, List<GenRoom>>();

        foreach (var origin in Rooms)
        {
            result.Add(origin, new List<GenRoom>());
            foreach (var destination in Rooms)
            {
                if (origin!=destination)
                {
                    if (GetDoorableTiles(GetConnectingTiles(origin,destination)).Count>0)
                    {
                        result[origin].Add(destination);
                    }
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Provides a map of any with doors connected rooms
    /// </summary>
    public Dictionary<GenRoom,List<GenRoom>> GetDooredMap()
    {
        Dictionary<GenRoom, List<GenRoom>> result = new Dictionary<GenRoom, List<GenRoom>>();

        foreach (var origin in Rooms)
        {
            result.Add(origin, new List<GenRoom>());
            foreach (var destination in Rooms)
            {
                if (origin != destination)
                {
                    if (GetConnectingTiles(origin, destination).Any(t=>t.Tile.AnyTypes(GenDetail.DetailType.Door)))
                    {
                        result[origin].Add(destination);
                    }
                }
            }
        }
        return result;
    }

    public bool IsReachable(GenRoom origin, GenRoom destination,Dictionary<GenRoom, List<GenRoom>> map = null)
    {
        // straight ripped out of the wikipedia pseudocode for Breadth-first search
        if (map==null)
        {
            map = GetDooredMap();
        }         

        List<GenRoom> visited = new List<GenRoom>();

        Queue<GenRoom> queue = new Queue<GenRoom>();
        queue.Enqueue(origin);
        visited.Add(origin);

        while ((queue.Count>0))
        {
            GenRoom node = queue.Dequeue();

            if (node == destination)
            {
                // we reached our destination
                return true;
            }
            foreach (var child in map[node])
            {
                if (!visited.Contains(child))
                {
                    queue.Enqueue(child);
                    visited.Add(child);
                }
            }
        }
        return false;

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
            if (!(room.Inner.MinX <= gx && room.Inner.MaxX >= gx && room.Inner.MinY <= gy && room.Inner.MaxY >= gy))
            {
                return false;
            }
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
    public bool IsCornerGR(int gx, int gy,GenRoom room, GenDetail.DetailType[] checkFor = null, GenDetail.DetailType[] noWall = null)
    {
        if (checkFor== null)
        {
            checkFor = new GenDetail.DetailType[] { GenDetail.DetailType.Wall };
        }
        if (noWall == null)
        {
            noWall = new GenDetail.DetailType[] { GenDetail.DetailType.Door };
        }

        bool vertical = false;
        bool horizontal = false;
        GenTile tile;
        if (room!=null)
        {
            tile = room.GetAtWorldspaceG(gx, gy);
            if (tile!=null)
            {
                GenTile check = room.GetAtWorldspaceG(gx+1, gy);
                if (check!=null && check.AnyTypes(checkFor)&&check.NonTypes(noWall))
                {
                    horizontal = true;
                }
                check = room.GetAtWorldspaceG(gx-1, gy);
                if (check != null && check.AnyTypes(checkFor) && check.NonTypes(noWall))
                {
                    horizontal = true;
                }
                check = room.GetAtWorldspaceG(gx, gy+1);
                if (check != null && check.AnyTypes(checkFor) && check.NonTypes(noWall))
                {
                    vertical = true;
                }
                check = room.GetAtWorldspaceG(gx, gy-1);
                if (check != null && check.AnyTypes(checkFor) && check.NonTypes(noWall))
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
    /// <summary>
    /// these setting should be 1 above the one you want
    /// </summary>
    public struct GrowSettings
    {
        public int Top;
        public int Right;
        public int Bot;
        public int Left;

        public GrowSettings(int top, int right, int bot, int left)
        {
            Top = top;
            Right = right;
            Bot = bot;
            Left = left;
        }
    }
    /// <summary>
    /// grows a room to wanted size
    /// </summary>
    /// <param name="gx">global starting positon</param>
    /// <param name="gy">global starting position</param>
    /// <param name="tWidth">target Width</param>
    /// <param name="tHeight">target Height</param>
    /// <param name="rect">the resulting Rect/best match</param>
    /// <param name="connectToWall">When hitting a wall should it be used?</param>
    /// <param name="startDir">the direction the rect should start growing in</param>
    /// <returns>if the procedure was successful</returns>
    public bool TryGrowRect(
        int gx, 
        int gy, 
        int tWidth, 
        int tHeight, 
        out GenRect rect, 
        bool connectToWall = false, 
        GenUtil.Direction4 startDir = GenUtil.Direction4.Right, 
        GrowSettings extra = new GrowSettings()) // merge to wall
    {
        // create base rect
        rect = new GenRect(gx, gx, gy, gy);
        GenRect helper = new GenRect(rect);
        // this is used to confirm what is inside the rect and already allowed
        List<GenTile> Confirmed = new List<GenTile>();
        // add the current tile to it
        var s = GetTiles(rect).ToList();
        Confirmed.AddRange(s);
        // used to get easier access to 

        // if we reached max growth in this direction
        // -1 is not reached
        // 0 is reached
        // 1 reached but grow 1 in the end
        int top = extra.Top-1;
        int right = extra.Right-1;
        int bot = extra.Bot-1;
        int left = extra.Left-1;

        GenUtil.Direction4 direction = startDir;

        int width = rect.WidthT;
        int height = rect.HeightT;
        int ohNo = 100;
        while (width < tWidth || height < tHeight)
        {
            Confirmed.Clear();
            s = GetTiles(rect).ToList();
            Confirmed.AddRange(s);

            ohNo--;
            helper = rect;
            switch (direction)
            {
                case GenUtil.Direction4.Right:
                    if (width==tWidth|| right>=0)
                    {
                        break;
                    }
                    helper = helper.Transform(0, 0, 1, 0);
                    List<GenTile> addR = GetTiles(helper).ToList();
                    addR = addR.Except(Confirmed).ToList();
                    if (addR.Any(t => t.AnyTypes(GenDetail.DetailType.Wall)))
                    {
                        // we hit a wall next to us
                        // they are automatically allowed

                        Confirmed.AddRange(addR);
                        rect = helper.Transform(0, 0, -1, 0);
                        right = 1;
                    }
                    else
                    {
                        // we check the next one for safety

                        helper = helper.Transform(0, 0, 1, 0);
                        List<GenTile> query = GetTiles(helper).ToList();
                        query = query.Except(Confirmed).ToList();

                        if (query.Any(t => t.AnyTypes(GenDetail.DetailType.Wall)))
                        {
                            if (connectToWall)
                            {
                                // try and pull the other way
                                if (left < 1)
                                {
                                    helper = helper.Transform(-1, 0, 0, 0);
                                    left = -1;
                                }

                                // we connect without problem
                                Confirmed.AddRange(query);
                                rect = helper.Transform(0, 0, -1, 0);
                                right = 1;

                                
                                
                            }
                            else
                            {
                                // we prevent connection and transform 2 back
                                helper = helper.Transform(0, 0, -2, 0);
                                right = 0;

                                rect = helper;                                
                            }
                        }
                        else
                        {
                            helper.Transform(0, 0, -1, 0);
                            rect = helper;
                        }
                        
                    }
                    break;
                case GenUtil.Direction4.Bottom:
                    if (height == tHeight || bot >= 0)
                    {
                        break;
                    }
                    helper = helper.Transform(0, 0, 0, 1);
                    List<GenTile> addB = GetTiles(helper).ToList();
                    addB = addB.Except(Confirmed).ToList();
                    if (addB.Any(t => t.AnyTypes(GenDetail.DetailType.Wall)))
                    {
                        // we hit a wall next to us
                        // they are automatically allowed

                        Confirmed.AddRange(addB);
                        rect = helper.Transform(0, 0, 0, -1);
                        bot = 1;
                    }
                    else
                    {
                        // we check the next one for safety

                        helper = helper.Transform(0, 0, 0, 1);
                        List<GenTile> query = GetTiles(helper).ToList();
                        query = query.Except(Confirmed).ToList();

                        if (query.Any(t => t.AnyTypes(GenDetail.DetailType.Wall)))
                        {
                            if (connectToWall)
                            {
                                if (top < 1)
                                {
                                    helper = helper.Transform(0, -1, 0, 0);
                                    top = -1;
                                }

                                // we connect without problem
                                Confirmed.AddRange(query);
                                rect = helper.Transform(0, 0, 0, -1);
                                bot = 1;
                            }
                            else
                            {
                                // we prevent connection and transform 2 back
                                helper = helper.Transform(0, 0, -0, -2);
                                bot = 0;

                                rect = helper;
                            }
                        }
                        else
                        {
                            helper.Transform(0, 0, 0, -1);
                            rect = helper;
                        }
                    }

                    break;
                case GenUtil.Direction4.Left:
                    if (width == tWidth || left >= 0)
                    {
                        break;
                    }
                    helper = helper.Transform(1, 0, 0, 0);
                    List<GenTile> addL = GetTiles(helper).ToList();
                    addL = addL.Except(Confirmed).ToList();
                    if (addL.Any(t => t.AnyTypes(GenDetail.DetailType.Wall)))
                    {
                        // we hit a wall next to us
                        // they are automatically allowed

                        Confirmed.AddRange(addL);
                        rect = helper.Transform(-1, 0, 0,0 );
                        left = 1;
                    }
                    else
                    {
                        // we check the next one for safety

                        helper = helper.Transform(1, 0, 0, 0);
                        List<GenTile> query = GetTiles(helper).ToList();
                        query = query.Except(Confirmed).ToList();

                        if (query.Any(t => t.AnyTypes(GenDetail.DetailType.Wall)))
                        {
                            if (connectToWall)
                            {
                                // try and pull the other way
                                if (right < 1)
                                {
                                    helper = helper.Transform(0, 0, -1, 0);
                                    right = -1;
                                }
                                // we connect without problem
                                Confirmed.AddRange(query);
                                rect = helper.Transform(-1, 0, 0, 0);
                                left = 1;
                            }
                            else
                            {
                                // we prevent connection and transform 2 back
                                helper = helper.Transform(-2, 0, 0, 0);
                                left = 0;

                                rect = helper;
                            }
                        }
                        else
                        {
                            helper.Transform(-1, 0, 0, 0);
                            rect = helper;
                        }
                    }

                    break;
                case GenUtil.Direction4.Top:
                    if (height == tHeight || top >= 0)
                    {
                        break;
                    }
                    helper = helper.Transform(0, 1, 0, 0);
                    List<GenTile> addT = GetTiles(helper).ToList();
                    addT = addT.Except(Confirmed).ToList();
                    if (addT.Any(t => t.AnyTypes(GenDetail.DetailType.Wall)))
                    {
                        // we hit a wall next to us
                        // they are automatically allowed

                        Confirmed.AddRange(addT);
                        rect = helper.Transform(0, -1, 0, 0);
                        top = 1;
                    }
                    else
                    {
                        // we check the next one for safety

                        helper = helper.Transform(0, 1, 0, 0);
                        List<GenTile> query = GetTiles(helper).ToList();
                        query = query.Except(Confirmed).ToList();

                        if (query.Any(t => t.AnyTypes(GenDetail.DetailType.Wall)))
                        {
                            if (connectToWall)
                            {
                                // try and pull the other way
                                if (bot < 1)
                                {
                                    helper = helper.Transform(0, 0, 0, -1);
                                    bot = -1;
                                }
                                // we connect without problem
                                Confirmed.AddRange(query);
                                rect = helper.Transform(0, -1, 0, 0);
                                top = 1;
                            }
                            else
                            {
                                // we prevent connection and transform 2 back
                                helper = helper.Transform(0, -2, 0, 0);
                                top = 0;

                                rect = helper;

                            }
                        }
                        else
                        {
                            helper.Transform(0, -1, 0, 0);
                            rect = helper;
                        }
                    }

                    break;
                default:
                    break;
            }

            width = rect.WidthT;
            if (left > 0) width++;
            if (right > 0) width++;
            height = rect.HeightT;
            if (top > 0) height++;
            if (bot > 0) height++;

            if (top>=0 && right>=0 && bot>=0 && left>=0)
            {
                break;
            }
            if (ohNo <= 0)
            {
                break;
            }
            direction = GenUtil.Rotate(direction, 1);
        }

        rect = rect.Transform(Mathf.Max(0, left), Mathf.Max(0, top), Mathf.Max(0, right), Mathf.Max(0, bot));

        if (rect.WidthT==tWidth&&rect.HeightT == tHeight)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// checks if Position is in the corner (floor, not wall)
    /// </summary>
    public bool IsCornerG(int gx, int gy,params GenDetail.DetailType[] checkFor )
    {
        if (checkFor.Length==0)
        {
            checkFor = new GenDetail.DetailType[] { GenDetail.DetailType.Wall };
        }

        
        GenTile tile = GetTile(gx, gy);
        // if it is a wall we dont need to bother as it is never a corner
        if (tile == null || tile.AnyTypes(checkFor))
        {
            return false;
        }    

        int horizontal = 0;
        int vertical = 0;


        GenTile r = GetTile(gx + 1, gy);
        if (r.AnyTypes(checkFor)) horizontal++;
        GenTile l = GetTile(gx - 1, gy);
        if (l.AnyTypes(checkFor)) horizontal++;

        GenTile t = GetTile(gx, gy - 1);
        if (t.AnyTypes(checkFor)) vertical++;
        GenTile b = GetTile(gx, gy + 1);
        if (b.AnyTypes(checkFor)) vertical++;

        return horizontal>0 && vertical>0;
    }

    public  bool CanBeDoor(int gx, int gy, params GenDetail.DetailType[] checkFor)
    {
        if (checkFor.Length == 0)
        {
            checkFor = new GenDetail.DetailType[] { GenDetail.DetailType.Wall };
        }
        bool horizontal = false;
        bool vertical = false;

        int walls = 0;




        GenTile r = GetTile(gx + 1, gy);
        if (r == null || r.AnyTypes(checkFor)) walls++;
        GenTile l = GetTile(gx - 1, gy);
        if (l == null || l.AnyTypes(checkFor)) walls++;

        if ((r==null  || r.AnyTypes(checkFor)) && (l ==null || l.AnyTypes(checkFor)))
        {
            horizontal = true;
        }
        GenTile t = GetTile(gx, gy-1);
        if (t == null || t.AnyTypes(checkFor)) walls++;
        GenTile b = GetTile(gx , gy+1);
        if (b==null || b.AnyTypes(checkFor)) walls++;

        if ((t == null ||  t.AnyTypes(checkFor) )&& (b == null || b.AnyTypes(checkFor)))
        {
            vertical = true;
        }


        return walls % 2 == 0 &&(horizontal ^ vertical);
    }

    

}
