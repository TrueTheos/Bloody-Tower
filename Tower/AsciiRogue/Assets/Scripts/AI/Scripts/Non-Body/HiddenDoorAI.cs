using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="AI/Omni/Hidden Door")]
public class HiddenDoorAI : BaseOmniAI
{
    public int TriggerRange;

    public override void Calculate(OmniBehaviour t)
    {
        // checks if the player is near to the position and 

        int dis = MapUtility.MoveDistance(PlayerMovement.playerMovement.position, t.Position);

        if (dis>TriggerRange)
        {
            // we draw a regular wall
            if (MapUtility.IsInbounds(t.Position))
            {
                MapManager.map[t.Position.x, t.Position.y].baseChar = "#";
                //MapManager.map[t.Position.x, t.Position.y].decoy = "#";
            }
        }
        else
        {
            MapManager.map[t.Position.x, t.Position.y].baseChar = "+";

            MapManager.map[t.Position.x, t.Position.y].exploredColor = new Color(.545f, .27f, .07f);

            MapManager.map[t.Position.x, t.Position.y] = new Tile
            {
                type = "Door",
                baseChar = "+",
                exploredColor = new Color(.545f, .27f, .07f),
                isWalkable = false,
                isOpaque = true
            };
            Door door = new Door();
            door.position = new Vector2Int(t.Position.x, t.Position.y);
            MapManager.map[t.Position.x, t.Position.y].structure = door;


            // we destroy this
            GameManager.manager.enemies.Remove(t.gameObject);
            Destroy(t.gameObject);
        }
    }


}
