using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiting : MonoBehaviour
{
    PlayerMovement playerMovement;
    GameManager manager;

    void Start()
    {
        playerMovement = GameManager.manager.player;
        manager = GameManager.manager;
    }

    void Update()
    {
        if(Controls.GetKeyDown(Controls.Inputs.Wait) && playerMovement.canMove && !manager.waiting && manager.isPlayerTurn)
        {
            manager.waitingCoroutine = manager.WaitTurn(1);
            manager.StartCoroutine(manager.waitingCoroutine);
            manager.UpdateMessages("You wait one turn.");
        }
    }
}
