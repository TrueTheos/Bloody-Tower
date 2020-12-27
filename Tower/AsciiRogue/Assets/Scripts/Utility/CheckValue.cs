using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CheckValue<T> where T : struct
{
    private T value;
    private bool valid;

    public CheckValue(T value)
    {
        this.value = value;
        this.valid = true;
    }

    public static CheckValue<T> Invalid()
    {
        return new CheckValue<T>() { valid = false };
    }

    public static implicit operator CheckValue<T>(T data)
    {
        return new CheckValue<T>(data);
    }
    public static implicit operator bool(CheckValue<T> value)
    {
        return value.valid;
    }
    public static implicit operator T(CheckValue<T> value)
    {
        return value.value;
    }


}
