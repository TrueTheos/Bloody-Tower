using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{

    public OptionInfo options;

    public Text text;

    public bool Selected = false;

    // Start is called before the first frame update
    void Start()
    {
        options = new OptionInfo(40, 25, 20, 1,
            new KeyValuePair<string, IOptionType>("Test Option", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Testing another", new TextOption(new List<string>() { "Options", "Attackpower","Automatic" })),
            new KeyValuePair<string, IOptionType>("tester", new TextOption(new List<string>() { "Hello", "Is it me", "you are looking", "for" })),
            new KeyValuePair<string, IOptionType>("Test Option 0", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 1", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 2", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 3 ", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 4", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 5", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 6", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 8", new TextOption(new List<string>() { "A", "Longer A", "B", "C" }))
            );
        text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown( KeyCode.Space))
        {
            Selected = !Selected;
            options.Focus = Selected;
        }
        if (Input.GetKeyDown( KeyCode.DownArrow))
        {
            if (Selected)
            {
                options.GetCurrentOption().Next();
            }
            else
            {
                options.MoveDown();
            }
            
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (Selected)
            {
                options.GetCurrentOption().Previous();
            }
            else
            {
                options.MoveUp();
            }
        }
        text.text = options.GetAsText();
    }
}
