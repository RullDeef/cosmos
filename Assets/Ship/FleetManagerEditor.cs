using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FleetManager))]
public class FleetManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        FleetManager fleetManager = (FleetManager)target;
        DrawDefaultInspector();

        // fleetManager.shipsAmount = EditorGUILayout.IntField("Amount of ships", fleetManager.shipsAmount);

        if (GUILayout.Button("add fleet to planet system"))
        {
            if (fleetManager.selectedSystem == null)
            {
                Debug.Log("First select system to add fleet to!");
            }
            else
            {
                fleetManager.CreateNewFleet(fleetManager.shipsAmount);
                Debug.Log("Added new fleet!");
            }
        }

        if (GUILayout.Button("remove all fleets"))
        {
            fleetManager.RemoveAllFleets();
            Debug.Log("All fleets have been destroyed!");
        }
    }
}
