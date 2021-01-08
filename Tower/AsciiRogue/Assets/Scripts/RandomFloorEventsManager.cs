using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomFloorEventsManager : MonoBehaviour
{
    public static RandomFloorEventsManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void Event1()
    {
        GameManager.manager.UpdateMessages("<i>I can hear trampling and a strange howling from the floor below, I have to act fast...</i>");
    }
}
