using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameData{

	private List<int> parentImgs;
	private Dictionary<int,ProcessedImage> imgDict;
	private int lastId;
	private List<int> otherAvailId;

	public bool otherAvail{
		get{
			return otherAvailId.Count > 0;
		}
	}

	public GameData()
	{
		parentImgs = new List<int> ();
		imgDict = new Dictionary<int, ProcessedImage> ();
		lastId = 0;
		otherAvailId = new List<int> ();
	}

	public GameData(List<int> i_parentImgs, Dictionary<int,ProcessedImage> i_imgDict, int i_lastId, List<int> i_otherAvailId)
	{
		parentImgs = i_parentImgs;
		imgDict = i_imgDict;
		lastId = i_lastId;
		otherAvailId = i_otherAvailId;
	}

	public void SetParents(List<int> i_parents)
	{
		parentImgs = i_parents;
	}

	public List<int> GetParents()
	{
		return parentImgs;
	}

	public ProcessedImage GetImage(int i_index)
	{
		return imgDict [i_index];
	}

	public void SetImages(Dictionary<int,ProcessedImage> i_images)
	{
		imgDict = i_images;
	}

	public void AddParent(ProcessedImage i_img)
	{
		parentImgs.Add (i_img.GetId());
		imgDict.Add (i_img.GetId(),i_img);
	}

	public void AddImages(List<ProcessedImage> i_images)
	{
		foreach (ProcessedImage img in i_images)
			imgDict.Add (img.GetId(),img);
	}

	public void SetCompleted(int i_index, bool i_completed)
	{
		imgDict [i_index].SetCompleted (i_completed);
	}

	public int GetLastId()
	{
		return lastId;
	}

	public List<int> GetOtherAvail()
	{
		return otherAvailId;
	}

	public void SetAvailableIds(int i_lastId, List<int> i_otheAvailId)
	{
		lastId = i_lastId;
		otherAvailId = i_otheAvailId;
	}

	public void NextLastId()
	{
		lastId++;
	}

	public PersistentGameData ToPersistent()
	{
		Dictionary<int,PersistentProcessedImage> auxImgDict = new Dictionary<int, PersistentProcessedImage> ();
		if (imgDict!=null && imgDict.Count>0)
			foreach (int index in imgDict.Keys)
				auxImgDict.Add (index,imgDict[index].ToPersistent());
		PersistentGameData auxData = new PersistentGameData (parentImgs,auxImgDict,lastId,otherAvailId);
		return auxData;
	}
}
