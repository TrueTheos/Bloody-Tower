using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ChanceTable<>))]
public class DropTableDrawer : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        var inst = property.FindPropertyRelative("Instances");
        var weight = property.FindPropertyRelative("Weights");

        int count = inst.arraySize;

        GUIStyle btnStyle = new GUIStyle();
        btnStyle.fixedWidth = 15;
        



    }



}
