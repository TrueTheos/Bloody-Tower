using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenTest : MonoBehaviour
{
    
    
    public void Test()
    {

        GenData data = new GenData(20,20);
        GenTile[,] tiles = new GenTile[5, 5];
        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                GenTile tile = GenTile.GetAt(x,y);
                tile.Details = new List<GenDetail>();
                tile.Details.Add(new GenDetail() { Char = '#', Type = GenDetail.DetailType.Wall });
                tiles[x, y] = tile;
            }
        }
        data.TileMap.Place(3, 2, tiles);
        GenUtil.Print(data);
    }

    [ContextMenu("Run Test")]
    public void RunGenerationTest()
    {

        GenData data = new GenData(40, 20);

        GenRoom startRoom = GenRoom.Sized(5,8,true);
        startRoom.SpacePriority = 2;

        GenRoom room2 = GenRoom.Sized(8, 8, true);
        room2.FillFloor('a');

        GenRoom room3 = GenRoom.Sized(5, 5, true);
        room3.FillFloor('b');
        room3.SpacePriority = 2;

        GenRoom room4 = GenRoom.Sized(10, 10, true);
        room4.FillFloor('c');

        data.PlaceRoom(1, 0, startRoom);
        data.PlaceRoom(4, 1, room2);
        data.PlaceRoom(25, 6, room3);
        data.PlaceRoom(23, 3, room4);

        data.FixOverlap();
        data.EdgeWalls('#');

        data.Print("test.txt", false);

    }


}
