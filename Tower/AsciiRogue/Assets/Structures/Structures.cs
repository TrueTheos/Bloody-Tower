using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Structure
{
    public string S_name; //name of the structure
    public string S_symbol;
    public string S_color;
    public abstract void Use();
    public abstract void WalkIntoTrigger();
}
