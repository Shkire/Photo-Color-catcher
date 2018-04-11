using System.Collections;
using System.Collections.Generic;

public class GraphType<E>
{
    public List<E> vertices;

    public Dictionary<E,List<E>> adjacentVertices;

    public GraphType()
    {
        vertices = new List<E>();
        adjacentVertices = new Dictionary<E, List<E>>();
    }

    public void AddVertex(E i_element)
    {
        if (!vertices.Contains(i_element))
        {
            vertices.Add(i_element);
            adjacentVertices.Add(i_element, new List<E>());
        }
    }

    public void AddAdjacent(E i_vertex, E i_adjacent)
    {
        if (vertices.Contains(i_vertex) && vertices.Contains(i_adjacent))
        {
            List<E> aux = adjacentVertices[i_vertex];
            if (!aux.Contains(i_adjacent))
                aux.Add(i_adjacent);
            aux = adjacentVertices[i_adjacent];
            if (!aux.Contains(i_vertex))
                aux.Add(i_vertex);
        }
    }

    public bool IsAdjacent(E i_vertex, E i_adjacent)
    {
        if (vertices.Contains(i_vertex) && vertices.Contains(i_adjacent) && adjacentVertices[i_vertex].Contains(i_adjacent))
            return true;
        else
            return false;
    }

    public bool AreConnected(E i_vertex1, E i_vertex2)
    {
        List<E> traveledVertices = new List<E>();
        List<E> activeVertices = new List<E>();
        List<E> aux;
        do
        {
            traveledVertices.Add(activeVertices[0]);
            aux = adjacentVertices[activeVertices[0]];
            activeVertices.RemoveAt(0);
            foreach (E vertex in aux)
            {
                if (!traveledVertices.Contains(vertex) && !activeVertices.Contains(vertex))
                    activeVertices.Add(vertex);
            }
        }
        while (!activeVertices.Contains(i_vertex2) || activeVertices.Count == 0);
        return activeVertices.Contains(i_vertex2);
    }

    public bool IsFullyConnected()
    {
        List<E> traveledVertices = new List<E>();
        List<E> activeVertices = new List<E>();
        activeVertices.Add(vertices[0]);
        List<E> aux;
        do
        {
            traveledVertices.Add(activeVertices[0]);
            aux = adjacentVertices[activeVertices[0]];
            activeVertices.RemoveAt(0);
            foreach (E vertex in aux)
            {
                if (!traveledVertices.Contains(vertex) && !activeVertices.Contains(vertex))
                    activeVertices.Add(vertex);
            }
        }
        while (activeVertices.Count == 0);
        foreach (E vertex in vertices)
        {
            if (!traveledVertices.Contains(vertex))
                return false;
        }
        return true;
    }

    public List<List<E>> GetConnectedVertices()
    {
        List<List<E>> connectedVertex = new List<List<E>>();
        List<E> remainingVertices = new List<E>(vertices);
        List<E> traveledVertices;
        List<E> activeVertices = new List<E>();
        List<E> aux;
        while (remainingVertices.Count != 0)
        {
            traveledVertices = new List<E>();
            activeVertices.Add(remainingVertices[0]);
            do
            {
                traveledVertices.Add(activeVertices[0]);
                aux = adjacentVertices[activeVertices[0]];
                remainingVertices.Remove(activeVertices[0]);
                activeVertices.RemoveAt(0);
                foreach (E vertex in aux)
                {
                    if (!traveledVertices.Contains(vertex) && !activeVertices.Contains(vertex))
                        activeVertices.Add(vertex);
                }
            }
            while (activeVertices.Count == 0);
            connectedVertex.Add(traveledVertices);
        }
        return connectedVertex;
    }
}
