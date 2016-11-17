using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using PlatformPattern = PlatformPatternConfig.PlatformPattern;
using CellContent = MapScheme.CellContent;

public class ProcessData_DAC_Problem{

	public Vector2 freeRowsInterval;
	public List<PlatformPatternConfig> availablePatterns;
	public int minSize;
	public int maxSize;
	public int mapSize;
	public List<CellContent>[,] scheme;
}

public class ImgProcessManager : Singleton<ImgProcessManager> {

	public int maxPixelsProcessedPerFrame;
	public int divisionFactor;
	public string imgPath;
	public List<ProcessedImage> images;
	public SpriteRenderer sprite;
	private int count;
	public Texture2D texturita;
	public LevelConfigList levelConfigList;

	#region PrivateVarForCoroutines
	private ProcessedImage p_img;
	private List<ProcessedImage> p_imgList;
	private UnityEngine.Color[] p_tempPixels;
	private int[] p_idList;
	private List<ProcessData_DAC_Problem> p_problems;
	#endregion

	protected ImgProcessManager () 
	{
	}


	public IEnumerator ProccesAndIndexImageCor(string i_path)
	{
		//Cojo una nueva id para la imagen
		int id = PersistenceManager.GetNewId();

		yield return null;

		//Creo la imagen a partir de su path
		yield return (StartCoroutine(CreateImage(i_path, id)));

		yield return null;

		//Cojo una lista de ids para los hijos
		p_idList = PersistenceManager.GetNewIdList(divisionFactor*divisionFactor);

		yield return null;

		//La divido
		yield return StartCoroutine(DivideImage(divisionFactor));

		yield return null;

		//Guardo las imagenes procesadas
		PersistenceManager.PushImgAndChildren(p_img,p_imgList);

		yield return null;

		Debug.Log("Creo el diccionario de datos de imagenes");
		//Creo el diccionario de datos de imágenes
		Dictionary<int,ProcessedImageData> tempDataDict = new Dictionary<int,ProcessedImageData> ();
		List<Coroutine> wait = new List<Coroutine> ();
		Debug.Log("Diccionario y lista de corrutinas creados");

		Debug.Log("Bucle for para cada imagen a analizar");
		//Para cada imagen
		foreach (ProcessedImage auxImg in p_imgList)
		{
			Debug.Log("Lanzo una corrutina de procesamiento de datos de imagen");
			//Proceso sus datos
			ProcessedImageData tempImgData = null;
			wait.Add(StartCoroutine (ProcessImageData (auxImg, tempImgData)));
			tempDataDict.Add(auxImg.GetId(),tempImgData);
			Debug.Log("Corrutina finalizada");
		}
		foreach (Coroutine waitCor in wait) 
		{
			Debug.Log("Espero a que acaben");
			yield return waitCor;
		}

		Debug.Log("Guardo datos procesados");
		//Guardo los datos de las imagenes procesadas
		PersistenceManager.PushImgData (tempDataDict);
		PersistenceManager.MainDataFlush ();
		PersistenceManager.ImgDataFlush ();
		Debug.Log("Datos guardados");
	}


	void Start () {
		PersistenceManager.MainLoad ();
	}

	void DidImageInit()
	{
		//StartCoroutine (imagen.ToTexture2D(this.gameObject));
	}

	void DidImageToTexture(object text)
	{
		sprite.sprite = Sprite.Create((Texture2D)text,new Rect(0,0,10,10),new Vector2(0,0));
		Debug.Log ("¡YA TENGO SPRITEEEE!");
	}

	public void Instantiate(int index,string name){
		GameObject go = new GameObject(name);
		SpriteRenderer sprtRend = go.AddComponent<SpriteRenderer> ();
		Sprite spr = new Sprite ();
		//spr = Sprite.Create (PersistenceManager.GetImage (index).ToTexture2D (), new Rect (0, 0, 25,25), new Vector2 (0, 0));
		sprtRend.sprite = spr;
	}
		
	public void ProccesAndIndexImage(string i_path)
	{
		Debug.Log("Lanzo corrutina de procesamiento");
		StartCoroutine(ProccesAndIndexImageCor (i_path));
		Debug.Log("Corrutina lanzada");
	}

