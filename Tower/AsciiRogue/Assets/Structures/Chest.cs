using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Chest : Structure
{
    public ItemScriptableObject itemInChest;
    public bool opened;

    GameManager manager = GameManager.manager;

    public override void Use()
    {
        if (!opened)
        {
            if (itemInChest == null) 
            {
                GameManager.manager.UpdateMessages("Nothing here.");
                return;
            }
            else
            {
                if (itemInChest.normalIdentifState)
                {
                    GameManager.manager.UpdateMessages($"You opened the chest. It contains <color={itemInChest.I_color}>{itemInChest.I_name}</color>.");
                }
                else
                {
                    GameManager.manager.UpdateMessages($"You opened the chest. It contains <color=purple>{itemInChest.I_unInName}</color>.");
                }
                opened = true;
            }
        }
        if (opened)
        {
            if(itemInChest == null)
            {
                GameManager.manager.UpdateMessages("The chest is empty.");
                return;
            }

            if (manager.playerStats.maximumInventorySpace > manager.playerStats.currentItems && manager.playerStats.currentWeight + itemInChest.I_weight <= manager.playerStats.maxWeight)
            {
                manager.playerStats.currentWeight += itemInChest.I_weight;

                manager.playerStats.currentItems++;
                //manager.playerStats.itemsInEq.Add(itemInChest);              
                manager.playerStats.AddItemFromChest(itemInChest);

                if (itemInChest.normalIdentifState)
                {
                    manager.UpdateMessages($"You picked up the <color={itemInChest.I_color}>{itemInChest.I_name}</color>.");
                    manager.UpdateInventoryQueue($"<color={itemInChest.I_color}>{itemInChest.I_name}</color>");
                }
                else
                {
                    manager.UpdateMessages($"You picked up the <color=purple>{itemInChest.I_unInName}</color>.");
                    manager.UpdateInventoryQueue($"<color=purple>{itemInChest.I_unInName}</color>");
                }               

                itemInChest = null;
            }
            else if (manager.playerStats.maximumInventorySpace < manager.playerStats.currentItems)
            {
                manager.UpdateMessages("I can't carry anything more.");               
            }
            else if (manager.playerStats.currentWeight + itemInChest.I_weight > manager.playerStats.maxWeight)
            {
                manager.UpdateMessages($"Its too heavy.");
            }
        }
    }

    public override void WalkIntoTrigger()
    {
      
    }
}
