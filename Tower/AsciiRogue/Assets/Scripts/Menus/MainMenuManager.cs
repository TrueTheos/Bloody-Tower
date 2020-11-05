using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public int MaxWidth;
    public int MaxHeight;


    public MenuInfo options;
    public MenuInfo NewGame;
    public TextPrintable Credits;
    public MenuInfo MainMenu;
    public ControlMenu ControlMenu;

    public IPrintable CurrentMenu;

    public SelectableList Menu;

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
        options = new MenuInfo(35, 15, 20, 1,
            new KeyValuePair<string, IOptionType>("Test Option", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Testing another", new TextOption(new List<string>() { "Options", "Attackpower","Automatic" })),
            new KeyValuePair<string, IOptionType>("tester", new TextOption(new List<string>() { "Hello", "Is it me", "you are", "looking", "for" })),
            new KeyValuePair<string, IOptionType>("Controls", new ButtonOption(()=> {ControlMenu.Reload(); Menu.AddMenu(ControlMenu); return false; })),
            new KeyValuePair<string, IOptionType>("Test Option 1", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 2", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 3 ", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 4", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 5", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 6", new TextOption(new List<string>() { "A", "Longer A", "B", "C" })),
            new KeyValuePair<string, IOptionType>("Test Option 8", new TextOption(new List<string>() { "A", "Longer A", "B", "C" }))
            );
        NewGame = new MenuInfo(35, 15, 20, 1,
            new KeyValuePair<string, IOptionType>("Start Game", new ButtonOption(StartGame)),
            new KeyValuePair<string, IOptionType>("Difficulty", new TextOption(new List<string>() { "Baby", "Normal", "Blood Tower", "Impossible" })),
            new KeyValuePair<string, IOptionType>("Start Item", new TextOption(new List<string>() { "Torch", "Random Ring", "Dagger" })),
            new KeyValuePair<string, IOptionType>("Base Skill", new TextOption(new List<string>() { "Slash", "Jump", "Heal Poison"}))
            );

        Credits = new TextPrintable(new List<string>() { "Theos", "Demidemon", "MCR" }, 20, 15, 3,2,1);

        ControlMenu = new ControlMenu(35, 15, 20, 1);

        MainMenu = new MenuInfo(
            35, 15, 20, 1,
            new KeyValuePair<string, IOptionType>("New Game",new ButtonOption(()=> { Menu.AddMenu(NewGame);return false; })),
            new KeyValuePair<string, IOptionType>("Settings",new ButtonOption(()=> { Menu.AddMenu(options);return false; })),
            new KeyValuePair<string, IOptionType>("Credits",new ButtonOption(()=> { Menu.AddMenu(Credits);return false; }))
            );

        Menu = new SelectableList(MainMenu);

        text = GetComponent<Text>();
    }

    public bool StartGame()
    {
        NewGameSettings = NewGame.Options;
        Settings = options.Options;
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Single);
        return true;
    }


    // Update is called once per frame
    void Update()
    {
        Menu.Update();

        char[,] Text = new char[MaxWidth, MaxWidth];
        Text.FillEmpty();
        
        Text.Place(2, 10, Menu.GetPixels());
        
        
        text.text = Text.ConvertToString();
    }
}
