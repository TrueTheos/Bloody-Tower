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
    public enum BUC { normal,blessed,cursed}
    public BUC _BUC;

    public bool identified;

    //BOOK INFO
    public int spellbookCooldown;
    public bool learned = false;
    public int learningTurns;
    public int durationLeft;

    [HideInInspector] public int Anvil_bonusToHealth;
    [HideInInspector] public int Anvil_bonusToStrength;
    [HideInInspector] public int Anvil_bonusToIntelligence;
    [HideInInspector] public int Anvil_bonusToDexterity;
    [HideInInspector] public int Anvil_bonusToEndurance;
    [HideInInspector] public int Anvil_bonusToNoise;

    [Range(0,9)]public int upgradeLevel;

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
                socket1.Use(GameManager.manager.playerStats, this);
                GameManager.manager.UpdateMessages($"You connected <color={gem.I_color}>{gem.I_name}</color> with <color={iso.I_color}>{iso.I_name}</color>.");
                GameManager.manager.ApplyChangesInInventory(GameManager.manager.gemToConnect);
            }
            else if(socket2 == null && sockets > 1)
            {
                socket2 = (Gem)gem;
                socket1.Use(GameManager.manager.playerStats, this);
                GameManager.manager.UpdateMessages($"You connected <color={gem.I_color}>{gem.I_name}</color> with <color={iso.I_color}>{iso.I_name}</color>.");
                GameManager.manager.ApplyChangesInInventory(GameManager.manager.gemToConnect);
            }
            else if(socket3 == null && sockets == 3)
            {
                socket3 = (Gem)gem;
                socket1.Use(GameManager.manager.playerStats, this);
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

    public void EquipWithGems(Item item)
    {
            if(socket1 != null)
            {
                socket1.Use(GameManager.manager.playerStats, item);
            }
            else if(socket2 != null)
            {
                socket2.Use(GameManager.manager.playerStats, item);
            }
            else if(socket3 != null)
            {
                socket3.Use(GameManager.manager.playerStats, item);
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

    public void OnEquip(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (Anvil_bonusToHealth != 0) { }
            if (Anvil_bonusToStrength != 0) player.__strength += Anvil_bonusToStrength;
            if (Anvil_bonusToIntelligence != 0) player.__intelligence += Anvil_bonusToIntelligence;
            if (Anvil_bonusToDexterity != 0) player.__dexterity += Anvil_bonusToDexterity;
            if (Anvil_bonusToEndurance != 0) player.__endurance += Anvil_bonusToEndurance;
            if (Anvil_bonusToNoise != 0) player.__noise += Anvil_bonusToNoise;
        }
    }

    public void OnUnequip(MonoBehaviour foo)
    {      
        if (foo is PlayerStats player)
        {
            if (Anvil_bonusToHealth != 0) { }
            if (Anvil_bonusToStrength != 0) player.__strength += -Anvil_bonusToStrength;
            if (Anvil_bonusToIntelligence != 0) player.__intelligence += -Anvil_bonusToIntelligence;
            if (Anvil_bonusToDexterity != 0) player.__dexterity += -Anvil_bonusToDexterity;
            if (Anvil_bonusToEndurance != 0) player.__endurance += -Anvil_bonusToEndurance;
            if (Anvil_bonusToNoise != 0) player.__noise += -Anvil_bonusToNoise;
        }
    }

}
