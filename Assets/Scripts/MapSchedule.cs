using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapSchedule {

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

    private List<CellContent>[,] schedule;

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

        schedule = new List<CellContent>[size, size];

    }

}
