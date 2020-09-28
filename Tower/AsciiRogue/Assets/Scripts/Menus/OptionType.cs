using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum OptionSelectionType
{
    ArrowLeftRight,
    List
}

public interface IOptionType 
{
    string GetCurrent();

    List<string> GetAll();

    int GetCount();

    void Next();
    void Previous();

    int GetCurrentIndex();
}
