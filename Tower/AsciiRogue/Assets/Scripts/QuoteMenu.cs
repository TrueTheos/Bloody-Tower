using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuoteMenu : MonoBehaviour
{
    [TextArea]
    public List<string> quotes = new List<string>();

    void Start()
    {
        GetComponent<Text>().text = quotes[Random.Range(0, quotes.Count)];
    }
}
