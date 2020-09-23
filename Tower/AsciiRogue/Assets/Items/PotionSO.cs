using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Items/Potion")]
public class PotionSO : ItemScriptableObject
{
    public string potionSplashColor;

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
        monsterDetection,
        bloodRestore
    }
    public potionEffect PotionEffect;

    public override void Use(MonoBehaviour foo, Item itemObject)
    {
        if(!itemObject.identified) itemObject.identified = true;

        switch (PotionEffect)
        {
            case potionEffect.fireResistance:
                FireResistance(foo,itemObject);
                break;
            case potionEffect.regeneration:
                Regeneration(foo, itemObject);
                break;
            case potionEffect.poisonResistance:
                PoisonResistance(foo, itemObject);
                break;
            case potionEffect.fullRestore:
                FullRestore(foo, itemObject);
                break;
            case potionEffect.levelVision:
                LevelVision(foo, itemObject);
                break;
            case potionEffect.soiledBandage:
                soiledBandage(foo, itemObject);
                break;
            case potionEffect.bandage:
                Bandage(foo, itemObject);
                break;
            case potionEffect.heavyBandage:
                HeavyBandage(foo, itemObject);
                break;
            case potionEffect.healing:
                healing(foo, itemObject);
                break;
            case potionEffect.poison:
                Poison(foo, itemObject);
                break;
            case potionEffect.blindness:
                Blindness(foo, itemObject);
                break;
            case potionEffect.monsterDetection:
                ShowMonsters(foo, itemObject);
                break;
            case potionEffect.bloodRestore:
                BloodRestore(foo);
                break;
        }

        GameManager.manager.ApplyChangesInInventory(this);      
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }

    private void ShowMonsters(MonoBehaviour foo, Item item)
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

    private void healing(MonoBehaviour foo, Item item)
    {
        if (foo is PlayerStats player)
        {
            switch (item._BUC)
            {
                case Item.BUC.blessed:
                    player.__currentHp += 10;
                    if (player.__currentHp > player.__maxHp)
                    {
                        player.__currentHp -= player.__currentHp - player.__maxHp;
                    }
                    player.__maxHp += 2;
                    break;
                case Item.BUC.cursed:
                    player.__maxHp -= 2;
                    player.__currentHp += 4;
                    if (player.__currentHp > player.__maxHp)
                    {
                        player.__currentHp -= player.__currentHp - player.__maxHp;
                    }
                    break;
                case Item.BUC.normal:
                    player.__currentHp += 5;
                    if (player.__currentHp > player.__maxHp)
                    {
                        player.__currentHp -= player.__currentHp - player.__maxHp;
                    }
                    break;
            }
        }
    }
    private void BloodRestore(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            int bloodRestored = Random.Range(1, 7) + Random.Range(1, 7);
            player.__blood += bloodRestored;
            //GameManager.manager.UpdateMessages($"You restore <color=red>{bloodRestored} blood </color>.");
        }
    }
    
    private void soiledBandage(MonoBehaviour foo, Item item)
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

    private void Bandage(MonoBehaviour foo, Item item)
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

    private void HeavyBandage(MonoBehaviour foo, Item item)
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

    private void FireResistance(MonoBehaviour foo, Item item)
    {
        if(foo is PlayerStats player)
        {
            player.FireResistance();
        }
    }

    private void Regeneration(MonoBehaviour foo, Item item)
    {
        if(foo is PlayerStats player)
        {
            player.Regeneration();
        }
    }

    public void PoisonResistance(MonoBehaviour foo, Item item)
    {
        if(foo is PlayerStats player)
        {
            player.PoisonResistance();
        }
    }

    public void FullRestore(MonoBehaviour foo, Item item)
    {
        if(foo is PlayerStats player)
        {
            player.FullRestore();
        }
    }

    public void LevelVision(MonoBehaviour foo, Item item)
    {
        if (foo is PlayerStats player)
        {
            player.FullVision();
            //GameManager.manager.UpdateMessages($"You drank the potion of full floor vision.");
        }
    }

    private void Poison(MonoBehaviour foo, Item item)
    {
        if(foo is PlayerStats player)
        {
            switch(item._BUC)
            {
                case Item.BUC.blessed:
                    GameManager.manager.UpdateMessages("It tastes like a <color=green>poison</color>.");
                    break;
                case Item.BUC.cursed:
                    //GameManager.manager.UpdateMessages($"You drank the <color=green>poison</color>.");
                    if (!player.isPoisoned)
                    {
                        player.poisonDuration = 15;
                        player.Poison();
                    }
                    break;
                case Item.BUC.normal:
                    //GameManager.manager.UpdateMessages($"You drank the <color=green>poison</color>.");
                    if (!player.isPoisoned)
                    {
                        player.poisonDuration = 8;
                        player.Poison();
                    }
                    break;
            }
        }
    }

    private void Blindness(MonoBehaviour foo, Item item)
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

