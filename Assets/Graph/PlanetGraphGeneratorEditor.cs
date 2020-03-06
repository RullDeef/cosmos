using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GraphGenerator))]
public class PlanetGraphGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if(GUILayout.Button("generate planet graph"))
        {
            if(((GraphGenerator)target).graph != null)
                ((GraphGenerator)target).ClearGraph();
            ((GraphGenerator)target).GeneratePlanetGraph();
            Debug.Log("Generated!");
        }

        if(GUILayout.Button("clear planet graph"))
        {
            ((GraphGenerator)target).ClearGraph();
            Debug.Log("Cleared!");
        }
    }
}
