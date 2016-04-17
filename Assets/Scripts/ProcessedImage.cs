using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProcessedImage{

	private int id;

	private string path;

	private UnityEngine.Color[] pixels;

	private int width;

	private int height;

	private Dictionary<Vector2,int> children;

	private bool completed;

	public ProcessedImage(string i_path)
	{
		id = PersistenceManager.GetNewId();
		Debug.Log ("Setting path");
		path = i_path;
		Debug.Log ("Creating text");
		Texture2D tempText = new Texture2D(1,1);
		Debug.Log ("Reading image");
		byte[] imgRead = System.IO.File.ReadAllBytes (Application.persistentDataPath+"/"+path);
		tempText.LoadImage (imgRead);
		Debug.Log ("Setting width");
		width = tempText.width;
		Debug.Log ("Setting height");
		height = tempText.height;
		Debug.Log ("Setting pixels");
		pixels = tempText.GetPixels();
		Debug.Log ("Initiating children");
		children = new Dictionary<Vector2, int> ();
		completed = false;
	}

	public ProcessedImage(UnityEngine.Color[] i_pixels, int i_width, int i_height)
	{
		id = PersistenceManager.GetNewId();
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

	public void Divide(int i_divisionFactor)
	{
		int childrenWidth = Mathf.CeilToInt ((float)width / i_divisionFactor);
		int childrenHeight = Mathf.CeilToInt ((float)height / i_divisionFactor);
		List<ProcessedImage> tempChildren = new List<ProcessedImage> ();
		Texture2D tempText = new Texture2D (width, height);
		tempText.SetPixels (pixels);
		tempText.Apply ();
		tempText = tempText.ResizeBilinear (childrenWidth * i_divisionFactor, childrenHeight * i_divisionFactor);
		UnityEngine.Color[] tempPixels = tempText.GetPixels ();
		for (int x = 0; x < i_divisionFactor; x++) 
		{
			for (int y = 0; y < i_divisionFactor; y++) 
			{
				UnityEngine.Color[] auxPixels = new UnityEngine.Color[childrenWidth * childrenHeight];
				for (int i = 0; i < childrenWidth; i++) {
					for (int j = 0; j < childrenHeight; j++) 
					{
						int origPos = x * childrenWidth + y * i_divisionFactor * childrenWidth * childrenHeight + i + j * childrenWidth * i_divisionFactor;
						auxPixels [i + j*childrenWidth] = new UnityEngine.Color(tempPixels [origPos].r,tempPixels [origPos].g,tempPixels [origPos].b,tempPixels [origPos].a);
					}
				}
				ProcessedImage auxImg = new ProcessedImage(auxPixels,childrenWidth,childrenHeight);
				children.Add (new Vector2 (x, y), auxImg.id);
				tempChildren.Add(auxImg);
			}
		}
		PersistenceManager.LevelDataSave (this,tempChildren);
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

	public PersistentProcessedImage ToPersistent()
	{
		float[][] auxPixels = new float[pixels.Length][];
		for (int i=0; i<pixels.Length; i++)
			auxPixels[i]=new float[4]{pixels[i].r,pixels[i].g,pixels[i].b,pixels[i].a};
		Dictionary<int[],int> auxChildren = new Dictionary<int[], int>();
		if (children!=null && children.Count>0)
			foreach (Vector2 index in children.Keys)
				auxChildren.Add (new int[2]{Mathf.FloorToInt(index.x),Mathf.FloorToInt(index.y)},children[index]);
		PersistentProcessedImage img = new PersistentProcessedImage (id, path, auxPixels, width, height, auxChildren);
		return img;
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
}
