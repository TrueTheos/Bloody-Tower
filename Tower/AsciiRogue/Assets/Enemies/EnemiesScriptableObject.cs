using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Enemy", menuName = "Enemies")]
public class EnemiesScriptableObject : ScriptableObject
{
    /*public enum attackEffects
    {
        none,
        poison,
    }
    [SerializeField] public attackEffects _atackEffects;*/

    public string E_name;
    public string E_symbol;
    public Color E_color;
    public int E_lvlMin;
    public int E_lvlMax;

    public string E_realName;
    public string E_realSymbol;
    public Color E_realColor;
    
    public enum E_modifier
    {
        strong,
        nimble,
        smart,
        tough,
        weak,
        clumsy,
        stupid,
        frail,
        giant,
        small,
        caster
    }
    
    public E_modifier __modifier;

    public enum E_difficulty { easy, normal, hard, boss}
    public E_difficulty _Difficulty;

    public enum E_behaviour { normal, cowardly, sluggish, recovers,  npc}
    public E_behaviour _Behaviour;

    public enum E_Attacks
    {
        normal,
        poisonBite, //for Giant rat
        fadingBite //for Sulyvahn's Beast
    }
    private E_Attacks _attacks;
    public List<E_Attacks> attacks;

    public List<ItemScriptableObject> E_possileDrops;

    [Header("Settings")]
    public bool leavesCorpse;


    //NPC
    [Header("NPC Settings")]
    public bool finishedDialogue;
    public ItemScriptableObject rewardItem;
}
