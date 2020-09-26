using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Prefab Room")]
public class PrefabRoom : ScriptableObject
{
    public string room;
    public int width, height;
    public List<string> enemyNames = new List<string>();
    public List<int> enemyPosition = new List<int>();
    public List<bool> enemySleeping = new List<bool>();
}
