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

	private ProcessedImage p_img;
	private List<ProcessedImage> p_imgList;
	private UnityEngine.Color[] p_tempPixels;
	private int[] p_idList;

	protected ImgProcessManager () 
	{
	}
		
	// Use this for initialization

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

		//Creo el diccionario de datos de imágenes
		Dictionary<int,ProcessedImageData> tempDataDict = new Dictionary<int,ProcessedImageData> ();
		List<Coroutine> wait = new List<Coroutine> ();

		//Para cada imagen
		foreach (ProcessedImage auxImg in p_imgList)
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
		for (int i = 0; i < i_width; i++) {
			for (int j = 0; j < i_height; j++) 
			{
				//Obtengo su posicion y obtengo el color correspondiente
				int origPos = i_x * i_width + i_y * i_divisionFactor * i_width * i_height + i + j * i_width * i_divisionFactor;
				auxPixels [i + j*i_width] = new UnityEngine.Color(p_tempPixels [origPos].r,p_tempPixels [origPos].g,p_tempPixels [origPos].b,p_tempPixels [origPos].a);

				yield return null;
			}
		}
		//Creo una imagen auxiliar
		ProcessedImage auxImg = new ProcessedImage(auxPixels,i_width,i_height,p_idList[i_x*i_divisionFactor+i_y]);
		//Añado el hijo al diccionario de hijos del padre
		p_img.AddChild (new Vector2 (i_x, i_y), auxImg.id);
		Debug.Log (i_x+","+i_y+"="+auxImg.id);
		//Añado el hijo a la lista de hijos
		p_imgList.Add(auxImg);

		yield return null;
	}
}

