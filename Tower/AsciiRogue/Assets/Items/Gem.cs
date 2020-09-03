using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Gem")]
public class Gem : ItemScriptableObject
{
    public enum statIncreaseType
    {
        strength,
        dexterity,
        intelligence,
        endurance,
        all
    }
    public statIncreaseType _statIncreaseType;

    public int amount;

    public override void Use(MonoBehaviour foo, Item itemObject)
    {
        switch(_statIncreaseType)
        {
            case statIncreaseType.strength:
                GameManager.manager.playerStats.__strength += amount;
                break;
            case statIncreaseType.dexterity:
                GameManager.manager.playerStats.__dexterity += amount;
                break;
            case statIncreaseType.intelligence:
                GameManager.manager.playerStats.__intelligence += amount;
                break;  
            case statIncreaseType.endurance:
                GameManager.manager.playerStats.__endurance += amount;
                break;
            case statIncreaseType.all:
                GameManager.manager.playerStats.__strength += amount;
                GameManager.manager.playerStats.__dexterity += amount;
                GameManager.manager.playerStats.__intelligence += amount;
                GameManager.manager.playerStats.__endurance += amount;
                break;
        }
    }

    public void DequipGems(MonoBehaviour foo)
    {
        switch(_statIncreaseType)
        {
            case statIncreaseType.strength:
                GameManager.manager.playerStats.__strength -= amount;
                break;
            case statIncreaseType.dexterity:
                GameManager.manager.playerStats.__dexterity -= amount;
                break;
            case statIncreaseType.intelligence:
                GameManager.manager.playerStats.__intelligence -= amount;
                break;  
            case statIncreaseType.endurance:
                GameManager.manager.playerStats.__endurance -= amount;
                break;
            case statIncreaseType.all:
                GameManager.manager.playerStats.__strength -= amount;
                GameManager.manager.playerStats.__dexterity -= amount;
                GameManager.manager.playerStats.__intelligence -= amount;
                GameManager.manager.playerStats.__endurance -= amount;
                break;
        }
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }

    public override void onEquip(MonoBehaviour foo)
    {
    }

    public override void onUnequip(MonoBehaviour foo)
    {
    }
}
