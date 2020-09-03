using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Items/Potion")]
public class PotionSO : ItemScriptableObject
{
    public enum potionEffect
    {
        fireResistance,
        regeneration,
        poisonResistance,
        fullRestore,
        levelVision,
        soiledBandage,
        bandage,
        heavyBandage,
        healing,
        poison,
        blindness,
        monsterDetection
    }
    public potionEffect PotionEffect;

    public override void Use(MonoBehaviour foo, Item itemObject)
    {
        if(!identified) identified = true;

        switch (PotionEffect)
        {
            case potionEffect.fireResistance:
                FireResistance(foo);
                break;
            case potionEffect.regeneration:
                Regeneration(foo);
                break;
            case potionEffect.poisonResistance:
                PoisonResistance(foo);
                break;
            case potionEffect.fullRestore:
                FullRestore(foo);
                break;
            case potionEffect.levelVision:
                LevelVision(foo);
                break;
            case potionEffect.soiledBandage:
                soiledBandage(foo);
                break;
            case potionEffect.bandage:
                Bandage(foo);
                break;
            case potionEffect.heavyBandage:
                HeavyBandage(foo);
                break;
            case potionEffect.healing:
                healing(foo);
                break;
            case potionEffect.poison:
                Poison(foo);
                break;
            case potionEffect.blindness:
                Blindness(foo);
                break;
            case potionEffect.monsterDetection:
                ShowMonsters(foo);
                break;
        }

        GameManager.manager.ApplyChangesInInventory(this);      
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }

    private void ShowMonsters(MonoBehaviour foo)
    {
        /*if(foo is PlayerStats player)
        {
            foreach(Transform child in DungeonGenerator.dungeonGenerator.floorManager.floorsGO[DungeonGenerator.dungeonGenerator.currentFloor].transform)
            {
                if(child.GetComponent<RoamingNPC>() != null)
                {
                    MapManager.map[child.gameObject.GetComponent<RoamingNPC>().__position.x, child.gameObject.GetComponent<RoamingNPC>().__position.y].decoy = $"<color=#{ColorUtility.ToHtmlStringRGB(child.gameObject.GetComponent<RoamingNPC>().enemySO.E_color)}>{child.gameObject.GetComponent<RoamingNPC>().enemySO.E_symbol}</color>";              
                }
            }
        }*/
    }

    private void healing(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            switch(_BUC)
            {
                case BUC.blessed:
                    player.__currentHp += 10;
                    if (player.__currentHp > player.__maxHp)
                    {
                        player.__currentHp -= player.__currentHp - player.__maxHp;
                    }
                    player.__maxHp += 2;
                    break;
                case BUC.cursed:
                    player.__maxHp -= 2;
                    player.__currentHp += 4;
                    if (player.__currentHp > player.__maxHp)
                    {
                        player.__currentHp -= player.__currentHp - player.__maxHp;
                    }
                    break;
                case BUC.normal:
                    player.__currentHp += 5;
                    if (player.__currentHp > player.__maxHp)
                    {
                        player.__currentHp -= player.__currentHp - player.__maxHp;
                    }
                    break;
            }
        }
    }
    
    private void soiledBandage(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            int i = 2 + Random.Range(1,3);
            player.__currentHp += i;
            if(player.__currentHp > player.__maxHp)
            {
                player.__currentHp -= player.__currentHp - player.__maxHp;
            }

            if(player.isBleeding)
            {
                player.bleegingDuration = 0;
                player.Bleeding();
            }

            GameManager.manager.UpdateMessages($"You applied the soiled bandage and regained {i} health.");
        }
    }

    private void Bandage(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            int i = 2 + Random.Range(1,6);
            player.__currentHp += i;
            if(player.__currentHp > player.__maxHp)
            {
                player.__currentHp -= player.__currentHp - player.__maxHp;
            }

            if(player.isBleeding)
            {
                player.bleegingDuration = 0;
                player.Bleeding();
            }

            GameManager.manager.UpdateMessages($"You applied the bandage and regained {i} health.");
        }
    }

    private void HeavyBandage(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            int i = 2 + Random.Range(1,6) + Random.Range(1,6);
            player.__currentHp += i;
            if(player.__currentHp > player.__maxHp)
            {
                player.__currentHp -= player.__currentHp - player.__maxHp;
            }

            if(player.isBleeding)
            {
                player.bleegingDuration = 0;
                player.Bleeding();
            }

            GameManager.manager.UpdateMessages($"You applied the heavy bandage and regained {i} health.");
        }
    }

    private void FireResistance(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.FireResistance();
        }
    }

    private void Regeneration(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.Regeneration();
        }
    }

    public void PoisonResistance(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.PoisonResistance();
        }
    }

    public void FullRestore(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.FullRestore();
        }
    }

    public void LevelVision(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            player.FullVision();
            GameManager.manager.UpdateMessages($"You drank the potion of full floor vision.");
        }
    }

    private void Poison(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            switch(_BUC)
            {
                case BUC.blessed:
                    GameManager.manager.UpdateMessages("It tastes like a <color=green>poison</color>.");
                    break;
                case BUC.cursed:
                    GameManager.manager.UpdateMessages($"You drank the <color=green>poison</color>.");
                    if (!player.isPoisoned)
                    {
                        player.poisonDuration = 15;
                        player.Poison();
                    }
                    break;
                case BUC.normal:
                    GameManager.manager.UpdateMessages($"You drank the <color=green>poison</color>.");
                    if (!player.isPoisoned)
                    {
                        player.poisonDuration = 8;
                        player.Poison();
                    }
                    break;
            }
        }
    }

    private void Blindness(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.Blindness();
        }
    }

    public override void onEquip(MonoBehaviour foo)
    {
    }

    public override void onUnequip(MonoBehaviour foo)
    {
    }
}

