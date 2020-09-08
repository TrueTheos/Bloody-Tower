using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenTile 
{

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
    
    public static GenTile Copy(GenTile tile)
    {
        GenTile newTile = new GenTile();
        foreach (var detail in tile.Details)
        {
            newTile.Details.Add(detail.Clone());
        }
        return newTile;
    }

    public bool AnyTypes(params GenDetail.DetailType[] types)
    {
        foreach (GenDetail detail in Details)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i]==detail.Type)
                {
                    return true;
                }
            }
        }
        return false;
    }
    public bool NonTypes(params GenDetail.DetailType[] types)
    {
        foreach (GenDetail detail in Details)
        {
            for (int i = 0; i < types.Length; i++)
            {
                if (types[i] == detail.Type)
                {
                    return false;
                }
            }
        }
        return true;
    }
    public int RemoveTypes(params GenDetail.DetailType[] types)
    {
        int removeCount = 0;
        foreach (var detail in Details.ToArray())
        {
            for(int i = 0; i < types.Length; i++)
            {
                if (types[i] == detail.Type)
                {
                    Details.Remove(detail);
                    removeCount++;
                }
            }
        }
        return removeCount;
    }
}
