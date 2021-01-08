using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IScoreBoardEntry
{
    object GetValue();
}

public class ScoreBoardEntry<T> : IScoreBoardEntry where T :struct
{
    private T _value;
    public T Value
    {
        get => _value;
        set => _value = value;
    }

    public object GetValue() => Value;

    public System.Func<T, string> Formatter;

    public readonly System.Func<T, string> DEFAULT_TOSTRING = (v) => v.ToString();


    public override string ToString()
    {
        if (Formatter != null)
        {
            return Formatter(Value);
        }
        else
        {
            return DEFAULT_TOSTRING(Value);
        }
    }

}
