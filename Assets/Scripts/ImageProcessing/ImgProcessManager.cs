using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ImgProcessManager : Singleton<ImgProcessManager> {

	public int divisionFactor;
	public string imgPath;
	public List<ProcessedImage> images;
	public SpriteRenderer sprite;
	private int count;
	public Texture2D texturita;
	public LevelConfigList levelConfigList;

	protected ImgProcessManager () 
	{
	}
		
	// Use this for initialization

	public IEnumerator ProccesAndIndexImageCor(string i_path)
	{
		//Cojo una nueva id para la imagen
		int id = PersistenceManager.GetNewId();

		//Creo la imagen a partir de su path
		//ProcessedImage img = new ProcessedImage (i_path, id);
		ProcessedImage img = CreateImage(i_path, id);

		//Cojo una lista de ids para los hijos
		int[] idList = PersistenceManager.GetNewIdList(divisionFactor*divisionFactor);

		//La divido
		//List<ProcessedImage> tempList = img.Divide (divisionFactor,idList);
		List<ProcessedImage> tempList = DivideImage (divisionFactor,idList);

		//Guardo las imagenes procesadas
		PersistenceManager.PushImgAndChildren(img,tempList);

		//Creo el diccionario de datos de imágenes
		Dictionary<int,ProcessedImageData> tempDataDict = new Dictionary<int,ProcessedImageData> ();
		List<Coroutine> wait = new List<Coroutine> ();

		//Para cada imagen
		foreach (ProcessedImage auxImg in tempList)
		{
			//Proceso sus datos
			ProcessedImageData tempImgData = null;
			wait.Add(StartCoroutine (ProcessImageData (auxImg, tempImgData)));
			tempDataDict.Add(auxImg.GetId(),tempImgData);
		}
		foreach (Coroutine waitCor in wait) 
		{
			yield return waitCor;
		}

		//Guardo los datos de las imagenes procesadas
		PersistenceManager.PushImgData (tempDataDict);
		PersistenceManager.MainDataFlush ();
		PersistenceManager.ImgDataFlush ();
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
		StartCoroutine(ProccesAndIndexImageCor (i_path));
	}

	IEnumerator ProcessImageData (ProcessedImage i_img, ProcessedImageData i_imgData)
	{
		//Saco todos los datos necesarios
		i_imgData = i_img.GetImageData();
		//Aplico el DAC
		GameObject dacController = new GameObject("DAC Controller");
		DACMapSchemeProblem dacProblem = dacController.AddComponent<DACMapSchemeProblem> ();
		dacProblem.Config (new Vector2(1f,21f),new List<PlatformPatternConfig> (levelConfigList.GetPatterns(i_imgData.GetDificulty())),24,i_imgData.GetScheme());
		//dacProblem.Config (new Vector2(1f,21f),new List<PlatformPatternConfig> (levelConfigList.GetPatterns(0)),24,i_imgData.GetScheme());
		yield return StartCoroutine(dacProblem.DivideAndConquer ());
	}

	ProcessedImage CreateImage (string i_path, int i_id)
	{
		//Creo una textura auxiliar
		Texture2D tempText = new Texture2D(1,1);
		//Leo la imagen correspondiente
		byte[] imgRead = System.IO.File.ReadAllBytes (Application.persistentDataPath+"/"+i_path);
		//Cargo la imagen en la textura
		tempText.LoadImage (imgRead);
		ProcessedImage resImg = new ProcessedImage (i_id, i_path, tempText);
		return resImg;
	}

	List<ProcessedImage> DivideImage (int i_divisionFactor, int[] i_idList)
	{
		/*
		//Si ya se ha divido
		if (children != null && children.Count > 0)
			//Sale
			return null;
		//Calcula el ancho de cada hijo
		int childrenWidth = Mathf.CeilToInt ((float)width / i_divisionFactor);
		//Calcula el alto de cada hijo
		int childrenHeight = Mathf.CeilToInt ((float)height / i_divisionFactor);
		//Crea una lista auxiliar de imagenes
		List<ProcessedImage> tempChildren = new List<ProcessedImage> ();
		//Crea una textura auxiliar
		Texture2D tempText = new Texture2D (width, height);
		//Le asigno los pixeles de la imagen padre
		tempText.SetPixels (pixels);
		//Aplico los cambios en la textura
		tempText.Apply ();
		//Aplico un reescalado bilineal
		tempText = tempText.ResizeBilinear (childrenWidth * i_divisionFactor, childrenHeight * i_divisionFactor);
		//Saco todos los pixeles de la textura
		UnityEngine.Color[] tempPixels = tempText.GetPixels ();
		//Para cada hijo
		for (int x = 0; x < i_divisionFactor; x++) 
		{
			for (int y = 0; y < i_divisionFactor; y++) 
			{
				//Creo una lista de pixeles auxiliar
				UnityEngine.Color[] auxPixels = new UnityEngine.Color[childrenWidth * childrenHeight];
				//Para cada pixel
				for (int i = 0; i < childrenWidth; i++) {
					for (int j = 0; j < childrenHeight; j++) 
					{
						//Obtengo su posicion y obtengo el color correspondiente
						int origPos = x * childrenWidth + y * i_divisionFactor * childrenWidth * childrenHeight + i + j * childrenWidth * i_divisionFactor;
						auxPixels [i + j*childrenWidth] = new UnityEngine.Color(tempPixels [origPos].r,tempPixels [origPos].g,tempPixels [origPos].b,tempPixels [origPos].a);
					}
				}
				//Creo una imagen auxiliar
				ProcessedImage auxImg = new ProcessedImage(auxPixels,childrenWidth,childrenHeight,i_idList[x*i_divisionFactor+y]);
				//Añado el hijo al diccionario de hijos del padre
				children.Add (new Vector2 (x, y), auxImg.id);
				Debug.Log (x+","+y+"="+auxImg.id);
				//Añado el hijo a la lista de hijos
				tempChildren.Add(auxImg);
			}
		}
		return tempChildren;
		*/
		return null;
	}
}

