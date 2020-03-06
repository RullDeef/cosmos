/**
 * \file    Planet.cs
 * \brief   File with Planet definition.
 */
using UnityEngine;

/**
 * \brief   Basic class for Planet logic.
 */
public class Planet : MonoBehaviour
{
    public Vector3 position
    {
        get { return transform.position; }
    }

    public int banksAmount = 1;
    public int factoryAmount = 1;
}