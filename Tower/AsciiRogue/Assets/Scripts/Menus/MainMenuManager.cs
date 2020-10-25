using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public int MaxWidth;
    public int MaxHeight;


    public OptionInfo options;
    public OptionInfo NewGame;
    public TextPrintable Credits;

    public IPrintable CurrentMenu;

    public SelectableList<MenuThing> MainMenu;

    public bool InMenu = true;

    public MenuThing CurrentSubMenu = MenuThing.Setting;


    public enum MenuThing
    {
        NewGame,
        Setting,
        Credits
    }
    public Text text;

    public bool Selected = false;

    public static Dictionary<string, IOptionType> Settings;
    public static Dictionary<string, IOptionType> NewGameSettings;

    // Start is called before the first frame update
    void Start()
    {
        options = new OptionInfo(35, 20, 20, 1,
            new KeyValuePair<string, IOptionType>("Test Option", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Testing another", new TextOption(new List<string>() { "Options", "Attackpower","Automatic" })),
            new KeyValuePair<string, IOptionType>("tester", new TextOption(new List<string>() { "Hello", "Is it me", "you are", "looking", "for" })),
            new KeyValuePair<string, IOptionType>("Test Option 0", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 1", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 2", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 3 ", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 4", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 5", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 6", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 8", new TextOption(new List<string>() { "A", "Longer A", "B", "C" }))
            );
        NewGame = new OptionInfo(35, 20, 20, 1,
            new KeyValuePair<string, IOptionType>("Start Game", new ButtonOption(StartGame)),
            new KeyValuePair<string, IOptionType>("Difficulty", new TextOption(new List<string>() { "Baby", "Normal", "Blood Tower", "Impossible" })),
            new KeyValuePair<string, IOptionType>("Start Item", new TextOption(new List<string>() { "Torch", "Random Ring", "Dagger" })),
            new KeyValuePair<string, IOptionType>("Base Skill", new TextOption(new List<string>() { "Slash", "Jump", "Heal Poison"}))
            );

        Credits = new TextPrintable(new List<string>() { "Theos", "Demidemon", "MCR" }, 20, 15, 3);


        MainMenu = new SelectableList<MenuThing>(
            15,
            25,
            1,
            new Dictionary<string, MenuThing>()
            {
                {"New Game",MenuThing.NewGame },
                {"Settings",MenuThing.Setting },
                {"Credits",MenuThing.Credits }
            });

        text = GetComponent<Text>();
    }

    public void StartGame()
    {
        NewGameSettings = NewGame.Options;
        Settings = options.Options;
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single);
    }


    // Update is called once per frame
    void Update()
    {
        if (Controls.GetKeyDown(Controls.Inputs.Use))
        {
            if (InMenu)
            {
                switch (MainMenu.GetSelected())
                {
                    case MenuThing.NewGame:
                        CurrentMenu = NewGame;
                        NewGame.ResetPos();
                        break;
                    case MenuThing.Setting:
                        CurrentMenu = options;
                        options.ResetPos();
                        break;
                    case MenuThing.Credits:
                        CurrentMenu = Credits;
                        break;
                    default:
                        break;
                }
                CurrentSubMenu = MainMenu.GetSelected();
                InMenu = false;
                
            }
            else
            {
                if (CurrentMenu is OptionInfo info)
                {
                    if (info.GetCurrentOption() is ButtonOption btn)
                    {
                        // we do something
                        btn?.Trigger();
                    }
                    else
                    {
                        Selected = !Selected;
                        info.Focus = Selected;
                    }                    
                }                
            }
            
        }
        if (Controls.GetKeyDown(Controls.Inputs.CancelButton))
        {
            if (InMenu)
            {
                // do nothing
            }
            else
            {
                if (Selected)
                {
                    Selected = false;
                    if (CurrentMenu is OptionInfo info)
                    {
                        info.Focus = false;
                    }
                }
                else
                {
                    InMenu = true;
                }
            }
        }
        if (Controls.GetKeyDown(Controls.Inputs.MoveDown))
        {
            if (InMenu)
            {
                MainMenu.MoveDown();
            }
            else
            {
                if (CurrentMenu is OptionInfo info)
                {
                    if (Selected)
                    {
                        info.GetCurrentOption().Next();
                    }
                    else
                    {
                        info.MoveDown();
                    }
                }
            }         
        }
        if (Controls.GetKeyDown(Controls.Inputs.MoveUp))
        {
            if (InMenu)
            {
                MainMenu.MoveUp();
            }
            else
            {
                if (CurrentMenu is OptionInfo info)
                {
                    if (Selected)
                    {
                        info.GetCurrentOption().Previous();
                    }
                    else
                    {
                        info.MoveUp();
                    }
                }
            }
            
        }
        char[,] Text = new char[MaxWidth, MaxWidth];
        Text.FillEmpty();
        if (InMenu)
        {
            Text.Place(2, 10, MainMenu.GetPixels());
        }
        else
        {
            switch (CurrentSubMenu)
            {
                case MenuThing.NewGame:
                    Text.Place(2, 9, CurrentMenu.GetPixels());
                    break;
                case MenuThing.Setting:
                    Text.Place(2, 9, CurrentMenu.GetPixels());
                    break;
                case MenuThing.Credits:
                    Text.Place(10, 13, CurrentMenu.GetPixels());
                    break;
                default:
                    break;
            }
        }
        text.text = Text.ConvertToString();
    }
}
