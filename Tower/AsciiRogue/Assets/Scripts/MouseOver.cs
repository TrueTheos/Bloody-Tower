using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string displayedText; //text that will be displayed when mouse over it
    public Tooltip tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.DisplayInfo(displayedText);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideInfo();
    }
}
