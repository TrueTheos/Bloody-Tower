using UnityEngine;

[CreateAssetMenu(menuName = "Items/Scroll")]
public class ScrollSO : ItemScriptableObject
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

    public int spellLevel;

    public enum spell
    {
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

    public int spellDuration;

    public override void Use(MonoBehaviour foo, Item itemObject)
    {
        if (GameManager.manager.playerStats.isBlind)
        {
            GameManager.manager.UpdateMessages("You can't read because you are <color=green>blind</color>!");
            return;
        }
        if (_type == type.self)
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
            //IF IS BLEEDING CNAT CAST BLOOD PACT;      
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
                        GameManager.manager.UpdateMessages("You can't use this scroll while you are <color=red>bleeding.</color>");
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

        if (foo is PlayerStats _player)
        {
            if (MapManager.map[_player.spell_pos.x, _player.spell_pos.y].enemy != null)
            {
                MapManager.map[_player.spell_pos.x, _player.spell_pos.y].enemy.GetComponent<RoamingNPC>().WakeUp();
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
                GameManager.manager.ApplyChangesInInventory(this);   
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Poison Bolt</color>. Monster is now poisoned.");
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
                 GameManager.manager.ApplyChangesInInventory(this);   
                 GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Root</color>. This monster can't move now.");
            }
        }
    }

    public void Invisiblity(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.invisibleDuration = 20 + player.__intelligence;
            player.Invisible();
            GameManager.manager.ApplyChangesInInventory(this);   
            GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Invisibility</color>.");
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
                GameManager.manager.ApplyChangesInInventory(this);   
                GameManager.manager.UpdateMessages($"You read the <color=red>Scroll of Blood for Blood</color>. You dealt {(20 + player.__intelligence) / 5} damage to the monster.");
            }
        }
    }

    public void BloodRestore(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.BloodRestore();
            Debug.Log("Blood restore");
            GameManager.manager.ApplyChangesInInventory(this);   
            GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Blood Restore</color>.");
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
            GameManager.manager.ApplyChangesInInventory(this);         
            GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Blood Pact</color>. You restore 25 health but you are <color=red>bleeding</color> now!");       
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
            GameManager.manager.ApplyChangesInInventory(this);   
            GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Cauterize</color>. You are no longer bleeding.");
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
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Remove Poison</color>. You are no longer poisoned!");
            }
            else GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Remove Poison</color>.");
            GameManager.manager.ApplyChangesInInventory(this);   
            
        }
    }

    public void CausticDart(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            if(MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy != null)
            {
                MapManager.map[player.spell_pos.x, player.spell_pos.y].enemy.GetComponent<RoamingNPC>().TakeDamage(10 + Mathf.FloorToInt(player.__intelligence / 7));
                GameManager.manager.ApplyChangesInInventory(this);   
                GameManager.manager.UpdateMessages($"You read the <color=green>Scroll of Poison Dart.</color> You dealt {10 + Mathf.FloorToInt(player.__intelligence / 7)} damage to the monster.");
            }
        }
    }

    public void Anoint(MonoBehaviour foo)
    {
        if(foo is PlayerStats player)
        {
            player.Anoint();
            GameManager.manager.ApplyChangesInInventory(this);   
            GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Anoint</color>.");
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
                GameManager.manager.ApplyChangesInInventory(this);   
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Purify</color>.");
            }
            else
            {
                GameManager.manager.ApplyChangesInInventory(this);   
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Purify</color>.");
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
