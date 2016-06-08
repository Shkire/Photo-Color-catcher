using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using PlatformPattern = PlatformPatternConfig.PlatformPattern;
using CellContent = MapScheme.CellContent;

public class DACMapSchemeProblem: MonoBehaviour{

	private Vector2 freeRowsInterval;
	private List<PlatformPatternConfig> availablePatterns;
	public int minSize;
	public int maxSize;
	private int mapSize;
	private List<CellContent>[,] scheme;

	public void Config(Vector2 i_freeRowsInterval, List<PlatformPatternConfig> i_availablePatterns, int i_mapSize, List<CellContent>[,] i_scheme)
	{
		freeRowsInterval = i_freeRowsInterval;
		availablePatterns = i_availablePatterns;
		minSize = 100;
		maxSize = 0;
		mapSize = i_mapSize;
		scheme = i_scheme;
	}

	public IEnumerator DivideAndConquer()
	{
		foreach (PlatformPatternConfig patternConfig in availablePatterns) 
		{
			minSize = (patternConfig.minLines < minSize) ? patternConfig.minLines : minSize;
			maxSize = (patternConfig.maxLines > maxSize) ? patternConfig.maxLines : maxSize;
		}

		int intervalSize = (int)freeRowsInterval.y - (int)freeRowsInterval.x + 1;

		float coin = Random.Range (0f, 1f);

		if (((intervalSize <= maxSize) && (coin>=0.5f)) || (intervalSize == minSize) || (intervalSize<2*minSize)) 
		{
			yield return StartCoroutine(ResolveProblem ());
		}
		else
		{
			yield return StartCoroutine(GenerateSubProblems ());
		}
		Destroy (this.gameObject);
	}

	IEnumerator GenerateSubProblems()
	{
		Vector2 firstSubInterval = Vector2.zero;
		Vector2 secondSubInterval = Vector2.zero;
		int division;
		do 
		{
			division = Random.Range ((int)freeRowsInterval.x, (int)freeRowsInterval.y);
			firstSubInterval = new Vector2 (freeRowsInterval.x, (float)division);
			secondSubInterval = new Vector2 ((float)division + 1, freeRowsInterval.y);
			yield return null;
		}
		while ((firstSubInterval.y - firstSubInterval.x + 1) < minSize || (secondSubInterval.y - secondSubInterval.x + 1) < minSize);
		List<PlatformPatternConfig> firstAvailablePatterns = new List<PlatformPatternConfig>();
		List<PlatformPatternConfig> secondAvailablePatterns = new List<PlatformPatternConfig>();
		foreach (PlatformPatternConfig patternConfig in availablePatterns) 
		{
			if (patternConfig.minLines <= (firstSubInterval.y - firstSubInterval.x + 1))
				firstAvailablePatterns.Add (patternConfig);
			if (patternConfig.minLines <= (secondSubInterval.y - secondSubInterval.x + 1))
				secondAvailablePatterns.Add (patternConfig);
		}
		GameObject go1 = new GameObject ();
		GameObject go2 = new GameObject ();
		DACMapSchemeProblem subProblem1 = go1.AddComponent<DACMapSchemeProblem> ();
		DACMapSchemeProblem subProblem2 = go2.AddComponent<DACMapSchemeProblem> ();
		yield return null;
		subProblem1.Config (firstSubInterval, firstAvailablePatterns, mapSize, scheme);
		subProblem2.Config (secondSubInterval, secondAvailablePatterns, mapSize, scheme);
		YieldInstruction wait1 = StartCoroutine (subProblem1.DivideAndConquer ());
		yield return wait1;
		YieldInstruction wait2 = StartCoroutine (subProblem2.DivideAndConquer ());
		yield return wait2;
	}

