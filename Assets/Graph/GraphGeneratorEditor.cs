using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GraphGenerator))]
public class GraphGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("generate planet graph"))
        {
            if (((GraphGenerator)target).graph != null)
                ((GraphGenerator)target).ClearGraph();
            ((GraphGenerator)target).GenerateGraph();
            Debug.Log("Generated!");
        }

        if (GUILayout.Button("clear planet graph"))
        {
            ((GraphGenerator)target).ClearGraph();
            Debug.Log("Cleared!");
        }
    }
}
