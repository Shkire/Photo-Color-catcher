using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ProcessedImage{

	private int id;

	private string path;

	private UnityEngine.Color[] pixels;

	private int width;

	private int height;

	private Dictionary<Vector2,int> children;

	private bool completed;

	public ProcessedImage(string i_path, int i_id)
	{
		//Le asigno un id
		id = i_id;
		//Le asigno su path
		path = i_path;
		//Creo una textura auxiliar
		Texture2D tempText = new Texture2D(1,1);
		//Leo la imagen correspondiente
		byte[] imgRead = System.IO.File.ReadAllBytes (Application.persistentDataPath+"/"+path);
		//Cargo la imagen en la textura
		tempText.LoadImage (imgRead);
		//Le asigno su ancho
		width = tempText.width;
		//Le asigno su ancho
		height = tempText.height;
		//Obtengo sus pixels y los guardo
		pixels = tempText.GetPixels();
		//Inicializo su diccionario de hijos
		children = new Dictionary<Vector2, int> ();
		//No está superada
		completed = false;
	}

	public ProcessedImage(UnityEngine.Color[] i_pixels, int i_width, int i_height, int i_id)
	{
		id = i_id;
		path = string.Empty;
		width = i_width;
		height = i_height;
		pixels = i_pixels;
		children = null;
		completed = false;
	}
	public ProcessedImage(int i_id, string i_path, UnityEngine.Color[] i_pixels, int i_width, int i_height, Dictionary<Vector2,int> i_children, bool i_completed)
	{
		id = i_id;
		path = i_path;
		pixels = i_pixels;
		width = i_width;
		height = i_height;
		children = i_children;
		completed = i_completed;
	}

	//Sacar al ProcessManager para que lo haga con un DAC a ser posible
	public List<ProcessedImage> Divide(int i_divisionFactor, int[] i_idList)
	{
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
	}

	public ProcessedImageData GetImageData()
	{
		float redData=0;
		float greenData=0;
		float blueData=0;
		float grayData = 0;
		foreach (UnityEngine.Color pixel in pixels) 
		{
			redData += pixel.r;
			greenData += pixel.g;
			blueData += pixel.b;
			grayData += pixel.grayscale;
		}
		float redSaturation = redData / pixels.Length;
		float greenSaturation = greenData / pixels.Length;
		float blueSaturation = blueData / pixels.Length;
		float totalData = redSaturation + greenSaturation + blueSaturation;
		redData = redSaturation / totalData;
		greenData = greenSaturation / totalData;
		blueData = blueSaturation / totalData;
		grayData = grayData / (width * height);
		return new ProcessedImageData (redData,greenData,blueData,redSaturation,greenSaturation,blueSaturation,grayData);
	}

	public int GetId()
	{
		return id;
	}

	public List<int> GetChildrenId()
	{
		List<int> ids = new List<int> ();
		foreach (int id in children.Values)
			ids.Add (id);
		return ids;
	}

	public int GetChildrenCount()
	{
		return GetChildrenId ().Count;
	}

	public void SetCompleted(bool i_completed)
	{
		completed = i_completed;
	}

	public bool IsCompleted()
	{
		return completed;
	}
		
	public int GetChildId(int x, int y)
	{
		return children [new Vector2 ((float)x, (float)y)];
	}

	public Texture2D ToTexture2D()
	{
		Texture2D text = new Texture2D (width, height);
		text.SetPixels (pixels);
		text.Apply();
		return text;
	}

	public Texture2D ToGrayscaleTexture2D()
	{
		Texture2D text = new Texture2D (width, height);
		UnityEngine.Color[] tempPixels = new UnityEngine.Color[pixels.Length];
		for (int i = 0; i < pixels.Length; i++)
			tempPixels [i] = new UnityEngine.Color (pixels [i].grayscale, pixels [i].grayscale, pixels [i].grayscale);
		text.SetPixels (tempPixels);
		text.Apply();
		return text;
	}

	public List<Vector2> GetChildrenPos()
	{
		return new List<Vector2> (children.Keys);
	}

	public Color[] GetPixels()
	{
		return pixels;
	}
}
