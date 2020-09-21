using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName ="Skill")]
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
        ShootArrow,
        ShootPoisonArrow,
        ShootHeavyArrow,
        ShootPiercingArrow,

        // Throwables
        
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

                //------------
            case SkillEffect.ShootArrow:
                return GameManager.manager.playerStats.itemsInEqGO.Any(i => i.iso.SkillsToLearn.Contains(this));
            case SkillEffect.ShootPoisonArrow:
                break;
            case SkillEffect.ShootHeavyArrow:
                break;
            case SkillEffect.ShootPiercingArrow:
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
            case SkillEffect.ShootArrow:
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
            case SkillEffect.ShootPoisonArrow:
                break;
            case SkillEffect.ShootHeavyArrow:
                break;
            case SkillEffect.ShootPiercingArrow:
                break;
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
            case SkillEffect.ShootArrow:
                Targeting.IsTargeting = true;
                break;
            case SkillEffect.PoisonCleanse:
                Activate(player);
                break;
            case SkillEffect.ShootPoisonArrow:
                break;
            case SkillEffect.ShootHeavyArrow:
                break;
            case SkillEffect.ShootPiercingArrow:
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
            case SkillEffect.ShootArrow:
                ActivateShootArrow(player);
                break;
            case SkillEffect.ShootPoisonArrow:
                ActivateShootPoisonArrow(player);
                break;
            case SkillEffect.ShootHeavyArrow:
                ActivateShootHeavyArrow(player);
                break;
            case SkillEffect.ShootPiercingArrow:
                ActivateShootPiercingArrow(player);
                break;
            default:
                break;
        }
    }

    private void ActivateShootPiercingArrow(PlayerStats player)
    {
        throw new NotImplementedException();
    }

    private void ActivateShootHeavyArrow(PlayerStats player)
    {
        throw new NotImplementedException();
    }

    private void ActivateShootPoisonArrow(PlayerStats player)
    {
        throw new NotImplementedException();
    }

    private void ActivateShootArrow(PlayerStats player)
    {
        var enemy = MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy;
        var npc = enemy.GetComponent<RoamingNPC>();
        int roll = UnityEngine.Random.Range(1, 101);
        int range = Mathf.Max(
            Mathf.Abs(
                PlayerMovement.playerMovement.position.x - Targeting.Position.x), 
            Mathf.Abs( 
                PlayerMovement.playerMovement.position.y - Targeting.Position.y));

        int calcRoll;
        calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping,-range);

        if (roll <= 20)
        {
            MissEnemyWakeUp(npc);
            return;
        }

        if (calcRoll > 50 || roll >= 80) //Do we hit?
        {
            int bowDamage = 0;
            if (player._Lhand!=null && player._Lhand.iso is WeaponsSO lw)
            {
                if (lw._weaponType == WeaponsSO.weaponType.bow)
                {
                    bowDamage = lw.BowDamage;
                }
            }
            if (bowDamage == 0&&  player._Rhand != null && player._Rhand.iso is WeaponsSO rw)
            {
                if (rw._weaponType == WeaponsSO.weaponType.bow)
                {
                    bowDamage = rw.BowDamage;
                }
            }
            // dont know where to get this from yet
            int ammoDamage = 4;

            int damage = CalculateRangedDamage(bowDamage,ammoDamage,player.__strength,player.__dexterity,npc.dex,npc.AC,npc.sleeping,range);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivatePoisionCleanse(PlayerStats player)
    {
        player.Bleeding();
        if (player.isPoisoned)
        {
            player.poisonDuration = 0;
        }
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
        if (calcRoll > 50 || roll >= 80) //Do we hit?
        {
            Vector2Int backPos = new Vector2Int(
                Targeting.Position.x - PlayerMovement.playerMovement.position.x,
                Targeting.Position.y - PlayerMovement.playerMovement.position.y);

            PlayerMovement.playerMovement.Move(PlayerMovement.playerMovement.position - backPos);

            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex,0.75f);
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
        if (calcRoll > 50 || roll >= 80) //Do we hit?
        {
            Vector2Int storePos = MapManager.FindFreeSpot();
            Vector2Int enemyPos = npc.__position;
            Vector2Int playerPos = PlayerMovement.playerMovement.position;

            npc.MoveTo(storePos.x, storePos.y);
            PlayerMovement.playerMovement.Move(enemyPos);
            npc.MoveTo(playerPos.x, playerPos.y);

            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex,0.75f);
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
            if (calcRoll>65)
            {
                // TODO: Stun here

            }
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex,1.25f);
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
        if (calcRoll > 50 || roll >= 80) //Do we hit?
        {
            if (calcRoll > 65)
            {
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
        if (calcRoll > 50 || roll >= 80) //Do we hit?
        {
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex,1,1.5f);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }

    private void ActivateArcEdge(PlayerStats player)
    {
        Vector2Int targetPos = new Vector2Int(Targeting.Position.x, Targeting.Position.y);
        Vector2Int playerPos = PlayerMovement.playerMovement.position;
        Vector2Int targetDelta = targetPos - playerPos;

        int maxSingle = Mathf.Abs(playerPos.x - targetPos.x) + Mathf.Abs(playerPos.y - targetPos.y);
        int maxBoth = 2;

        // hor and vert
        Vector2Int d1 = new Vector2Int(-1, -1);
        if (
            !(
            Mathf.Abs(d1.x - targetDelta.x) + Mathf.Abs(d1.y-targetDelta.y) > maxBoth ||
            Mathf.Abs( d1.x - targetDelta.x) > maxSingle || Mathf.Abs(d1.y - targetDelta.y) > maxSingle))
        {
            AttackArcEdge(player, d1 + playerPos);
        }
        Vector2Int d2 = new Vector2Int(-1, 0);
        if (
            !(
            Mathf.Abs(d2.x - targetDelta.x) + Mathf.Abs(d2.y - targetDelta.y) > maxBoth ||
            Mathf.Abs(d2.x - targetDelta.x) > maxSingle || Mathf.Abs(d2.y - targetDelta.y) > maxSingle))
        {
            AttackArcEdge(player, d2 + playerPos);
        }
        Vector2Int d3 = new Vector2Int(-1, +1);
        if (
            !(
            Mathf.Abs(d3.x - targetDelta.x) + Mathf.Abs(d3.y - targetDelta.y) > maxBoth ||
            Mathf.Abs(d3.x - targetDelta.x) > maxSingle || Mathf.Abs(d3.y - targetDelta.y) > maxSingle))
        {
            AttackArcEdge(player, d3 + playerPos);
        }

        Vector2Int d4 = new Vector2Int(0, 1);
        if (
            !(
            Mathf.Abs(d4.x - targetDelta.x) + Mathf.Abs(d4.y - targetDelta.y) > maxBoth ||
            Mathf.Abs(d4.x - targetDelta.x) > maxSingle || Mathf.Abs(d4.y - targetDelta.y) > maxSingle))
        {
            AttackArcEdge(player, d4 + playerPos);
        }
        Vector2Int d5 = new Vector2Int(0, -1);
        if (
            !(
            Mathf.Abs(d5.x - targetDelta.x) + Mathf.Abs(d5.y - targetDelta.y) > maxBoth ||
            Mathf.Abs(d5.x - targetDelta.x) > maxSingle || Mathf.Abs(d5.y - targetDelta.y) > maxSingle))
        {
            AttackArcEdge(player, d5 + playerPos);
        }

        Vector2Int d6 = new Vector2Int(1, -1);
        if (
            !(
            Mathf.Abs(d6.x - targetDelta.x) + Mathf.Abs(d6.y - targetDelta.y) > maxBoth ||
            Mathf.Abs(d6.x - targetDelta.x) > maxSingle || Mathf.Abs(d6.y - targetDelta.y) > maxSingle))
        {
            AttackArcEdge(player, d6 + playerPos);
        }
        Vector2Int d7 = new Vector2Int(1, 0);
        if (
            !(
            Mathf.Abs(d7.x - targetDelta.x) + Mathf.Abs(d7.y - targetDelta.y) > maxBoth ||
            Mathf.Abs(d7.x - targetDelta.x) > maxSingle || Mathf.Abs(d7.y - targetDelta.y) > maxSingle))
        {
            AttackArcEdge(player, d7 + playerPos);
        }
        Vector2Int d8 = new Vector2Int(1, 1);
        if (
            !(
            Mathf.Abs(d8.x - targetDelta.x) + Mathf.Abs(d8.y - targetDelta.y) > maxBoth ||
            Mathf.Abs(d8.x - targetDelta.x) > maxSingle || Mathf.Abs(d8.y - targetDelta.y) > maxSingle))
        {
            AttackArcEdge(player, d8 + playerPos);
        }
    }
    private void AttackArcEdge(PlayerStats player, Vector2Int target)
    {
        var enemy = MapManager.map[target.x, target.y].enemy;
        if (enemy==null)
        {
            return;
        }
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
            damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex,0.075f);
            DealDamageToEnemy(npc, damage);
        }
        else //WE MISSED BUT WE WAKE UP ENEMY
        {
            MissEnemyWakeUp(npc);
        }
    }
    
    private void ActivateDashSlash(PlayerStats player)
    {
        if (MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null || !MapManager.map[Targeting.Position.x, Targeting.Position.y].isWalkable)
        {
            return;
        }

        List<Vector2Int> moveLine = LineAlg.GetPointsOnLine(
            PlayerMovement.playerMovement.position.x, 
            PlayerMovement.playerMovement.position.y, 
            Targeting.Position.x, 
            Targeting.Position.y);

        foreach (var pos in moveLine)
        {
            if (pos != Targeting.Position && pos != PlayerMovement.playerMovement.position)
            {
                if (MapManager.map[pos.x,pos.y].enemy != null)
                {
                    var enemy = MapManager.map[pos.x, pos.y].enemy;
                    var npc = enemy.GetComponent<RoamingNPC>();
                    int roll = UnityEngine.Random.Range(1, 101);
                    

                    int calcRoll;
                    calcRoll = CalculateRoll(roll, player.__dexterity, npc.dex, npc.AC, npc.sleeping);

                    if (roll <= 20)
                    {
                        MissEnemyWakeUp(npc);
                        continue;
                    }
                    int damage = 0;
                    if (calcRoll > 60 || roll >= 80) //Do we hit?
                    {
                        damage = CalculateDamage(player._Lhand, player._Rhand, player.__dexterity, player.__strength, npc.AC, npc.dex, 0.5f);
                        DealDamageToEnemy(npc, damage);
                    }
                    else //WE MISSED BUT WE WAKE UP ENEMY
                    {
                        MissEnemyWakeUp(npc);
                    }
                }
            }
        }
        PlayerMovement.playerMovement.Move(new Vector2Int(Targeting.Position.x, Targeting.Position.y));
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
                List<Vector2Int> moveLine = LineAlg.GetPointsOnLine(
                     PlayerMovement.playerMovement.position.x,
                    PlayerMovement.playerMovement.position.y,
                    Targeting.Position.x,
                    Targeting.Position.y);
                foreach (var pos in moveLine)
                {
                    if (pos != Targeting.Position && pos != PlayerMovement.playerMovement.position)
                    {
                        MapManager.map[pos.x, pos.y].decoy = $"<color=red>x</color>";
                    }
                }
                break;
            case SkillEffect.ArcEdge:
                Vector2Int targetPos = new Vector2Int(Targeting.Position.x, Targeting.Position.y);
                Vector2Int playerPos = PlayerMovement.playerMovement.position;
                Vector2Int targetDelta = targetPos - playerPos;

                int maxSingle = Mathf.Abs(playerPos.x - targetPos.x) + Mathf.Abs(playerPos.y - targetPos.y);
                int maxBoth = 2;

                // hor and vert
                Vector2Int d1 = new Vector2Int(-1, -1);
                if (
                    !(
                    Mathf.Abs(d1.x - targetDelta.x) + Mathf.Abs(d1.y - targetDelta.y) > maxBoth ||
                    Mathf.Abs(d1.x - targetDelta.x) > maxSingle || Mathf.Abs(d1.y - targetDelta.y) > maxSingle))
                {
                    if(d1 + playerPos != targetPos)
                        MapManager.map[d1.x + playerPos.x, d1.y + playerPos.y].decoy = $"<color=red>x</color>";
                }
                Vector2Int d2 = new Vector2Int(-1, 0);
                if (
                    !(
                    Mathf.Abs(d2.x - targetDelta.x) + Mathf.Abs(d2.y - targetDelta.y) > maxBoth ||
                    Mathf.Abs(d2.x - targetDelta.x) > maxSingle || Mathf.Abs(d2.y - targetDelta.y) > maxSingle))
                {
                    if (d2 + playerPos != targetPos)
                        MapManager.map[d2.x + playerPos.x, d2.y + playerPos.y].decoy = $"<color=red>x</color>";
                }
                Vector2Int d3 = new Vector2Int(-1, +1);
                if (
                    !(
                    Mathf.Abs(d3.x - targetDelta.x) + Mathf.Abs(d3.y - targetDelta.y) > maxBoth ||
                    Mathf.Abs(d3.x - targetDelta.x) > maxSingle || Mathf.Abs(d3.y - targetDelta.y) > maxSingle))
                {
                    if (d3 + playerPos != targetPos)
                        MapManager.map[d3.x + playerPos.x, d3.y + playerPos.y].decoy = $"<color=red>x</color>";
                }

                Vector2Int d4 = new Vector2Int(0, 1);
                if (
                    !(
                    Mathf.Abs(d4.x - targetDelta.x) + Mathf.Abs(d4.y - targetDelta.y) > maxBoth ||
                    Mathf.Abs(d4.x - targetDelta.x) > maxSingle || Mathf.Abs(d4.y - targetDelta.y) > maxSingle))
                {
                    if (d4 + playerPos != targetPos)
                        MapManager.map[d4.x + playerPos.x, d4.y + playerPos.y].decoy = $"<color=red>x</color>";
                }
                Vector2Int d5 = new Vector2Int(0, -1);
                if (
                    !(
                    Mathf.Abs(d5.x - targetDelta.x) + Mathf.Abs(d5.y - targetDelta.y) > maxBoth ||
                    Mathf.Abs(d5.x - targetDelta.x) > maxSingle || Mathf.Abs(d5.y - targetDelta.y) > maxSingle))
                {
                    if (d5 + playerPos != targetPos)
                        MapManager.map[d5.x + playerPos.x, d5.y + playerPos.y].decoy = $"<color=red>x</color>";
                }

                Vector2Int d6 = new Vector2Int(1, -1);
                if (
                    !(
                    Mathf.Abs(d6.x - targetDelta.x) + Mathf.Abs(d6.y - targetDelta.y) > maxBoth ||
                    Mathf.Abs(d6.x - targetDelta.x) > maxSingle || Mathf.Abs(d6.y - targetDelta.y) > maxSingle))
                {
                    if (d6 + playerPos != targetPos)
                        MapManager.map[d6.x + playerPos.x, d6.y + playerPos.y].decoy = $"<color=red>x</color>";
                }
                Vector2Int d7 = new Vector2Int(1, 0);
                if (
                    !(
                    Mathf.Abs(d7.x - targetDelta.x) + Mathf.Abs(d7.y - targetDelta.y) > maxBoth ||
                    Mathf.Abs(d7.x - targetDelta.x) > maxSingle || Mathf.Abs(d7.y - targetDelta.y) > maxSingle))
                {
                    if (d7 + playerPos != targetPos)
                        MapManager.map[d7.x + playerPos.x, d7.y + playerPos.y].decoy = $"<color=red>x</color>";
                }
                Vector2Int d8 = new Vector2Int(1, 1);
                if (
                    !(
                    Mathf.Abs(d8.x - targetDelta.x) + Mathf.Abs(d8.y - targetDelta.y) > maxBoth ||
                    Mathf.Abs(d8.x - targetDelta.x) > maxSingle || Mathf.Abs(d8.y - targetDelta.y) > maxSingle))
                {
                    if (d8 + playerPos != targetPos)
                        MapManager.map[d8.x + playerPos.x, d8.y + playerPos.y].decoy = $"<color=red>x</color>";
                }
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
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
            case SkillEffect.Divide:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
            case SkillEffect.Sweep:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
            case SkillEffect.JumpSmash:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
            case SkillEffect.FlatBash:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
            case SkillEffect.Sever:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
            case SkillEffect.DashSlash:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy == null && MapManager.map[Targeting.Position.x, Targeting.Position.y].isWalkable;
            case SkillEffect.ArcEdge:
                return Targeting.Position != PlayerMovement.playerMovement.position;
            case SkillEffect.ShadowFang:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
            case SkillEffect.VampireBite:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null && MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy.GetComponent<RoamingNPC>().sleeping;
            case SkillEffect.NutCracker:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
            case SkillEffect.HookSwitch:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
            case SkillEffect.BackingJavelin:
                return MapManager.map[Targeting.Position.x, Targeting.Position.y].enemy != null;
            case SkillEffect.PoisonCleanse:
                return true;
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
                return true;
            case SkillEffect.Divide:
                return true;
            case SkillEffect.Sweep:
                return true;
            case SkillEffect.JumpSmash:
                return true;
            case SkillEffect.FlatBash:
                return true;
            case SkillEffect.Sever:
                return true;
            case SkillEffect.DashSlash:
                return true;
            case SkillEffect.ArcEdge:
                return true;
            case SkillEffect.ShadowFang:
                return true;
            case SkillEffect.VampireBite:
                return true;
            case SkillEffect.NutCracker:
                return true;
            case SkillEffect.HookSwitch:
                return true;
            case SkillEffect.BackingJavelin:
                return true;
            case SkillEffect.PoisonCleanse:
                return true;
            default:
                return true;
        }
        return true;
    }








    private static int CalculateRoll(int roll, int playerDexterity, int npcDex, int npcAC, bool enemySleepig, int additionalChange = 0)
    {
        int calcRoll;
        if (enemySleepig)
        {
            calcRoll = roll + playerDexterity - npcDex / 4 - npcAC - additionalChange;
        }
        else
        {
            calcRoll = roll + playerDexterity - npcDex - npcAC - additionalChange;
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

    private int CalculateDamage(Item pLhand, Item pRhand, int pDex, int pStrength, int npcAC, int npcDex, float damageAmplification = 1, float critChanceAmp = 1)
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
        if (UnityEngine.Random.Range(1, 100) < (10 - npcAC + npcDex - pDex)*critChanceAmp)
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
    private int CalculateRangedDamage(int weaponDamage, int ammoDamage, int pStrength, int pDex, int npcDex, int npcAC, bool npcSleep, int range)
    {
        int r = UnityEngine.Random.Range(1, 101);

        if (r <= 10 + Mathf.Max(0,pDex-(npcSleep?npcDex/4: npcDex)))
        {
            return Mathf.FloorToInt((weaponDamage + ammoDamage + pStrength / 5) - range * 1.5f)-npcAC;
        }
        else
        {
            return (weaponDamage + ammoDamage + pStrength / 5) - range - npcAC;
        }
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
