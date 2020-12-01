using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Painting : Structure
{
    public string paintingText;

    public override void Use()
    {
        if (GameManager.manager.playerStats.__sanity <= 75)
        {
            if (GameManager.manager.playerStats.__sanity <= 20)
            {
                GameManager.manager.UpdateMessages("You cannot focus your eyes on the painting. You shake when you try to look at it. You're too scared.");
                return;
            }
            else if (GameManager.manager.playerStats.__sanity <= 50)
            {
                if (UnityEngine.Random.Range(1, 100) <= 10)
                {
                    GameManager.manager.UpdateMessages("You look at the painting but what you see makes you want to vomit. You immediately look away.");
                    return;
                }
            }
            else
            {
                if (UnityEngine.Random.Range(1, 100) <= 5)
                {
                    GameManager.manager.UpdateMessages("You look at the painting but all you see is full of scary eyes. You immediately look away.");
                    return;
                }
            }
        }

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
