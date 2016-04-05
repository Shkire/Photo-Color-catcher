using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ProcessedImageList
{
	private List<string> parentImgs;
	private Dictionary<string,ProcessedImage> imgDict;

	public ProcessedImageList()
	{
		parentImgs = new List<string> ();
		imgDict = new Dictionary<string, ProcessedImage> ();
	}

	public void Add(ProcessedImage img)
	{
		if (Contains (img.id))
			Remove (img.id);
		imgDict.Add (img.id, img);
	}

	public bool Contains(string id)
	{
		return imgDict.ContainsKey (id);
	}

	public void Remove(string id)
	{
		imgDict.Remove (id);
		parentImgs.Remove (id);
	}
}
