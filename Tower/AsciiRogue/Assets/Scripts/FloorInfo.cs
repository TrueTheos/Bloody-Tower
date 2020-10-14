using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorInfo : MonoBehaviour
{
    public List<FloorMessage> onEnterMessage = new List<FloorMessage>();
}

public class FloorMessage
{
    public Vector2Int messagePosition = new Vector2Int();
    public string message;
}
