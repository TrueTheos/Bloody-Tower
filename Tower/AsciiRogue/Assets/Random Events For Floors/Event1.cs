using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event1 : RandomEvents
{ 

    public override void ExecuteEvent()
    {
        GameManager.manager.UpdateMessages("<i>I can hear trampling and a strange howling from the floor below, I have to act fast...</i>");
    }
}
