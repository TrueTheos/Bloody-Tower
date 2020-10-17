using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Items/Readable")]
public class Readable : ItemScriptableObject
{
    [TextArea]
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
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("<i>You read...</i>");
        GameManager.manager.UpdateMessages(text);
    }
}


