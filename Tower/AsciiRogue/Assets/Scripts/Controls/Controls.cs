using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Controls
{
    public static bool IsInitialized = false;

    public static bool AutoSave = true;

    public enum Inputs
    {
        MoveUp,
        MoveUpRight,
        MoveUpLeft,

        MoveRight,
        MoveLeft,

        MoveDownRight,
        MoveDown,
        MoveDownLeft,

        CheatOpen,
        CheatActivate,

        GrimoirOpen,
        GrimoirUp,
        GrimoirDown,

        TechnicOpen,
        TechnicUp,
        TechnicDown,

        InventoryOpen,
        InventoryUp,
        InventoryDown,
        InventoryChoice1,
        InventoryChoice2,
        InventoryChoice3,

        SkillsOpen,
        SkillsUp,
        SkillsDown,

        Use,

        CancelButton,

        LookKey,

        Help,

        Wait,

        WhatCanISee,

    }

    private static Dictionary<string, KeyCode> defaultControls = new Dictionary<string, KeyCode>()
    {
        { Inputs.MoveDown.ToString(),KeyCode.Keypad2},
        { Inputs.MoveDownLeft.ToString(),KeyCode.Keypad1},
        { Inputs.MoveDownRight.ToString(),KeyCode.Keypad3},
        { Inputs.MoveLeft.ToString(),KeyCode.Keypad4},
        { Inputs.MoveRight.ToString(),KeyCode.Keypad6},
        { Inputs.MoveUp.ToString(),KeyCode.Keypad8},
        { Inputs.MoveUpLeft.ToString(),KeyCode.Keypad7},
        { Inputs.MoveUpRight.ToString(),KeyCode.Keypad9},

        { Inputs.CheatOpen.ToString(),KeyCode.C},
        { Inputs.CheatActivate.ToString(),KeyCode.Return},
        
        { Inputs.GrimoirOpen.ToString(),KeyCode.G},
        { Inputs.GrimoirDown.ToString(),KeyCode.Keypad2},
        { Inputs.GrimoirUp.ToString(),KeyCode.Keypad8},

        { Inputs.TechnicOpen.ToString(),KeyCode.T},
        { Inputs.TechnicDown.ToString(),KeyCode.Keypad2},
        { Inputs.TechnicUp.ToString(),KeyCode.Keypad8},

        { Inputs.InventoryOpen.ToString(),KeyCode.I},
        { Inputs.InventoryUp.ToString(),KeyCode.Keypad8},
        { Inputs.InventoryDown.ToString(),KeyCode.Keypad2},
        { Inputs.InventoryChoice1.ToString(),KeyCode.Alpha1},
        { Inputs.InventoryChoice2.ToString(),KeyCode.Alpha2},
        { Inputs.InventoryChoice3.ToString(),KeyCode.Alpha3},

        { Inputs.SkillsUp.ToString(),KeyCode.Alpha8},
        { Inputs.SkillsDown.ToString(),KeyCode.Alpha2},

        { Inputs.Use.ToString(),KeyCode.Space},

        { Inputs.CancelButton.ToString(),KeyCode.Escape},

        { Inputs.LookKey.ToString(),KeyCode.LeftControl},

        { Inputs.Help.ToString(),KeyCode.Slash},

        { Inputs.Wait.ToString(),KeyCode.Period},

        { Inputs.WhatCanISee.ToString(),KeyCode.Keypad0},


               
    };


    private static Dictionary<string, KeyCode> CurrentControls = new Dictionary<string, KeyCode>();

    public static bool GetKey(Inputs name)
    {
        if (CurrentControls.ContainsKey(name.ToString()))
        {
            return Input.GetKey(CurrentControls[name.ToString()]);
        }
        else
        {
            return Input.GetKey(defaultControls[name.ToString()]);
        }
    }
    public static bool GetKeyDown(Inputs name)
    {
        if (CurrentControls.ContainsKey(name.ToString()))
        {
            return Input.GetKeyDown(CurrentControls[name.ToString()]);
        }
        else
        {
            return Input.GetKeyDown(defaultControls[name.ToString()]);
        }
    }
    public static bool GetKeyUp(Inputs name)
    {
        if (CurrentControls.ContainsKey(name.ToString()))
        {
            return Input.GetKeyDown(CurrentControls[name.ToString()]);
        }
        else
        {
            return Input.GetKeyDown(defaultControls[name.ToString()]);
        }
    }

    
    static Controls()
    {
        // initialize controls

        CurrentControls = new Dictionary<string, KeyCode>(defaultControls);
        
        LoadConfig();
        
        SaveConfig();        

        IsInitialized = true;   
    }

    private static void ResetConfig()
    {
        CurrentControls = new Dictionary<string, KeyCode>(defaultControls);
    }

    public static void ChangeConfig(Inputs name, KeyCode key)
    {
        CurrentControls[name.ToString()] = key;
        if (AutoSave)
        {
            SaveConfig();
        }
    }

    private static bool LoadConfig()
    {
        if (ConfigExists())
        {
            using (StreamReader sr = new StreamReader(Application.persistentDataPath + "/controls.bloodC"))
            {
                while (!sr.EndOfStream)
                {
                    string line = sr.ReadLine();

                    if (line[0]=='#')
                    {
                        continue;
                    }

                    string[] parts = line.Split(' ');
                    if (parts.Length >= 2 &&  System.Enum.TryParse<KeyCode>(parts[1],out KeyCode res))
                    {

                    }

                }

            }
        }
        return false;
    }

    private static bool SaveConfig()
    {
        using (StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/controls.bloodC"))
        {
            sw.WriteLine("# Input Config");
            foreach (KeyValuePair<string, KeyCode> c in CurrentControls)
            {
                sw.WriteLine(c.Key + " " + c.Value);
            }
        }
        return true;
    }

    private static bool ConfigExists()
    {
        Debug.Log(Application.persistentDataPath);
        return File.Exists(Application.persistentDataPath + "\\controls.bloodC");
    }



}
