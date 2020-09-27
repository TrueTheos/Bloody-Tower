using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChanceTable<T>
{
    [SerializeField]
    List<T> Instances = new List<T>();
    [SerializeField]
    List<int> Weights = new List<int>();


    public ChanceTable()
    {

    }



    public T GetValue()
    {
        int sum = 0;
        foreach (var item in Weights)
        {
            sum += item;
        }

        int res = Random.Range(1, sum );

        int subTotal = 0;
        for (int i = 0; i < Instances.Count; i++)
        {
            subTotal += Weights[i];
            if (res < subTotal)
            {
                return Instances[i];
            }
        }
        return default;
    }


}
