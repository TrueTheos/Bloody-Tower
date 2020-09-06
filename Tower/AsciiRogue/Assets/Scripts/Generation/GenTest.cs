using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenTest : MonoBehaviour
{
    public Text TestDrawer;
    [ContextMenu("Run Test")]
    public void Test()
    {
        StartCoroutine(RunGenerationTest());

        //GenData data = new GenData(20,20);
        //GenTile[,] tiles = new GenTile[5, 5];
        //for (int x = 0; x < 5; x++)
        //{
        //    for (int y = 0; y < 5; y++)
        //    {
        //        GenTile tile = GenTile.GetEmpty();
        //        tile.Details.Add(new GenDetail() { Char = '#', Type = GenDetail.DetailType.Wall });
        //        tiles[x, y] = tile;
        //    }
        //}
        //data.TileMap.Place(3, 2, tiles);
        //GenUtil.Print(data);
    }

    
    public IEnumerator RunGenerationTest()
    {
        GenUtil.PrintCount = 0;
        GenData sym = new GenData(20, 15);

        GenRoom Hallway = GenRoom.Sized(15, 5);
        Hallway.FillFloor('.');
        Hallway.SpacePriority = 2;
        sym.PlaceRoom(1, 1, Hallway);

        TestDrawer.text = sym.Print(false);
        yield return new WaitForSeconds(1f);


        GenRoom OtherRooom = GenRoom.Sized(8, 10);
        OtherRooom.FillFloor('.');
        OtherRooom.SpacePriority =3;
        sym.PlaceRoom(11, 3, OtherRooom);

        TestDrawer.text = sym.Print(false);
        yield return new WaitForSeconds(1f);

        sym.EdgeWalls('#');

        TestDrawer.text = sym.Print(false);
        yield return new WaitForSeconds(1f);
        
        OtherRooom.SpacePriority = 1;
        sym.FixOverlap();

        TestDrawer.text = sym.Print(false);
        yield return new WaitForSeconds(1f);
        


        int wantX = 3;
        int wantY = 2;

        bool done = false;

        while (!done)
        {
            var got = Hallway.GetAtWorldspaceG(wantX, wantY);

            if (!sym.IsInsideRoom(wantX, wantY))
            {                
                done = true;
            }
            else
            {
                GenTile tile = GenTile.GetEmpty();
                tile.Details.Add(new GenDetail() { Type = GenDetail.DetailType.Entity, Char = 'O' });
                GenTile[,] feature = new GenTile[,]
                {
                    {tile }
                };

                int fposX = wantX;
                int fposY = wantY;

                GenTile[,] final = GenUtil.GetSymetry(feature, ref fposX, ref fposY, Hallway, GenUtil.Axis.Vertical);

                Hallway.SetTilesAtG(fposX, fposY, final);

                TestDrawer.text = sym.Print(false);
                yield return new WaitForSeconds(1f);


                wantX += 2;
            }


        }

        






        /*
        GenData data = new GenData(40, 20);

        GenRoom startRoom = GenRoom.Sized(5,8,true);
        startRoom.SpacePriority = 0;

        GenRoom room2 = GenRoom.Sized(8, 8, true);
        room2.FillFloor('a');

        GenRoom room3 = GenRoom.Sized(5, 5, true);
        room3.FillFloor('b');
        room3.SpacePriority = 2;

        GenRoom room4 = GenRoom.Sized(10, 10, true);
        room4.FillFloor('c');

        data.PlaceRoom(1, 0, startRoom);
        data.PlaceRoom(4, 1, room2);
        data.PlaceRoom(25, 11, room3);
        data.PlaceRoom(23, 3, room4);

        data.FixOverlap();
        data.EdgeWalls('#');

        data.Print(false);
        */
    }


}
