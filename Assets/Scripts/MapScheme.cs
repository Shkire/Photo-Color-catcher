using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapScheme{

    public enum CellContent
    {
        Player,
        Platform,
        Flag,
        Camera,
        DecorationUp,
        DecorationDown,
        DecorationLeft,
        DecorationRigth,
        DecorationHoriz,
        DecorationVert,
        EnemySpawn
    }

    private int size;

    private List<CellContent>[,] scheme;

    public int mapSize
    {
        get
        {
            return size;
        }
        set
        {
            size = value;
        }
    }

    public void Config()
    {

		scheme = new List<CellContent>[size, size];

    }

}
