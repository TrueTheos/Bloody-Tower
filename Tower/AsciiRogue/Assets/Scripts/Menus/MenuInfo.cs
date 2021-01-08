using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MenuInfo : IPrintable
{

    public Dictionary<string, IOptionType> Options = new Dictionary<string, IOptionType>();
    public List<string> Indexer = new List<string>();

    public int MaxWidth;
    public int MaxHeight;

    public int LeftWidth;
    public int VerticalSpacing;

    public int CurrentPos = 0;
    public int TopIndex = 0;

    public bool Focus;

    public int MaxVisible => (MaxHeight-2) / 4;

    public int MaxWidthName
    {
        get
        {
            int max = 0;
            foreach (var item in Options)
            {
                if (item.Key.Length>max)
                {
                    max = item.Key.Length;
                }
            }
            return max;
        }
    }
    public void LooseFocus() => Focus = false;

    public MenuInfo(int maxWidth, int maxHeight, int leftWidth, int verticalSpacing,params KeyValuePair<string, IOptionType>[] options)
    {
        MaxWidth = maxWidth;
        MaxHeight = maxHeight;
        LeftWidth = leftWidth;
        VerticalSpacing = verticalSpacing;
        for (int i = 0; i < options.Length; i++)
        {
            Options.Add(options[i].Key, options[i].Value);
            Indexer.Add(options[i].Key);
        }
    }

    public MenuInfo(params KeyValuePair<string,IOptionType>[] options)
    {
        for (int i = 0; i < options.Length; i++)
        {
            Options.Add(options[i].Key, options[i].Value);
            Indexer.Add(options[i].Key);
        }
    }

    public string GetValueType(string optionName)
    {
        return Options[optionName].GetCurrent();

    }

    public virtual char[,] GetPixels()
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

        for (int i = 0; i < Mathf.Min(Indexer.Count-TopIndex,MaxVisible); i++)
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
                char[,] left = GetBorder(LeftWidth, 3, currIndex == CurrentPos).Place(1, 1, entry.AsPlaceable());

                char[,] right = GetBorder(rightWidth, 3, currIndex == CurrentPos && Focus).Place(1, 1, value.GetCurrent().AsPlaceable());

                res.Place(0, startPos, left);
                res.Place(leftWidth, startPos, right);
            }            
        }
        return res;
    }

    public IOptionType GetCurrentOption() => Options[Indexer[CurrentPos]];

    public string GetAsText()
    {
        char[,] chars = GetPixels();

        StringBuilder sb = new StringBuilder();
        for (int y = 0; y < chars.GetLength(1); y++)
        {
            for (int x = 0; x < chars.GetLength(0); x++)
            {
                sb.Append(chars[x, y]);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public void MoveUp()
    {
        CurrentPos = Mathf.Clamp(CurrentPos - 1, 0, Options.Count - 1);
        TopIndex = Mathf.Clamp(TopIndex, CurrentPos - MaxVisible + 1, CurrentPos);
    }
    public void MoveDown()
    {
        CurrentPos = Mathf.Clamp(CurrentPos + 1, 0, Options.Count - 1);
        TopIndex = Mathf.Clamp(TopIndex,CurrentPos-MaxVisible+1,CurrentPos);
        
    }
    public void ResetPos()
    {
        CurrentPos = 0;
        TopIndex =0;
    }

       
    public static char[,] GetBorder(int width, int height, bool highlight = false)
    {
        char[,] res = new char[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                res[x, y] = ' ';
            }
        }
        for (int x = 1; x < width-1; x++)
        {
            res[x, 0] = highlight ? '═' :'─';
            res[x, height-1] = highlight ? '═': '─';
        }
        for (int y = 1; y < height-1; y++)
        {
            res[0, y] = highlight ?'║' : '│';
            res[width - 1, y] = highlight ?'║' : '│';
        }
        res[0, 0] = highlight ?'╔' : '┌';
        res[width-1, 0] = highlight ?'╗' : '┐';
        res[0, height-1] = highlight ? '╚': '└';
        res[width-1, height-1] = highlight ?'╝' : '┘';
        return res;
    }


    public virtual bool Trigger()
    {
        var opt = GetCurrentOption();

        if (opt is ButtonOption btn)
        {
            return btn.Trigger.Invoke();
        }
        if (opt is TextOption txt)
        {
            Focus = !Focus;
        }
        if (opt is NumberOption num)
        {
            if (Focus)
            {
                Focus = false;

            }
            else
            {
                Focus = true;

            }
        }

        return false;
    }
    public virtual bool Cancel()
    {
        var opt = GetCurrentOption();
        
        if (Focus)
        {
            Focus = false;
        }
        else
        {
            return true;
        }

        return false;
    }

    public virtual bool Update()
    {
        if (Controls.GetKeyDown(Controls.Inputs.InventoryDown))
        {
            if (Focus)
            {
                GetCurrentOption()?.Next();
            }
            else
            {
                MoveDown();
            }            
        }
        if (Controls.GetKeyDown(Controls.Inputs.InventoryUp))
        {
            if (Focus)
            {
                GetCurrentOption()?.Previous();
            }
            else
            {
                MoveUp();
            }
        }
        if (Controls.GetKeyDown(Controls.Inputs.Use))
        {
            return Trigger();
        }
        if (Controls.GetKeyDown(Controls.Inputs.CancelButton))
        {
            return Cancel();
        }
        if (Focus && GetCurrentOption() is NumberOption num)
        {
            
        }
        return false;
    }


}
