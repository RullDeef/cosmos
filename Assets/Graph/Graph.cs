/**
 * \file    Graph.cs
 * \brief   File with Graph definition.
 */
using System.Collections.Generic;
using UnityEngine;

/**
 * \brief   Core class for any low-level graph manipulations.
 */
public class Graph
{
    /**
     * List of nodes that are exist in graph and connections between
     * them. (graph is bidirectional: connections[i,j] = connections[j,i]).
     */
    public List<PlanetSystem> nodes;
    public bool[,] connections;

    // Retrive node by its index in node list.
    public PlanetSystem this[int index]
    {
        get => nodes[index];
    }

    // Get connection using two indecies.
    public bool this[int i, int j]
    {
        get => connections[i, j];
    }

    /**
     * Returns Vector3 width dimentions of cube
     * which this graph can be fitted in.
     */
    public Vector3 GetSize()
    {
        Vector3 topLeftFront = Vector3.zero;
        Vector3 botRightBack = Vector3.zero;

        foreach(PlanetSystem node in nodes)
        {
            topLeftFront.x = Mathf.Max(topLeftFront.x, node.position.x);
            topLeftFront.y = Mathf.Max(topLeftFront.y, node.position.y);
            topLeftFront.z = Mathf.Max(topLeftFront.z, node.position.z);
            botRightBack.x = Mathf.Min(botRightBack.x, node.position.x);
            botRightBack.y = Mathf.Min(botRightBack.y, node.position.y);
            botRightBack.z = Mathf.Min(botRightBack.z, node.position.z);
        }

        return topLeftFront - botRightBack;
    }

    //TODO: rewrite this method with spherical approach.
    /**
     * Returns a half of max distance between two nodes.
     */
    public float GetRadius()
    {
        return 0.5f * GetSize().magnitude;
    }

    /**
     * Returns true if given nodes are connected and false otherwise.
     */
    public bool AreConnected(PlanetSystem node1, PlanetSystem node2)
    {
        int i = nodes.IndexOf(node1);
        int j = nodes.IndexOf(node2);

        return this[i, j];
    }

    /**
     * Returns list of nodes which are connected to the given one.
     */
    public List<PlanetSystem> GetNeighbors(PlanetSystem node)
    {
        List<PlanetSystem> neighbors = new List<PlanetSystem>();

        int i = nodes.IndexOf(node);
        for (int j = 0; j < nodes.Count; j++)
        {
            if (this[i, j])
                neighbors.Add(this[j]);
        }

        return neighbors;
    }

    /**
     * Shifts all nodes so that the center
     * of the entire graph is located at zero.
     * 
     * Used in Generate method only.
     */
    private void NormalizeLocation()
    {
        Vector3 center = Vector3.zero;

        foreach(PlanetSystem node in nodes)
            center += node.position;

        center /= nodes.Count;

        foreach(PlanetSystem node in nodes)
            node.position -= center;
    }

    /**
     * Generates node graph by given params.
     * 
     * nodesCount - amount of generated nodes.
     * minDistance - minimal distance between two nodes.
     * 
     * Maximum distance will be calculated from minimum as follows:
     *      max = min * (sqrt(2) - 0.1)
     */
    public static Graph Generate(int nodesCount, float minDistance = 32.0f)
    {
        Graph graph = new Graph();

        // choose min and max distances to place nodes
        float maxDistance = minDistance * (Mathf.Sqrt(2.0f) - 0.1f);

        // generate first node
        graph.nodes = new List<PlanetSystem>() { new PlanetSystem() };

        // generate exactly 'nodesCount' nodes
        for(int i = 1; i < nodesCount; i++)
        {
            // choose random position
            Vector3 pos = Random.onUnitSphere;
            pos = pos.normalized * (graph.GetRadius() + maxDistance);

            // check distance to closest node
            float closestDist = Mathf.Infinity, currentDist;
            for(int j = 0; j < i; j++)
            {
                currentDist = (graph[j].position - pos).magnitude;
                closestDist = Mathf.Min(closestDist, currentDist);
            }

            // compare it with lower and upper bounds
            if(minDistance < closestDist && closestDist < maxDistance)
                graph.nodes.Add(new PlanetSystem());
            else i--;
        }

        // generate connections between nodes that are close enough
        graph.connections = new bool[nodesCount, nodesCount];

        for(int i = 1; i < nodesCount; i++)
        for(int j = 0; j < i; j++)
        {
            float currentDist = (graph[i].position - graph[j].position).magnitude;
            bool connected = currentDist < maxDistance;
            graph.connections[i, j] = connected;
            graph.connections[j, i] = connected;
        }

        graph.NormalizeLocation();
        return graph;
    }
}
