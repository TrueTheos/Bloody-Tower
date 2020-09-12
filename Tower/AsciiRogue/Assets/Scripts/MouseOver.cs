using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private TextMeshProUGUI textComponent;    

    public string displayedText; //text that will be displayed when mouse over it
    public Tooltip tooltip;

    private void Start()
    {
        textComponent = GetComponent<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltip.DisplayInfo(displayedText);
        textComponent.fontStyle = FontStyles.Bold;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltip.HideInfo();
        textComponent.fontStyle = FontStyles.Normal;
    }
}
