using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenTile 
{
    public int PosX;
    public int PosY;

    public List<GenDetail> Details;

    /*
    public enum TileLocation { }

    public GenRoom PRoom; // parent Room
    public GenHallway PHallway; // parent Hallway
    */

    private GenTile()
    {
        Details = new List<GenDetail>();
    }


    public static GenTile GetEmpty()
    {
        return new GenTile();
    }

    public static GenTile GetAt(int x, int y)
    {
        return new GenTile() { PosX = x, PosY = y };
    }

}