	IEnumerator ProcessImageData (ProcessedImage i_img, ProcessedImageData i_imgData)
	{
		//Saco todos los datos necesarios
		i_imgData = i_img.GetImageData();
		//Aplico el DAC
		GameObject dacController = new GameObject("DAC Controller");
		p_problems = new List<ProcessData_DAC_Problem> ();
		p_problems.Add (new ProcessData_DAC_Problem ());
		ProcessData_DAC_Config (0, new Vector2(1f,21f),new List<PlatformPatternConfig> (levelConfigList.GetPatterns(i_imgData.GetDificulty())),24,i_imgData.GetScheme());
		//dacProblem.Config (new Vector2(1f,21f),new List<PlatformPatternConfig> (levelConfigList.GetPatterns(0)),24,i_imgData.GetScheme());
		yield return StartCoroutine(ProcessDataDivideAndConquer (0));
	}

	IEnumerator CreateImage (string i_path, int i_id)
	{
		//Creo una textura auxiliar
		Texture2D tempText = new Texture2D(1,1);

		yield return null;

		//Leo la imagen correspondiente
		byte[] imgRead = System.IO.File.ReadAllBytes (Application.persistentDataPath+"/"+i_path);

		yield return null;

		//Cargo la imagen en la textura
		tempText.LoadImage (imgRead);

		yield return null;

		p_img = new ProcessedImage (i_id, i_path, tempText);
	}

	IEnumerator DivideImage (int i_divisionFactor)
	{
		//Si ya se ha divido
		if (p_img.GetChildrenCount() > 0)
			//Sale
			yield break;

		//Calcula el ancho de cada hijo
		int childrenWidth = Mathf.CeilToInt ((float)p_img.width / i_divisionFactor);
		//Calcula el alto de cada hijo
		int childrenHeight = Mathf.CeilToInt ((float)p_img.height / i_divisionFactor);
		//Crea una lista auxiliar de imagenes
		p_imgList = new List<ProcessedImage> ();

		yield return null;

		//Crea una textura auxiliar
		Texture2D tempText = new Texture2D (p_img.width, p_img.height);
		//Le asigno los pixeles de la imagen padre
		tempText.SetPixels (p_img.pixels);
		//Aplico los cambios en la textura
		tempText.Apply ();

		yield return null;

		//Aplico un reescalado bilineal
		tempText = tempText.ResizeBilinear (childrenWidth * i_divisionFactor, childrenHeight * i_divisionFactor);

		yield return null;

		//Saco todos los pixeles de la textura
		p_tempPixels = tempText.GetPixels ();

		yield return null;

		List<Coroutine> waitChildren = new List<Coroutine> ();
		//Para cada hijo
		for (int x = 0; x < i_divisionFactor; x++) 
		{
			for (int y = 0; y < i_divisionFactor; y++) 
			{
				waitChildren.Add (StartCoroutine (CreateChild (x, y, childrenWidth, childrenHeight, i_divisionFactor)));
			}
		}

		foreach (Coroutine wait in waitChildren) 
		{
			yield return wait;
		}
	}

	IEnumerator CreateChild(int i_x, int i_y, int i_width, int i_height, int i_divisionFactor)
	{
		//Creo una lista de pixeles auxiliar
		UnityEngine.Color[] auxPixels = new UnityEngine.Color[i_width * i_height];

		//Para cada pixel
		int counter = 0;
		for (int i = 0; i < i_width; i++) {
			for (int j = 0; j < i_height; j++,counter++) 
			{
				//Obtengo su posicion y obtengo el color correspondiente
				int origPos = i_x * i_width + i_y * i_divisionFactor * i_width * i_height + i + j * i_width * i_divisionFactor;
				auxPixels [i + j*i_width] = new UnityEngine.Color(p_tempPixels [origPos].r,p_tempPixels [origPos].g,p_tempPixels [origPos].b,p_tempPixels [origPos].a);

				if (counter % maxPixelsProcessedPerFrame == maxPixelsProcessedPerFrame-1) 
				{
					yield return null;
				}	

			}
		}

		yield return null;

		//Creo una imagen auxiliar
		ProcessedImage auxImg = new ProcessedImage(auxPixels,i_width,i_height,p_idList[i_x*i_divisionFactor+i_y]);

		//Añado el hijo al diccionario de hijos del padre
		p_img.AddChild (new Vector2 (i_x, i_y), auxImg.id);

		//Añado el hijo a la lista de hijos
		p_imgList.Add(auxImg);

		yield return null;
	}

