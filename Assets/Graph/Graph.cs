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
            Vector3 pos = node.transform.position;
            topLeftFront.x = Mathf.Max(topLeftFront.x, pos.x);
            topLeftFront.y = Mathf.Max(topLeftFront.y, pos.y);
            topLeftFront.z = Mathf.Max(topLeftFront.z, pos.z);
            botRightBack.x = Mathf.Min(botRightBack.x, pos.x);
            botRightBack.y = Mathf.Min(botRightBack.y, pos.y);
            botRightBack.z = Mathf.Min(botRightBack.z, pos.z);
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

        return connections[i, j];
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
            if (connections[i, j])
                neighbors.Add(nodes[j]);
        }

        return neighbors;
    }

    /**
     * Shifts all nodes so that the center
     * of the entire graph is located at zero.
     * 
     * Used in Generate method only.
     */
    public void NormalizeLocation()
    {
        Vector3 center = Vector3.zero;

        foreach(PlanetSystem node in nodes)
            center += node.transform.position;

        center /= nodes.Count;

        foreach(PlanetSystem node in nodes)
            node.transform.position -= center;
    }
}
