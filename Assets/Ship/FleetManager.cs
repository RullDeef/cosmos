using System.Collections.Generic;
using UnityEngine;

public class FleetManager : MonoBehaviour
{
    public Ship shipPrefab;

    public PlanetSystem selectedSystem;

    public Fleet selectedFleet;

    public List<Fleet> fleets = new List<Fleet>();

    public int shipsAmount;

    public Fleet CreateNewFleet(int shipsAmount)
    {
        Fleet fleet = new Fleet() { owner = selectedSystem.gameObject };
        Transform ownerTransform = selectedSystem.transform;

        // generate new ships using loop
        for (int i = 0; i < shipsAmount; i++)
        {
            // place new ship correctly
            float radius = GameObject.FindObjectOfType<PlanetSystemGenerator>().GetShipOrbitRadius();
            Vector3 direction = Random.onUnitSphere;
            Vector3 position = ownerTransform.position + radius * direction;

            Quaternion rotation = Quaternion.FromToRotation(Vector3.right, direction);
            
            Transform shipTransform = Instantiate(shipPrefab, position, rotation, transform).transform;
            Ship ship = shipTransform.GetComponent<Ship>();
            ship.target = fleet.owner.transform;
            
            fleet.ships.Add(ship);
            Debug.Log("New ship generated!");
        }

        fleets.Add(fleet);
        return fleet;
    }

    public void RemoveAllFleets()
    {
        try
        {
            while (transform.childCount != 0)
            {
                Transform childTransform = transform.GetChild(0);
                if (Application.isEditor)
                    DestroyImmediate(childTransform.gameObject);
                else
                    Destroy(childTransform.gameObject);
            }
        }
        catch
        {
            Debug.Log("Seems like you have deleted ships manually! All ok");
        }

        fleets.Clear();
    }

    public void SendFleet()
    {

    }
}
