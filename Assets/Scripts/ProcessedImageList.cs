using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProcessedImageList
{
	private List<int> parentImgs;
	private Dictionary<int,ProcessedImage> imgDict;
	//private int lastId;
	public int lastId;
	private List<int> otherAvailId;

	public bool otherAvail{
		get{
			return otherAvailId.Count > 0;
		}
	}

	public ProcessedImageList()
	{
		parentImgs = new List<int> ();
		imgDict = new Dictionary<int, ProcessedImage> ();
		otherAvailId = new List<int> ();
	}

	public ProcessedImageList(List<int> i_parentImgs, Dictionary<int,ProcessedImage> i_imgDict, int i_lastId, List<int> i_otherAvailId)
	{
		parentImgs = i_parentImgs;
		imgDict = i_imgDict;
		lastId = i_lastId;
		otherAvailId = i_otherAvailId;
	}

	public void Add(ProcessedImage img)
	{
		if (Contains (img.GetId()))
			Remove (img.GetId());
		imgDict.Add (img.GetId(), img);
	}

	public bool Contains(int id)
	{
		return imgDict.ContainsKey (id);
	}

	public void Remove(int id)
	{
		imgDict.Remove (id);
		parentImgs.Remove (id);
	}

	public int GetLastId()
	{
		return lastId;
	}

	public int GetAvailable()
	{
		return otherAvailId [0];
	}

	public ProcessedImage GetImage(int index)
	{
		return imgDict[index];
	}

	public Dictionary<int,ProcessedImage>.KeyCollection GetAllIds()
	{
		return imgDict.Keys;
	}

	public PersistentProcessedImageList ToPersistent()
	{
		Dictionary<int,PersistentProcessedImage> auxImgDict = new Dictionary<int, PersistentProcessedImage> ();
		foreach (int index in imgDict.Keys)
			auxImgDict.Add (index,imgDict[index].ToPersistent());
		PersistentProcessedImageList imgList = new PersistentProcessedImageList (parentImgs,auxImgDict,lastId,otherAvailId);
		return imgList;
	}
}
