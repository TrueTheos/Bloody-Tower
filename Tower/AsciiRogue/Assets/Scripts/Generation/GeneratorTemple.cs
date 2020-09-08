using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class GeneratorTemple
{
    public static RangeInt TempleWidth = new RangeInt(45, 10);
    public static RangeInt TempleHeight = new RangeInt(22, 5);

    public static RangeInt RandomWidth = new RangeInt(5, 21);
    public static RangeInt RandomHeight = new RangeInt(5, 21);




    public static TMPro.TMP_Text Logging;

    public static string LastOutput;

    public enum RoomTypes
    {
        FourPillars,
        PillarHallway,
        AltarRoom,
        Empty,
        Outer, // if something else should be put in here
        RandomPillars,
        PillarSpam
    }



    public static IEnumerator GetSimpleTemple()
    {
        GenData temple = new GenData(Random.Range(TempleWidth.start, TempleWidth.end), Random.Range(TempleHeight.start, TempleHeight.end));

        Dictionary<GenRoom, RoomTypes> rooms = new Dictionary<GenRoom, RoomTypes>();

        GenTile pillar = GenTile.GetEmpty();
        //pillar.Details.Add(new GenDetail() { Char = '\u01C1', Type = GenDetail.DetailType.Decoration });
        pillar.Details.Add(new GenDetail() { Char = 'O', Type = GenDetail.DetailType.Decoration });

        GenTile[,] pillars = new GenTile[,]
        {
            {GenTile.Copy(pillar),GenTile.Copy(pillar) },
            {GenTile.Copy(pillar),GenTile.Copy(pillar) }
        };

        GenRoom outer = GenRoom.Sized(temple.Width, temple.Height);
        outer.FillFloor('+');
        outer.SpacePriority = -2;
        temple.PlaceRoom(0, 0, outer);
        temple.EdgeWalls('#', outer);

        // -----------
        Log(temple);
        yield return new WaitForSeconds(0.25f);
        // -----------

        int w = Random.Range(9, 12);
        int h = Random.Range(10, 16);

        // EntryHall
        temple.TryGrowRect(1, outer.Outer.GetCenter().y, w, h, out GenRect EntrySize, false);
        GenRoom EntryHall = GenRoom.At(EntrySize.MinX, EntrySize.MinY, EntrySize.WidthT, EntrySize.HeightT);
        EntryHall.FillFloor('.');
        temple.PlaceRoom(EntrySize.MinX, EntrySize.MinY, EntryHall);
        temple.EdgeWalls('#');
        EntryHall.GetAtWorldspaceG(
            EntrySize.MinX + 1, EntrySize.GetCenter().y)
            .Details
            .Add(new GenDetail() { Char = '>', Type = GenDetail.DetailType.Entity, Entity = GenDetail.EntityType.StairsDown });
        rooms.Add(EntryHall, RoomTypes.FourPillars);

        int posX = EntrySize.MinX + 2;
        int posy = EntrySize.MinY + 2;
        GenTile[,] sym = GenUtil.GetSymetry(pillars.GetCopy(), ref posX, ref posy, EntryHall, GenUtil.Axis.Horizontal | GenUtil.Axis.Vertical);

        EntryHall.PlaceDetailsAt(posX, posy, sym);
        temple.FixOverlap();

        // -----------
        Log(temple);
        yield return new WaitForSeconds(0.25f);
        // -----------

        // hall to big thing
        int whall = Random.Range(10, 25);
        int hhall = Random.Range(5, 7);
        int space = Random.Range(2, 4);

        temple.TryGrowRect(EntrySize.MaxX +1, EntrySize.GetCenter().y, whall, hhall, out GenRect HallSize);
        GenRoom PillarHall = GenRoom.Sized(HallSize.WidthT, HallSize.HeightT);
        PillarHall.FillFloor('.');
        PillarHall.SpacePriority = 1;
        temple.PlaceRoom(HallSize.MinX, HallSize.MinY, PillarHall);
        temple.EdgeWalls('#', PillarHall);

        int currBar = HallSize.MinX + space;
        GenTile[,] singlePillar = new GenTile[,] { { GenTile.Copy(pillar) } };
        while (temple.IsInsideRoom(currBar, HallSize.MinY + 1, PillarHall))
        {
            
            
            int fx = currBar;
            int fy = HallSize.MinY + 1;
            GenTile[,] feature = GenUtil.GetSymetry(singlePillar, ref fx, ref fy, PillarHall, GenUtil.Axis.Vertical);
            PillarHall.PlaceDetailsAt(fx, fy, feature);
            currBar += space;
        }
        temple.FixOverlap();

        // -----------
        Log(temple);
        yield return new WaitForSeconds(0.25f);
        // -----------

        // holy water or something

        int waterHeight = Random.Range(2, 4);
        int waterWidth = Random.Range(2, 4);
        int waterPillarWidth = Random.Range(2, 3);

        int waterRoomHeight = waterHeight + 4 + waterPillarWidth * 2;
        int waterRoomWidth = waterWidth + 6 + waterPillarWidth * 2;

        temple.TryGrowRect(HallSize.MaxX + 1, HallSize.GetCenter().y, waterRoomWidth, waterRoomHeight, out GenRect WaterSize, false, GenUtil.Direction4.Top);

        GenRoom waterRoom = GenRoom.Sized(WaterSize.WidthT, WaterSize.HeightT);
        waterRoom.FillFloor();
        waterRoom.SpacePriority = 2;
        temple.PlaceRoom(WaterSize.MinX, WaterSize.MinY, waterRoom);
        temple.EdgeWalls('#', waterRoom);

        GenTile waterSingle = GenTile.GetEmpty();
        waterSingle.Details.Add(new GenDetail() { Char = '~', Type = GenDetail.DetailType.Background });
        GenTile[,] water = GenUtil.Fill(waterWidth, waterHeight, waterSingle);

        waterRoom.PlaceDetailsAt(WaterSize.MinX + 3 + waterPillarWidth, WaterSize.MinY +2 + waterPillarWidth, water);

        int waterPX = WaterSize.MinX + 3;
        int waterPY = WaterSize.MinY + 2;

        GenTile[,] waterPillarPlace = GenUtil.GetSymetry(pillars.GetCopy(), ref waterPX, ref waterPY, waterRoom, GenUtil.Axis.Horizontal | GenUtil.Axis.Vertical);
        waterRoom.PlaceDetailsAt(waterPX, waterPY, waterPillarPlace);


        temple.FixOverlap();

        // -----------
        Log(temple);
        yield return new WaitForSeconds(0.25f);
        // -----------

        // pillar spam
        int spamWidth = Random.Range(10, 20);
        int spamHeight = WaterSize.HeightT + Random.Range(8, 15);

        temple.TryGrowRect(WaterSize.MaxX + 1, WaterSize.GetCenter().y, spamWidth, spamHeight, out GenRect SpamSize, true, GenUtil.Direction4.Top);
        GenRoom Spam = GenRoom.Sized(SpamSize.WidthT, SpamSize.HeightT);
        Spam.FillFloor();
        Spam.SpacePriority = 1;
        temple.PlaceRoom(SpamSize.MinX, SpamSize.MinY, Spam);
        temple.EdgeWalls('#', Spam);

        int spamPX = SpamSize.MinX + 2;
        int spamPY = SpamSize.MinY + 2;

        for (int x = spamPX; temple.IsInsideRoom(x,spamPY,Spam); x+=2)
        {
            for (int y = spamPY; temple.IsInsideRoom(x, y, Spam); y+=2)
            {
                GenTile p = GenTile.Copy(pillar);
                Spam.AddDetails( x, y,p);
            }
        }
        Spam.AddDetail(
            SpamSize.MaxX - 1, 
            SpamSize.GetCenter().y, 
            new GenDetail()
            { Char = '<', Type = GenDetail.DetailType.Entity, Entity = GenDetail.EntityType.StairsUp });

        temple.FixOverlap();

        // -----------
        Log(temple);
        yield return new WaitForSeconds(0.25f);
        // -----------

        //temple.Rooms.Remove(outer); // we dont have boundries
        for (int x = outer.Inner.MinX; x < outer.Outer.MaxX; x++)
        {
            for (int y = outer.Inner.MinY; y < outer.Outer.MaxY; y++)
            {
                outer.RemoveTileAtG(x, y);
            }
        }


        //GenRoom Mark = GenRoom.Sized(0, 0);
        //Mark.FillFloor('X');
        //Mark.SpacePriority = 100;
        

        // lets go ham with randomly sized rooms
        int spawnAttemptsRemaining = 1000;

        while (spawnAttemptsRemaining-->0)// lol the arrow
        {
            int rWidth = Random.Range(RandomWidth.start, RandomWidth.end);
            int rHeight = Random.Range(RandomHeight.start, RandomHeight.end);

            int rX = Random.Range(1, temple.Width - rWidth-1);
            int rY = Random.Range(1, temple.Height - rHeight-1);
            GenRect rHopeSize = new GenRect(rX, rX + rWidth + 1, rY, rY + rWidth - 1);

            bool valid = true;

            // check if we are inside or around some other room

            foreach (var room in temple.Rooms)
            {
                if (room.Outer.IsEnclosing(rHopeSize) || room.Outer.IsInside(rHopeSize))
                {
                    valid = false;
                }
            }
            if (!valid || temple.IsInsideRoom(rX,rY) || temple.GetTile(rX,rY)!=null)
            {
                continue;
            }

            temple.TryGrowRect(rX, rY, rWidth, rHeight, out GenRect rSize, true);

            GenRoom add = GenRoom.Sized(rSize.WidthT, rSize.HeightT);
            add.FillFloor();
            add.SpacePriority = 01;

            temple.PlaceRoom(rSize.MinX, rSize.MinY, add);
            temple.EdgeWalls('#', add);

            temple.FixOverlap();
            /*
            if (rWidth * 2 < rHeight || rHeight * 2 < rWidth)
            {
                if (Random.Range(0,10)>4)
                {
                    // we are making a hallway

                    //TODO: hallway

                    temple.PlaceRoom(rX, rY, Mark);
                    Log(temple);
                    yield return new WaitForSeconds(0.25f);
                    temple.Rooms.Remove(Mark);
                    continue;
                }
            }
            */


            int randomChance = Random.Range(0, 4);
            switch (randomChance)
            {
                case 0:
                    // random pillars in the room 

                    for (int i = 0; i < 7 + (rSize.WidthT + rSize.HeightT)/5; i++)
                    {
                        int px = Random.Range(rSize.MinX + 1, rSize.MaxX);
                        int py = Random.Range(rSize.MinY + 1, rSize.MaxY);
                        add.AddDetails(px, py, pillar);
                    }

                    break;
                case 1:
                    // random water
                    for (int i = 0; i < 15 + (rSize.WidthT + rSize.HeightT)/3; i++)
                    {
                        int px = Random.Range(rSize.MinX + 1, rSize.MaxX);
                        int py = Random.Range(rSize.MinY + 1, rSize.MaxY);
                        GenDetail littleWater= new GenDetail() { Char = '~', Type = GenDetail.DetailType.Background};
                        add.AddDetail(px, py, littleWater);
                    }
                    break;
                case 2:
                    // random room inside if possible else empty
                    if (rSize.WidthT>=7&& rSize.HeightT >= 7)
                    {
                        int insideX = rSize.GetCenter().x;
                        int insideY = rSize.GetCenter().y;

                        temple.TryGrowRect(insideX, insideY, 100, 100, out GenRect insideSize, false);
                        GenRoom inside = GenRoom.Sized(insideSize.WidthT, insideSize.HeightT);
                        inside.FillFloor();
                        inside.SpacePriority = 2;
                        temple.PlaceRoom(insideSize.MinX, insideSize.MinY, inside);
                        temple.EdgeWalls('#',inside);
                        temple.FixOverlap();

                    }
                    else
                    {
                        for (int i = 0; i < 7; i++)
                        {
                            int px = Random.Range(rSize.MinX + 1, rSize.MaxX);
                            int py = Random.Range(rSize.MinY + 1, rSize.MaxY);
                            add.AddDetails(px, py, pillar);
                        }
                    }
                    break;
                default:
                    break;
            }
            
            Log(temple);
            yield return new WaitForSeconds(0.25f);
        }

        // -----------
        Log(temple);
        yield return new WaitForSeconds(0.25f);
        // -----------

        // spawn enemies and items

        // every room has the chance to have something in it 
        // 50 % chest

        // 50 %  + 20 % chance to have an item

        // enemies are more complecated

        temple.FixOverlap();

        foreach (var room in temple.Rooms)
        {
            // chest
            if (Random.value < 0.5f)
            {
                List<GenTile> corner = new List<GenTile>();

                for (int x = 0; x < room.Width; x++)
                {
                    for (int y = 0; y < room.Height; y++)
                    {
                        if (temple.IsCornerG(room.PosX + x, room.PosY+y))
                        {
                            corner.Add(temple.GetTile(room.PosX + x, room.PosY + y));
                        }
                    }
                }
                corner.GetRandom().Details.Add(new GenDetail() { Char = '=', Type = GenDetail.DetailType.Entity, Entity = GenDetail.EntityType.Chest });
            }

            if (Random.value<0.5f)
            {
                char item = 'i';

                var ran = room.Data
                    .ToList()
                    .Where(t => t.NonTypes(GenDetail.DetailType.Wall, GenDetail.DetailType.Entity, GenDetail.DetailType.Decoration))
                    .ToList();
                if (ran.Count>0)
                {
                    ran.GetRandom()
                    .Details
                    .Add(new GenDetail() { Char = item, Type = GenDetail.DetailType.Entity, Entity = GenDetail.EntityType.Item });
                }
                    
            }
            if (Random.value < 0.2f)
            {
                char item = 'i';

                var ran = room.Data
                    .ToList()
                    .Where(t => t.NonTypes(GenDetail.DetailType.Wall, GenDetail.DetailType.Entity, GenDetail.DetailType.Decoration))
                    .ToList();
                if (ran.Count>0)
                {
                    ran.GetRandom()
                      .Details
                      .Add(new GenDetail() { Char = item, Type = GenDetail.DetailType.Entity, Entity = GenDetail.EntityType.Item });
                }
                    
            }
            int enemyCount = 0;
            switch (Random.Range(0,12))
            {
                case 0:
                case 1:
                    enemyCount = 0;
                    break;
                case 2:
                case 3:
                case 4:                    
                case 5:
                    enemyCount = 1;
                    break;
                case 6:
                case 7:
                case 8:
                case 9:
                    enemyCount = 2;
                    break;
                case 10:
                case 11:
                    enemyCount = 3;
                    break;
                default:
                    break;
            }

            for (int i = 0; i < enemyCount; i++)
            {
                char enemyChar = 'b';

                var en = room.Data
                    .ToList()
                    .Where(t => t.NonTypes(GenDetail.DetailType.Wall, GenDetail.DetailType.Entity, GenDetail.DetailType.Decoration))
                    .ToList();
                if (en.Count>0)
                {
                    en.GetRandom()
                    ?.Details
                    .Add(new GenDetail() { Char = enemyChar, Type = GenDetail.DetailType.Entity, Entity = GenDetail.EntityType.Enemy });
                }
                    
            }




            Log(temple);
            yield return new WaitForSeconds(0.25f);
        }

        // add doors between every possible room because i dont have much time

        temple.Rooms.Remove(outer);
        temple.FixOverlap();

        var adj = temple.GetAdjacentRoomMap();

        for (int i = 0; i < temple.Rooms.Count-1; i++)
        {
            for (int j = i+1; j < temple.Rooms.Count; j++)
            {
                if (adj.ContainsKey(temple.Rooms[i]))
                {
                    if (adj[temple.Rooms[i]].Contains(temple.Rooms[j]))
                    {
                        // we can search for one place for a door
                        GenRoom a = temple.Rooms[i];
                        GenRoom b = temple.Rooms[j];
                        temple.GetDoorableTiles(temple.GetConnectingTiles(a, b))
                            .GetRandom()
                            .Tile
                            .Details
                            .Add(new GenDetail() { Char = '+', Type = GenDetail.DetailType.Entity, Entity = GenDetail.EntityType.Door });

                        Log(temple);
                        yield return new WaitForSeconds(0.25f);
                    }
                }
            }
        }




        Log(temple);
        yield return new WaitForSeconds(0.25f);

        Debug.Log("Done");


        LastOutput = GenUtil.Print(temple, false);
    }
    public static void Log(GenData data)
    {
        if (Logging!=null)
        {
            Logging.text = GenUtil.Print(data, false);
        }
    }

}
