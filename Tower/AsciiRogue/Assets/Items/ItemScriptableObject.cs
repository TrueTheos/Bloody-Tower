using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class ItemScriptableObject : ScriptableObject
{
    public string I_name; //name that will show if item is identified
    public string I_unInName; //name that will show if item isn't identified

    public string I_symbol;
    public string I_color;

    [Range(0, 1f)] public float chanceOfSpawning1to10;
    [Range(0, 1f)] public float chanceOfSpawning11to20;
    [Range(0, 1f)] public float chanceOfSpawning21to30;
    [Range(0, 1f)] public float chanceOfSpawning31to40;

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
        Key,
        Ammo,
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

    public enum damageType
    {
        normal,
        light,
        magic
    }
    public damageType I_damageType;

    public int lightFactor = 0;

    public bool normalIdentifState;


    public bool pickupableByMonsters;

    [TextArea]
    public string effect;
    public int worth;

    [Header("Skill learning")]
    public List<SkillScriptableObject> SkillsToLearn;

    [Header("Bonus statistics")]
    public int bonusToHealth;
    public int bonusToStrength;
    public int bonusToIntelligence;
    public int bonusToDexterity;
    public int bonusToEndurance;
    public int bonusToNoise;

    public abstract void Use(MonoBehaviour foo, Item itemObject);
    public abstract void OnPickup(MonoBehaviour foo);

    public abstract void onEquip(MonoBehaviour foo);
    public abstract void onUnequip(MonoBehaviour foo);

}
