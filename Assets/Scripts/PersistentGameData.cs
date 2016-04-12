using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PersistentGameData {

	private List<int> parentImgs;
	private Dictionary<int,PersistentProcessedImage> imgDict;
	private int lastId;
	private List<int> otherAvailId;

	public PersistentGameData(List<int> i_parentImgs,Dictionary<int,PersistentProcessedImage> i_imgDict,int i_lastId,List<int> i_otherAvailId)
	{
		parentImgs = i_parentImgs;
		imgDict = i_imgDict;
		lastId = i_lastId;
		otherAvailId = i_otherAvailId;
	}

	public GameData ToNonPersistent()
	{
		Dictionary<int,ProcessedImage> auxImgDict = new Dictionary<int, ProcessedImage> ();
		foreach (int index in imgDict.Keys)
			auxImgDict.Add (index,imgDict[index].ToNonPersistent());
		GameData auxData = new GameData (parentImgs,auxImgDict,lastId,otherAvailId);
		return auxData;
	}
}
