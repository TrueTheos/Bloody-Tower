using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyOption : IOptionType
{
    public KeyCode Key;
    public string Name;

    public KeyOption(string name, KeyCode key)
    {
        Key = key;
        Name = name;
    }

    public List<string> GetAll()
    {
        return new List<string>() { GetCurrent() };
    }

    public int GetCount()
    {
        return 1;
    }

    public string GetCurrent()
    {
        return Key.ToString();
    }

    public int GetCurrentIndex()
    {
        return 1;
    }

    public void Next()
    {
        
    }

    public void Previous()
    {
        
    }
}
