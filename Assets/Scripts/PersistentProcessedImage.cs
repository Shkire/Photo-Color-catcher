using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PersistentProcessedImage{

	private int id;

	private string path;

	private float[][] pixels;

	private int width;

	private int height;

	private Dictionary<int[],int> children;

	public PersistentProcessedImage(int i_id, string i_path, float[][] i_pixels, int i_width, int i_height, Dictionary<int[],int> i_children)
	{
		id = i_id;
		path = i_path;
		pixels = i_pixels;
		width = i_width;
		height = i_height;
		children = i_children;
	}

	public ProcessedImage ToNonPersistent()
	{
		UnityEngine.Color[] auxPixels = new UnityEngine.Color[pixels.Length];
		for (int i = 0; i < pixels.Length; i++) 
		{
			auxPixels [i] = new UnityEngine.Color (pixels[i][0],pixels[i][1],pixels[i][2],pixels[i][3]);
		}
		Dictionary<Vector2,int> auxChildren = new Dictionary<Vector2, int> ();
		foreach (int[] index in children.Keys)
			auxChildren.Add (new Vector2(index[0],index[1]),children[index]);
		ProcessedImage img = new ProcessedImage (id,path,auxPixels,width,height,auxChildren,false);
		return img;
	}
}