	public IEnumerator ProcessDataDivideAndConquer(int i_pos)
	{
		foreach (PlatformPatternConfig patternConfig in p_problems[i_pos].availablePatterns) 
		{
			p_problems[i_pos].minSize = (patternConfig.minLines < p_problems[i_pos].minSize) ? patternConfig.minLines : p_problems[i_pos].minSize;
			p_problems[i_pos].maxSize = (patternConfig.maxLines > p_problems[i_pos].maxSize) ? patternConfig.maxLines : p_problems[i_pos].maxSize;
		}

		int intervalSize = (int)p_problems[i_pos].freeRowsInterval.y - (int)p_problems[i_pos].freeRowsInterval.x + 1;

		float coin = Random.Range (0f, 1f);

		if (((intervalSize <= p_problems[i_pos].maxSize) && (coin>=0.5f)) || (intervalSize == p_problems[i_pos].minSize) || (intervalSize<2*p_problems[i_pos].minSize)) 
		{
			yield return StartCoroutine(ProcessData_DAC_ResolveProblem (i_pos));
		}
		else
		{
			yield return StartCoroutine(ProcessData_DAC_GenerateSubProblems (i_pos));
		}
	}

	public void ProcessData_DAC_Config(int i_pos, Vector2 i_freeRowsInterval, List<PlatformPatternConfig> i_availablePatterns, int i_mapSize, List<CellContent>[,] i_scheme)
	{
		p_problems[i_pos].freeRowsInterval = i_freeRowsInterval;
		p_problems[i_pos].availablePatterns = i_availablePatterns;
		p_problems[i_pos].minSize = 100;
		p_problems[i_pos].maxSize = 0;
		p_problems[i_pos].mapSize = i_mapSize;
		p_problems[i_pos].scheme = i_scheme;
	}

	IEnumerator ProcessData_DAC_GenerateSubProblems(int i_pos)
	{
		Vector2 firstSubInterval = Vector2.zero;
		Vector2 secondSubInterval = Vector2.zero;
		int division;
		do 
		{
			division = Random.Range ((int)p_problems[i_pos].freeRowsInterval.x, (int)p_problems[i_pos].freeRowsInterval.y);
			firstSubInterval = new Vector2 (p_problems[i_pos].freeRowsInterval.x, (float)division);
			secondSubInterval = new Vector2 ((float)division + 1, p_problems[i_pos].freeRowsInterval.y);
			yield return null;
		}
		while ((firstSubInterval.y - firstSubInterval.x + 1) < p_problems[i_pos].minSize || (secondSubInterval.y - secondSubInterval.x + 1) < p_problems[i_pos].minSize);
		List<PlatformPatternConfig> firstAvailablePatterns = new List<PlatformPatternConfig>();
		List<PlatformPatternConfig> secondAvailablePatterns = new List<PlatformPatternConfig>();
		foreach (PlatformPatternConfig patternConfig in p_problems[i_pos].availablePatterns) 
		{
			if (patternConfig.minLines <= (firstSubInterval.y - firstSubInterval.x + 1))
				firstAvailablePatterns.Add (patternConfig);
			if (patternConfig.minLines <= (secondSubInterval.y - secondSubInterval.x + 1))
				secondAvailablePatterns.Add (patternConfig);
		}
		int firstPos = p_problems.Count;
		p_problems.Add (new ProcessData_DAC_Problem ());
		int secondPos = p_problems.Count;
		p_problems.Add (new ProcessData_DAC_Problem ());
		yield return null;
		ProcessData_DAC_Config (firstPos, firstSubInterval, firstAvailablePatterns, p_problems[i_pos].mapSize, p_problems[i_pos].scheme);
		ProcessData_DAC_Config (secondPos, secondSubInterval, secondAvailablePatterns, p_problems[i_pos].mapSize, p_problems[i_pos].scheme);
		YieldInstruction wait1 = StartCoroutine (ProcessDataDivideAndConquer (firstPos));
		yield return wait1;
		YieldInstruction wait2 = StartCoroutine (ProcessDataDivideAndConquer (secondPos));
		yield return wait2;
	}

