using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Spellbook")]
public class SpellbookSO : ItemScriptableObject
{
    public enum school
    {
        chaos,
        Blood,
        Plague
    }
    public school _school;

    public enum type
    {
        self,
        target
    }
    public type _type;

    public enum spell
    {
        DrainLife,
        BloodForBlood,
        BloodRestore,
        BloodPact,
        Cauterize,
        RemovePoison,
        CausticDart,
        Anoint,
        Purify,
        Invisiblity,
        Root,
        Poisonbolt
    }
    public spell _spell;

    public int spellLevel;

    public int coolDown;

    public override void Use(MonoBehaviour foo)
    {
        if(spellLevel == 1 && GameManager.manager.playerStats.__intelligence >= 20)
        {
            if(_type == type.self)
            {
                UseSpell(foo);
            }
            else
            {
                if(foo is PlayerStats player)
                {
                    player.usedScrollOrBook = this;
                    player.usingSpellScroll = true;
                    player.spell_pos = MapManager.playerPos;
                }

                GameManager.manager.CloseEQ();
            }
        }
        else if(spellLevel == 2 && GameManager.manager.playerStats.__intelligence >= 40)
        {
            if(_type == type.self)
            {
                UseSpell(foo);
            }
            else
            {
                if(foo is PlayerStats player)
                {
                    player.usedScrollOrBook = this;
                    player.usingSpellScroll = true;
                    player.spell_pos = MapManager.playerPos;
                }

                GameManager.manager.CloseEQ();
            }
        }
        else if(spellLevel == 3 && GameManager.manager.playerStats.__intelligence >= 90)
        {
            if(_type == type.self)
            {
                UseSpell(foo);
            }
            else
            {
                if(foo is PlayerStats player)
                {
                    player.usedScrollOrBook = this;
                    player.usingSpellScroll = true;
                    player.spell_pos = MapManager.playerPos;
                }

                GameManager.manager.CloseEQ();
            }
        }
        else
        {
            if(foo is PlayerStats player)
            if(spellLevel == 1)
            {
                GameManager.manager.UpdateMessages($"You need more intelligence to read this book! You need {20 - player.__intelligence}");
            }
            else if(spellLevel == 2)
            {
                GameManager.manager.UpdateMessages($"You need more intelligence to read this book! You need {40 - player.__intelligence}");
            }
            else if(spellLevel == 3)
            {
                GameManager.manager.UpdateMessages($"You need more intelligence to read this book! You need {90 - player.__intelligence}");
            }
            
        }       
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }

    public void UseSpell(MonoBehaviour foo)
    {
        switch(_spell)
        {
            case spell.BloodForBlood:
                BloodForBlood(foo);
                break;
            case spell.BloodRestore:
                BloodRestore(foo); 
                break;
            case spell.BloodPact:
                if(foo is PlayerStats player)
                {
                    if(!player.isBleeding)
                    {
                        BloodPact(foo);
                    }
                    else
                    {
                        GameManager.manager.UpdateMessages("You can't read this book while you are <color=red>bleeding.</color>");
                    }
                }               
                break;
            case spell.Cauterize:
                Cauterize(foo);  
                break;
            case spell.RemovePoison:
                RemovePoison(foo);  
                break;
            case spell.CausticDart:
                CausticDart(foo);
                break;
            case spell.Anoint:
                Anoint(foo);
                break;
            case spell.Purify:
                Purify(foo);
                break;
            case spell.Invisiblity:
                Invisiblity(foo);
                break;
            case spell.Root:
                Root(foo);
                break;
            case spell.Poisonbolt:
                Poisonbolt(foo);
                break;
        }
        if (!GameManager.manager.playerStats.itemsInEq[GameManager.manager.selectedItem].identified)
        {
            GameManager.manager.playerStats.itemsInEq[GameManager.manager.selectedItem].identified = true; //make item identifyied
            GameManager.manager.UpdateInventoryText(); //update item names to identifyed names (ring -> ring of fire resistance)
            GameManager.manager.UpdateItemStats(GameManager.manager.playerStats.itemsInEq[GameManager.manager.selectedItem], GameManager.manager.playerStats.itemInEqGO[GameManager.manager.selectedItem]); //show full statistics           
        }

        GameManager.manager.UpdateItemStats(this, GameManager.manager.playerStats.itemInEqGO[GameManager.manager.selectedItem]);
    }

