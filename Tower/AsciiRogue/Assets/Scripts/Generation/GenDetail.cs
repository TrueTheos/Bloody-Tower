using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenDetail
{

    public enum EntityType
    {
        None,
        Enemy,
        Item,
        Chest,
        Altar,
        Anvil,
        StairsUp,
        StairsDown,
        Door,
        KeyDoor,
        BloodDoor,

    }
    public int Priority
    {
        get
        {
            switch (Type)
            {
                case DetailType.Entity:
                    return 3;
                case DetailType.Decoration:
                    return 2;
                case DetailType.Background:
                    return 0;
                case DetailType.Wall:
                    return 1;
                case DetailType.Door:
                    return 2;
                case DetailType.Stairs:
                    return 100;
                case DetailType.Nothing:
                    return -1000;
                case DetailType.Floor:
                default:
                    return -1;
            }
        }
    }
    public enum DetailType
    {
        Entity, // interactable, moveable, non-static things
        Decoration, // unpassable tiles
        Background, // or Floor, not sure what is better -> walkable
        Wall,
        Door,
        Stairs,
        Nothing,
        Floor // default 
    }

    public EntityType Entity;
    public DetailType Type;

    public char Char = '¶';
    public string Name = "Invalid";


    public GenDetail Clone()
    {
        GenDetail detail = new GenDetail();
        detail.Entity = Entity;
        detail.Type = Type;
        detail.Char = Char;
        detail.Name = Name;
        return detail;
    }




}
