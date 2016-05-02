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

	public void ProccesAndIndexImage(string i_path)
	{
		PersistenceManager.MainLoad ();
		ProcessedImage img = new ProcessedImage (i_path);
		img.Divide (divisionFactor);
		Dictionary<int,ProcessedImageData> tempDataDict = new Dictionary<int,ProcessedImageData> ();
		foreach (int id in img.GetChildrenId())
			tempDataDict.Add(id,PersistenceManager.GetImage (id).GetImageData ());
		PersistenceManager.LevelDataSave (tempDataDict);
	}
	void Start () {
		ProccesAndIndexImage (imgPath);
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

	void LoadImage(string path)
	{
		images.Add (new ProcessedImage (path));
	}

	void DivideImage (int idx)
	{
		if (images [idx] != null)
			//StartCoroutine(images [idx].Divide (divisionFactor));
			images[idx].Divide(divisionFactor);
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
		
}
