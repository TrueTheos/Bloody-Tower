using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : Structure
{
    public string paintingText;

    public override void Use()
    {
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("");
        GameManager.manager.UpdateMessages("<i>You look at the painting hanging on the wall and you see...</i>");
        GameManager.manager.UpdateMessages("<color=#a38745>===============================================================================</color>");
        GameManager.manager.UpdateMessages(paintingText);
        GameManager.manager.UpdateMessages("<color=#a38745>===============================================================================</color>");
    }

    public override void WalkIntoTrigger()
    {
        
    }
}
