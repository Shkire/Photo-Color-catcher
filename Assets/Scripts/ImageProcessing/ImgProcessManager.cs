using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using PlatformPattern = PlatformPatternConfig.PlatformPattern;
using CellContent = MapScheme.CellContent;

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
		grayAverage = grayAverage / i_img.pixels.Length;
		maxValueAverage = maxValueAverage / i_img.pixels.Length;





		//Para que???
		//i_imgData = new ProcessedImageData (redData,greenData,blueData,redSaturation,greenSaturation,blueSaturation,grayData);
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
		auxText.Apply();
		int cellWidth = Mathf.CeilToInt ((float)auxText.width / (mapSize-2));
		int cellHeight = Mathf.CeilToInt ((float)auxText.height / (mapSize-2));
		auxText = auxText.ResizeBilinear (cellWidth*(mapSize-2),cellHeight*(mapSize-2));

		float[,] cellsData = new float[mapSize-2,mapSize-2];

		//List of coroutines to wait
		List<Coroutine> waitCells = new List<Coroutine> ();

		for (int x = 0; x < mapSize-2; x++) 
		{
			for (int y = 0; y < mapSize-2; y++) 
			{
				//Start child creation coroutine and adds it to coroutine list
				waitCells.Add (StartCoroutine (GetCellsData (new Vector2((float)x,(float)y), auxText.width, auxText.height, mapSize-2,auxText.GetPixels(),cellsData)));
			}
		}

		//Waits for all coroutines
		foreach (Coroutine wait in waitCells) 
		{
			yield return wait;
		}

		List<CellContent>[,] scheme = new List<CellContent>[mapSize,mapSize];
		float[] columnData = new float[mapSize-2];
		float[] rowData = new float[mapSize-2];

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

		float columnValue = 0;

		for (int i=0; i<columnData.Length; i++)
		{
			columnValue += columnData[i]/columnData.Length;
		}

		float rowValue = 0;

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
		int i = 0;
		int j = 0;
		for (i = 0; i < i_width; i++) {
			for (j = 0; j < i_height; j++/*,counter++*/) 
			{
				//Calculates pixel position on parent image
				int origPos = (int)i_pos.x * i_width + (int)i_pos.y * i_mapSize * i_width * i_height + i + j * i_width * i_mapSize;
				i_cellsData [(int)i_pos.x, (int)i_pos.y] += ((i_tempPixels [origPos].r + i_tempPixels [origPos].g + i_tempPixels [origPos].b) / 3);

				/*
				//If number of pixels copied this frame is maxPixelsProcessedPerFrame continues in next frame
				if (counter % maxPixelsProcessedPerFrame == maxPixelsProcessedPerFrame-1) 
				{
					yield return null;
				}
				*/
			}
		}
		i_cellsData [(int)i_pos.x, (int)i_pos.y] = i_cellsData [(int)i_pos.x, (int)i_pos.y] / (i * j);

		yield return null;
	}
		


	#endregion
}