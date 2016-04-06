using UnityEngine;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;

[System.Serializable]
public class ProcessedImage{

	private string id;

	private string path;

	private UnityEngine.Color[] pixels;

	private int width;

	private int height;

	private Dictionary<Vector2,string> children;

	/*
	public ProcessedImage()
	{
		img2 = null;
	}
	*/

	public ProcessedImage(string i_path)
	{
		id = PersistenceManager.GetNewId();
		path = i_path;
		Texture2D tempText = new Texture2D(1,1);
		byte[] imgRead = System.IO.File.ReadAllBytes (Application.persistentDataPath+"/"+path);
		tempText.LoadImage (imgRead);
		width = tempText.width;
		height = tempText.height;
		pixels = tempText.GetPixels();
		children = new Dictionary<Vector2, string> ();
	}

	/*
	public IEnumerator InitProcessedImage(GameObject callingGo)
	{
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y <height; y++)
			{
				SetRGBValue (img.GetPixel (x, y).R, img.GetPixel (x, y).G, img.GetPixel (x, y).B, x, y);
			}
			yield return null;
		}
		callingGo.SendMessage ("DidImageInit");
	}
	*/

	/*
	public void SetRGBValue (int rValue, int gValue, int bValue, int xValue, int yValue)
	{
		rData [xValue, yValue] = rValue;
		gData [xValue, yValue] = gValue;
		bData [xValue, yValue] = bValue;
	}

	private int[] GetRGBValue (int xValue, int yValue)
	{
		return new int[]{rData [xValue, yValue], gData [xValue, yValue], bData [xValue, yValue]};
	}

	public IEnumerator ToTexture2D(GameObject callingGo)
	{
		Texture2D text = new Texture2D (width, height);
		for (int x = 0; x < width; x++) 
		{
			for (int y = 0; y < height; y++) 
			{
				UnityEngine.Color color = new UnityEngine.Color (Mathf.InverseLerp(0f,255f,(GetRGBValue(x,y)[0])),Mathf.InverseLerp(0f,255f,(GetRGBValue(x,y)[1])),Mathf.InverseLerp(0f,255f,(GetRGBValue(x,y)[2])));
				text.SetPixel(x, height-y-1,color);
				Debug.Log("X:"+x+",Y:"+y+":"+text.GetPixel (x, y));
			}
			yield return null;
		}
		text.Apply ();
		byte[] savingBytes;
		savingBytes = text.EncodeToPNG ();
		System.IO.File.WriteAllBytes (Application.persistentDataPath + "/pruebaTextura.png", savingBytes);
		GameObject jameobjet = new GameObject ();
		jameobjet.AddComponent<SpriteRenderer>();
		jameobjet.GetComponent<SpriteRenderer>().sprite=Sprite.Create(text,new Rect(0,0,width,height),new Vector2(0,0));
		Debug.Log ("SACABÓOOO");
		callingGo.SendMessage ("DidImageToTexture",text);
	}


	public IEnumerator ToString()
	{
		for (int x = width-1; x >= 0; x--) 
		{
			for (int y = height-1; y >=0; y--) 
			{
				Debug.Log ("X:"+x+";Y:"+y+"=("+rData[x,y]+","+gData[x,y]+","+bData[x,y]+")");
			}
			yield return null;
		}
	}
	*/

	public void Divide(int i_divisionFactor)
	{
		int childrenWidth = Mathf.CeilToInt ((float)width / i_divisionFactor);
		int childrenHeight = Mathf.CeilToInt ((float)height / i_divisionFactor);
		Texture2D tempText = new Texture2D (1, 1);
		tempText.SetPixels (pixels);
		tempText = tempText.ResizeBilinear (childrenWidth * i_divisionFactor, childrenHeight * i_divisionFactor);
		for (int x = 0; x < i_divisionFactor; x++) 
		{
			for (int y = 0; y < i_divisionFactor; y++) 
			{
				UnityEngine.Color[] auxPixels = new UnityEngine.Color[childrenWidth * childrenHeight];
				for (int i = 0; i < childrenWidth; i++) {
					for (int j = 0; j < childrenHeight; j++) 
					{
						int origPos = x * childrenWidth + y * i_divisionFactor * childrenWidth * childrenHeight + i + j * childrenWidth * i_divisionFactor;
						auxPixels [i + j*childrenWidth] = new UnityEngine.Color(pixels [origPos].r,pixels [origPos].g,pixels [origPos].b,pixels [origPos].a);
					}
				}
				ProcessedImage auxImg = new ProcessedImage(auxPixels);
				children.Add (new Vector2 (x, y), auxImg.id);
			}
		}
	}
}
