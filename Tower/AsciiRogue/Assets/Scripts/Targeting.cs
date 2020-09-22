using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Targeting
{
    private static bool _isTargeting;


    public static bool IsTargeting
    {
        get
        {
            return _isTargeting;
        }
        set
        {
            if (_isTargeting!=value)
            {
                if (value)
                {
                    // enable
                    StartTargeting();
                }
                else
                {
                    // disable
                    StopTargeting();
                }
            }
        }
    }

    /// <summary>
    /// determins if the Targeting is allowed to target places that have not been explored
    /// </summary>
    public static bool RequireDiscovery;
    /// <summary>
    /// determins how far the targeting can go. -1 will allow for unlimited range
    /// </summary>
    public static int MaxRange = -1;

    /// <summary>
    /// the current position of targeting
    /// </summary>
    public static Vector2Int Position;

    /// <summary>
    /// the previous position of targeting
    /// </summary>
    private static Vector2Int LastPosition;
    private static Vector2Int changePosition;
    /// <summary>
    /// initializes the targeting to be fresh
    /// </summary>
    public static void StartTargeting()
    {
        _isTargeting = true;
        Position = PlayerMovement.playerMovement.position;
        LastPosition = Position;
    }
    /// <summary>
    /// Disables targeting, preserves Values
    /// </summary>
    public static void StopTargeting()
    {
        _isTargeting = false;
        MaxRange = -1;
    }
    /// <summary>
    /// revert the changes done in the last update, if a targeting is not valid
    /// </summary>
    public static void Revert()
    {
        Position = LastPosition;
    }
    
    public static void UpdateTargeting()
    {
        if (IsTargeting)
        {
            changePosition = Position;
            if (PlayerMovement.playerMovement.canMove) PlayerMovement.playerMovement.canMove = false;

            if ((Input.GetKeyDown(KeyCode.W) ||Input.GetKeyDown( KeyCode.Keypad8))&& CellIsAvailable(Position.x, Position.y + 1))
            {
                MoveTargeting(0, 1);
            }
            else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.Keypad2)) && CellIsAvailable(Position.x, Position.y - 1))
            {
                MoveTargeting(0, -1);
            }
            else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Keypad4)) && CellIsAvailable(Position.x - 1, Position.y))
            {
                MoveTargeting(-1, 0);
            }
            else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Keypad6)) && CellIsAvailable(Position.x + 1, Position.y))
            {
                MoveTargeting(1, 0);
            }
            else if ( Input.GetKeyDown(KeyCode.Keypad1) && CellIsAvailable(Position.x -1, Position.y -1))
            {
                MoveTargeting(-1,-1);
            }
            else if ( Input.GetKeyDown(KeyCode.Keypad3) && CellIsAvailable(Position.x + 1, Position.y - 1))
            {
                MoveTargeting(1, -1);
            }
            else if ( Input.GetKeyDown(KeyCode.Keypad9) && CellIsAvailable(Position.x + 1, Position.y + 1))
            {
                MoveTargeting(1, 1);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad7) && CellIsAvailable(Position.x - 1, Position.y + 1))
            {
                MoveTargeting(-1, 1);
            }
            if (changePosition!=Position)
            {
                LastPosition = changePosition;
            }
        }
    }

    /// <summary>
    /// Moves the targeting to a different position
    /// </summary>
    /// <param name="dx">delta X</param>
    /// <param name="dy">delta Y</param>
    /// <returns>returns the new targeting position</returns>
    public static void MoveTargeting(int dx, int dy)
    {
        Position = Position + new Vector2Int(dx, dy);
    }
    /// <summary>
    /// checks a cell for being not wall or door that is stopping us
    /// </summary>
    /// <param name="x">global X position</param>
    /// <param name="y">global Y position</param>
    /// <returns>if the cell is available</returns>
    private static bool CellIsAvailable(int x, int y)
    {
        return y > 0
            && y < DungeonGenerator.dungeonGenerator.mapHeight
            && x > 0
            && x < DungeonGenerator.dungeonGenerator.mapWidth
            && MapManager.map[x, y].type != "Wall"
            && MapManager.map[x, y].type != "Door"
            && (Vector2Int.Distance(MapManager.playerPos, new Vector2Int(x, y)) < MaxRange || MaxRange == -1);
    }
    

}
