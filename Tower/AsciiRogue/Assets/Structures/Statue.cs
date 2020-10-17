using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : Structure
{
    public Vector2Int position;

    public string statueName;
    public Color statueColor;

    public override void Use()
    {
        Vector2Int runDirection = position - MapManager.playerPos;

        Vector2Int runCell = position + runDirection;      

        if (MapManager.map[runCell.x, runCell.y].isWalkable && !(MapManager.map[runCell.x, runCell.y].structure is Door door) && !(MapManager.map[runCell.x, runCell.y].structure is BloodDoor bdoor) && !(MapManager.map[runCell.x, runCell.y].structure is Stairs stairs))
        {
            MapManager.map[position.x, position.y].structure = null;
            MapManager.map[position.x, position.y].isWalkable = true;
            MapManager.map[position.x, position.y].letter = "";
            MapManager.map[position.x, position.y].isOpaque = false;
            MapManager.map[position.x, position.y].specialNameOfTheCell = "";
            MapManager.map[position.x, position.y].timeColor = new Color(0, 0, 0);

            GameManager.manager.UpdateMessages("You push the statue");

            MapManager.map[runCell.x, runCell.y].structure = this;
            MapManager.map[runCell.x, runCell.y].isWalkable = false;
            MapManager.map[runCell.x, runCell.y].letter = "}";
            MapManager.map[runCell.x, runCell.y].isOpaque = true;
            MapManager.map[runCell.x, runCell.y].timeColor = statueColor;
            MapManager.map[runCell.x, runCell.y].specialNameOfTheCell = statueName;
            if (MapManager.map[runCell.x, runCell.y].structure is Statue s) s.position = runCell;
        }
        else
        {
            GameManager.manager.UpdateMessages("The statue won't move");
        }
    }

    public override void WalkIntoTrigger()
    {
        
    }
}
