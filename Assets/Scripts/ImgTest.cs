using UnityEngine;
using System.Collections;

public class ImgTest : MonoBehaviour {

	public string imgPath;
	private ProcessedImage imagen;
	public SpriteRenderer sprite;
	private int count;
	public Texture2D texturita;

	// Use this for initialization
	void Start () {
		imagen = new ProcessedImage (imgPath);
		StartCoroutine (imagen.InitProcessedImage(this.gameObject));
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

	void DidImageInit()
	{
		StartCoroutine (imagen.ToTexture2D(texturita,this.gameObject));
	}

	void DidImageToTexture()
	{
		sprite.sprite = Sprite.Create(texturita,new Rect(0,0,10,10),new Vector2(0,0));
		Debug.Log ("¡YA TENGO SPRITEEEE!");
	}
}
