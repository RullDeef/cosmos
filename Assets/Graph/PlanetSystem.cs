using System.Collections.Generic;
using UnityEngine;


public class PlanetSystem : MonoBehaviour
{
    public Vector3 position
    {
        get { return transform.position; }
    }

    public List<Planet> planets;
}