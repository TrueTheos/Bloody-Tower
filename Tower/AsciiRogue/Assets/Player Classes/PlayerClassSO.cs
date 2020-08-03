using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Player Class", menuName = "Player Classes")]
public class PlayerClassSO : ScriptableObject
{
    public string C_name;
    public int C_maxHealth;
    [Range(3, 25)] public int C_strength;
    [Range(3, 25)] public int C_intelligence;
    [Range(3, 25)] public int C_dexterity;
    [Range(3, 25)] public int C_Endurance;
    [Range(3, 25)] public int C_charisma;

}
