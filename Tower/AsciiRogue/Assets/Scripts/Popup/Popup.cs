using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Popup
{

    public static bool Showing { get; private set; } = false;

    static Dictionary<Controls.Inputs, (bool showNumber,string DisplayName,System.Action callback)> responds;

    public static Status LastStatus { get; private set; } = Status.Closed;

    public static GameObject RootPopup;

    public static UnityEngine.UI.Text Text;

    public static bool CanCancel = true;

    private static KeyValuePair<Controls.Inputs, (bool showNumber, string DisplayName, System.Action callback)> lastInput;
    public static KeyValuePair<Controls.Inputs, (bool showNumber, string DisplayName, System.Action callback)> LastInput => lastInput;

    public enum Status
    {
        Waiting,
        Closed,
        Canceled
    }

    public static void Init(GameObject rootPopup, UnityEngine.UI.Text txt)
    {
        RootPopup = rootPopup;
        Text = txt;
    }

    /// <summary>
    /// Configures a new Text Popup which can be closed by pressing the <see cref="Controls.Inputs.Use"/> key
    /// </summary>
    /// <param name="text">The text to display</param>
    /// <param name="callbackName">The name of the method that should be called when confirmed</param>
    /// <param name="target">On what object the method should be called</param>
    public static void NewTextConfirmation(string text,  string callbackName, GameObject target,bool canCancel = true)
    {
        ClearResponds();
        CanCancel = canCancel;

        AddResponds(Controls.Inputs.Use, (false, text, () => { target.SendMessage(callbackName); }));

    }
    public static void NewTextConfirmation(string text, System.Action callback, bool canCancel = true)
    {
        ClearResponds();
        CanCancel = canCancel;

        AddResponds(Controls.Inputs.Use, (false, text, callback));
    }

    public static void NewDecission()
    {
        ClearResponds();
        CanCancel = true;
    }
    public static void NewDecission(bool canCancel, params (string desc, string callbackName,GameObject target )[] entries)
    {
        ClearResponds();
        CanCancel = canCancel;
        for (int i = 0; i < entries.Length; i++)
        {
            var t = entries[i];
            Controls.Inputs btn = NextInventoryChoice();
            
            AddResponds(btn, (true, t.desc, () => t.target.SendMessage(t.callbackName)));
        }
    }
    public static void NewDecission(bool canCancel, params (string desc, System.Action callback)[] entries)
    {
        ClearResponds();
        CanCancel = canCancel;
        for (int i = 0; i < entries.Length; i++)
        {
            var t = entries[i];
            Controls.Inputs btn = NextInventoryChoice();
            AddResponds(btn, (true, t.desc, t.callback));
        }

    }

    private static Controls.Inputs NextInventoryChoice()
    {
        switch (responds.Count)
        {
            case 0:
                return Controls.Inputs.InventoryChoice1;
            case 1:
                return Controls.Inputs.InventoryChoice2;
            case 2:
                return Controls.Inputs.InventoryChoice3;
            case 3:
                return Controls.Inputs.InventoryChoice4;
            default:
                return Controls.Inputs.Use;
        }
    }
    public static void AddDecissionOption(string desc, System.Action callback)
    {
        Controls.Inputs btn = NextInventoryChoice();
        AddResponds(btn, (true, desc, callback));
    }
    public static void AddDecissionOption(string desc, string callbackName, GameObject target)
    {
        Controls.Inputs btn = NextInventoryChoice();
        AddResponds(btn, (true, desc, ()=>target.SendMessage(callbackName)));
    }
    private static void AddResponds(Controls.Inputs input, (bool showNumber, string DisplayName, System.Action callback) data)
    {
        responds.Add(input, data);
    }

    private static void ClearResponds()
    {
        CanCancel = true;
        if (responds==null)
        {
            responds = new Dictionary<Controls.Inputs, (bool showNumber, string DisplayName, System.Action callback)>();
        }
        responds.Clear();
    }

    private static void UpdateText()
    {
        int num = 1;
        Text.text = "";
        foreach (var item in responds)
        {
            if (item.Value.DisplayName!=string.Empty)
            {
                if (num > 1)
                {
                    Text.text += "\n";
                }
                if (item.Value.showNumber)
                {
                    Text.text += num + ". ";
                }
                Text.text += item.Value.DisplayName;
            }
            num++;
        }
    }
    public static void Show()
    {
        UpdateText();
        RootPopup.SetActive(true);
        Showing = true;
        LastStatus = Status.Waiting;        
    }
    public static void Hide(bool cancel = false)
    {
        RootPopup.SetActive(false);
        Showing = false;
        if (cancel)
        {
            LastStatus = Status.Canceled;
        }
        else
        {
            LastStatus = Status.Closed;
        }        
    }

    /// <summary>
    /// check if a decission was made
    /// </summary>
    /// <returns>true if decission was made</returns>
    public static bool CheckDecission()
    {
        if (Showing)
        {
            if (CanCancel && Controls.GetKeyDown(Controls.Inputs.CancelButton))
            {
                // we cancel the popup
                Hide(true);
                lastInput = default;
                return true;
            }

            foreach (var entry in responds)
            {
                if (Controls.GetKeyDown(entry.Key))
                {
                    // the use did an input
                    Hide();
                    entry.Value.callback?.Invoke();
                    lastInput = entry;                    
                    return true;
                }
            }
        }
        return false;
    }



}
