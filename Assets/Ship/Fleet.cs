using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Fleet
{
    public List<Ship> ships = new List<Ship>();

    // primarily planet system, but can be any
    public GameObject owner;
}