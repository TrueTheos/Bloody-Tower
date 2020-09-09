using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GenTest : MonoBehaviour
{
    public TMPro.TMP_Text TestDrawer;
    public TMPro.TMP_Text Overlay;

    public enum TestType
    {
        Grow,
        GenericTest,
        CornerCheck,
        Temple,
        IsItQuick
    }
    public TestType Type;

    [ContextMenu("Run Test")]
    public void Test()
    {
        //StartCoroutine(RunGenerationTest());
        //StartCoroutine(GrowTest());

        switch (Type)
        {
            case TestType.Grow:
                StartCoroutine(GrowTest());
                break;
            case TestType.GenericTest:
                StartCoroutine(RunGenerationTest());
                break;
            case TestType.CornerCheck:
                StartCoroutine(CornerCheck());
                break;
            case TestType.Temple:
                GeneratorTemple.Logging = TestDrawer;
                GeneratorTemple.Overlay = Overlay;
                StartCoroutine(GeneratorTemple.GetSimpleTemple());
                break;
            case TestType.IsItQuick:
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();

                watch.Start();
                CleanerTemple.GetSimpleTemple();
                TestDrawer.text = CleanerTemple.LastOutput;

                watch.Stop();
                Debug.Log(watch.ElapsedMilliseconds + "ms for generation");
                break;
            default:
                break;
        }
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

    public IEnumerator GrowTest()
    {
        TestDrawer.text = "Grow Test";
        GenUtil.PrintCount = 0;

        GenData growArea = new GenData(40, 25);

        GenRoom left = GenRoom.Sized(10, 15);
        left.FillFloor('.');
        growArea.PlaceRoom(0, 0, left);


        GenRoom right = GenRoom.Sized(10, 15);
        right.FillFloor('.');
        growArea.PlaceRoom(11, 0, right);

        growArea.EdgeWalls('#');


        TestDrawer.text = growArea.Print(false);
        yield return new WaitForSeconds(1f);

        

        
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 13; y++)
            {

                {
                    TestDrawer.text = growArea.Print(false);
                    //yield return new WaitForSeconds(0.05f);

                    bool sucL = growArea.TryGrowRect(1 + x, 1 + y, 5, 6, out GenRect oL, true);

                    GenRoom resultL = GenRoom.Sized(oL.WidthT, oL.HeightT);
                    resultL.FillFloor('O');
                    growArea.PlaceRoom(oL.MinX, oL.MinY, resultL);
                    growArea.EdgeWalls('#');


                    bool sucR = growArea.TryGrowRect(12 + x, 1 + y, 5, 6, out GenRect oR, false);

                    GenRoom resultR = GenRoom.Sized(oR.WidthT, oR.HeightT);
                    resultR.FillFloor('O');
                    growArea.PlaceRoom(oR.MinX, oR.MinY, resultR);
                    growArea.EdgeWalls('#');



                    GenRoom dotL = GenRoom.Sized(1, 1);
                    dotL.FillFloor('+');
                    growArea.PlaceRoom(1+ x, 1 + y, dotL);

                    GenRoom dotR = GenRoom.Sized(1, 1);
                    dotR.FillFloor('X');
                    growArea.PlaceRoom(12 + x, 1 + y, dotR);


                    TestDrawer.text = growArea.Print(false);
                    yield return new WaitForSeconds(0.50f);



                    growArea.Rooms.Remove(dotR);
                    growArea.Rooms.Remove(dotL);
                    growArea.Rooms.Remove(resultL);
                    growArea.Rooms.Remove(resultR);
                }                
            }
        }
        /*
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 13; y++)
            {

                {
                    TestDrawer.text = growArea.Print(false);
                    //yield return new WaitForSeconds(0.05f);

                    bool suc = growArea.TryGrowRect(3 + x, 3 + y, 5, 6, out GenRect o, false);

                    GenRoom result = GenRoom.Sized(o.WidthT, o.HeightT);
                    result.FillFloor('O');
                    growArea.PlaceRoom(o.MinX, o.MinY, result);
                    growArea.EdgeWalls('#');

                    GenRoom dot = GenRoom.Sized(1, 1);
                    dot.FillFloor('X');
                    growArea.PlaceRoom(3 + x, 3 + y, dot);

                    TestDrawer.text = growArea.Print(false);
                    yield return new WaitForSeconds(0.50f);

                    growArea.Rooms.Remove(dot);
                    growArea.Rooms.Remove(result);
                }
            }
        }

        */



    }

    public IEnumerator CornerCheck()
    {
        GenData dat = new GenData(20, 15);

        GenRoom r1 = GenRoom.Sized(8, 8);
        r1.FillFloor('.');
        r1.SpacePriority = 2;
        dat.PlaceRoom(0, 0, r1);

        GenRoom r2 = GenRoom.Sized(10, 7);
        r2.FillFloor('.');
        r2.SpacePriority = 1;
        dat.PlaceRoom(5, 4,r2);

        dat.FixOverlap();
        dat.EdgeWalls('#');

        TestDrawer.text = dat.Print(false);
        yield return new WaitForSeconds(0.5f);

        for (int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if (dat.IsCornerG(x,y))
                {
                    GenRoom corner = GenRoom.Sized(1, 1);
                    corner.FillFloor('X');
                    dat.PlaceRoom(x, y, corner);

                    TestDrawer.text = dat.Print(false);
                    yield return new WaitForSeconds(0.25f);
                }
            }
        }



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
                tile.Details.Add(new GenDetail() { Type = GenDetail.DetailType.Decoration, Char = 'O' });
                GenTile[,] feature = new GenTile[,]
                {
                    {tile }
                };

                int fposX = wantX;
                int fposY = wantY;

                GenTile[,] final = GenUtil.GetSymetry(feature, ref fposX, ref fposY, Hallway, GenUtil.Axis.Vertical);

                Hallway.SetTilesAtG(fposX, fposY, final);

                TestDrawer.text = sym.Print(false);
                yield return new WaitForSeconds(0.5f);


                wantX += 2;
            }
        }

        for (int x = 0; x < 20; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                if (sym.IsCornerG(x, y, GenDetail.DetailType.Wall,GenDetail.DetailType.Decoration))
                {
                    GenRoom corner = GenRoom.Sized(1, 1);
                    corner.FillFloor('X');
                    sym.PlaceRoom(x, y, corner);

                    TestDrawer.text = sym.Print(false);
                    yield return new WaitForSeconds(0.25f);
                }
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
