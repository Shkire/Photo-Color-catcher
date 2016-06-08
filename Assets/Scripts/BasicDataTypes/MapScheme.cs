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
	public MapScheme(int i_difficulty, int i_size, LevelConfig i_levelConfig)
	{
		size = i_size;
		scheme = new List<CellContent>[size, size];
		for (int i = 0; i < size; i++)
			for (int j = 0; j < size; j++)
				scheme [i, j] = new List<CellContent> ();
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
		//Randomizar una posicion en y
		//Coger los intervalos y comprobar si se encuentra dentro de alguno
		//Una vez hecho seleccionar que patrones se pueden usar para rellenar el hueco superior
		//Seleccionar el size del patron
		//Repetir hasta que se cumpla que se deje hueco por encima y debajo para minSize
			//Si el patron es linea
			//
		//intervalos disponibles
		List<Vector2> freeRows = new List<Vector2> ();
		freeRows.Add (new Vector2 (2, size - 1));
		PlatformPatternConfig chosenPattern = null;
		int yPos;
		int patternSize;
		bool canContinue = false;
		//bucle principal
		while (freeRows.Count > 0) 
		{
			canContinue = false;
			//bucle de seleccion de posicion
			while(!canContinue)
			{
				yPos = rnd.Next (2, size - 2);
				foreach(Vector2 rowInterval in freeRows)
					if ()
			}
		}

		int patternStartingRow = rnd.Next (2, size - 2);
		int availableRows = (int)freeRows [0].x - patternStartingRow;
		int minSize = 1000;
		Vector2 rowIntervalSelected = freeRows [0];
		foreach (PlatformPatternConfig patternConfig in i_levelConfig.patternConfigList)
			if (patternConfig.minLines < minSize)
				minSize = patternConfig.minLines;
		List<PlatformPatternConfig> availablePatterns = new List<PlatformPatternConfig> ();
		for (int i = 0; i < i_levelConfig.patternConfigList.Length; i++) 
		{
			if (i_levelConfig.patternConfigList [i].minLines <= availableRows)
				availablePatterns.Add (i_levelConfig.patternConfigList [i]);
		}
		bool canContinue =false;
		int patternSelected;
		int patternSize;
		do 
		{
			patternSelected = rnd.Next (0, availablePatterns.Count);
			patternSize = rnd.Next (availablePatterns [patternSelected].minLines, availablePatterns [patternSelected].maxLines + 1);
			//comprobar con iversa tb
			if ((rowIntervalSelected.y - (patternStartingRow+patternSize)) == 0 || (rowIntervalSelected.y - (patternStartingRow+patternSize)) >= minSize)
				canContinue=true;
		}
		while (!canContinue);
		if ((patternStartingRow - rowIntervalSelected.x) != 0)
			freeRows.Add (new Vector2(rowIntervalSelected.x,patternStartingRow-1));
		if ((rowIntervalSelected.y - (patternStartingRow+patternSize)) != 0)
			freeRows.Add (new Vector2(patternStartingRow+patternSize+1,rowIntervalSelected.y));
		freeRows.Remove (rowIntervalSelected);
		switch (availablePatterns [patternSelected].pattern) 
		{
		case PlatformPattern.Line:
			int pos = rnd.Next (patternStartingRow, patternStartingRow + patternSize);
			List<int> sizeList = new List<int> ();
			int platformSize;
			canContinue = false;
			int counter;
			float probability;
			do {
				counter = 0;
				platformSize = rnd.Next (availablePatterns [patternSelected].minPiecesByPlatform, availablePatterns [patternSelected].maxPiecesByPlatform + 1);
				foreach (int auxSize in sizeList)
					counter += auxSize;
				if ((sizeList.Count * 2 + counter - 2 + platformSize) <= (size - 2)) {
					sizeList.Add (platformSize);
					probability = (float)rnd.NextDouble ();
					if ((probability <= Mathf.Lerp (0, availablePatterns [patternSelected].maxPlatforms, sizeList.Count)) || ((sizeList.Count + 1) * 2 + counter - 2 + availablePatterns [patternSelected].minPiecesByPlatform) > (size - 2))
						canContinue = true;
				}					
			} while (!canContinue);
			int emptyCounter = 0;
			counter = 0;
			foreach (int auxSize in sizeList)
				counter += auxSize;
			List<int> emptyList = new List<int> ();
			int emptyBlocks;
			for (int i = 0; i <= sizeList.Count; i++) {
				if (i == 0)
					emptyBlocks = rnd.Next (0, size - 1 - counter - (sizeList.Count - emptyList.Count + 1) - emptyCounter);
				else if (i < sizeList.Count)
					emptyBlocks = rnd.Next (1, size - 1 - counter - (sizeList.Count - emptyList.Count + 1) - emptyCounter);
				else
					emptyBlocks = size - 2 - counter - emptyCounter;
				emptyCounter += emptyBlocks;
				emptyList.Add (emptyBlocks);
			}
			//Colocar cada empty y plataforma
			int xPos = 1;
			for (int i = 0; i < emptyList.Count; i++) {
				if (emptyList [i] != 0) {
					for (int j = 0; j < emptyList [i]; j++) {
						if (!scheme [xPos, pos].Contains (CellContent.EmptySpace) && !scheme [xPos, pos].Contains (CellContent.Platform))
							scheme [xPos, pos].Add (CellContent.EmptySpace);
						xPos++;
					}
				}
				if (sizeList [i] != 0 && i < emptyList.Count - 1) {
					for (int j = 0; j < emptyList [i]; j++) {
						if (!scheme [xPos, pos].Contains (CellContent.EmptySpace) && !scheme [xPos, pos].Contains (CellContent.Platform))
							scheme [xPos, pos].Add (CellContent.Platform);
						xPos++;
					}
				}
			}
			for (int auxPos = rowIntervalSelected.x; auxPos <= rowIntervalSelected.y; auxPos++) 
			{
				if (auxPos != pos) 
				{
					for (int i = 1; i < size - 1; i++) 
					{
						if (!scheme [i, auxPos].Contains (CellContent.EmptySpace) && !scheme [i, auxPos].Contains (CellContent.Platform))
							scheme [i, auxPos].Add (CellContent.EmptySpace);
					}
				}
			}
			break;
		case PlatformPattern.Stair:
			break;
		case PlatformPattern.ZigZag:
			break;
		case PlatformPattern.Copy:
			break;
		default:
			break;
		}
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
