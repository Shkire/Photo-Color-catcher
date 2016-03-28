using UnityEngine;
using System.Collections;
using System.Drawing;

public class ProcessedImage{

	private int[,] rData;

	private int[,] gData;

	private int[,] bData;

	public int width;

	public int height;

	public Bitmap img;

	public ProcessedImage(string path)
	{
		img = new Bitmap (Application.persistentDataPath+"/"+path);
		width = img.Width;
		Debug.Log ("Width: " + width);
		height = img.Height;
		Debug.Log ("Height: " + height);
		rData = new int[img.Width, img.Height];
		gData = new int[img.Width, img.Height];
		bData = new int[img.Width, img.Height];
	}

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

	public IEnumerator ToTexture2D(Texture2D text,GameObject callingGo)
	{
		text = new Texture2D (width, height);
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
		callingGo.SendMessage ("DidImageToTexture");
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
}
