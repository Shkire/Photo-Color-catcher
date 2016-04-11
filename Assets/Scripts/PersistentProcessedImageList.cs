using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PersistentProcessedImageList{
	private List<int> parentImgs;
	private Dictionary<int,PersistentProcessedImage> imgDict;
	private int lastId;
	private List<int> otherAvailId;

	public PersistentProcessedImageList(List<int> i_parentImgs, Dictionary<int,PersistentProcessedImage> i_imgDict, int i_lastId, List<int> i_otherAvailId)
	{
		parentImgs = i_parentImgs;
		imgDict = i_imgDict;
		lastId = i_lastId;
		otherAvailId = i_otherAvailId;
	}

	public ProcessedImageList ToNonPersistent()
	{
		Dictionary<int,ProcessedImage> auxImgDict = new Dictionary<int, ProcessedImage> ();
		foreach (int index in imgDict.Keys)
			auxImgDict.Add (index,imgDict[index].ToNonPersistent());
		ProcessedImageList imgList = new ProcessedImageList (parentImgs,auxImgDict,lastId,otherAvailId);
		return imgList;
	}
}