	IEnumerator ResolveProblem()
	{
		//Dejamos los patrones validos
		int intervalSize = (int)(freeRowsInterval.y - freeRowsInterval.x + 1);
		for (int i = 0; i < availablePatterns.Count; i++) 
		{
			if (availablePatterns [i].maxLines < intervalSize) 
			{
				availablePatterns.RemoveAt (i);
				i--;
			}
		}

		yield return null;

		//Seleccionamos un patron
		PlatformPatternConfig patternSelected = availablePatterns [Random.Range (0, availablePatterns.Count)];

		//Definimos variables utiles
		int lineYPos;
		List<int>[] platformSizeList;
		List<int>[] emptyList;
		int platformSize;
		int emptySize;
		bool canContinue;
		int counter;
		int emptyCounter;
		float probability;
		int xPos;
		int offset;
		int startingYPos;
		int auxInt;

		switch (patternSelected.pattern) 
		{
		//Si es patron linea
		case PlatformPattern.Line:
			//Posicion de inicio del patron dentro de las posibles filas
			lineYPos = Random.Range ((int)freeRowsInterval.x, (int)freeRowsInterval.y);
			//Inicializamos el almacen de tamaños de plataformas
			platformSizeList = new List<int>[1];
			platformSizeList [0] = new List<int> ();
			//No se podra continuar aun
			canContinue = false;

			//Obtenemos los tamaños de las plataformas a añadir
			while (!canContinue) {
				//Reseteamos el contador
				counter = 0;
				//Cogemos un tamaño aleatorio de plataforma
				platformSize = Random.Range (patternSelected.minPiecesByPlatform, patternSelected.maxPiecesByPlatform + 1);
				//Actualizamos el contador al valor actual de la suma de plataformas
				foreach (int auxSize in platformSizeList[0])
					counter += auxSize;
				//Si los huecos entre plataformas + las plataformas que hay + la plataforma nueva <= tamaño util del mapa (sin bordes)
				if ((platformSizeList [0].Count + counter + platformSize) <= (mapSize - 2)) {
					platformSizeList [0].Add (platformSize);
					//Moneda
					probability = Random.Range (0f, 1f);
					//Si la moneda satisface ser menor que la proporcion de plataformas añadidas sobre el total o los huecos entre plataformas + todas plataformas + la ultima plataforma == tamaño util del mapa
					if ((probability <= Mathf.Lerp (0, patternSelected.maxPlatforms, platformSizeList [0].Count)) || (platformSizeList [0].Count + counter + platformSize - 1) == (mapSize - 2))
						//continua
						canContinue = true;
				}
				yield return null;
			}

			//Colocamos el contador de huecos a 0
			emptyCounter = 0;
			//Reseteamos el contador de plataformas
			counter = 0;
			//Sumamos todas las plataformas
			foreach (int auxSize in platformSizeList[0])
				counter += auxSize;
			//Inicializamos el almacen de huecos
			emptyList = new List<int>[1];
			emptyList [0] = new List<int> ();

			//Obtenemos los tamaños de los huecos
			for (int i = 0; i <= platformSizeList [0].Count; i++) {
				//Si es la primera pieza, tiene un hueco delante que puede ser 0
				if (i == 0)
					//El hueco va de 0 a el tamaño util del mapa - un hueco entre cada 2 plataformas y los huecos ya ocupados
					emptySize = Random.Range (0, (mapSize - 1 - (counter + emptyCounter) - (platformSizeList [0].Count - emptyList [0].Count - 1)));
				else if (i < platformSizeList [0].Count)
					//El hueco va de 1 a el tamaño util del mapa - un hueco entre cada 2 plataformas y los huecos ya ocupados
					emptySize = Random.Range (1, (mapSize - 1 - (counter + emptyCounter) - (platformSizeList [0].Count - emptyList [0].Count - 1)));
				else
					//Se rellena lo que quede
					emptySize = mapSize - 2 - counter - emptyCounter;
				emptyCounter += emptySize;
				emptyList [0].Add (emptySize);

				yield return null;
			}

			//Colocar las piezas
			for (int i = (int)freeRowsInterval.x; i <= freeRowsInterval.y; i++) 
			{
				if (i == lineYPos) 
				{
					xPos = 1;
					for (int j = 0; j < emptyList [0].Count; j++) 
					{
						for (int k = 0; k < emptyList [0] [j]; k++) 
						{
							if (!scheme [xPos, i].Contains (CellContent.EmptySpace) && !scheme [xPos, i].Contains (CellContent.Platform))
								scheme [xPos, i].Add (CellContent.EmptySpace);
							xPos++;
							yield return null;
						}
						if (j != emptyList [0].Count - 1)
						{
							for (int k = 0; k < platformSizeList [0] [j]; k++) 
							{
								if (!scheme [xPos, i].Contains (CellContent.EmptySpace) && !scheme [xPos, i].Contains (CellContent.Platform))
									scheme [xPos, i].Add (CellContent.Platform);
								xPos++;
								yield return null;
							}
						}
					}
				}
				else
				{
					for (int j=1; j<=mapSize-2; j++)
					{
						if (!scheme [j, i].Contains (CellContent.EmptySpace) && !scheme [j, i].Contains (CellContent.Platform))
							scheme [j, i].Add (CellContent.EmptySpace);
						yield return null;
					}
				}
			}
			break;
		case PlatformPattern.Stair:
			//Inicializamos el almacen de tamaños de plataformas
			platformSizeList = new List<int>[1];
			platformSizeList [0] = new List<int> ();
			//Reseteamos el contador de plataformas
			counter = 0;
			int maxPlatformSize;
			//Seleccionar x plataformas (numero) 
			for (int i = 1; i <= Mathf.FloorToInt ((freeRowsInterval.y - freeRowsInterval.x + 1) / 2); i++) {
				//Define el tamaño maximo que puede tener la plataforma
				maxPlatformSize = Mathf.Min (patternSelected.maxPiecesByPlatform, (mapSize - 2 - counter - (patternSelected.minPiecesByPlatform * (Mathf.FloorToInt ((freeRowsInterval.y - freeRowsInterval.x + 1) / 2) - i) + (Mathf.FloorToInt ((freeRowsInterval.y - freeRowsInterval.x + 1) / 2) - (i - 1)))));
				//Elige el tamaño de la plataforma
				platformSize = Random.Range (patternSelected.minPiecesByPlatform, maxPlatformSize + 1);
				//Añade la plataforma
				platformSizeList [0].Add (platformSize);
				//Suma al contador la plataforma y el hueco tras ella (entre cada escalon hay minimo un hueco en x)
				counter += platformSize + 1;
			}
			yield return null;
			//Resetea el contador de huecos
			emptyCounter = 0;
			//Resetea el contador
			counter = 0;
			//Sumamos todas las plataformas
			foreach (int auxSize in platformSizeList[0])
				counter += auxSize;
			//Inicializamos el almacen de huecos
			emptyList = new List<int>[1];
			emptyList [0] = new List<int> ();

			//Obtenemos los tamaños de los huecos
			for (int i = 0; i <= platformSizeList [0].Count; i++) {
				//Si es la primera pieza, tiene un hueco delante que puede ser 0
				if (i == 0)
					//El hueco va de 0 a el tamaño util del mapa - un hueco entre cada 2 plataformas y los huecos ya ocupados
					emptySize = Random.Range (0, (mapSize - 1 - (counter + emptyCounter) - (platformSizeList [0].Count - emptyList [0].Count - 1)));
				else if (i < platformSizeList [0].Count)
					//El hueco va de 1 a el tamaño util del mapa - un hueco entre cada 2 plataformas y los huecos ya ocupados
					emptySize = Random.Range (1, (mapSize - 1 - (counter + emptyCounter) - (platformSizeList [0].Count - emptyList [0].Count - 1)));
				else
					//Se rellena lo que quede
					emptySize = mapSize - 2 - counter - emptyCounter;
				emptyCounter += emptySize;
				emptyList [0].Add (emptySize);
				yield return null;
			}

			//Colocar las piezas
			offset = 0;
			if (((int)freeRowsInterval.y - freeRowsInterval.x + 1) % 2 == 1) {
				offset = Random.Range (0, 2);
			}
			startingYPos = (int)freeRowsInterval.x;
			auxInt = 0;
			xPos = 1;
			for (int i = (int)freeRowsInterval.x; i <= freeRowsInterval.y; i++)
			{
				if ((i - startingYPos) % 2 == offset) 
				{
					for (int j = 1; j < xPos; j++) 
					{
						if (!scheme [j, i].Contains (CellContent.EmptySpace) && !scheme [j, i].Contains (CellContent.Platform))
							scheme [j, i].Add (CellContent.EmptySpace);
						yield return null;
					}
					if (auxInt < emptyList [0].Count) 
					{
						for (int j = 0; j < emptyList [0] [auxInt]; j++) 
						{
							if (!scheme [xPos, i].Contains (CellContent.EmptySpace) && !scheme [xPos, i].Contains (CellContent.Platform))
								scheme [xPos, i].Add (CellContent.EmptySpace);
							xPos++;
							yield return null;
						}
					}
					if (auxInt < platformSizeList [0].Count) 
					{
						for (int j = 0; j < platformSizeList [0] [auxInt]; j++) 
						{
							if (!scheme [xPos, i].Contains (CellContent.EmptySpace) && !scheme [xPos, i].Contains (CellContent.Platform))
								scheme [xPos, i].Add (CellContent.Platform);
							xPos++;
							yield return null;
						}
					}
					auxInt++;
					for (int j = xPos; j <= mapSize-2; j++) 
					{
						if (!scheme [j, i].Contains (CellContent.EmptySpace) && !scheme [j, i].Contains (CellContent.Platform))
							scheme [j, i].Add (CellContent.EmptySpace);
						yield return null;
					}
				} 
				else
				{
					for (int j = 1; j <= mapSize - 2; j++) 
					{
						if (!scheme [j, i].Contains (CellContent.EmptySpace) && !scheme [j, i].Contains (CellContent.Platform))
							scheme [j, i].Add (CellContent.EmptySpace);
						yield return null;
					}
				}
			}

			break;
		case PlatformPattern.ZigZag:
			//Inicializamos el almacen de tamaños de plataformas
			platformSizeList = new List<int>[Mathf.FloorToInt ((freeRowsInterval.y - freeRowsInterval.x + 1) / 2)];
			//Inicializamos el almacen de huecos
			emptyList = new List<int>[platformSizeList.Length];

			//Obtenemos los tamaños de las plataformas a añadir
			for (int i = 0; i < platformSizeList.Length; i++) {
				platformSizeList [i] = new List<int> ();
				canContinue = false;
				while (!canContinue) {
					//Reseteamos el contador
					counter = 0;
					//Cogemos un tamaño aleatorio de plataforma
					platformSize = Random.Range (patternSelected.minPiecesByPlatform, patternSelected.maxPiecesByPlatform + 1);
					//Actualizamos el contador al valor actual de la suma de plataformas
					foreach (int auxSize in platformSizeList[i])
						counter += auxSize;
					//Si los huecos entre plataformas + las plataformas que hay + la plataforma nueva <= tamaño util del mapa (sin bordes)
					if ((platformSizeList [i].Count + counter + platformSize) <= (mapSize - 2)) {
						platformSizeList [i].Add (platformSize);
						//Moneda
						probability = Random.Range (0f, 1f);
						//Si la moneda satisface ser menor que la proporcion de plataformas añadidas sobre el total o los huecos entre plataformas + todas plataformas + la ultima plataforma == tamaño util del mapa
						if ((probability <= Mathf.Lerp (0, patternSelected.maxPlatforms, platformSizeList [i].Count)) || (platformSizeList [i].Count + counter + platformSize - 1) == (mapSize - 2))
							//continua
							canContinue = true;
					}
					yield return null;
				}

				//Colocamos el contador de huecos a 0
				emptyCounter = 0;
				//Reseteamos el contador de plataformas
				counter = 0;
				//Sumamos todas las plataformas
				foreach (int auxSize in platformSizeList[i])
					counter += auxSize;
				emptyList [i] = new List<int> ();

				//Obtenemos los tamaños de los huecos
				for (int j = 0; j <= platformSizeList [i].Count; j++) {
					//Si es la primera pieza, tiene un hueco delante que puede ser 0
					if (j == 0)
						//El hueco va de 0 a el tamaño util del mapa - un hueco entre cada 2 plataformas y los huecos ya ocupados
						emptySize = Random.Range (0, (mapSize - 1 - (counter + emptyCounter) - (platformSizeList [i].Count - emptyList [i].Count - 1)));
					else if (i < platformSizeList [i].Count)
						//El hueco va de 1 a el tamaño util del mapa - un hueco entre cada 2 plataformas y los huecos ya ocupados
						emptySize = Random.Range (1, (mapSize - 1 - (counter + emptyCounter) - (platformSizeList [i].Count - emptyList [i].Count - 1)));
					else
						//Se rellena lo que quede
						emptySize = mapSize - 2 - counter - emptyCounter;
					emptyCounter += emptySize;
					emptyList [i].Add (emptySize);
				}
				yield return null;
			}

			//Colocar las piezas
			offset = 0;
			if (((int)freeRowsInterval.y - freeRowsInterval.x + 1) % 2 == 1) {
				offset = Random.Range (0, 2);
			}
			startingYPos = (int)freeRowsInterval.x;
			auxInt = 0;
			for (int i = (int)freeRowsInterval.x; i <= freeRowsInterval.y; i++) 
			{
				if ((i - startingYPos) % 2 == offset) 
				{
					xPos = 1;
					for (int j = 0; j < emptyList [auxInt].Count; j++) 
					{
						for (int k = 0; k < emptyList [auxInt] [j]; k++) 
						{
							if (!scheme [xPos, i].Contains (CellContent.EmptySpace) && !scheme [xPos, i].Contains (CellContent.Platform))
								scheme [xPos, i].Add (CellContent.EmptySpace);
							xPos++;
							yield return null;
						}
						if (j != emptyList [auxInt].Count - 1)
						{
							for (int k = 0; k < platformSizeList [auxInt] [j]; k++) 
							{
								if (!scheme [xPos, i].Contains (CellContent.EmptySpace) && !scheme [xPos, i].Contains (CellContent.Platform))
									scheme [xPos, i].Add (CellContent.Platform);
								xPos++;
								yield return null;
							}
						}
					}
				}
				else
				{
					for (int j=1; j<=mapSize-2; j++)
					{
						if (!scheme [j, i].Contains (CellContent.EmptySpace) && !scheme [j, i].Contains (CellContent.Platform))
							scheme [j, i].Add (CellContent.EmptySpace);
						yield return null;
					}
				}
			}
			break;
		case PlatformPattern.Copy:
			break;
		}

		yield return null;
	}
}
