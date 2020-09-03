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
        if(Input.GetKeyDown(KeyCode.Question))
        {
            //manager.UpdateMessages()
        }
    }
}
