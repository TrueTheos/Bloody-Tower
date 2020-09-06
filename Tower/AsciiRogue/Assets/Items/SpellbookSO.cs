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
    public int spellDuration;

    [Header("Learning Settings")]
    public int duration = 5; //how many times u can try to read it
    public int learnDuration; //how many turns does it take to learn it

    public override void Use(MonoBehaviour foo, Item itemObject)
    {
        GameManager.manager.readingBook = this;

        if(GameManager.manager.playerStats.isBlind)
        {
            GameManager.manager.UpdateMessages("You can't read because you are <color=green>blind</color>!");
            return;
        }

        if (!itemObject.learned && itemObject.durationLeft > 0)
        {
            GameManager.manager.UpdateMessages("You start to page through the ornate tome...");
            itemObject.durationLeft--;
            GameManager.manager.waitingCoroutine = GameManager.manager.WaitTurn(learnDuration);
            GameManager.manager.StartCoroutine(GameManager.manager.waitingCoroutine);
        }

        if (itemObject.durationLeft <= 0)
        {
            GameManager.manager.UpdateMessages("<color=lightblue>Puff!</color> Book fades away...");
            GameManager.manager.ApplyChangesInInventory(this);
            return;
        }

        /*if (_type == type.self)
        {
            UseSpell(foo);
        }
        else
        {
            if (foo is PlayerStats player)
            {
                player.usedScrollOrBook = this;
                player.usingSpellScroll = true;
                player.spell_pos = MapManager.playerPos;
            }

            GameManager.manager.CloseEQ();
        }*/
    }

    public override void OnPickup(MonoBehaviour foo)
    {
        
    }

    public void CastSpell(MonoBehaviour foo)
    {
        if (_type == type.self)
        {
            UseSpell(foo);
            GameManager.manager.CloseEQ();
        }
        else
        {
            if (foo is PlayerStats player)
            {
                player.usedScrollOrBook = this;
                player.usingSpellScroll = true;
                player.spell_pos = MapManager.playerPos;
            }

            GameManager.manager.CloseEQ();
        }
    }

    public void UseSpell(MonoBehaviour foo)
    {
        switch (_spell)
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

        if(foo is PlayerStats _player)
        {
            if (MapManager.map[_player.spell_pos.x, _player.spell_pos.y].enemy != null)
            {
                MapManager.map[_player.spell_pos.x, _player.spell_pos.y].enemy.GetComponent<RoamingNPC>().WakeUp();
            }
        }
    }

    public void DrainLife(MonoBehaviour foo, Item item)
    {
        if(foo is PlayerStats player)
        {
            if(MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy != null)
            {
                int damage = Random.Range(1, 10) + Random.Range(1, 10) + player.__intelligence / 10;
                MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy.GetComponent<RoamingNPC>().TakeDamage(damage);
                item.spellbookCooldown = 20;
                GameManager.manager.UpdateMessages($"You read the <color=red>Book of Drain Life</color>. You drained <color=red>{damage}</color> health.");
            }
        }
    }

    public void Poisonbolt(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            if(MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy != null)
            {
                MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy.GetComponent<RoamingNPC>().dotDuration = spellDuration;
                MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy.GetComponent<RoamingNPC>().DamageOverTurn(); 
                GameManager.manager.UpdateMessages("You read the <color=red>Book of Poison Bolt</color>. Monster is now poisoned.");
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
                GameManager.manager.UpdateMessages("You read the <color=red>Book of Root</color>. Monster can't move!");
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
                GameManager.manager.UpdateMessages($"You read the <color=red>Book of Blood for Blood</color>. You dealt {(20 + player.__intelligence) / 5} damage to the monster.");
            }
        }
    }

    public void BloodRestore(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.BloodRestore();
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
     
            GameManager.manager.UpdateMessages("You read the <color=red>Book of Blood Pact</color>. You restore 25 health but you are <color=red>bleeding<color> now!");       
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

            GameManager.manager.UpdateMessages("You read the <color=red>Book of Cauterize</color>. You are no longer bleeding!");
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
                GameManager.manager.UpdateMessages("You read the <color=red>Book of Remove Poison</color>. You are no longer poisoned!");
            } 
            else GameManager.manager.UpdateMessages("You read the <color=red>Book of Remove Poison</color>.");
        }
    }

    public void CausticDart(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            if(MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy != null)
            {
                MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy.GetComponent<RoamingNPC>().TakeDamage(10 + Mathf.FloorToInt(player.__intelligence / 7));
                GameManager.manager.UpdateMessages($"You read the <color=green>Book of Poison Dart.</color> You dealt {10 + Mathf.FloorToInt(player.__intelligence / 7)} damage to the monster.");
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

    public override void onEquip(MonoBehaviour foo)
    {
    }

    public override void onUnequip(MonoBehaviour foo)
    {
    }
}
