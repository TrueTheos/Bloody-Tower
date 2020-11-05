using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMenu : MenuInfo
{

    public ControlMenu(int maxWidth, int maxHeight, int leftWidth, int verticalSpacing):base(maxWidth,maxHeight,leftWidth,verticalSpacing)
    {
        Reload();
    }
    

    public void Reload()
    {
        Options.Clear();
        Indexer.Clear();

        Dictionary<string, KeyCode> config = Controls.GetConfig();

        ResetPos();
        Options.Add("Reset Controls", new ButtonOption(() => { Controls.ResetConfig(); Reload(); return true; }));
        Indexer.Add("Reset Controls");

        foreach (var c in config)
        {
            Options.Add(c.Key, new KeyOption(c.Key, c.Value));
            Indexer.Add(c.Key);
        }
    }


    public override char[,] GetPixels()
    {
        char[,] res = new char[MaxWidth, MaxHeight];
        for (int x = 0; x < MaxWidth; x++)
        {
            for (int y = 0; y < MaxHeight; y++)
            {
                res[x, y] = ' ';
            }
        }


        int leftWidth = LeftWidth;
        int rightWidth = MaxWidth - LeftWidth;

        for (int i = 0; i < Mathf.Min(Indexer.Count - TopIndex, MaxVisible); i++)
        {
            int currIndex = TopIndex + i;

            int startPos = 1 + i * 4;

            string entry = Indexer[currIndex];
            IOptionType value = Options[entry];

            if (value is ButtonOption button)
            {
                char[,] t = GetBorder(MaxWidth, 3, currIndex == CurrentPos).Place(1, 1, entry.AsPlaceable());
                res.Place(0, startPos, t);
            }
            else
            {
                if (value is KeyOption k)
                {
                    char[,] left = GetBorder(LeftWidth, 3, currIndex == CurrentPos).Place(1, 1, k.Name.AsPlaceable());

                    char[,] right = GetBorder(rightWidth, 3, currIndex == CurrentPos && Focus).Place(1, 1, Focus && currIndex == CurrentPos ? "<Press Key>".AsPlaceable() : k.Key.ToString().AsPlaceable());

                    res.Place(0, startPos, left);
                    res.Place(leftWidth, startPos, right);

                }
                else
                {
                    char[,] left = GetBorder(LeftWidth, 3, currIndex == CurrentPos).Place(1, 1, entry.AsPlaceable());

                    char[,] right = GetBorder(rightWidth, 3, currIndex == CurrentPos && Focus).Place(1, 1, value.GetCurrent().AsPlaceable());

                    res.Place(0, startPos, left);
                    res.Place(leftWidth, startPos, right);
                }
            }            
        }
        return res;
    }

    public override bool Trigger()
    {
        var info = GetCurrentOption();

        if (info is ButtonOption btn)
        {
            return btn.Trigger();
        }
        else
        {
            Focus = !Focus;
        }

        return false;
    }
    public override bool Cancel()
    {
        if (Focus)
        {
            LooseFocus();
        }
        else
        {
            return true;
        }
        return false;
    }

    public override bool Update()
    {
        if (Focus)
        {
            if (Controls.GetKeyDown(Controls.Inputs.CancelButton))
            {
                Cancel();
            }

            foreach (KeyCode key in (KeyCode[])System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    // there is a button press

                    if (GetCurrentOption() is KeyOption opt)
                    {
                        Controls.ChangeConfig(opt.Name, key);
                        opt.Key = key;
                    }
                    LooseFocus();
                    break;
                }
            }
        }
        else
        {
            if (Controls.GetKeyDown(Controls.Inputs.Use))
            {
                return Trigger();
            }
            if (Controls.GetKeyDown(Controls.Inputs.CancelButton))
            {
                return Cancel();
            }
            if (Controls.GetKeyDown(Controls.Inputs.MoveDown))
            {
                MoveDown();
            }
            if (Controls.GetKeyDown(Controls.Inputs.MoveUp))
            {
                MoveUp();
            }
        }
        
        return false;

    }




}
