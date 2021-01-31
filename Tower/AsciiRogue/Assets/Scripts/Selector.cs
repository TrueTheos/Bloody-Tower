using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{

    public static Selector Current;

    public RectTransform selector;

    public Vector2Int currentPos;

    public int maxX, maxY;

    public bool active;

    public int height;

    public Text lookingAt;


    private void Awake()
    {
        Current = this;
    }

    void Update()
    {    
        if(Controls.GetKey(Controls.Inputs.LookKey))
        {
            if (!active)
            {
                selector.gameObject.SetActive(true);
                active = true;
                PlayerMovement.playerMovement.canMove = false;

                selector.anchoredPosition = new Vector2(MapManager.playerPos.x * 14 + 26, MapManager.playerPos.y * 20 - 455);
                currentPos = MapManager.playerPos;
            }

            if (Controls.GetKeyUp(Controls.Inputs.MoveUp))
            {
                currentPos.y += 1;
                selector.anchoredPosition = new Vector2(currentPos.x * 14 + 26, currentPos.y * 20 - 455);

                SelectedTile(currentPos.x, currentPos.y);
            }
            if (Controls.GetKeyUp(Controls.Inputs.MoveDown))
            {
                currentPos.y -= 1;
                selector.anchoredPosition = new Vector2(currentPos.x * 14 + 26, currentPos.y * 20 - 455);

                SelectedTile(currentPos.x, currentPos.y);
            }
            if (Controls.GetKeyUp(Controls.Inputs.MoveLeft))
            {
                currentPos.x -= 1;
                selector.anchoredPosition = new Vector2(currentPos.x * 14 + 26, currentPos.y * 20 - 455);

                SelectedTile(currentPos.x, currentPos.y);
            }
            if (Controls.GetKeyUp(Controls.Inputs.MoveRight))
            {
                currentPos.x += 1;
                selector.anchoredPosition = new Vector2(currentPos.x * 14 + 26, currentPos.y * 20 - 455);

                SelectedTile(currentPos.x, currentPos.y);
            }

            /*if(Controls.GetKeyUp(Controls.Inputs.Use) && MapManager.map[currentPos.x, currentPos.y].enemy)
            {
                GameManager.manager.gameObject.GetComponent<Bestiary>().UpdateText(MapManager.map[currentPos.x, currentPos.y].enemy.GetComponent<RoamingNPC>().enemySO);
            }*/
        }
        if (Controls.GetKeyUp(Controls.Inputs.LookKey))
        {
            active = false;

            selector.gameObject.SetActive(false);

            PlayerMovement.playerMovement.canMove = true;
        }
    }

    public void SelectedTile(int x, int y)
    {
        try
        {
            if(MapManager.map[x, y].isExplored && MapManager.map[x, y].specialNameOfTheCell != "") 
            {
                lookingAt.text = $"<color=red>{MapManager.map[x, y].specialNameOfTheCell}</color>"; 
                return;
            }
            else if (MapManager.map[x, y].isExplored && !MapManager.map[x, y].enemy && !MapManager.map[x, y].item) 
            {
                lookingAt.text = $"<color=yellow>{MapManager.map[x, y].type}</color>"; 
                return;
            }
            else if (MapManager.map[x, y].isExplored && MapManager.map[x, y].enemy) 
            {
                lookingAt.text = $"<color=red>{MapManager.map[x, y].enemy.GetComponent<RoamingNPC>().EnemyName}</color>"; 
                return;
            }
            else if (MapManager.map[x, y].isExplored && MapManager.map[x, y].item)
            {
                if(MapManager.map[x,y].item.GetComponent<Item>().identified)
                {
                     lookingAt.text = $"<color=green>{MapManager.map[x, y].item.GetComponent<Item>().iso.I_name}</color>";
                }
                else
                {
                     lookingAt.text = $"<color=green>{MapManager.map[x, y].item.GetComponent<Item>().iso.I_unInName}</color>";
                }
            }
            else lookingAt.text = "<color=grey>Darkness</color>";
        }
        catch
        {
            lookingAt.text = "<color=grey>Darkness</color>";
        }
    }
}
