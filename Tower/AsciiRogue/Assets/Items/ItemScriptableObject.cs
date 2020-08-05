using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ItemScriptableObject : ScriptableObject
{
    public enum hand
    {
        left,
        right
    }
    public hand _handSwitch;

    public int itemLevel;

    public string I_name;
    public string I_unInName;
    public string I_symbol;
    public string I_color;

    public int levelMin, levelMax;

    public int I_weight;

    public enum rareness
    {
        common,
        rare,
        very_rare,
        mythical
    }
    public rareness I_rareness;

    public enum itemType
    {
        Armor,
        Artefact,
        Comestible,
        Ring,
        Tool,
        Gem,
        Potion,
        Scroll,
        Spellbook,
        Wand,
        Weapon,
        Key
    }
    public itemType I_itemType;

    public enum whereToPutIt
    {
        none,
        head,
        body,
        hand,
        ring,
        legs
    }
    public whereToPutIt I_whereToPutIt;

    public enum BUC
    {
        normal,
        blessed,        
        cursed
    }
    public BUC _BUC;

    public bool normalIdentifState;
    public bool identified;

    public bool isEquipped;
    public bool pickupableByMonsters;

    [TextArea]
    public string effect;
    public int worth;

    public abstract void Use(MonoBehaviour foo);
    public abstract void OnPickup(MonoBehaviour foo);

}
