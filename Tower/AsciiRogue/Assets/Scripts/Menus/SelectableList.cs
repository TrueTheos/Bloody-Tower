using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SelectableList<T> : IPrintable where T : System.Enum 
{

    public List<string> Entries;
    public Dictionary<string, T> Mapping;

    private int _selectedIndex = 0;
    public int SelectedIndex
    {
        get
        {
            return _selectedIndex;
        }
        set
        {
            _selectedIndex = Mathf.Clamp(value, 0, Entries.Count-1);
        }
    }

    public int MaxWidth;
    public int MaxHeight;

    public int Spacing;

    public SelectableList(int maxWidth, int maxHeight, int spacing, Dictionary<string, T> mapping)
    {
        MaxWidth = maxWidth;
        MaxHeight = maxHeight;
        Spacing = spacing;
        Entries = new List<string>();
        Mapping = new Dictionary<string, T>();
        foreach (var item in mapping)
        {
            Entries.Add(item.Key);
            Mapping.Add(item.Key, item.Value);
        }
    }

    public T GetSelected()
    {
        return Mapping[Entries[SelectedIndex]];
    }

    public void MoveDown()
    {
        SelectedIndex = SelectedIndex+1;
    }
    public void MoveUp()
    {
        SelectedIndex = SelectedIndex - 1;
    }
    public void Reset()
    {
        SelectedIndex = 0;
    }

    public char[,] GetPixels()
    {
        int height = Mathf.Min(MaxHeight, Entries.Count * 4);
        char[,] res = new char[MaxWidth, height];
        for (int x = 0; x < MaxWidth; x++)
        {
            for (int y = 0; y < height; y++)
            {
                res[x, y] = ' ';
            }
        }

        for (int i = 0; i < Mathf.Min(MaxHeight/4,Entries.Count); i++)
        {
            int startPos = i * 4;

            string entry = Entries[i];

            char[,] obj = OptionInfo.GetBorder(MaxWidth, 3, i == SelectedIndex).Place(1,1,entry.AsPlaceable());
            res.Place(0, startPos, obj);
        }
        return res;
    }
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



}
