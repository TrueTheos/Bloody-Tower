using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corps : Structure
{
    public EnemiesScriptableObject enemyBody;
    public ItemScriptableObject itemInCorpse;

    public override void Use()
    {
        //eat or take item
    }

    public override void WalkIntoTrigger()
    {
        
    }
}
