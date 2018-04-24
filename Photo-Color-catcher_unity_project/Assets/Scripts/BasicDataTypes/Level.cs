using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public OnArrayImage _img;

    public Dictionary<Vector2,LevelCell> _cells;

    public GraphType<Vector2> _graph;
}
