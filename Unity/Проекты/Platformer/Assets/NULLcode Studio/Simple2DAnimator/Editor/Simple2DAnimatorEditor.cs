/********************************************************/
/**  © 2020 NULLcode Studio. All Rights Reserved.
/**  Разработано в рамках проекта: https://null-code.ru
/**  Поддержать нас: https://boosty.to/null-code
/********************************************************/
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Simple2DAnimatorController))]
public class Simple2DAnimatorEditor : Editor
{
    GUIStyle Style()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.normal.textColor = Color.white;
        style.fixedWidth = 200;
        style.fixedHeight = 50;
        return style;
    }

    public override void OnInspectorGUI()
    {
        Simple2DAnimatorController t = (Simple2DAnimatorController)target;
        EditorGUILayout.Separator();
        GUI.backgroundColor = Color.magenta;
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Открыть в редакторе", Style()))
        {
            Simple2DAnimatorControllerEditor.OpenWindow(t);
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        EditorGUILayout.Separator();
        GUI.backgroundColor = Color.white;
        DrawDefaultInspector();
    }
}