	IEnumerator ProcessData_DAC_ResolveProblem(int i_pos)
	{
		//Dejamos los patrones validos
		int intervalSize = (int)(p_problems[i_pos].freeRowsInterval.y - p_problems[i_pos].freeRowsInterval.x + 1);
		for (int i = 0; i < p_problems[i_pos].availablePatterns.Count; i++) 
		{
			if (p_problems[i_pos].availablePatterns [i].maxLines < intervalSize) 
			{
				p_problems[i_pos].availablePatterns.RemoveAt (i);
				i--;
			}
		}

		yield return null;

		//Seleccionamos un patron
		PlatformPatternConfig patternSelected = p_problems[i_pos].availablePatterns [Random.Range (0, p_problems[i_pos].availablePatterns.Count)];

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
			lineYPos = Random.Range ((int)p_problems[i_pos].freeRowsInterval.x, (int)p_problems[i_pos].freeRowsInterval.y);
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
				if ((platformSizeList [0].Count + counter + platformSize) <= (p_problems[i_pos].mapSize - 2)) {
					platformSizeList [0].Add (platformSize);
					//Moneda
					probability = Random.Range (0f, 1f);
					//Si la moneda satisface ser menor que la proporcion de plataformas añadidas sobre el total o los huecos entre plataformas + todas plataformas + la ultima plataforma == tamaño util del mapa
					if ((probability <= Mathf.Lerp (0, patternSelected.maxPlatforms, platformSizeList [0].Count)) || (platformSizeList [0].Count + counter + platformSize - 1) == (p_problems[i_pos].mapSize - 2))
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
					emptySize = Random.Range (0, (p_problems[i_pos].mapSize - 1 - (counter + emptyCounter) - (platformSizeList [0].Count - emptyList [0].Count - 1)));
				else if (i < platformSizeList [0].Count)
					//El hueco va de 1 a el tamaño util del mapa - un hueco entre cada 2 plataformas y los huecos ya ocupados
					emptySize = Random.Range (1, (p_problems[i_pos].mapSize - 1 - (counter + emptyCounter) - (platformSizeList [0].Count - emptyList [0].Count - 1)));
				else
					//Se rellena lo que quede
					emptySize = p_problems[i_pos].mapSize - 2 - counter - emptyCounter;
				emptyCounter += emptySize;
				emptyList [0].Add (emptySize);

				yield return null;
			}

			//Colocar las piezas
			for (int i = (int)p_problems[i_pos].freeRowsInterval.x; i <= p_problems[i_pos].freeRowsInterval.y; i++) 
			{
				if (i == lineYPos) 
				{
					xPos = 1;
					for (int j = 0; j < emptyList [0].Count; j++) 
					{
						for (int k = 0; k < emptyList [0] [j]; k++) 
						{
							if (!p_problems[i_pos].scheme [xPos, i].Contains (CellContent.EmptySpace) && !p_problems[i_pos].scheme [xPos, i].Contains (CellContent.Platform))
								p_problems[i_pos].scheme [xPos, i].Add (CellContent.EmptySpace);
							xPos++;
							yield return null;
						}
						if (j != emptyList [0].Count - 1)
						{
							for (int k = 0; k < platformSizeList [0] [j]; k++) 
							{
								if (!p_problems[i_pos].scheme [xPos, i].Contains (CellContent.EmptySpace) && !p_problems[i_pos].scheme [xPos, i].Contains (CellContent.Platform))
									p_problems[i_pos].scheme [xPos, i].Add (CellContent.Platform);
								xPos++;
								yield return null;
							}
						}
					}
				}
				else
				{
					for (int j=1; j<=p_problems[i_pos].mapSize-2; j++)
					{
						if (!p_problems[i_pos].scheme [j, i].Contains (CellContent.EmptySpace) && !p_problems[i_pos].scheme [j, i].Contains (CellContent.Platform))
							p_problems[i_pos].scheme [j, i].Add (CellContent.EmptySpace);
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
			for (int i = 1; i <= Mathf.FloorToInt ((p_problems[i_pos].freeRowsInterval.y - p_problems[i_pos].freeRowsInterval.x + 1) / 2); i++) {
				//Define el tamaño maximo que puede tener la plataforma
				maxPlatformSize = Mathf.Min (patternSelected.maxPiecesByPlatform, (p_problems[i_pos].mapSize - 2 - counter - (patternSelected.minPiecesByPlatform * (Mathf.FloorToInt ((p_problems[i_pos].freeRowsInterval.y - p_problems[i_pos].freeRowsInterval.x + 1) / 2) - i) + (Mathf.FloorToInt ((p_problems[i_pos].freeRowsInterval.y - p_problems[i_pos].freeRowsInterval.x + 1) / 2) - (i - 1)))));
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
					emptySize = Random.Range (0, (p_problems[i_pos].mapSize - 1 - (counter + emptyCounter) - (platformSizeList [0].Count - emptyList [0].Count - 1)));
				else if (i < platformSizeList [0].Count)
					//El hueco va de 1 a el tamaño util del mapa - un hueco entre cada 2 plataformas y los huecos ya ocupados
					emptySize = Random.Range (1, (p_problems[i_pos].mapSize - 1 - (counter + emptyCounter) - (platformSizeList [0].Count - emptyList [0].Count - 1)));
				else
					//Se rellena lo que quede
					emptySize = p_problems[i_pos].mapSize - 2 - counter - emptyCounter;
				emptyCounter += emptySize;
				emptyList [0].Add (emptySize);
				yield return null;
			}

			//Colocar las piezas
			offset = 0;
			if (((int)p_problems[i_pos].freeRowsInterval.y - p_problems[i_pos].freeRowsInterval.x + 1) % 2 == 1) {
				offset = Random.Range (0, 2);
			}
			startingYPos = (int)p_problems[i_pos].freeRowsInterval.x;
			auxInt = 0;
			xPos = 1;
			for (int i = (int)p_problems[i_pos].freeRowsInterval.x; i <= p_problems[i_pos].freeRowsInterval.y; i++)
			{
				if ((i - startingYPos) % 2 == offset) 
				{
					for (int j = 1; j < xPos; j++) 
					{
						if (!p_problems[i_pos].scheme [j, i].Contains (CellContent.EmptySpace) && !p_problems[i_pos].scheme [j, i].Contains (CellContent.Platform))
							p_problems[i_pos].scheme [j, i].Add (CellContent.EmptySpace);
						yield return null;
					}
					if (auxInt < emptyList [0].Count) 
					{
						for (int j = 0; j < emptyList [0] [auxInt]; j++) 
						{
							if (!p_problems[i_pos].scheme [xPos, i].Contains (CellContent.EmptySpace) && !p_problems[i_pos].scheme [xPos, i].Contains (CellContent.Platform))
								p_problems[i_pos].scheme [xPos, i].Add (CellContent.EmptySpace);
							xPos++;
							yield return null;
						}
					}
					if (auxInt < platformSizeList [0].Count) 
					{
						for (int j = 0; j < platformSizeList [0] [auxInt]; j++) 
						{
							if (!p_problems[i_pos].scheme [xPos, i].Contains (CellContent.EmptySpace) && !p_problems[i_pos].scheme [xPos, i].Contains (CellContent.Platform))
								p_problems[i_pos].scheme [xPos, i].Add (CellContent.Platform);
							xPos++;
							yield return null;
						}
					}
					auxInt++;
					for (int j = xPos; j <= p_problems[i_pos].mapSize-2; j++) 
					{
						if (!p_problems[i_pos].scheme [j, i].Contains (CellContent.EmptySpace) && !p_problems[i_pos].scheme [j, i].Contains (CellContent.Platform))
							p_problems[i_pos].scheme [j, i].Add (CellContent.EmptySpace);
						yield return null;
					}
				} 
				else
				{
					for (int j = 1; j <= p_problems[i_pos].mapSize - 2; j++) 
					{
						if (!p_problems[i_pos].scheme [j, i].Contains (CellContent.EmptySpace) && !p_problems[i_pos].scheme [j, i].Contains (CellContent.Platform))
							p_problems[i_pos].scheme [j, i].Add (CellContent.EmptySpace);
						yield return null;
					}
				}
			}

			break;
		case PlatformPattern.ZigZag:
			//Inicializamos el almacen de tamaños de plataformas
			platformSizeList = new List<int>[Mathf.FloorToInt ((p_problems[i_pos].freeRowsInterval.y - p_problems[i_pos].freeRowsInterval.x + 1) / 2)];
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
					if ((platformSizeList [i].Count + counter + platformSize) <= (p_problems[i_pos].mapSize - 2)) {
						platformSizeList [i].Add (platformSize);
						//Moneda
						probability = Random.Range (0f, 1f);
						//Si la moneda satisface ser menor que la proporcion de plataformas añadidas sobre el total o los huecos entre plataformas + todas plataformas + la ultima plataforma == tamaño util del mapa
						if ((probability <= Mathf.Lerp (0, patternSelected.maxPlatforms, platformSizeList [i].Count)) || (platformSizeList [i].Count + counter + platformSize - 1) == (p_problems[i_pos].mapSize - 2))
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
						emptySize = Random.Range (0, (p_problems[i_pos].mapSize - 1 - (counter + emptyCounter) - (platformSizeList [i].Count - emptyList [i].Count - 1)));
					else if (i < platformSizeList [i].Count)
						//El hueco va de 1 a el tamaño util del mapa - un hueco entre cada 2 plataformas y los huecos ya ocupados
						emptySize = Random.Range (1, (p_problems[i_pos].mapSize - 1 - (counter + emptyCounter) - (platformSizeList [i].Count - emptyList [i].Count - 1)));
					else
						//Se rellena lo que quede
						emptySize = p_problems[i_pos].mapSize - 2 - counter - emptyCounter;
					emptyCounter += emptySize;
					emptyList [i].Add (emptySize);
				}
				yield return null;
			}

			//Colocar las piezas
			offset = 0;
			if (((int)p_problems[i_pos].freeRowsInterval.y - p_problems[i_pos].freeRowsInterval.x + 1) % 2 == 1) {
				offset = Random.Range (0, 2);
			}
			startingYPos = (int)p_problems[i_pos].freeRowsInterval.x;
			auxInt = 0;
			for (int i = (int)p_problems[i_pos].freeRowsInterval.x; i <= p_problems[i_pos].freeRowsInterval.y; i++) 
			{
				if ((i - startingYPos) % 2 == offset) 
				{
					xPos = 1;
					for (int j = 0; j < emptyList [auxInt].Count; j++) 
					{
						for (int k = 0; k < emptyList [auxInt] [j]; k++) 
						{
							if (!p_problems[i_pos].scheme [xPos, i].Contains (CellContent.EmptySpace) && !p_problems[i_pos].scheme [xPos, i].Contains (CellContent.Platform))
								p_problems[i_pos].scheme [xPos, i].Add (CellContent.EmptySpace);
							xPos++;
							yield return null;
						}
						if (j != emptyList [auxInt].Count - 1)
						{
							for (int k = 0; k < platformSizeList [auxInt] [j]; k++) 
							{
								if (!p_problems[i_pos].scheme [xPos, i].Contains (CellContent.EmptySpace) && !p_problems[i_pos].scheme [xPos, i].Contains (CellContent.Platform))
									p_problems[i_pos].scheme [xPos, i].Add (CellContent.Platform);
								xPos++;
								yield return null;
							}
						}
					}
				}
				else
				{
					for (int j=1; j<=p_problems[i_pos].mapSize-2; j++)
					{
						if (!p_problems[i_pos].scheme [j, i].Contains (CellContent.EmptySpace) && !p_problems[i_pos].scheme [j, i].Contains (CellContent.Platform))
							p_problems[i_pos].scheme [j, i].Add (CellContent.EmptySpace);
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