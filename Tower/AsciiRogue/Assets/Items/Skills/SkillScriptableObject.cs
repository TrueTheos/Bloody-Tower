using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Skill")]
public class SkillScriptableObject : ScriptableObject,IRestrictTargeting
{
    public string Name;

    public string Description;

    public List<WeaponsSO.weaponType> ValidWeapons;

    public string DisplayColor;
    
    public int Range;

    public int BloodCost;
    [Tooltip("if the spell requires some preperation or is instantly cast when selected")]
    public bool IsInstant;

    public enum SkillEffect
    {
        HeavySwing,

    }

    public SkillEffect Effect;

    ///<summary> if the Skill should be shown in the window</summary>
    public bool IsShown(PlayerStats player)
    {
        switch (Effect)
        {
            case SkillEffect.HeavySwing:
                
                break;
            default:
                break;
        }
        return true;
    }
    ///<summary> if the skill can be cast (makes them gray if visible)</summary>
    public bool IsCastable(PlayerStats player)
    {
        switch (Effect)
        {
            case SkillEffect.HeavySwing:
                foreach (var item in ValidWeapons)
                {
                    if (item == WeaponsSO.weaponType.melee)
                    {
                        if (player._Lhand == null || player._Rhand == null)
                        {
                            return true;
                        }
                        if (player._Lhand == null || player._Rhand == null)
                        {
                            return true;
                        }
                    }
                }
                return false;
            default:
                break;
        }

        return true;
    }
    // when selected by the player
    public void Prepare(PlayerStats player)
    {
        switch (Effect)
        {
            case SkillEffect.HeavySwing:
                Targeting.IsTargeting = true;
                break;
            default:
                break;
        }
    }
    // when the effect actually is triggered (can be called from prepare if wanted)
    public void Activate(PlayerStats player)
    {
        switch (Effect)
        {
            case SkillEffect.HeavySwing:
                ActivateHeavySwing(player);
                break;
            default:
                break;
        }
    }

    private void ActivateHeavySwing(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = Random.Range(1, 101);

        int calcRoll;

        if (npc.sleeping)
        {
            calcRoll = roll + player.__dexterity - npc.dex / 4 - npc.AC;
        }
        else
        {
            calcRoll = roll + player.__dexterity - npc.dex - npc.AC;
        }

        if (roll <= 0)
        {
            GameManager.manager.UpdateMessages("You missed!");
            PlayerMovement.playerMovement.WakeUpEnemy(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 60 || roll >= 80) //Do we hit?
        {
            if (player._Lhand?.iso is WeaponsSO weaponL && CheckWeaponType(weaponL))
            {
                for (int x = 0; x < weaponL.attacks.Count; x++)
                {
                    damage += Random.Range(1, weaponL.attacks[x].y);

                    //IF WEAPON IS BLOOD SWORD WE INCREASE ITS DAMAGE
                    if (weaponL.I_name == "Bloodsword")
                    {
                        weaponL.attacks[0] = new Vector2Int(1, weaponL.attacks[0].y + 1);
                        weaponL.bloodSwordCounter = GameManager.manager.tasks.bloodSwordCooldown;
                    }
                }
            }
            if (player._Rhand?.iso is WeaponsSO weaponR && CheckWeaponType(weaponR))
            {
                for (int x = 0; x < weaponR.attacks.Count; x++)
                {
                    damage += Random.Range(1, weaponR.attacks[x].y);

                    //IF WEAPON IS BLOOD SWORD WE INCREASE ITS DAMAGE
                    if (weaponR.I_name == "Bloodsword")
                    {
                        weaponR.attacks[0] = new Vector2Int(1, weaponR.attacks[0].y + 1);
                        weaponR.bloodSwordCounter = GameManager.manager.tasks.bloodSwordCooldown;
                    }
                }
            }

            damage = Mathf.FloorToInt( damage * 1.5f); // the additional bonus damage

            //CRIT?
            if (Random.Range(1, 100) < 10 - npc.AC + npc.dex - player.__dexterity)
            {
                //manager.UpdateMessages($"<color=green>We crit, chance = 5 + {roamingNpcScript.dex} - {playerStats.__dexterity}</color>");
                damage += Mathf.FloorToInt((Random.Range(1, 4) + Mathf.FloorToInt(player.__strength / 5)) * 1.5f);
                //manager.UpdateMessages($"<color=green>You attacked for {damage} (d4 + ({playerStats.__strength} / 5) * 1.5)</color>");

            }
            else
            {
                damage += Random.Range(1, 4) + Mathf.FloorToInt(player.__strength / 5);
                //manager.UpdateMessages($"<color=green>You attacked for {damage} (d4 + {playerStats.__strength} / 5)</color>");
            }

            if (npc.sleeping)
            {
                PlayerMovement.playerMovement.WakeUpEnemy(npc);
                npc.TakeDamage(Mathf.FloorToInt(damage * PlayerMovement.playerMovement.sleepingDamage));
                GameManager.manager.UpdateMessages($"You dealt <color=red>{damage}</color> damage to <color=#{ColorUtility.ToHtmlStringRGB(npc.EnemyColor)}>{npc.EnemyName}</color>");
            }
            else
            {
                npc._x = npc.howLongWillFololwInvisiblepLayer;
                npc.TakeDamage(damage);
                GameManager.manager.UpdateMessages($"You dealt <color=red>{damage}</color> damage to <color=#{ColorUtility.ToHtmlStringRGB(npc.EnemyColor)}>{npc.EnemyName}</color>");
            }
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            GameManager.manager.UpdateMessages("You missed!");
            PlayerMovement.playerMovement.WakeUpEnemy(npc);
        }
    }


    public bool CheckWeaponType(WeaponsSO weapon)
    {
        foreach (var item in ValidWeapons)
        {
            if (item== WeaponsSO.weaponType.melee)
            {
                if (weapon == null)
                {
                    return true;
                }
            }
            if (weapon._weaponType==item)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsValidTarget()
    {
        switch (Effect)
        {
            case SkillEffect.HeavySwing:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
            default:
                return true;
        }
    }

    public bool AllowTargetingMove()
    {
        switch (Effect)
        {
            case SkillEffect.HeavySwing:
                return true;
            default:
                return true;
        }
    }
}
