using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHasStats
{
    int dex { get; }
    int str { get; }
    int intell { get; }
    int end { get; }
    int lvl { get; }

    int ac { get; }

    int maxHp { get; }
    int currHp { get; }
}
