using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardInfo
{
    public RunData.EntryType Type;
    public System.Func<object, string> Formatter;
    public System.Func<object, int> Weighting; // 0 if it should not be shown at all, else 10 should be default

    public ScoreBoardInfo(RunData.EntryType type, Func<object, string> formatter, Func<object, int> weighting)
    {
        Type = type;
        Formatter = formatter;
        Weighting = weighting;
    }
}
