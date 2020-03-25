using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FleetManager))]
public class FleetManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        FleetManager fleetManager = (FleetManager)target;
        DrawDefaultInspector();

        if (GUILayout.Button("add fleet to planet system"))
        {
            if (fleetManager.CreateNewFleet(fleetManager.shipsAmount) != null)
                Debug.Log("Added new fleet!");
        }

        if (GUILayout.Button("remove all fleets"))
        {
            fleetManager.RemoveAllFleets();
            Debug.Log("All fleets have been destroyed!");
        }
    }
}
