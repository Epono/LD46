using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class ScriptableObjectsHelper : EditorWindow
{
    [MenuItem("Window/SO Helper")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ScriptableObjectsHelper));
    }

    private void OnEnable()
    {
        var variablesArray = Resources.LoadAll<FloatVariable>("ScriptableObjects");
        List<FloatVariable> variables = variablesArray.ToList();

        foreach (FloatVariable variable in variables)
        {
            if (variable.name.StartsWith("AgentSimple"))
            {
                agentSimple.Add(variable);
            }
            else if (variable.name.StartsWith("AgentDouble"))
            {
                agentDouble.Add(variable);
            }
            else if (variable.name.StartsWith("Player"))
            {
                player.Add(variable);
            }
            else if (variable.name.StartsWith("TowerSimple"))
            {
                towerSimple.Add(variable);
            }
            else if (variable.name.StartsWith("TowerDouble"))
            {
                towerDouble.Add(variable);
            }
            else
            {
                others.Add(variable);
            }
        }
    }

    List<FloatVariable> agentSimple = new List<FloatVariable>();
    List<FloatVariable> agentDouble = new List<FloatVariable>();
    List<FloatVariable> player = new List<FloatVariable>();
    List<FloatVariable> towerSimple = new List<FloatVariable>();
    List<FloatVariable> towerDouble = new List<FloatVariable>();
    List<FloatVariable> others = new List<FloatVariable>();

    void OnGUI()
    {
        GUILayout.Label("Player", EditorStyles.boldLabel);
        foreach (FloatVariable variable in player)
        {
            variable.Value = EditorGUILayout.Slider(variable.name.Replace("Player", ""), variable.Value, 0, 100);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        GUILayout.Label("Agent Simple", EditorStyles.boldLabel);
        foreach (FloatVariable variable in agentSimple)
        {
            variable.Value = EditorGUILayout.Slider(variable.name.Replace("AgentSimple", ""), variable.Value, 0, 100);
        }
        EditorGUILayout.Space();

        GUILayout.Label("Agent Double", EditorStyles.boldLabel);
        foreach (FloatVariable variable in agentDouble)
        {
            variable.Value = EditorGUILayout.Slider(variable.name.Replace("AgentDouble", ""), variable.Value, 0, 100);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        GUILayout.Label("Tower Simple", EditorStyles.boldLabel);
        foreach (FloatVariable variable in towerSimple)
        {
            variable.Value = EditorGUILayout.Slider(variable.name.Replace("TowerSimple", ""), variable.Value, 0, 100);
        }
        EditorGUILayout.Space();

        GUILayout.Label("Tower Double", EditorStyles.boldLabel);
        foreach (FloatVariable variable in towerDouble)
        {
            variable.Value = EditorGUILayout.Slider(variable.name.Replace("TowerDouble", ""), variable.Value, 0, 100);
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();


        GUILayout.Label("Others", EditorStyles.boldLabel);
        foreach (FloatVariable variable in others)
        {
            variable.Value = EditorGUILayout.Slider(variable.name, variable.Value, 0, 100
                );
        }


    }
}
