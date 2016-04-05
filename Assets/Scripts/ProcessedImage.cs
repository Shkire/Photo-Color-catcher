using UnityEngine;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;

[System.Serializable]
public class ProcessedImage{

	public string id;

	private int[,] rData;

	private int[,] gData;

	private int[,] bData;

	public int width;

	public int height;

	public Bitmap img;

	public Texture2D img2;

	public Texture2D img3;

	//public Dictionary<Vector2,ProcessedImage> children;
	public List<ProcessedImage> children;

	public ProcessedImage()
	{
		img2 = null;
	}

	public ProcessedImage(string path)
	{
		img2 = new Texture2D(1,1);
		byte[] imgRead = System.IO.File.ReadAllBytes (Application.persistentDataPath+"/"+path);
		img2.LoadImage (imgRead);
		width = img2.width;
		height = img2.height;
		//img = new Bitmap (Application.persistentDataPath+"/"+path);
		//width = img.Width;
		Debug.Log ("Width: " + img2.width);
		//height = img.Height;
		Debug.Log ("Height: " + img2.height);
		//rData = new int[img.Width, img.Height];
		//gData = new int[img.Width, img.Height];
		//bData = new int[img.Width, img.Height];
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

	public IEnumerator Divide(int divisionFactor)
	{
		int childrenWidth = Mathf.CeilToInt ((float)width / divisionFactor);
		//Debug.Log ("Children width unceiled:"+((float)width / divisionFactor));
		//Debug.Log ("Children width:"+childrenWidth);
		int childrenHeight = Mathf.CeilToInt ((float)height / divisionFactor);
		//Debug.Log ("Children height unceiled:"+(height / divisionFactor));
		//Debug.Log ("Children height:"+childrenHeight);
		//Texture2D
		img3 = img2.ResizeBilinear (childrenWidth * divisionFactor, childrenHeight * divisionFactor);
		Debug.Log ("IMG3: 20,20:"+img3.GetPixel(20,20));
		//img3.Resize (childrenWidth * divisionFactor, childrenHeight * divisionFactor);
		Debug.Log ("IMG3: 20,20:"+img3.GetPixel(20,20));
		img3.Apply ();
		Debug.Log ("Img resized size:"+img3.width+","+img3.height);
		Debug.Log ("IMG2: 20,20:"+img2.GetPixel(20,20));
		Debug.Log ("IMG3: 20,20:"+img3.GetPixel(20,20));
		//children = new Dictionary<Vector2,ProcessedImage> ();
		children = new List<ProcessedImage>();
		for (int x = 0; x < divisionFactor; x++) 
		{
			for (int y = 0; y < divisionFactor; y++) 
			{
				Texture2D auxText = new Texture2D (childrenWidth,childrenHeight);
				for (int i=0; i<childrenWidth;i++)
				{
					for (int j=0; j<childrenHeight;j++)
					{
						auxText.SetPixel(i,j,img3.GetPixel((childrenWidth*x)+i,(childrenHeight*y)+j));
					}

					yield return null;
				}
				auxText.Apply ();
				ProcessedImage auxImg = new ProcessedImage ();
				auxImg.img2 = auxText;
				Debug.Log ("Width: " + auxImg.img2.width);
				Debug.Log ("Height: " + auxImg.img2.height);
				//children.Add (new Vector2 (x, y), auxImg);
				children.Add(auxImg);
			}
		}
	}
}
