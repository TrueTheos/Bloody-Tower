using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Help : MonoBehaviour
{
    GameManager manager;

    void Start()
    {
        manager = GetComponent<GameManager>();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Slash))
        {
            manager.UpdateMessages("<color=yellow>7 8 9</color>");
            manager.UpdateMessages("<color=yellow>4   6 - Movement</color>");
            manager.UpdateMessages("<color=yellow>1 2 3</color>");
            manager.UpdateMessages("<color=yellow>Right Click - Info about enemy, '.' - Wait turn</color>");
            manager.UpdateMessages("<color=yellow>I - Inventory, G - Grimoire, T - Skills, ESC - Close Window</color>");
            manager.UpdateMessages("<color=yellow>8 & 2 - Select Item in the inventory</color>");
            manager.UpdateMessages("<color=yellow>Space/Enter - Use item</color>");
            manager.UpdateMessages("<color=yellow>If you level up press</color> <color=purple>+</color> <color=yellow>next to the statistic that you want to level up</color>");
        }
    }
}
