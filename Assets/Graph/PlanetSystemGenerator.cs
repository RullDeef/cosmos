using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * \brief   Monobehaviour.
 *          Planet system spawner.
 */
public class PlanetSystemGenerator : MonoBehaviour
{
    /**
     * Amount of planets in one system.
     */
    public int planetsAmount = 3;

    /**
     * Planet orbits radius step.
     */
    public float planetOrbitStep = 0.4f;

    /**
     * Prefabs for planets and systems.
     */
    public GameObject planetPrefab;
    public GameObject systemPrefab;

    /**
     *  Randomly generates new system with given planets amount.
     */
    public PlanetSystem GeneratePlanetSystem()
    {
        GameObject instance;
        PlanetSystem system;

        instance = Instantiate(systemPrefab, transform);

        system = instance.GetComponent<PlanetSystem>();

        // generate planets as children
        for (int i = 0; i < planetsAmount; i++)
        {
            float radius = (i + 1) * planetOrbitStep;
            float angle = Random.Range(0, 360);

            Matrix4x4 rotation = Matrix4x4.Rotate(Quaternion.Euler(0, 0, angle));
            Vector3 planetPosition = new Vector3(radius, 0, 0);
            planetPosition = rotation.MultiplyPoint(planetPosition);

            GameObject planet = Instantiate(planetPrefab, system.transform);
            planet.transform.position = planetPosition;

            system.planets.Add(planet.GetComponent<Planet>());
        }

        return system;
    }
}
