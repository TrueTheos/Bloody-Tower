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
            GameManager.manager.CloseEQ();

            if (foo is PlayerStats player)
            {
                player.usedScrollOrBook = this;
                player.usingSpellScroll = true;

                Targeting.IsTargeting = true;
            }            
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
            if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null)
            {
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().WakeUp();
            }
        }
    }

    public void Poisonbolt(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null)
            {
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().WakeUp();
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().dotDuration = spellDuration;
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().DamageOverTurn();
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Poison Bolt</color>. Monster is now poisoned.");
            }
            else if (MapManager.map[Targeting.Position.x, Targeting.Position.y].hasPlayer)
            {
                if (player.isPoisoned)
                {
                    player.poisonDuration += spellDuration;
                    GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Poison Bolt</color>.");
                }
                else
                {
                    player.IncreasePoisonDuration(spellDuration);
                    player.Poison();
                    GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Poison Bolt</color>. You are now <color=green>poisoned</color>.");
                }
            }
            else
            {
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Poison Bolt</color> but nothing happens.");
            }
        }
    }

    public void Root(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null)
            {
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().WakeUp();
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().rooted = true;
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().rootDuration = 10 + Mathf.FloorToInt(player.__intelligence / 10);
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Root</color>. Monster can't move!");
            }
        }
    }

    public void Invisiblity(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null)
            {
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().WakeUp();
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().MakeInvisible();
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Invisiblity</color>.");
            }
            else if (MapManager.map[Targeting.Position.x, Targeting.Position.y].hasPlayer)
            {
                player.invisibleDuration = 20 + player.__intelligence;
                player.Invisible();
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Invisiblity</color>.");
            }
            else
            {
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Invisiblity</color> but nothing happens.");
            }
        }
    }

    public void BloodForBlood(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null)
            {
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().WakeUp();
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().TakeDamage(Mathf.FloorToInt((20 + player.__intelligence) / 5), ItemScriptableObject.damageType.magic);
                player.TakeDamage(5, damageType.normal);
                GameManager.manager.UpdateMessages($"You read the <color=red>Scroll of Blood for Blood</color>. You dealt {(20 + player.__intelligence) / 5} damage to the monster.");
            }
            else if (MapManager.map[Targeting.Position.x, Targeting.Position.y].hasPlayer)
            {
                player.TakeDamage(5, damageType.normal);
                GameManager.manager.UpdateMessages($"You read the <color=red>Scroll of Blood for Blood</color>. You feel piercing pain.");
            }
            else
            {
                GameManager.manager.UpdateMessages($"You read the <color=red>Scroll of Blood for Blood</color> but nothing happens.");
            }
        }
    }

    public void BloodRestore(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (MapManager.map[Targeting.Position.x, Targeting.Position.y].hasPlayer)
            {
                player.BloodRestore();
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Blood Restore</color>.");
            }
            else if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null)
            {
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().WakeUp();
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Blood Restore</color> but nothing happens.");
            }
            else
            {
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Blood Restore</color> but nothing happens.");
            }
        }
    }

    public void BloodPact(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (MapManager.map[Targeting.Position.x, Targeting.Position.y].hasPlayer)
            {
                player.__currentHp += 27; //25 + 2 because 2 is dealed from bleeding
                if (player.__currentHp > player.__maxHp)
                {
                    player.__currentHp -= (player.__currentHp - player.__maxHp);

                }

                player.IncreaseBleedingDuration(20 - (player.__intelligence / 7));
                player.Bleeding();

                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Blood Pact</color>. You restore 25 health but you are <color=red>bleeding</color> now!");
            }
            else if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null)
            {
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().WakeUp();
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Blood Pact</color> but nothing happens.");
            }
            else
            {
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Blood Pact</color> but nothing happens.");
            }
        }
    }

    public void Cauterize(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (MapManager.map[Targeting.Position.x, Targeting.Position.y].hasPlayer)
            {
                if (player.isBleeding)
                {
                    player.CureBleeding();
                }

                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Cauterize</color>. You are no longer bleeding!");
            }
            else if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null)
            {
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().WakeUp();
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Cauterize</color> but nothing happens.");
            }
            else
            {
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Cauterize</color> but nothing happens.");
            }
        }
    }

    public void RemovePoison(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (MapManager.map[Targeting.Position.x, Targeting.Position.y].hasPlayer)
            {
                if (player.isPoisoned)
                {
                    player.CurePoison();
                    GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Remove Poison</color>. You are no longer poisoned!");
                }
                else GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Remove Poison</color> but nothing happens.");
            }
            else if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null)
            {
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().WakeUp();
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Remove Poison</color> but nothing happens.");
            }
            else
            {
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Remove Poison</color> but nothing happens.");
            }
        }
    }

    public void CausticDart(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null)
            {
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().WakeUp();
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().TakeDamage(10 + Mathf.FloorToInt(player.__intelligence / 7), damageType.magic);
                GameManager.manager.UpdateMessages($"You read the <color=green>Scroll of Poison Dart.</color> You dealt {10 + Mathf.FloorToInt(player.__intelligence / 7)} damage to the monster.");
            }
            else if (MapManager.map[Targeting.Position.x, Targeting.Position.y].hasPlayer)
            {
                player.TakeDamage(10 + Mathf.FloorToInt(player.__intelligence / 7), damageType.normal);
                GameManager.manager.UpdateMessages($"You read the <color=green>Scroll of Poison Dart.</color> You feel piercing pain.");
            }
            else
            {
                GameManager.manager.UpdateMessages($"You read the <color=green>Scroll of Poison Dart</color> but nothing happens.");
            }
        }
    }

    public void Anoint(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (MapManager.map[Targeting.Position.x, Targeting.Position.y].hasPlayer)
            {
                player.Anoint();
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Anoint</color>.");
            }
            else if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null)
            {
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().WakeUp();
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Anoint</color> but nothing happens.");
            }
            else
            {
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Anoint</color> but nothing happens.");
            }
        }
    }

    public void Purify(MonoBehaviour foo)
    {
        if (foo is PlayerStats player)
        {
            if (MapManager.map[Targeting.Position.x, Targeting.Position.y].hasPlayer)
            {
                if (player.isPoisoned)
                {
                    player.CurePoison();
                    GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Purify</color>.");
                }
                else
                {
                    GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Purify</color> but nothing happens.");
                }
            }
            else if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null)
            {
                MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().WakeUp();
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Purify</color> but nothing happens.");
            }
            else
            {
                GameManager.manager.UpdateMessages("You read the <color=red>Scroll of Purify</color> but nothing happens.");
            }
        }
    }

    public override void onEquip(MonoBehaviour foo)
    {
    }

    public override void onUnequip(MonoBehaviour foo)
    {
    }

    public bool AllowTargetingMove()
    {
        // TODO: code is required here
        return true;
    }
}
