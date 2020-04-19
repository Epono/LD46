using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonoScript))]
public class ScriptInspector : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        MonoScript ms = target as MonoScript;
        System.Type type = ms.GetClass();
        if (type != null && type.IsSubclassOf(typeof(ScriptableObject)) && !type.IsSubclassOf(typeof(UnityEditor.Editor)))
        {
            if (GUILayout.Button("Create Instance"))
            {
                ScriptableObject asset = ScriptableObject.CreateInstance(type);
                string path = AssetDatabase.GenerateUniqueAssetPath("Assets/" + type.Name + ".asset");
                AssetDatabase.CreateAsset(asset, path);
                EditorGUIUtility.PingObject(asset);
            }
        }
        else
        {
            base.OnInspectorGUI();
        }
    }
}
        