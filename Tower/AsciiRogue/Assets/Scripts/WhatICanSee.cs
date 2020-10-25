using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhatICanSee : MonoBehaviour
{
    FOVNEW fv;
    PlayerStats playerStats;

    private void Start()
    {
        playerStats = GameManager.manager.playerStats;
        fv = GetComponent<FOVNEW>();
    }

    void Update()
    {
        if(Controls.GetKeyDown(Controls.Inputs.WhatCanISee))
        {
            Debug.Log("LOLO");
            fv.Compute(MapManager.playerPos, playerStats.viewRange);
            foreach (var visibleTile in fv.visibleTiles)
            {
                if(MapManager.map[visibleTile.x,visibleTile.y].enemy != null)
                {
                    if(MapManager.map[visibleTile.x, visibleTile.y].enemy.GetComponent<RoamingNPC>().sleeping)
                    {
                        GameManager.manager.UpdateMessages($"You see <color=lightblue>sleeping</color> <color={MapManager.map[visibleTile.x, visibleTile.y].enemy.GetComponent<RoamingNPC>().EnemyColor}>{MapManager.map[visibleTile.x, visibleTile.y].enemy.GetComponent<RoamingNPC>().EnemyName}</color>.");
                    }
                    else
                    {
                        GameManager.manager.UpdateMessages($"You see <color={MapManager.map[visibleTile.x, visibleTile.y].enemy.GetComponent<RoamingNPC>().EnemyColor}>{MapManager.map[visibleTile.x, visibleTile.y].enemy.GetComponent<RoamingNPC>().EnemyName}</color>.");
                    }
                }
            }
        }
    }
}
