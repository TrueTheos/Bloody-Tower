using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RunData 
{
    Dictionary<string, object> Entries = new Dictionary<string, object>();


    public void Set(string name,object value)
    {
        Entries[name] = value;
    }
    public T Get<T>(string name)
    {
        if (Entries.ContainsKey(name))
        {
            if(Entries[name] is T val)
            {
                return val;
            } 
        }
        return default;
    }
    public object Get(string name)
    {
        if (!Entries.ContainsKey(name))
        {
            return null;
        }
        return Entries[name];
    }
    public int GetSumInt(params string[] names)
    {
        int sum = 0;
        for (int i = 0; i < names.Length; i++)
        {
            sum += Get<int>(names[i]);
        }
        return sum;
    }
    // this allows for some to be more weigted than others
    public int GetSumInt(params (string name,int weight)[] nameWeight)
    {
        int sum = 0;
        for (int i = 0; i < nameWeight.Length; i++)
        {
            sum += Get<int>(nameWeight[i].name) * nameWeight[i].weight;
        }
        return sum;
    }
    public float GetSumFloat(params string[] names)
    {
        float sum = 0;
        for (int i = 0; i < names.Length; i++)
        {
            sum += Get<float>(names[i]);
        }
        return sum;
    }
    public bool Has<T>(string name)
    {
        if (Entries.ContainsKey(name))
        {
            return Entries[name] is T;
        }
        else
        {
            return false;
        }
    }
    public bool Has(string name)
    {
        if (Entries.ContainsKey(name))
        {
            return Entries[name] != null;
        }
        else
        {
            return false;
        }
    }

    public bool AddNumber(string name,int change)
    {
        if (Entries.ContainsKey(name))
        {
            object val = Entries[name];
            switch (val)
            {
                case int i:
                    Entries[name] = i + change;
                    return true;
                case float f:
                    Entries[name] = f + change;
                    return true;
                default:
                    return false;
            }
        }
        else
        {
            Entries.Add(name, change);
            return true;
        }
    }
    public bool AddNumber(string name,float change)
    {
        if (Entries.ContainsKey(name))
        {
            object val = Entries[name];
            switch (val)
            {
                case int i:
                    Entries[name] = (int)(i + change);
                    return true;
                case float f:
                    Entries[name] = f + change;
                    return true;
                default:
                    return false;
            }
        }
        else
        {
            Entries.Add(name, change);
            return true;
        }
    }
    public bool MultiplyNumber(string name, int factor)
    {
        if (Entries.ContainsKey(name))
        {
            object val = Entries[name];
            switch (val)
            {
                case int i:
                    Entries[name] = i * factor;
                    return true;
                case float f:
                    Entries[name] = f * factor;
                    return true;
                default:
                    return false;
            }
        }
        else
        {
            return false;
        }
    }
    public bool MultiplyNumber(string name, float factor)
    {
        if (Entries.ContainsKey(name))
        {
            object val = Entries[name];
            switch (val)
            {
                case int i:
                    Entries[name] = (int)(i * factor);
                    return true;
                case float f:
                    Entries[name] = f * factor;
                    return true;
                default:
                    return false;
            }
        }
        else
        {
            return false;
        }
    }

    public enum EntryType
    {
        Scoreboard,
        Optional,
        Fluff,
        Data,
    }
}
