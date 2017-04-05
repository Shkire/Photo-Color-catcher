using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using PlatformPattern = PlatformPatternConfig.PlatformPattern;
using CellContent = MapScheme.CellContent;

/// <summary>
/// Auxiliary class for solving "Divide and Conquer" problem of image data analysis and processing.
/// </summary>
public class ProcessData_DAC_Problem{

	/// <summary>
	/// The interval of rows to fill.
	/// </summary>
	public Vector2 freeRowsInterval;
	/// <summary>
	/// The available patterns for filling the rows.
	/// </summary>
	public List<PlatformPatternConfig> availablePatterns;
	/// <summary>
	/// The minimum size of the available patterns.
	/// </summary>
	public int minSize;
	/// <summary>
	/// The maximum size of the available patterns.
	/// </summary>
	public int maxSize;
	/// <summary>
	/// The size of the map.
	/// </summary>
	public int mapSize;
	/// <summary>
	/// The scheme of cells on map.
	/// </summary>
	public List<CellContent>[,] scheme;

}
	
/// <summary>
/// Singleton object for image processing.
/// </summary>
public class ImgProcessManager : Singleton<ImgProcessManager> {

	/// <summary>
	/// The max pixels processed per frame.
	/// </summary>
	public int maxPixelsProcessedPerFrame;
	/// <summary>
	/// The number of images in which to divide per column and row (n x n images in total).
	/// </summary>
	public int divisionFactor;

	/// <summary>
	/// The size of the map.
	/// </summary>
	public int mapSize;

	public LevelConfigList levelConfigList;

	#region PrivateVarForCoroutines

	//A EVALUAR
	private List<ProcessData_DAC_Problem> p_problems;
	#endregion

	//Protected constructor of singleton
	protected ImgProcessManager () 
	{
	}

	/// <summary>
	/// Processes and indexes the image of the path.
	/// </summary>
	/// <param name="i_path">Image path.</param>
	public void ProcessAndIndexImage(string i_path)
	{
		StartCoroutine(ProcessAndIndexImageCor (i_path));
	}

	void Start () {
		PersistenceManager.MainLoad ();
	}

	/*
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
	*/

	#region ImageProcessingAndDivision

