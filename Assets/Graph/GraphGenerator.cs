/**
 * \file    GraphGenerator.cs
 * \brief   File with GraphGenerator definition.
 */
using System.Collections.Generic;
using UnityEngine;

/**
 * \brief   Monobehaviour.
 *          Graph generator and holder.
 */
[ExecuteAlways]
public class GraphGenerator : MonoBehaviour
{
    /**
     * Amount of planet systems to generate.
     */
    public int systemsAmount = 10;

    /**
     * Minimum distance between two systems.
     */
    public float systemDistance = 3.0f;

    /**
     * Amount of planets in one system.
     */
    public int planetsAmount = 3;

    /**
     * Planet orbits radius step.
     */
    public float planetOrbitStep = 0.4f;

    /**
     * Prefabs for planets, systems and connections between systems.
     */
    public GameObject planetPrefab;
    public GameObject systemPrefab;
    public GameObject connectionPrefab;

    /**
     * Actual low-level graph object.
     */
    public Graph graph;

    private void Start()
    {
        ClearGraph();
        GenerateGraph();
    }

    private void OnDestroy()
    {
        ClearGraph();
    }

    /**
     *  Generates new Graph.
     * 
     *  Maximum distance will be calculated from minimum as follows:
     *      max = min * (sqrt(2) - 0.1)
     */
    public void GenerateGraph()
    {
        graph = new Graph();

        // choose min and max distances to place nodes
        float maxDistance = systemDistance * (Mathf.Sqrt(2.0f) - 0.1f);

        // generate first node
        graph.nodes = new List<PlanetSystem>() { GeneratePlanetSystem(planetsAmount) };

        for (int i = 1; i < systemsAmount; i++)
        {
            // choose random position
            Vector3 pos = Random.onUnitSphere;
            pos = pos.normalized * (graph.GetRadius() + maxDistance);

            // check distance to closest node
            float closestDist = Mathf.Infinity, currentDist;
            for (int j = 0; j < i; j++)
            {
                currentDist = (graph[j].transform.position - pos).magnitude;
                closestDist = Mathf.Min(closestDist, currentDist);
            }

            // compare it with lower and upper bounds
            if (systemDistance < closestDist && closestDist < maxDistance)
            {
                PlanetSystem system = GeneratePlanetSystem(planetsAmount);
                system.transform.position = pos;
                graph.nodes.Add(system);
            }
            else
            {
                i--;
            }
        }

        // generate connections between nodes that are close enough
        graph.connections = new bool[systemsAmount, systemsAmount];

        for (int i = 1; i < systemsAmount; i++)
        {
            for (int j = 0; j < i; j++)
            {
                float currentDist = (graph[i].transform.position - graph[j].transform.position).magnitude;
                bool connected = currentDist < maxDistance;
                graph.connections[i, j] = connected;
                graph.connections[j, i] = connected;
            }
        }

        graph.NormalizeLocation();

        // instantiate connections between systems
        for (int i = 1; i < graph.nodes.Count; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (graph.connections[i, j])
                {
                    InstantiateConnection(i, j);
                }
            }
        }
    }

    /**
     *  Randomly generates new system with given planets amount.
     */
    public PlanetSystem GeneratePlanetSystem(int planetsAmount)
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

    /**
     * Destroys all spawned pbjects and resets graph to null.
     */
    public void ClearGraph()
    {
        // destroy all child objects
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        graph = null;
    }

    /**
     * Creates new PlanetObject and asociates it with given planet.
     * 
     * Used only in GenerateSystemGraph method.
     */
    /*
    private void InstantiatePlanet(Planet planet)
    {
        Instantiate(planetPrefab, planet.position, Quaternion.identity,
            transform).GetComponent<PlanetObject>().planet = planet;
    }
    */

    /**
     * Creates new Connection object that links
     * two planet systems with given indices.
     * 
     * Used only in GenerateSystemGraph method.
     */
    private void InstantiateConnection(int i, int j)
    {
        PlanetSystem from = graph.nodes[i];
        PlanetSystem to = graph.nodes[j];

        LineRenderer connection = Instantiate(connectionPrefab,
            from.transform.position, Quaternion.identity,
                transform).GetComponent<LineRenderer>();

        Vector3 delta = to.transform.position - from.transform.position;

        for (int t = 1; t <= 10; t++)
            connection.SetPosition(t, t / 10.0f * delta);
    }
}
