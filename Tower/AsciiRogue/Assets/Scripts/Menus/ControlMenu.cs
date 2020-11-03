using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlMenu : MenuInfo
{
    


    public void Reload()
    {

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
