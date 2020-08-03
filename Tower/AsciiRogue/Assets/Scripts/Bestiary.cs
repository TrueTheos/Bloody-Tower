using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Bestiary : MonoBehaviour
{
    public Canvas canvas;

    public Text enemyName;
    public Text enemySymbol;

    public Text enemyStr;
    public Text enemyDex;
    public Text enemyInt;
    public Text enemyEnd;
    public Text enemyLevel;
    public Text enemyExp;
    //public Text enemyAC;

    public List<EnemiesScriptableObject> savedEnemies = new List<EnemiesScriptableObject>();

    private bool isEnabled;

    public void UpdateText(EnemiesScriptableObject enemy)
    {
        /*if (!EnemyKnown(enemy))
        { 
            enemyName.color = enemy.E_color;
            enemyName.text = enemy.E_name;

            enemySymbol.color = enemy.E_color;
            enemySymbol.text = enemy.E_symbol;

            enemyStr.text = "???";
            enemyDex.text = "???";
            enemyInt.text = "???";
            enemyEnd.text = "???";
            enemyLevel.text = "???";
            enemyExp.text = "???";

            canvas.enabled = true;
            isEnabled = true;
        }
        else
        {
            enemyName.color = enemy.E_color;
            enemyName.text = enemy.E_name;

            enemySymbol.color = enemy.E_color;
            enemySymbol.text = enemy.E_symbol;

            enemyStr.text = enemy.E_str.ToString();
            enemyDex.text = enemy.E_dex.ToString();
            enemyInt.text = enemy.E_int.ToString();
            enemyEnd.text = enemy.E_end.ToString();
            enemyLevel.text = enemy.E_lvl.ToString();
            enemyExp.text = enemy.E_xpAfterKilling.ToString();
            //enemyAC.text = enemy.E_armorClass.ToString();

            canvas.enabled = true;
            isEnabled = true;
        }  */     
    }

    public void UpdateEnemyList(EnemiesScriptableObject enemy)
    {
        if (!EnemyKnown(enemy))
        {
            savedEnemies.Add(enemy);
        }
    }

    private bool EnemyKnown(EnemiesScriptableObject enemy)
    {
        for (int i = 0; i < savedEnemies.Count; i++)
        {
            if (savedEnemies[i] == enemy)
            {
                return true;
            }
        }
        return false;
    }

    public void Update()
    {
        if(isEnabled && Input.GetKeyUp(KeyCode.Escape))
        {
            isEnabled = false;
            canvas.enabled = false;
        }
    }
}
