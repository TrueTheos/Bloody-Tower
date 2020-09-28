using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextOption : IOptionType
{
    public List<string> Options;

    public int CurrentSelected = 0;

    public TextOption(List<string> options)
    {
        Options = options;
    }


    public void Move(int down = 1)
    {
        CurrentSelected = Mathf.Clamp(CurrentSelected + down, 0, Options.Count-1);
    }

    public void Reset()
    {
        CurrentSelected = 0;
    }

    public List<string> GetAll()
    {
        return Options;
    }

    public int GetCount()
    {
        return Options.Count;
    }

    public string GetCurrent()
    {
        return Options[CurrentSelected];
    }

    public int GetCurrentIndex()
    {
        return CurrentSelected;
    }

    public void Next()
    {
        Move(1);
    }

    public void Previous()
    {
        Move(-1);
    }
}
