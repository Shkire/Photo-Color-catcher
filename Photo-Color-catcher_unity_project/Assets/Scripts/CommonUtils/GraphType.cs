using System.Collections;
using System.Collections.Generic;

public class GraphType<E>
{
    public List<E> vertexs;

    public Dictionary<E,List<E>> adjacentVertexs;

    public GraphType()
    {
        vertexs = new List<E>();
        adjacentVertexs = new Dictionary<E, List<E>>();
    }

    public void AddVertex(E i_element)
    {
        vertexs.Add(i_element);
        adjacentVertexs.Add(i_element, new List<E>());
    }

    public void AddAdjacent(E i_vertex, E i_adjacent)
    {
        if (vertexs.Contains(i_vertex) && vertexs.Contains(i_adjacent))
        {
            List<E> aux = adjacentVertexs[i_vertex];
            if (!aux.Contains(i_adjacent))
                aux.Add(i_adjacent);
            aux = adjacentVertexs[i_adjacent];
            if (!aux.Contains(i_vertex))
                aux.Add(i_vertex);
        }
    }

    public bool IsAdjacent(E i_vertex, E i_adjacent)
    {
        if (vertexs.Contains(i_vertex) && vertexs.Contains(i_adjacent) && adjacentVertexs[i_vertex].Contains(i_adjacent))
            return true;
        else
            return false;
    }

    public bool AreConnected(E i_vertex1, E i_vertex2)
    {
        List<E> traveledVertexs = new List<E>();
        List<E> activeVertexs = new List<E>();
        List<E> aux;
        do
        {
            traveledVertexs.Add(activeVertexs[0]);
            aux = adjacentVertexs[activeVertexs[0]];
            activeVertexs.RemoveAt(0);
            foreach (E vertex in aux)
            {
                if (!traveledVertexs.Contains(vertex) && !activeVertexs.Contains(vertex))
                    activeVertexs.Add(vertex);
            }
        }
        while (!activeVertexs.Contains(i_vertex2) || activeVertexs.Count == 0);
        return activeVertexs.Contains(i_vertex2);
    }

    public bool IdFullyConnected()
    {
        List<E> traveledVertexs = new List<E>();
        List<E> activeVertexs = new List<E>();
        List<E> aux;
        do
        {
            traveledVertexs.Add(activeVertexs[0]);
            aux = adjacentVertexs[activeVertexs[0]];
            activeVertexs.RemoveAt(0);
            foreach (E vertex in aux)
            {
                if (!traveledVertexs.Contains(vertex) && !activeVertexs.Contains(vertex))
                    activeVertexs.Add(vertex);
            }
        }
        while (activeVertexs.Count == 0);
        foreach (E vertex in vertexs)
        {
            if (!traveledVertexs.Contains(vertex))
                return false;
        }
        return true;
    }
}