    public void DrainLife(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            if(MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy != null)
            {
                MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy.GetComponent<RoamingNPC>().TakeDamage(Random.Range(1,10) + Random.Range(1,10) + player.__intelligence / 10);
                coolDown = 20;
                GameManager.manager.UpdateMessages("You read the <color=red>Book of Drain Life</color>.");
            }
        }
    }

    public void Poisonbolt(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            if(MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy != null)
            {
                MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy.GetComponent<RoamingNPC>().DamageOverTurn(); 
                GameManager.manager.UpdateMessages("You read the <color=red>Book of Poison Bolt</color>.");
            }
        }
    }

    public void Root(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            if(MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy != null)
            {
                MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy.GetComponent<RoamingNPC>().rooted = true;
                MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy.GetComponent<RoamingNPC>().rootDuration = 10 + Mathf.FloorToInt(player.__intelligence / 10);  
                GameManager.manager.UpdateMessages("You read the <color=red>Book of Root</color>.");
            }
        }
    }

    public void Invisiblity(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.invisibleDuration = 20 + player.__intelligence;
            player.Invisible();
            GameManager.manager.UpdateMessages("You read the <color=red>Book of Invisiblity</color>.");
        }
    }

    public void BloodForBlood(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            if(MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy != null)
            {               
                MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy.GetComponent<RoamingNPC>().TakeDamage(Mathf.FloorToInt((20 + player.__intelligence) / 5));
                player.TakeDamage(5);
                Debug.Log("Blood purge" + Mathf.FloorToInt((20 + player.__intelligence) / 5));
                GameManager.manager.UpdateMessages("You read the <color=red>Book of Blood for Blood</color>.");
            }
        }
    }

    public void BloodRestore(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.BloodRestore();
            Debug.Log("Blood restore"); 
            GameManager.manager.UpdateMessages("You read the <color=red>Book of Blood Restore</color>.");
        }
    }

    public void BloodPact(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.__currentHp += 27; //25 + 2 because 2 is dealed from bleeding
            if(player.__currentHp> player.__maxHp)
            {
                player.__currentHp -= (player.__currentHp - player.__maxHp);
                    
            }   

            player.Bleeding();

            Debug.Log("Blood pact");         
            GameManager.manager.UpdateMessages("You read the <color=red>Book of Blood Pact</color>.");       
        }
    }

    public void Cauterize(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            if(player.isBleeding)
            {
                player.bleegingDuration = 0;
                player.Bleeding();
            }           

            Debug.Log("cauterize");
            GameManager.manager.UpdateMessages("You read the <color=red>Book of Cauterize</color>.");
        }
    }

    public void RemovePoison(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            if(player.isPoisoned)
            {
                player.poisonDuration = 0;
                player.Poison();
            } 
            GameManager.manager.UpdateMessages("You read the <color=red>Book of Remove Poison</color>.");
        }
    }

    public void CausticDart(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            if(MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy != null)
            {
                MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy.GetComponent<RoamingNPC>().TakeDamage(10 + Mathf.FloorToInt(player.__intelligence / 7));
                GameManager.manager.UpdateMessages("You read the <color=green>Book of Poison Dart.</color>");
            }
        }
    }

    public void Anoint(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.Anoint();
            GameManager.manager.UpdateMessages("You read the <color=red>Book of Anoint</color>.");
        }
    }

    public void Purify(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            if(player.isPoisoned)
            {
                player.poisonDuration = 0;
                player.Poison();
                GameManager.manager.UpdateMessages("You read the <color=red>Book of Purify</color>.");
            }
        }
    }
}
