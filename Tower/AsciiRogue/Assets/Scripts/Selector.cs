using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Selector : MonoBehaviour
{
    public RectTransform selector;

    public Vector2Int currentPos;

    public int maxX, maxY;

    public bool active;

    public int height;

    public Text lookingAt;

    void Update()
    {    
        if(Input.GetKey(KeyCode.LeftControl))
        {
            if (!active)
            {
                selector.gameObject.SetActive(true);
                active = true;
                PlayerMovement.playerMovement.canMove = false;

                selector.anchoredPosition = new Vector2(MapManager.playerPos.x * 14 + 26, MapManager.playerPos.y * 20 - 455);
                currentPos = MapManager.playerPos;
            }

            if (Input.GetKeyUp(KeyCode.W))
            {
                currentPos.y += 1;
                selector.anchoredPosition = new Vector2(currentPos.x * 14 + 26, currentPos.y * 20 - 455);

                SelectedTile(currentPos.x, currentPos.y);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                currentPos.y -= 1;
                selector.anchoredPosition = new Vector2(currentPos.x * 14 + 26, currentPos.y * 20 - 455);

                SelectedTile(currentPos.x, currentPos.y);
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                currentPos.x -= 1;
                selector.anchoredPosition = new Vector2(currentPos.x * 14 + 26, currentPos.y * 20 - 455);

                SelectedTile(currentPos.x, currentPos.y);
            }
            if (Input.GetKeyUp(KeyCode.D))
            {
                currentPos.x += 1;
                selector.anchoredPosition = new Vector2(currentPos.x * 14 + 26, currentPos.y * 20 - 455);

                SelectedTile(currentPos.x, currentPos.y);
            }

            if(Input.GetKeyUp(KeyCode.Return) && MapManager.map[currentPos.x, currentPos.y].enemy)
            {
                GameManager.manager.gameObject.GetComponent<Bestiary>().UpdateText(MapManager.map[currentPos.x, currentPos.y].enemy.GetComponent<RoamingNPC>().enemySO);
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            active = false;

            selector.gameObject.SetActive(false);

            PlayerMovement.playerMovement.canMove = true;
        }
    }

    private void SelectedTile(int x, int y)
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
                if(MapManager.map[x,y].item.GetComponent<Item>().iso.identified)
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
