using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Simple graph data type.
/// </summary>
[System.Serializable]
public class GraphType<E>
{
    /// <summary>
    /// List of vertices of the graph.
    /// </summary>
    public List<E> _vertices;

    /// <summary>
    /// Dictionary of list of adjacent vertices.
    /// </summary>
    public Dictionary<E,List<E>> _adjacentVertices;

    public GraphType()
    {
        _vertices = new List<E>();
        _adjacentVertices = new Dictionary<E, List<E>>();
    }

    /// <summary>
    /// Adds the vertex to the graph.
    /// </summary>
    /// <param name="i_element">Vertex.</param>
    public void AddVertex(E i_element)
    {
        if (!_vertices.Contains(i_element))
        {
            _vertices.Add(i_element);
            _adjacentVertices.Add(i_element, new List<E>());
        }
    }

    /// <summary>
    /// Makes the vertices adjacent.
    /// </summary>
    /// <param name="i_vertex">Vertex.</param>
    /// <param name="i_adjacent">Adjacent vertex.</param>
    public void AddAdjacent(E i_vertex, E i_adjacent)
    {
        if (_vertices.Contains(i_vertex) && _vertices.Contains(i_adjacent))
        {
            List<E> aux = _adjacentVertices[i_vertex];
            if (!aux.Contains(i_adjacent))
                aux.Add(i_adjacent);
            aux = _adjacentVertices[i_adjacent];
            if (!aux.Contains(i_vertex))
                aux.Add(i_vertex);
        }
    }

    /// <summary>
    /// Checks if the second vertex is adjacent to the first.
    /// </summary>
    /// <returns><c>true</c> if he second vertex is adjacent to the first; otherwise, <c>false</c>.</returns>
    /// <param name="i_vertex">Vertex.</param>
    /// <param name="i_adjacent">Adjacent vertex.</param>
    public bool IsAdjacent(E i_vertex, E i_adjacent)
    {
        if (_vertices.Contains(i_vertex) && _vertices.Contains(i_adjacent) && _adjacentVertices[i_vertex].Contains(i_adjacent))
            return true;
        else
            return false;
    }

    /// <summary>
    /// Checks if both vertex are connected.
    /// </summary>
    /// <returns><c>true</c>, if there's a way through adjacent vertices from the first vertex to the second, <c>false</c> otherwise.</returns>
    /// <param name="i_vertex1">First vertex.</param>
    /// <param name="i_vertex2">Second vertex.</param>
    public bool AreConnected(E i_vertex1, E i_vertex2)
    {
        List<E> traveledVertices = new List<E>();
        List<E> activeVertices = new List<E>();
        activeVertices.Add(i_vertex1);
        List<E> aux;

        //While second vertex is not reached and there are active vertices to explore.
        do
        {
            traveledVertices.Add(activeVertices[0]);
            aux = _adjacentVertices[activeVertices[0]];
            activeVertices.RemoveAt(0);

            //Adds all the adjacent vertices to the active vertex that haven't been reached.
            foreach (E vertex in aux)
            {
                if (!traveledVertices.Contains(vertex) && !activeVertices.Contains(vertex))
                    activeVertices.Add(vertex);
            }
        }
        while (!activeVertices.Contains(i_vertex2) && activeVertices.Count != 0);
        return activeVertices.Contains(i_vertex2);
    }

    /// <summary>
    /// Checks if the graph is fully connected.
    /// </summary>
    /// <returns><c>true</c> if all the vertices of the graph are connected to each other; otherwise, <c>false</c>.</returns>
    public bool IsFullyConnected()
    {
        List<E> traveledVertices = new List<E>();
        List<E> activeVertices = new List<E>();
        activeVertices.Add(_vertices[0]);
        List<E> aux;

        //While there are active vertices to explore.
        do
        {
            traveledVertices.Add(activeVertices[0]);
            aux = _adjacentVertices[activeVertices[0]];
            activeVertices.RemoveAt(0);

            //Adds all the adjacent vertices to the active vertex that haven't been reached.
            foreach (E vertex in aux)
            {
                if (!traveledVertices.Contains(vertex) && !activeVertices.Contains(vertex))
                    activeVertices.Add(vertex);
            }
        }
        while (activeVertices.Count != 0);

        //Checks if all vertices have been reached.
        foreach (E vertex in _vertices)
        {
            if (!traveledVertices.Contains(vertex))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Gets which vertices of the graph are connected to each others.
    /// </summary>
    /// <returns>A list containing all vertices of the graph split on lists. All vertices on each list are connected to each other.</returns>
    public List<List<E>> GetConnectedVertices()
    {
        List<List<E>> connectedVertex = new List<List<E>>();
        List<E> remainingVertices = new List<E>(_vertices);
        List<E> traveledVertices;
        List<E> activeVertices = new List<E>();
        List<E> aux;

        //While there are active vertices to explore.
        while (remainingVertices.Count != 0)
        {
            traveledVertices = new List<E>();
            activeVertices.Add(remainingVertices[0]);
            do
            {
                traveledVertices.Add(activeVertices[0]);
                aux = _adjacentVertices[activeVertices[0]];
                remainingVertices.Remove(activeVertices[0]);
                activeVertices.RemoveAt(0);

                //Adds all the adjacent vertices to the active vertex that haven't been reached.
                foreach (E vertex in aux)
                {
                    if (!traveledVertices.Contains(vertex) && !activeVertices.Contains(vertex))
                        activeVertices.Add(vertex);
                }
            }
            while (activeVertices.Count != 0);

            connectedVertex.Add(traveledVertices);
        }
        return connectedVertex;
    }
}
