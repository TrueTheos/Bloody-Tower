using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SelectableList : IPrintable 
{


    public MenuInfo RootMenu;

    public Stack<IPrintable> InfoStack;

    public IPrintable GetCurrent() => InfoStack.Count > 0 ? InfoStack.Peek() : RootMenu;

    public SelectableList(MenuInfo root)
    {
        RootMenu = root;
        InfoStack = new Stack<IPrintable>();
    }

    
    public char[,] GetPixels()
    {
        return GetCurrent().GetPixels();
    }
    public string GetAsText()
    {
        return GetCurrent().GetAsText();
    }
    
    public void PopInfo(IPrintable info = null)
    {
        if (InfoStack.Count==0)
        {
            return;
        }
        // i know this is hacky way of doing it, but it works
        IPrintable tmp = InfoStack.Peek();
        info = info ?? tmp;
        if (tmp != info)
        {
            return;
        }

        // pop the menu
        InfoStack.Pop();
            
        
    }
    public void AddMenu(IPrintable info)
    {
        InfoStack.Push(info);
    }


    public void Update()
    {
        var c = GetCurrent();

        if (c is MenuInfo m)
        {
            if (m.Update())
            {
                PopInfo(m);
            }
        }
        else
        {
            if (Controls.GetKeyDown(Controls.Inputs.CancelButton))
            {
                PopInfo();
            }
        }
    }


}
