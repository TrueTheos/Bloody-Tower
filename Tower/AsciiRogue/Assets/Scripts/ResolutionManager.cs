using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RuntimePresets;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
    public RectTransform ConsoleRect;
    public Text ConsoleText;

    public Text BorderText;

    public RectTransform MessageRect;
    public Text MessageText;

    public Transform PointATrans;
    public Transform PointBTrans;

    public RectTransform FloorRect;
    public RectTransform TileRect;
    public RectTransform EffectRect;

    public RectTransform ItemName, ItemType, ItemRareness, ItemEffects;

    [Header("Console")]
    public Preset C_RestTransform;

    [Header("UI Border")]
    public Preset B_Text;

    [Header("MessageLog")]
    public Preset M_RectTransfrom;

    [Header("GameObjects")]
    public Preset PointA_Pos;
    public Preset PointB_Pos;

    [Header("Floor & Object & Effects")]
    public Preset Floor_Rect;
    public Preset Tile_Rect;
    public Preset Effect_Rect;

    [Header("Inventory")]
    public Preset item_Name, item_Type, item_Rareness, item_Effects;

    //[Header("Inventory Pointer")]

    [Header("Console")]
    public Preset C_RestTransform2;

    [Header("UI Border")]
    public Preset B_Text2;

    [Header("MessageLog")]
    public Preset M_RectTransfrom2;

    [Header("GameObjects")]
    public Preset PointA_Pos2;
    public Preset PointB_Pos2;

    [Header("Floor & Object & Effects")]
    public Preset Floor_Rect2;
    public Preset Tile_Rect2;
    public Preset Effect_Rect2;

    [Header("Inventory")]
    public Preset item_Name2, item_Type2, item_Rareness2, item_Effects2;

    public Vector2Int borderSize;

    public static ResolutionManager resolutionManager;

    public enum resolutionType { small, big}
    public resolutionType _Resolution;

    public void Start()
    {
        resolutionManager = this;

        /*_Resolution = resolutionType.big;
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);*/
        _Resolution = resolutionType.big;
        Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
        SetBigUi();
        MousePointer.mousePointer.UpdateMapRect();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.P))
        {
            _Resolution = resolutionType.small;
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            SetUiSmall();
            MousePointer.mousePointer.UpdateMapRect();

            DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            _Resolution = resolutionType.big;
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
            SetBigUi();
            MousePointer.mousePointer.UpdateMapRect();

            DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
        }
    }

    private void SetUiSmall()
    {
        try
        {
            ConsoleRect.TransferValuesFrom(C_RestTransform);
            //C_RestTransform.ApplyTo(ConsoleRect);

            BorderText.TransferValuesFrom(B_Text);

            MessageRect.TransferValuesFrom(M_RectTransfrom);

            PointATrans.position = PointA_Pos.GetComponent<Transform>().position;
            PointBTrans.position = PointB_Pos.GetComponent<Transform>().position;
            //PointATrans.TransferValuesFrom(PointA_Pos);
            //PointBTrans.TransferValuesFrom(PointB_Pos);

            FloorRect.TransferValuesFrom(Floor_Rect);
            TileRect.TransferValuesFrom(Tile_Rect);
            EffectRect.TransferValuesFrom(Effect_Rect);

            ItemName.TransferValuesFrom(item_Name);
            ItemType.TransferValuesFrom(item_Type);
            ItemRareness.TransferValuesFrom(item_Rareness);
            ItemEffects.TransferValuesFrom(item_Effects);
        }
        catch { }
    }

    private void SetBigUi()
    {
        try
        {
            ConsoleRect.TransferValuesFrom(C_RestTransform2);
            //C_RestTransform2.ApplyTo(ConsoleRect);

            BorderText.TransferValuesFrom(B_Text2);

            MessageRect.TransferValuesFrom(M_RectTransfrom2);

            PointATrans.position = PointA_Pos2.GetComponent<Transform>().position;
            PointBTrans.position = PointB_Pos2.GetComponent<Transform>().position;
            //PointATrans.TransferValuesFrom(PointA_Pos2);
            //PointBTrans.TransferValuesFrom(PointB_Pos2);

            FloorRect.TransferValuesFrom(Floor_Rect2);
            TileRect.TransferValuesFrom(Tile_Rect2);
            EffectRect.TransferValuesFrom(Effect_Rect2);

            ItemName.TransferValuesFrom(item_Name2);
            ItemType.TransferValuesFrom(item_Type2);
            ItemRareness.TransferValuesFrom(item_Rareness2);
            ItemEffects.TransferValuesFrom(item_Effects2);
        }
        catch { }
    }
}
