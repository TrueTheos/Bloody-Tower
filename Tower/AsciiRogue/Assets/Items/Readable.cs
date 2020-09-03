using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Items/Readable")]
public class Readable : ItemScriptableObject
{
    public string text;

    public override void onEquip(MonoBehaviour foo)
    {
    }

    public override void OnPickup(MonoBehaviour foo)
    {
    }

    public override void onUnequip(MonoBehaviour foo)
    {
    }

    public override void Use(MonoBehaviour foo, Item itemObject)
    {
        GameManager.manager.UpdateMessages(text);
        GameManager.manager.ApplyChangesInInventory(this);
        GameManager.manager.UpdateMessages("As soon as you read the contents of the scroll, it broke into small pieces due to corrosion");
    }
}
