using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberOption : IOptionType
{
    public int Number;
    public string Name;


    public NumberOption(string name, int number)
    {
        Number = number;
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
        return Number.ToString();
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
