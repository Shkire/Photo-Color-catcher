using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using PlatformPattern = PlatformPatternConfig.PlatformPattern;

public class MapScheme{

    public enum CellContent
    {
        Player,
        Platform,
        Flag,
        Camera,
		Teleport,
        DecorationUp,
        DecorationDown,
        DecorationLeft,
        DecorationRigth,
        DecorationHoriz,
        DecorationVert,
        EnemySpawn,
		EmptySpace
    }

	public enum TeleportType
	{
		Horizontal = 1,
		Vertical = 2
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

	public MapScheme()
	{
		size = 24;
		scheme = new List<CellContent>[size, size];
		for (int i = 0; i < size; i++) 
		{
			for (int j = 0; j < size; j++) 
			{
				scheme [i, j] = new List<CellContent> ();
			}	
		}
	}

	/*
		//Teleport setting
		System.Random rnd = new System.Random(Guid.NewGuid().GetHashCode());
		int verticalNum =0;
		int horizontalNum=0;
		TeleportType teleportType;
		int aux;
		List<Vector2> teleportList = new List<Vector2> ();
		Vector2 vecAux;

		int teleportNum = rnd.Next (i_levelConfig.teleportMin, i_levelConfig.teleportMax+1);
		for (int i = 0; i < teleportNum; i++) {
			if (verticalNum == i_levelConfig.maxTeleportNumBySide)
				teleportType = TeleportType.Horizontal;
			else if (horizontalNum == i_levelConfig.maxTeleportNumBySide)
				teleportType = TeleportType.Vertical;
			else
				teleportType = (TeleportType)rnd.Next (1, 3);
			do
			{
				aux = rnd.Next (1, size);
				if (teleportType==TeleportType.Horizontal)
					vecAux=new Vector2(aux,0);
				else
					vecAux=new Vector2(0,aux);
			}
			while (teleportList.Contains (vecAux));
			if (teleportType == TeleportType.Horizontal) {
				teleportList.Add (vecAux);
				teleportList.Add (new Vector2 (vecAux.x, size));
				horizontalNum++;
			} 
			else 
			{
				teleportList.Add (vecAux);
				teleportList.Add (new Vector2 (size,vecAux.y));
				verticalNum++;
			}
		}

		for (int i = 0; i < size; i++) {
			if (!scheme [0, i].Contains (CellContent.Platform) && !scheme [0, i].Contains (CellContent.Teleport))
				scheme [0, i].Add (CellContent.Platform);
			if (!scheme [i, 0].Contains (CellContent.Platform) && !scheme [i, 0].Contains (CellContent.Teleport))
				scheme [i, 0].Add (CellContent.Platform);
			if (!scheme [0, size - 1].Contains (CellContent.Platform) && !scheme [0, size - 1].Contains (CellContent.Teleport))
				scheme [0, size - 1].Add (CellContent.Platform);
			if (!scheme [size - 1, 0].Contains (CellContent.Platform) && !scheme [size - 1, 0].Contains (CellContent.Teleport))
				scheme [size - 1, 0].Add (CellContent.Platform);
		}
	*/
    public void Config()
    {
		scheme = new List<CellContent>[size, size];
    }

	public List<CellContent>[,] GetScheme()
	{
		return scheme;
	}
}
