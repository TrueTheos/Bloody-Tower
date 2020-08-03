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
        heavyBandage
    }
    public potionEffect PotionEffect;

    public override void Use(MonoBehaviour foo)
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
        }

        GameManager.manager.ApplyChangesInInventory(this);      
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        
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

    public void Sickness(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            switch (_BUC)
            {
                case BUC.blessed:
                    player.TakeDamage(1);
                    break;
                case BUC.normal:
                    player.TakeDamage(Random.Range(1, 10));
                    break;
                case BUC.cursed:
                    player.TakeDamage(Random.Range(1, 20));
                    break;
            }           
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
}

