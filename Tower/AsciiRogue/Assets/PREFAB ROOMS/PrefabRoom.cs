using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Prefab Room")]
public class PrefabRoom : ScriptableObject
{
    public string room;
    public int height, width;

    [Header("Enemies")]
    public List<EnemiesScriptableObject> enemyNames = new List<EnemiesScriptableObject>();
    public List<bool> enemySleeping = new List<bool>();

    [Header("Items")]
    public List<ItemScriptableObject> itemsToSpawn = new List<ItemScriptableObject>();
}
