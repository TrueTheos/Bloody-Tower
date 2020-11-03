using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOption : IOptionType
{
    public System.Func<bool> Trigger;

    public ButtonOption(System.Func<bool> trigger)
    {
        Trigger = trigger;
    }

    public List<string> GetAll()
    {
        return new List<string>();
    }

    public int GetCount()
    {
        return 0;
    }

    public string GetCurrent()
    {
        return "";
    }

    public int GetCurrentIndex()
    {
        return 0;
    }

    public void Next()
    {
        
    }

    public void Previous()
    {
        
    }
}
