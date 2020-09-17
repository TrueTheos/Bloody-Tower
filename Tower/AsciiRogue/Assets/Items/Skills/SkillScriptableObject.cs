using System;
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
        // melee technics
        HeavySwing,
        Thrust,
        Divide,
        Sweep,
        JumpSmash,
        FlatBash,
        Sever,
        DashSlash,
        ArcEdge,
        ShadowFang,
        VampireBite,
        NutCracker,
        HookSwitch,
        BackingJavelin,
        PoisonCleanse,

        // ranged skills

        
    }

    public SkillEffect Effect;

    ///<summary> if the Skill should be shown in the window</summary>
    public bool IsShown(PlayerStats player)
    {
        switch (Effect)
        {
            case SkillEffect.HeavySwing:
                break;
            case SkillEffect.Thrust:
                break;
            case SkillEffect.Divide:
                break;
            case SkillEffect.Sweep:
                break;
            case SkillEffect.JumpSmash:
                break;
            case SkillEffect.FlatBash:
                break;
            case SkillEffect.Sever:
                break;
            case SkillEffect.DashSlash:
                break;
            case SkillEffect.ArcEdge:
                break;
            case SkillEffect.ShadowFang:
                break;
            case SkillEffect.VampireBite:
                break;
            case SkillEffect.NutCracker:
                break;
            case SkillEffect.HookSwitch:
                break;
            case SkillEffect.BackingJavelin:
                break;
            case SkillEffect.PoisonCleanse:
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
            case SkillEffect.Thrust:
            case SkillEffect.Divide:
            case SkillEffect.Sweep:
            case SkillEffect.JumpSmash:
            case SkillEffect.FlatBash:
            case SkillEffect.Sever:
            case SkillEffect.DashSlash:
            case SkillEffect.ArcEdge:
            case SkillEffect.ShadowFang:
            case SkillEffect.VampireBite:
            case SkillEffect.NutCracker:
            case SkillEffect.HookSwitch:
            case SkillEffect.BackingJavelin:
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
            case SkillEffect.PoisonCleanse:
                return player.isPoisoned;
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
            case SkillEffect.Thrust:
            case SkillEffect.Divide:
            case SkillEffect.Sweep:
            case SkillEffect.JumpSmash:
            case SkillEffect.FlatBash:
            case SkillEffect.Sever:
            case SkillEffect.DashSlash:
            case SkillEffect.ArcEdge:
            case SkillEffect.ShadowFang:
            case SkillEffect.VampireBite:
            case SkillEffect.NutCracker:
            case SkillEffect.HookSwitch:
            case SkillEffect.BackingJavelin:            
                Targeting.IsTargeting = true;
                break;
            case SkillEffect.PoisonCleanse:
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
            case SkillEffect.Thrust:
                ActivateThrust(player);
                break;
            case SkillEffect.Divide:
                ActivateDivide(player);
                break;
            case SkillEffect.Sweep:
                ActivateSweep(player);
                break;
            case SkillEffect.JumpSmash:
                ActivateJumpSmash(player);
                break;
            case SkillEffect.FlatBash:
                ActivateFlatBash(player);
                break;
            case SkillEffect.Sever:
                ActivateSever(player);
                break;
            case SkillEffect.DashSlash:
                ActivateDashSlash(player);
                break;
            case SkillEffect.ArcEdge:
                ActivateArcEdge(player);
                break;
            case SkillEffect.ShadowFang:
                ActivateShadowFang(player);
                break;
            case SkillEffect.VampireBite:
                ActivateVampireBite(player);
                break;
            case SkillEffect.NutCracker:
                ActivateNutCracker(player);
                break;
            case SkillEffect.HookSwitch:
                ActivateHookSwitch(player);
                break;
            case SkillEffect.BackingJavelin:
                ActivateBackingJavelin(player);
                break;
            case SkillEffect.PoisonCleanse:
                ActivatePoisionCleanse(player);
                break;
            default:
                break;
        }
    }

    private void ActivatePoisionCleanse(PlayerStats player)
    {
        
    }

    private void ActivateBackingJavelin(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 60 || roll >= 80) //Do we hit?
        {
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateHookSwitch(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 60 || roll >= 80) //Do we hit?
        {
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateNutCracker(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 60 || roll >= 80) //Do we hit?
        {
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateVampireBite(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 60 || roll >= 80) //Do we hit?
        {
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateShadowFang(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 60 || roll >= 80) //Do we hit?
        {
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateArcEdge(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 60 || roll >= 80) //Do we hit?
        {
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateDashSlash(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        if (MapManager.TryFindClosestPositionTowards(PlayerMovement.playerMovement.position,npc.__position,out Vector2Int outpos))
        {
            // we teleport to the spot
            PlayerMovement.playerMovement.Move(outpos);
        }
        else
        {
            return;
        }


        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 60 || roll >= 80) //Do we hit?
        {
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex,0.5f);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateSever(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 50 || roll >= 80) //Do we hit?
        {
            if (calcRoll>65)
            {
                // TODO: generate blood
                player.__blood += UnityEngine.Random.Range(1, 21);
            }
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateFlatBash(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 50 || roll >= 80) //Do we hit?
        {
            if (calcRoll>65)
            {
                // TODO: stun enemy

            }
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex,0.75f);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateJumpSmash(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 60 || roll >= 80) //Do we hit?
        {
            if (calcRoll> 65)
            {
                // TODO: stun the enemy

            }
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex,1.25f);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateSweep(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 50 || roll >= 80) //Do we hit?
        {
            if (calcRoll> 65)
            {
                // TODO: slow the enemy
                
            }
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex,0.75f);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateDivide(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 60 || roll >= 80) //Do we hit?
        {
            if (calcRoll>65 && npc.__currentHp<=5)
            {
                DealDamageToEnemy(npc, 1000);
                // TODO: two corpses and the blood

            }
            else
            {
                damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex, 1.25f);
                DealDamageToEnemy(npc, damage);
            }            
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateHeavySwing(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll,player.__dexterity,npc.dex,npc.AC,npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 60 || roll >= 80) //Do we hit?
        {
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex,1.25f);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateThrust(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }
        int damage = 0;
        if (calcRoll > 50 || roll >= 80) //Do we hit?
        {
            damage = CalculateDamage(player._Lhand,player._Rhand,player.__dexterity,player.__strength,npc.AC-1,npc.dex,0.75f);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }



    public void DrawPreperation()
    {
        switch (Effect)
        {
            case SkillEffect.HeavySwing:
                // we dont have anything to draw
                break;
            case SkillEffect.Thrust:
                break;
            case SkillEffect.Divide:
                break;
            case SkillEffect.Sweep:
                break;
            case SkillEffect.JumpSmash:
                break;
            case SkillEffect.FlatBash:
                break;
            case SkillEffect.Sever:
                break;
            case SkillEffect.DashSlash:
                break;
            case SkillEffect.ArcEdge:
                break;
            case SkillEffect.ShadowFang:
                break;
            case SkillEffect.VampireBite:
                break;
            case SkillEffect.NutCracker:
                break;
            case SkillEffect.HookSwitch:
                break;
            case SkillEffect.BackingJavelin:
                break;
            case SkillEffect.PoisonCleanse:
                break;
            default:
                break;
        }
    }

    public bool IsValidTarget()
    {
        switch (Effect)
        {
            case SkillEffect.HeavySwing:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
            case SkillEffect.Thrust:
                break;
            case SkillEffect.Divide:
                break;
            case SkillEffect.Sweep:
                break;
            case SkillEffect.JumpSmash:
                break;
            case SkillEffect.FlatBash:
                break;
            case SkillEffect.Sever:
                break;
            case SkillEffect.DashSlash:
                break;
            case SkillEffect.ArcEdge:
                break;
            case SkillEffect.ShadowFang:
                break;
            case SkillEffect.VampireBite:
                break;
            case SkillEffect.NutCracker:
                break;
            case SkillEffect.HookSwitch:
                break;
            case SkillEffect.BackingJavelin:
                break;
            case SkillEffect.PoisonCleanse:
                break;
            default:
                return true;
        }
        return true;
    }

    public bool AllowTargetingMove()
    {
        switch (Effect)
        {
            case SkillEffect.HeavySwing:
                return true;
            case SkillEffect.Thrust:
                break;
            case SkillEffect.Divide:
                break;
            case SkillEffect.Sweep:
                break;
            case SkillEffect.JumpSmash:
                break;
            case SkillEffect.FlatBash:
                break;
            case SkillEffect.Sever:
                break;
            case SkillEffect.DashSlash:
                break;
            case SkillEffect.ArcEdge:
                break;
            case SkillEffect.ShadowFang:
                break;
            case SkillEffect.VampireBite:
                break;
            case SkillEffect.NutCracker:
                break;
            case SkillEffect.HookSwitch:
                break;
            case SkillEffect.BackingJavelin:
                break;
            case SkillEffect.PoisonCleanse:
                break;
            default:
                return true;
        }
        return true;
    }








    private static int CalculateRoll(int roll, int playerDexterity, int npcDex, int npcAC, bool enemySleepig)
    {
        int calcRoll;
        if (enemySleepig)
        {
            calcRoll = roll + playerDexterity - npcDex / 4 - npcAC;
        }
        else
        {
            calcRoll = roll + playerDexterity - npcDex - npcAC;
        }

        return calcRoll;
    }

    private static void MissEnemyWakeUp(RoamingNPC npc)
    {
        GameManager.manager.UpdateMessages("You missed!");
        PlayerMovement.playerMovement.WakeUpEnemy(npc);
    }

    private static void DealDamageToEnemy(RoamingNPC npc, int damage)
    {
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

    private int CalculateDamage(Item pLhand, Item pRhand, int pDex, int pStrength, int npcAC, int npcDex, float damageAmplification = 1)
    {
        int damage = 0;
        if (pLhand?.iso is WeaponsSO weaponL && CheckWeaponType(weaponL))
        {
            for (int x = 0; x < weaponL.attacks.Count; x++)
            {
                damage += UnityEngine.Random.Range(1, weaponL.attacks[x].y);

                //IF WEAPON IS BLOOD SWORD WE INCREASE ITS DAMAGE
                if (weaponL.I_name == "Bloodsword")
                {
                    weaponL.attacks[0] = new Vector2Int(1, weaponL.attacks[0].y + 1);
                    weaponL.bloodSwordCounter = GameManager.manager.tasks.bloodSwordCooldown;
                }
            }
        }
        if (pRhand?.iso is WeaponsSO weaponR && CheckWeaponType(weaponR))
        {
            for (int x = 0; x < weaponR.attacks.Count; x++)
            {
                damage += UnityEngine.Random.Range(1, weaponR.attacks[x].y);

                //IF WEAPON IS BLOOD SWORD WE INCREASE ITS DAMAGE
                if (weaponR.I_name == "Bloodsword")
                {
                    weaponR.attacks[0] = new Vector2Int(1, weaponR.attacks[0].y + 1);
                    weaponR.bloodSwordCounter = GameManager.manager.tasks.bloodSwordCooldown;
                }
            }
        }

        damage = Mathf.FloorToInt(damage * damageAmplification); // the additional bonus damage

        //CRIT?
        if (UnityEngine.Random.Range(1, 100) < 10 - npcAC + npcDex - pDex)
        {
            //manager.UpdateMessages($"<color=green>We crit, chance = 5 + {roamingNpcScript.dex} - {playerStats.__dexterity}</color>");
            damage += Mathf.FloorToInt((UnityEngine.Random.Range(1, 4) + Mathf.FloorToInt(pStrength / 5)) * 1.5f);
            //manager.UpdateMessages($"<color=green>You attacked for {damage} (d4 + ({playerStats.__strength} / 5) * 1.5)</color>");

        }
        else
        {
            damage += UnityEngine.Random.Range(1, 4) + Mathf.FloorToInt(pStrength / 5);
            //manager.UpdateMessages($"<color=green>You attacked for {damage} (d4 + {playerStats.__strength} / 5)</color>");
        }

        return damage;
    }

    public bool CheckWeaponType(WeaponsSO weapon)
    {
        foreach (var item in ValidWeapons)
        {
            if (item == WeaponsSO.weaponType.melee)
            {
                if (weapon == null)
                {
                    return true;
                }
            }
            if (weapon?._weaponType==item)
            {
                return true;
            }
        }
        return false;
    }


}
