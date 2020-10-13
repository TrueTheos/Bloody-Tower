using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for AI which does NOT have a physical present but should still be updated every turn
/// </summary>
public class OmniBehaviour : MonoBehaviour
{
    // omni defines something without body (we can change that)


    public Vector2Int Position;

    public BaseOmniAI AI;

}
