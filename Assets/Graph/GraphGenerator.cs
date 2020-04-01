using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(PlanetSystemGenerator))]
public class GraphGenerator : MonoBehaviour
{
    /**
     * \brief   Количество генерируемых систем.
     */
    public int systemsAmount = 10;

    /**
     * \brief   Минимальное расстояние между системами.
     */
    public float systemDistance = 3.0f;

    /**
     * \brief   Prefab для связей между системами.
     */
    public GameObject connectionPrefab;

    /**
     * \brief   Сам низкоуровневый граф.
     */
    public Graph graph;

    private void Start()
    {
        // dont do it
        // Issue: Граф сбрасывается самопроизвольно при старте игры
        // поэтому необходимо вручную генерировать его заново.
        ClearGraph();
        GenerateGraph();
    }

    private void OnDestroy()
    {
        ClearGraph();
    }

    /**
     * \brief   Создаёт ноый граф.
     * 
     * Максимальное расстояние между системами будет рассчитано по формуле:
     *
     *      max = min * (sqrt(2) - 0.1)
     */
    public void GenerateGraph()
    {
        PlanetSystemGenerator planetSystemGenerator = GetComponent<PlanetSystemGenerator>();

        graph = new Graph();

        // choose min and max distances to place nodes
        float maxDistance = systemDistance * (Mathf.Sqrt(2.0f) - 0.1f);

        // generate first node
        graph.nodes = new List<PlanetSystem>() { planetSystemGenerator.GeneratePlanetSystem() };

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
                PlanetSystem system = planetSystemGenerator.GeneratePlanetSystem();
                // assign name to system
                system.gameObject.name += $" {i}";
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
            for (int j = 0; j < i; j++)
                if (graph.connections[i, j])
                    InstantiateConnection(i, j);
    }

    /**
     * \brief   Удаляет все созданные объекты и сбрасывает граф.
     */
    public void ClearGraph()
    {
        // destroy all child objects
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        graph = null;
    }

    /**
     * \brief   Создаёт связь между системами с переданными индексами.
     * 
     * Используется только в методе GenerateSystemGraph.
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
