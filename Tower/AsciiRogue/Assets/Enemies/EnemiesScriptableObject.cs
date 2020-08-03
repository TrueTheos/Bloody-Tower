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
    public int E_currentLevel;

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

    /*public int E_str;
    public int E_dex;
    public int E_int;
    public int E_end;*/
    public int E_xpAfterKilling;

    
    //public int E_armorClass;
    //public List<Vector2Int> attacks;
    //public List<attackEffects> attackAdditionalEffects;
    //public ItemScriptableObject E_itemDroppedAfterBeingKilled;

    [Header("Settings")]
    public bool leavesCorpse;


    //NPC
    [Header("NPC Settings")]
    public bool finishedDialogue;
    public ItemScriptableObject rewardItem;
}
