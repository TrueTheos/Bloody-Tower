using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Wand")]
public class WandSO : ItemScriptableObject, IRestrictTargeting
{
    public int charges;
    [SerializeField] public int chargesLeft;
    public enum spellType
    {
        point,
        ray,
        noTargetting
    }
    public spellType _spellType;

    public enum spell
    {
        ligtningbolt,
        randomTp
    }
    public spell _spell;

    public int maxDistance;

    public override void Use(MonoBehaviour foo, Item itemObject)
    {
        if(foo is PlayerStats player && chargesLeft > 0)
        {
            player.usingWand = true;
            Targeting.IsTargeting = true;
            player.usedWand = this;          
        }
        else if(chargesLeft < 1) //no charges left
        {
            GameManager.manager.UpdateMessages($"<color={I_color}>{I_name}</color> has no more charges!");
        }
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }

    public void UseWandSpell()
    {
        chargesLeft--;
        GameManager.manager.UpdateMessages($"You zap the <color={I_color}>{I_name}</color>.");

        bool loopBreaker = false;

        switch (_spell) 
        {
            case spell.randomTp:
                int randomX = 0;
                int randomY = 0;
                for (int i = 0; i < 500; i++) //500 is nuber of tries
                {
                    if (loopBreaker) break;

                    randomX = Random.Range(1, DungeonGenerator.dungeonGenerator.mapWidth);
                    randomY = Random.Range(1, DungeonGenerator.dungeonGenerator.mapHeight);

                    if(MapManager.map[randomX, randomY].isWalkable && MapManager.map[randomX, randomY].enemy == null && MapManager.map[randomX, randomY].structure == null)
                    {
                        MapManager.map[MapManager.playerPos.x, MapManager.playerPos.y].hasPlayer = false;
                        MapManager.map[MapManager.playerPos.x, MapManager.playerPos.y].letter = "";
                        PlayerMovement.playerMovement.position = new Vector2Int(randomX, randomY);
                        MapManager.map[randomX, randomY].hasPlayer = true;
                        MapManager.map[randomX, randomY].letter = "<color=green>@</color>";
                        MapManager.playerPos = new Vector2Int(randomX, randomY);

                        loopBreaker = true;

                        foreach(GameObject enemy in GameManager.manager.enemies)
                        {
                            enemy.GetComponent<RoamingNPC>().playerDetected = false;
                        }

                        GameManager.manager.FinishPlayersTurn();
                        DungeonGenerator.dungeonGenerator.DrawMap(true, MapManager.map);
                    }
                }
                break;
        }

        if (!GameManager.manager.playerStats.itemsInEqGO[GameManager.manager.selectedItem].identified)
        {
            GameManager.manager.playerStats.itemsInEqGO[GameManager.manager.selectedItem].identified = true; //make item identifyied
            GameManager.manager.UpdateInventoryText(); //update item names to identifyed names (ring -> ring of fire resistance)
            GameManager.manager.UpdateItemStats(GameManager.manager.playerStats.itemsInEqGO[GameManager.manager.selectedItem]); //show full statistics           
        }

        GameManager.manager.UpdateItemStats(GameManager.manager.playerStats.itemsInEqGO[GameManager.manager.selectedItem]);
    }

    public void SetCharges()
    {
        chargesLeft = charges;
    }

    public override void onEquip(MonoBehaviour foo)
    {
    }

    public override void onUnequip(MonoBehaviour foo)
    {
    }
    public bool IsValidTarget()
    {
        // TODO: code is required here
        return true;
    }

    public bool AllowTargetingMove()
    {
        // TODO: code is required here
        return true;
    }
}
