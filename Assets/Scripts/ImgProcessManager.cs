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

	public IEnumerator ProccesAndIndexImage(string i_path)
	{
		//Cojo una nueva id para la imagen
		int id = PersistenceManager.GetNewId();
		//Creo la imagen a partir de su path
		ProcessedImage img = new ProcessedImage (i_path, id);
		//Cojo una lista de ids para los hijos
		int[] idList = PersistenceManager.GetNewIdList(divisionFactor*divisionFactor);
		//La divido
		List<ProcessedImage> tempList = img.Divide (divisionFactor,idList);
		//Guardo las imagenes procesadas
		PersistenceManager.ProcessedLevelSave(img,tempList);
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
		PersistenceManager.LevelDataSave (tempDataDict);
	}


	void Start () {
		PersistenceManager.MainLoad ();
		//ProccesAndIndexImage (imgPath);
		//PersistenceManager.LoadImageList ();
		//images = new List<ProcessedImage> ();
		//LoadImage (imgPath);
		//Debug.Log ("Dividing image");
		//DivideImage (0);
		//foreach (int index in PersistenceManager.GetAllIds())
			//ImgProcessManager.Instance.Instantiate (index, "Creada");
		//PersistenceManager.SaveImage (images[0]);
		//imagen = new ProcessedImage (imgPath);
		//StartCoroutine (imagen.InitProcessedImage(this.gameObject));
	}
	/*
	void FixedUpdate(){
		StartCoroutine (imagen.ToTexture2D (texturita));
	}
	*/
	// Update is called once per frame
	/*
	void FixedUpdate () {
		if ((Input.GetKey(KeyCode.Q)) && count < imagen.width + imagen.height) 
		{
			imagen.SetRGBValue (255, 255, 255, count / imagen.width, count % imagen.width);
			count++;
		}
		sprite.sprite = Sprite.Create(imagen.ToTexture2D(),new Rect(0,0,10,10),new Vector2(0,0));
	}
	*/

	/*
	private IEnumerator InitImageProcessing()
	{
		imagen=new ProcessedImage (Application.persistentDataPath+"/"+imgPath);
		sprite.sprite = Sprite.Create(imagen.ToTexture2D(),new Rect(0,0,10,10),new Vector2(0,0));
		yield return null;
	}
	*/

	/*
	void LoadImage(string path)
	{
		images.Add (new ProcessedImage (path));
	}
	*/

	/*
	void DivideImage (int idx)
	{
		if (images [idx] != null)
			//StartCoroutine(images [idx].Divide (divisionFactor));
			images[idx].Divide(divisionFactor);
	}
	*/

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
		
	public void ProccesAndIndexImage()
	{
		StartCoroutine(ProccesAndIndexImage (imgPath));
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
}

