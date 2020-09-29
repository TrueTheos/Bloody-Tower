using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class TextPrintable : IPrintable
{

    public List<string> Lines;

    public int MaxWidth;
    public int MaxHeight;
    public int Spacing;

    public TextPrintable(List<string> lines, int maxWidth, int maxHeight, int spacing)
    {
        Lines = lines;
        MaxWidth = maxWidth;
        MaxHeight = maxHeight;
        Spacing = spacing;
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

    public char[,] GetPixels()
    {
        char[,] res = new char[MaxWidth, MaxHeight];
        res = res.FillEmpty();

        for (int i = 0; i < Lines.Count; i++)
        {
            res.Place(0, i * Spacing, Lines[i].AsPlaceable());
        }
        return res;
    }
}
