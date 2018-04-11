using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class RGB_Content
{
    public bool r;

    public bool g;

    public bool b;

    public RGB_Content(bool i_r, bool i_g, bool i_b)
    {
        r = i_r;
        g = i_g;
        b = i_b;
    }
}

[System.Serializable]
public class OnMatrixImage
{

    public UnityEngine.Color[] pixels;

    public int width;

    public int height;

    public UnityEngine.Color average;

    public float grayscale;

    public RGB_Content goal;

    public OnMatrixImage(int i_width, int i_height)
    {
        width = i_width;
        height = i_height;
        pixels = new Color[width * height];
    }

    public void ResizeBilinear(int i_newWidth, int i_newHeight)
    {
        if (i_newWidth != width || i_newHeight != height)
        {
            //Creates auxiliar texture
            Texture2D tempText = new Texture2D(width, height);
            //Loads parent image
            tempText.SetPixels(pixels);
            //Apply texture changes
            tempText.Apply();
            //Resizes texture
            tempText = tempText.ResizeBilinear(i_newWidth, i_newHeight);
            //Gets OnMatrixImage
            pixels = tempText.GetPixels();
            width = tempText.width;
            height = tempText.height;
        }
    }

    /*
	{
		get
		{
			return p_height;
		}
	}

	private Dictionary<Vector2,int> children;

	private bool completed;

	public ProcessedImage(int i_id, string i_path, Texture2D i_texture)
	{
		//Le asigno un id
		p_id = i_id;
		//Le asigno su path
		path = i_path;
		//Le asigno su ancho
		p_width = i_texture.width;
		//Le asigno su ancho
		p_height = i_texture.height;
		//Obtengo sus pixels y los guardo
		p_pixels = i_texture.GetPixels();
		//Inicializo su diccionario de hijos
		children = new Dictionary<Vector2, int> ();
		//No está superada
		completed = false;
	}

	public ProcessedImage(UnityEngine.Color[] i_pixels, int i_width, int i_height, int i_id)
	{
		p_id = i_id;
		path = string.Empty;
		p_width = i_width;
		p_height = i_height;
		p_pixels = i_pixels;
		children = null;
		completed = false;
	}
	public ProcessedImage(int i_id, string i_path, UnityEngine.Color[] i_pixels, int i_width, int i_height, Dictionary<Vector2,int> i_children, bool i_completed)
	{
		p_id = i_id;
		path = i_path;
		p_pixels = i_pixels;
		p_width = i_width;
		p_height = i_height;
		children = i_children;
		completed = i_completed;
	}

	public ProcessedImageData GetImageData()
	{
		float redData=0;
		float greenData=0;
		float blueData=0;
		float grayData = 0;
		foreach (UnityEngine.Color pixel in p_pixels) 
		{
			redData += pixel.r;
			greenData += pixel.g;
			blueData += pixel.b;
			grayData += pixel.grayscale;
		}
		float redSaturation = redData / p_pixels.Length;
		float greenSaturation = greenData / p_pixels.Length;
		float blueSaturation = blueData / p_pixels.Length;
		float totalData = redSaturation + greenSaturation + blueSaturation;
		redData = redSaturation / totalData;
		greenData = greenSaturation / totalData;
		blueData = blueSaturation / totalData;
		grayData = grayData / (p_width * p_height);
		//return new ProcessedImageData (redData,greenData,blueData,redSaturation,greenSaturation,blueSaturation,grayData);
		return null;
	}

	public int GetId()
	{
		return p_id;
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
		Texture2D text = new Texture2D (p_width, p_height);
		text.SetPixels (p_pixels);
		text.Apply();
		return text;
	}

	public Texture2D ToGrayscaleTexture2D()
	{
		Texture2D text = new Texture2D (p_width, p_height);
		UnityEngine.Color[] tempPixels = new UnityEngine.Color[p_pixels.Length];
		for (int i = 0; i < p_pixels.Length; i++)
			tempPixels [i] = new UnityEngine.Color (p_pixels [i].grayscale, p_pixels [i].grayscale, p_pixels [i].grayscale);
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
		return p_pixels;
	}

	public void AddChild(Vector2 i_pos, int i_id)
	{
		if (children == null)
			children = new Dictionary<Vector2, int> ();
		children.Add (i_pos, i_id);
	}
	*/
}
