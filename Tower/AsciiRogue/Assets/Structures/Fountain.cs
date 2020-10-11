﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fountain : Structure
{
    public bool used = false;
    public Vector2Int position;


    public override void Use()
    {
        int random = Random.Range(1, 22);

        if(random <= 9)
        {
            GameManager.manager.UpdateMessages("You take a sip, you feel refreshed.");
            GameManager.manager.playerStats.__currentHp += Random.Range(1, 8);
        }
        else if(random <= 18)
        {
            GameManager.manager.UpdateMessages("You take a sip, this liquid is tasteless.");
        }
        else if(random <= 19)
        {
            GameManager.manager.UpdateMessages("The water is contaminated! You gag and vomit.");
            GameManager.manager.playerStats.TakeDamage(Random.Range(1, 8), ItemScriptableObject.damageType.normal);
        }
        else if(random <= 20)
        {
            if(GameManager.manager.playerStats.isBlind)
            {             
                GameManager.manager.UpdateMessages("You hear something hissing!");
            }
            else
            {
                GameManager.manager.UpdateMessages("An endless stream of snakes pour forth!");
            }

            var hamp = GameManager.manager.enemySpawner.Hamp;
            GameManager.manager.enemySpawner.SpawnAt(position.x - 1, position.y - 1, hamp, "false");
            GameManager.manager.enemySpawner.SpawnAt(position.x - 1, position.y, hamp, "false");
            GameManager.manager.enemySpawner.SpawnAt(position.x - 1, position.y + 1, hamp, "false");
            GameManager.manager.enemySpawner.SpawnAt(position.x, position.y + 1, hamp, "false");
            GameManager.manager.enemySpawner.SpawnAt(position.x, position.y - 1, hamp, "false");
            GameManager.manager.enemySpawner.SpawnAt(position.x + 1, position.y - 1, hamp, "false");
            GameManager.manager.enemySpawner.SpawnAt(position.x + 1, position.y, hamp, "false");
            GameManager.manager.enemySpawner.SpawnAt(position.x + 1, position.y + 1, hamp, "false");
        }
        else if(random <= 21)
        {
            GameManager.manager.UpdateMessages("You take a sip, it tastes weird...");

            foreach (var item in GameManager.manager.playerStats.itemsInEqGO)
            {
                item.cursed = true;
            }
        }

        GameManager.manager.FinishPlayersTurn();
    }

    public override void WalkIntoTrigger()
    {

    }
}