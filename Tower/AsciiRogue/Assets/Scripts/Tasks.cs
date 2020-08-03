using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tasks : MonoBehaviour
{
    public event Action everyTurnTasks;
    

    public WeaponsSO bloodSword;
    public int bloodSwordStartingDamage;
    public int bloodSwordCooldown;

    public void EventsOnStartOfTheGame()
    {
        bloodSword.attacks[0] = new Vector2Int(1, bloodSword.attacks[0].y);
        bloodSword.bloodSwordCounter = bloodSwordCooldown;
    }

    public void DoTasks()
    {
        everyTurnTasks?.Invoke();
    }
}
