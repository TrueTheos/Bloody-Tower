using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Prefab Room")]
public class PrefabRoom : ScriptableObject
{
    [Header("# - Wall")]
    [Header(". - Floor")]
    [Header("= - Chest")]
    [Header("\" - Mushroom")]
    [Header("& - Cobweb")]
    [Header("~ - Blood")]
    [Header("2 - Pillar")]
    [Header("3 - Blood Torch")]
    [Header("7 - Prefab Monster")]
    [Header("9 - Prefab Item")]
    [Header("{ - Fountain")]
    [Header("} - Statue")]
    public string room;
    public int height, width;
    [Space]
    [Tooltip("Omni AI which should be spawned with the room")]
    public PrefabAIDictionary OmniAIs;

    [Header("Enemies")]
    public List<EnemiesScriptableObject> enemyNames = new List<EnemiesScriptableObject>();
    public List<bool> enemySleeping = new List<bool>();

    [Header("Items")]
    public List<ItemScriptableObject> itemsToSpawn = new List<ItemScriptableObject>();
}
