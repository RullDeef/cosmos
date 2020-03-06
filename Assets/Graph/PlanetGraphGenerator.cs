/**
 * \file    GraphGenerator.cs
 * \brief   File with GraphGenerator definition.
 */
using UnityEngine;

/**
 * \brief   Monobehaviour.
 *          Graph generator and holder.
 */
[ExecuteAlways]
public class GraphGenerator : MonoBehaviour
{
    /**
     * Amount of planets to generate.
     */
    public int amount = 10;

    /**
     * Minimum distance between two planets.
     */
    public float minDistance = 3.0f;

    /**
     * Prefabs for planets and connections between plannets.
     */
    public GameObject planetPrefab, connectionPrefab;

    /**
     * Actual low-level graph object.
     */
    public Graph graph;

    private void Start()
    {
        ClearGraph();
        GeneratePlanetGraph();
    }

    private void OnDestroy()
    {
        ClearGraph();
    }

    /**
     * Generates new Graph.
     */
    public void GeneratePlanetGraph()
    {
        graph = Graph.Generate(amount, minDistance);

        // instantiate planets
        foreach(Planet planet in graph.planets)
            InstantiatePlanet(planet);

        // instantiate connections between them
        for(int i = 1; i < graph.planets.Count; i++)
        {
            for(int j = 0; j < i; j++)
            {
                if (graph.connections[i, j])
                    InstantiateConnection(i, j);
            }
        }
    }

    /**
     * Destroys all Planet pbjects and reset graph to null.
     */
    public void ClearGraph()
    {
        // destroy all child objects
        for(int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        graph = null;
    }

    /**
     * Creates new PlanetObject and asociates it with given planet.
     * 
     * Used only in GeneratePlanetGraph method.
     */
    private void InstantiatePlanet(Planet planet)
    {
        Instantiate(planetPrefab, planet.position, Quaternion.identity,
            transform).GetComponent<PlanetObject>().planet = planet;
    }

    /**
     * Creates new Connection object that links
     * two planets with given indices.
     * 
     * Used only in GeneratePlanetGraph method.
     */
    private void InstantiateConnection(int i, int j)
    {
        Planet planetFrom = graph.planets[i];
        Planet planetTo = graph.planets[j];

        LineRenderer connection = Instantiate(connectionPrefab,
            planetFrom. position, Quaternion.identity,
                transform).GetComponent<LineRenderer>();

        Vector3 delta = planetTo.position - planetFrom.position;
        connection.SetPosition(1, delta);
    }
}