	/// <summary>
	/// Coroutine that processes and indexes the image.
	/// </summary>
	/// <param name="i_path">The path of the image to process.</param>
	public IEnumerator ProcessAndIndexImageCor(string i_path)
	{
		ProcessedImage parentImg = null;
		List<ProcessedImage> imgList = null;

		//Get a new ID
		int id = PersistenceManager.GetNewId();

		yield return null;

		//Start image creation coroutine and wait until finished
		yield return (StartCoroutine(CreateImage(i_path, id, parentImg)));

		yield return null;

		//Get list with IDs for children
		int[] idList = PersistenceManager.GetNewIdList(divisionFactor*divisionFactor);

		yield return null;

		//Start image division coroutine and wait until finished
		yield return StartCoroutine(DivideImage(divisionFactor, idList, parentImg, imgList));

		yield return null;

		//Push parent and children images info
		PersistenceManager.PushImgAndChildren(parentImg,imgList);

		yield return null;

		Debug.Log("Creo el diccionario de datos de imagenes");
		//Creo el diccionario de datos de imágenes
		Dictionary<int,ProcessedImageData> tempDataDict = new Dictionary<int,ProcessedImageData> ();
		List<Coroutine> wait = new List<Coroutine> ();
		Debug.Log("Diccionario y lista de corrutinas creados");

		Debug.Log("Bucle for para cada imagen a analizar");
		//Para cada imagen
		foreach (ProcessedImage auxImg in imgList)
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
		
	/// <summary>
	/// Coroutine that creates the processed image from image path.
	/// </summary>
	/// <param name="i_path">Path of the image.</param>
	/// <param name="i_id">ID of the image.</param>
	/// <param name="i_img">Image object, that will be written.</param>
	IEnumerator CreateImage (string i_path, int i_id, ProcessedImage i_img)
	{
		//Creates auxiliary Texture2D
		Texture2D tempText = new Texture2D(1,1);

		yield return null;

		//Reads image bytes from file path
		byte[] imgRead = System.IO.File.ReadAllBytes (Application.persistentDataPath+"/"+i_path);

		yield return null;

		//Loads image into Texture
		tempText.LoadImage (imgRead);

		yield return null;

		//Creates the ProcessedImage
		i_img = new ProcessedImage (i_id, i_path, tempText);
	}
		
	/// <summary>
	/// Coroutine that divides the image into children images.
	/// </summary>
	/// <param name="i_divisionFactor">Number of children per row and column.</param>
	/// <param name="i_idList">List of IDs for children.</param>
	/// <param name="i_img">Parent image.</param>
	/// <param name="i_imgList">List of images where children will be added.</param>
	IEnumerator DivideImage (int i_divisionFactor, int[] i_idList, ProcessedImage i_img, List<ProcessedImage> i_imgList)
	{
		//If it has ben divided yet
		if (i_img.GetChildrenCount() > 0)
			//Breaks
			yield break;

		//Calculates child width
		int childrenWidth = Mathf.CeilToInt ((float)i_img.width / i_divisionFactor);
		//Calculates child height
		int childrenHeight = Mathf.CeilToInt ((float)i_img.height / i_divisionFactor);
		//Initializes img list
		i_imgList = new List<ProcessedImage> ();

		yield return null;

		//Creates auxiliar texture with parent image size
		Texture2D tempText = new Texture2D (i_img.width, i_img.height);
		//Loads parent image into texture
		tempText.SetPixels (i_img.pixels);
		//Apply texture changes
		tempText.Apply ();

		yield return null;

		//Resizes texture
		tempText = tempText.ResizeBilinear (childrenWidth * i_divisionFactor, childrenHeight * i_divisionFactor);

		yield return null;

		//Gets texture pixels
		UnityEngine.Color[] tempPixels = tempText.GetPixels ();

		yield return null;

		//List of coroutines to wait
		List<Coroutine> waitChildren = new List<Coroutine> ();

		for (int x = 0; x < i_divisionFactor; x++) 
		{
			for (int y = 0; y < i_divisionFactor; y++) 
			{
				//Start child creation coroutine and adds it to coroutine list
				waitChildren.Add (StartCoroutine (CreateChild (new Vector2((float)x,(float)y), childrenWidth, childrenHeight, i_divisionFactor,i_idList,i_img,i_imgList,tempPixels)));
			}
		}

		//Waits for all coroutines
		foreach (Coroutine wait in waitChildren) 
		{
			yield return wait;
		}
	}

	//Problema de escritura de hijos??
	//Guardar bytes en vez de pixels??
	//Guardar imagen original en vez de partida??
	/// <summary>
	/// Coroutine that creates a child image.
	/// </summary>
	/// <param name="i_pos">Child image position.</param>
	/// <param name="i_width">Child image width.</param>
	/// <param name="i_height">Child image height.</param>
	/// <param name="i_divisionFactor">Number of children per row and column.</param>
	/// <param name="i_idList">List of IDs for child creation.</param>
	/// <param name="i_img">Parent image.</param>
	/// <param name="i_imgList">Children image list.</param>
	/// <param name="i_tempPixels">Parent image pixels.</param>
	IEnumerator CreateChild(Vector2 i_pos, int i_width, int i_height, int i_divisionFactor, int[] i_idList, ProcessedImage i_img, List<ProcessedImage> i_imgList, UnityEngine.Color[] i_tempPixels)
	{
		//Auxiliary pixel array for pixel copy
		UnityEngine.Color[] auxPixels = new UnityEngine.Color[i_width * i_height];

		int counter = 0;
		for (int i = 0; i < i_width; i++) {
			for (int j = 0; j < i_height; j++,counter++) 
			{
				//Calculates pixel position on parent image
				int origPos = (int)i_pos.x * i_width + (int)i_pos.y * i_divisionFactor * i_width * i_height + i + j * i_width * i_divisionFactor;
				//Copies pixel to child position in auxiliary array
				auxPixels [i + j*i_width] = new UnityEngine.Color(i_tempPixels [origPos].r,i_tempPixels [origPos].g,i_tempPixels [origPos].b,i_tempPixels [origPos].a);

				//If number of pixels copied this frame is maxPixelsProcessedPerFrame continues in next frame
				if (counter % maxPixelsProcessedPerFrame == maxPixelsProcessedPerFrame-1) 
				{
					yield return null;
				}	

			}
		}

		yield return null;

		//Creates an auxiliay child image
		ProcessedImage auxImg = new ProcessedImage(auxPixels,i_width,i_height,i_idList[(int)i_pos.x*i_divisionFactor+(int)i_pos.y]);

		//Adds child image ID to parent's children ID dict
		i_img.AddChild (i_pos, auxImg.id);

		//Adds child image to children list
		i_imgList.Add(auxImg);

		yield return null;
	}

	#endregion

	#region ImageDataAnalysis

	IEnumerator ProcessImageData (ProcessedImage i_img, ProcessedImageData i_imgData)
	{
		//Saco todos los datos necesarios
		//i_imgData = i_img.GetImageData();
		//Aplico el DAC
		//GameObject dacController = new GameObject("DAC Controller");
		float redPercentage = 0;
		float greenPercentage = 0;
		float bluePercentage = 0;
		float grayAverage = 0;
		float maxValueAverage = 0;
		foreach (UnityEngine.Color pixel in i_img.pixels) 
		{
			redPercentage += pixel.r;
			greenPercentage += pixel.g;
			bluePercentage += pixel.b;
			grayAverage += pixel.grayscale;
			maxValueAverage += pixel.maxColorComponent;
		}
		float redSaturation = redPercentage / i_img.pixels.Length;
		float greenSaturation = greenPercentage / i_img.pixels.Length;
		float blueSaturation = bluePercentage / i_img.pixels.Length;
		float totalData = redSaturation + greenSaturation + blueSaturation;
		redPercentage = redSaturation / totalData;
		greenPercentage = greenSaturation / totalData;
		bluePercentage = blueSaturation / totalData;
		grayAverage = grayAverage / i_img.pixels;
		maxValueAverage = maxValueAverage / i_img.pixels;





		//Para que???
		i_imgData = new ProcessedImageData (redData,greenData,blueData,redSaturation,greenSaturation,blueSaturation,grayData);
		/*
		p_problems = new List<ProcessData_DAC_Problem> ();
		p_problems.Add (new ProcessData_DAC_Problem ());
		ProcessData_DAC_Config (0, new Vector2(1f,21f),new List<PlatformPatternConfig> (levelConfigList.GetPatterns(i_imgData.GetDificulty())),24,i_imgData.GetScheme());
		//dacProblem.Config (new Vector2(1f,21f),new List<PlatformPatternConfig> (levelConfigList.GetPatterns(0)),24,i_imgData.GetScheme());
		yield return StartCoroutine(ProcessDataDivideAndConquer (0));
		*/
		//Dividir en tantas piezas como extension tenga el mapa
		/*
		 * Coger cada hijo como en createchild y calcular media
		 * Almacenar en array los valores medios de los hijos
		 * Calcular medias por columna
		 * for que rellene plataformas
		 * */
		Texture2D auxText = new Texture2D (i_img.width, i_img.height);
		auxText.SetPixels (i_img.pixels);
		auxText.Apply;
		int cellWidth = Mathf.CeilToInt ((float)auxText.width / (mapSize-2));
		int cellHeight = Mathf.CeilToInt ((float)auxText.height / (mapSize-2));
		auxText = auxText.ResizeBilinear (cellWidth*(mapSize-2),cellHeight*(mapSize-2));

		float[,] cellsData = new float[mapSize-2,mapSize-2]();

		//List of coroutines to wait
		List<Coroutine> waitCells = new List<Coroutine> ();

		for (int x = 0; x < mapSize-2; x++) 
		{
			for (int y = 0; y < mapSize-2; y++) 
			{
				//Start child creation coroutine and adds it to coroutine list
				waitCells.Add (StartCoroutine (GetCellsData (new Vector2((float)x,(float)y), auxText.width, auxText.height, mapSize-2,auxText.GetPixels(),)));
			}
		}

		//Waits for all coroutines
		foreach (Coroutine wait in waitCells) 
		{
			yield return wait;
		}

		List<CellContent>[,] scheme = new List<CellContent>[mapSize,mapSize]();
		float[] columnData = new float[mapSize-2]();
		float[] rowData = new float[mapSize-2]();

		for (int i = 0; i<mapSize-2; i++)
		{
			for (int j = 0; j<mapSize-2; j++)
			{
				columnData[i] += cellsData[i,j]/(mapSize-2);
				rowData[j] += cellsData[i,j]/(mapSize-2);
			}
		}

		for (int i = 0; i<mapSize-2; i++)
		{
			for (int j = 0; j<mapSize-2; j++)
			{
				if (scheme[i+1,j+1] != null)
				{
					scheme[i+1,j+1] = new List<CellContent>();
					if (cellsData[i,j] >= rowData[j])
					{
						scheme[i+1,j+1].Add(CellContent.Platform);
						scheme[i+1,j+2] = new List<CellContent>();
						scheme[i+1,j+2].Add(CellContent.EmptySpace);
					}
					else
					{
						scheme[i+1,j+1].Add(CellContent.EmptySpace);	
					}
				}
			}
		}

		scheme[0,0] = new List<CellContent>();
		scheme[0,0].Add(CellContent.Platform);
		scheme[0,mapSize-1] = new List<CellContent>();
		scheme[0,mapSize-1].Add(CellContent.Platform);
		scheme[mapSize-1,0] = new List<CellContent>();
		scheme[mapSize-1,0].Add(CellContent.Platform);
		scheme[mapSize-1,mapSize-1] = new List<CellContent>();
		scheme[mapSize-1,mapSize-1].Add(CellContent.Platform);

		float columnValue;

		for (int i=0; i<columnData.Length; i++)
		{
			columnValue += columnData[i]/columnData.Length;
		}

		float rowValue;

		for (int i=0; i<rowData.Length; i++)
		{
			rowValue += rowData[i]/columnData.Length;
		}

		for (int i=0; i<columnData.Length; i++)
		{
			scheme[i+1,0] = new List<CellContent>();
			scheme[i+1,mapSize-1] = new List<CellContent>();
			if (columnData[i]>=columnValue)
			{
				scheme[i+1,0].Add(CellContent.Platform);
				scheme[i+1,mapSize-1].Add(CellContent.Platform);
			}
			else
			{
				if (columnData[i]>=columnValue)
				{
					scheme[i+1,0].Add(CellContent.EmptySpace);
					scheme[i+1,mapSize-1].Add(CellContent.EmptySpace);
				}
			}
		}

		//Ir rellenando array de plataformas
	}

	public IEnumerator GetCellsData(Vector2 i_pos, int i_width, int i_height, int i_mapSize, UnityEngine.Color[] i_tempPixels, float[,] i_cellsData)
	{

		//int counter = 0;
		int i;
		int j;
		for (i = 0; i < i_width; i++) {
			for (j = 0; j < i_height; j++/*,counter++*/) 
			{
				//Calculates pixel position on parent image
				int origPos = (int)i_pos.x * i_width + (int)i_pos.y * i_mapSize * i_width * i_height + i + j * i_width * i_mapSize;
				i_cellsData [i_pos.x, i_pos.y] += (i_tempPixels [origPos].r + i_tempPixels [origPos].g + i_tempPixels [origPos].b) / 3;

				/*
				//If number of pixels copied this frame is maxPixelsProcessedPerFrame continues in next frame
				if (counter % maxPixelsProcessedPerFrame == maxPixelsProcessedPerFrame-1) 
				{
					yield return null;
				}
				*/
			}
		}
		i_cellsData [i_pos.x, i_pos.y] = i_cellsData [i_pos.x, i_pos.y] / (i * j);
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

	#endregion
}