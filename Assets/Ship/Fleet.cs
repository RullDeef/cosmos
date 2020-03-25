using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fleet
{
    public List<Ship> ships = new List<Ship>();

    // primarily planet system, but can be any
    private GameObject _owner;
    
    public GameObject GetOwner()
    {
        if (_owner == null)
            Debug.Log("Trying to require owner when it is unassigned!");
        return _owner;
    }

    public void SetOwner(GameObject obj, float orbitRadius)
    {
        _owner = obj;
        Transform ownerTrasnform = obj.transform;
        foreach (Ship ship in ships)
        {
            // set target pos
            ship.targetPosition = ownerTrasnform.position;
            ship.orbitRadius = orbitRadius;
        }
    }
}