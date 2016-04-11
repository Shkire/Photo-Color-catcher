using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public static class PersistenceManager
{
	private static ProcessedImageList currentGame;

	public static void SaveImage(ProcessedImage img)
	{
		if (currentGame == null) 
		{
			LoadImageList ();
		}
		FileStream file; 
		ProcessedImageList imgList = new ProcessedImageList();
		if (File.Exists (Application.persistentDataPath + "/savedData.phgm"))
		{
			file = File.Open (Application.persistentDataPath + "/savedData.phgm",FileMode.Create);
		}
		else
			file = File.Create (Application.persistentDataPath + "/savedData.phgm");
		imgList.Add(img);
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(file,imgList.ToPersistent());
		file.Close();
	}

	public static void LoadImageList()
	{
		ProcessedImageList imgList = new ProcessedImageList();
		if (File.Exists (Application.persistentDataPath + "/savedData.phgm"))
		{
			FileStream file = File.Open(Application.persistentDataPath + "/savedData.phgm",FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();
			imgList = ((PersistentProcessedImageList)(bf.Deserialize(file))).ToNonPersistent();
			file.Close();
		}
		foreach (int index in imgList.GetAllIds())
			ImgProcessManager.Instance.Instantiate (index, "Cargada");
		currentGame=imgList;
	}

	public static int GetNewId()
	{
		/*
		if (currentGame == null) {
			LoadImageList ();
		}
		return (currentGame.otherAvail) ? currentGame.GetAvailable() : currentGame.GetLastId() + 1;
*/
		if (currentGame == null) 
		{
			LoadImageList ();
		}
		currentGame.lastId++;
		return currentGame.lastId;
	}

	public static ProcessedImage GetImage(int index)
	{
		return currentGame.GetImage(index);
	}

	public static Dictionary<int,ProcessedImage>.KeyCollection GetAllIds()
	{
		return currentGame.GetAllIds ();
	}

	public static void IndexImage(ProcessedImage img){
		currentGame.Add (img);
	}
}
