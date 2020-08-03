using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public ItemScriptableObject iso;

    public int sockets = 0;

    public Gem socket1;
    public Gem socket2;
    public Gem socket3;

    public bool isEquipped = false;

    public bool equippedPreviously = false;

    public bool cursed;

    public enum hand
    {
        none, left, right
    }
    public hand _handSwitch;

    public void AddGem(ItemScriptableObject gem)
    {
        if(sockets > 0)
        {
            if(socket1 == null && sockets > 0)
            {
                socket1 = (Gem)gem;
                GameManager.manager.UpdateMessages($"You connected <color={gem.I_color}>{gem.I_name}</color> with <color={iso.I_color}>{iso.I_name}</color>.");
                GameManager.manager.ApplyChangesInInventory(GameManager.manager.gemToConnect);
            }
            else if(socket2 == null && sockets > 1)
            {
                socket2 = (Gem)gem;
                GameManager.manager.UpdateMessages($"You connected <color={gem.I_color}>{gem.I_name}</color> with <color={iso.I_color}>{iso.I_name}</color>.");
                GameManager.manager.ApplyChangesInInventory(GameManager.manager.gemToConnect);
            }
            else if(socket3 == null && sockets == 3)
            {
                socket3 = (Gem)gem;
                GameManager.manager.UpdateMessages($"You connected <color={gem.I_color}>{gem.I_name}</color> with <color={iso.I_color}>{iso.I_name}</color>.");
                GameManager.manager.ApplyChangesInInventory(GameManager.manager.gemToConnect);
            }
            else
            {
                GameManager.manager.UpdateMessages($"<color={iso.I_color}>{iso.I_name}</color> has no free sockets.");
                return;
            }
        }
        else
        {
            GameManager.manager.UpdateMessages($"<color={iso.I_color}>{iso.I_name}</color> has no sockets.");
            return;
        }
    }

    public void EquipWithGems()
    {
            if(socket1 != null)
            {
                socket1.Use(GameManager.manager.playerStats);
            }
            else if(socket2 != null)
            {
                socket2.Use(GameManager.manager.playerStats);
            }
            else if(socket3 != null)
            {
                socket3.Use(GameManager.manager.playerStats);
            }
    }
    public void UnequipWithGems()
    {
            if(socket1 != null)
            {
                socket1.DequipGems(GameManager.manager.playerStats);
            }
            else if(socket2 != null)
            {
                socket2.DequipGems(GameManager.manager.playerStats);
            }
            else if(socket3 != null)
            {
                socket3.DequipGems(GameManager.manager.playerStats);
            }
    }
}
