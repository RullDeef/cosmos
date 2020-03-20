using System.Collections.Generic;
using UnityEngine;


public class PlanetSystem : MonoBehaviour
{
    public List<Planet> planets;

    /**
     * Ships counter.
     */
    public int shipsAmount = 10;

    /**
     * Selection logic implementation.
     */
    public bool selected = false;

    public void Select()
    {
        selected = true;
    }

    public void Deselect()
    {
        selected = false;
    }

    public void SwitchSelection()
    {
        if (selected)
            Deselect();
        else
            Select();
    }
}