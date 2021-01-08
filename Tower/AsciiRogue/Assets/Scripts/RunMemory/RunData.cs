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

    public enum EntryType
    {
        Scoreboard,
        Optional,
        Fluff,
        Data,
    }
}
