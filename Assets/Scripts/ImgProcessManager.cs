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

	protected ImgProcessManager () 
	{
	}
		
	// Use this for initialization
	void Start () {
		images = new List<ProcessedImage> ();
		LoadImage (imgPath);
		DivideImage (0);
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
			StartCoroutine(images [idx].Divide (divisionFactor));
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
		
}
