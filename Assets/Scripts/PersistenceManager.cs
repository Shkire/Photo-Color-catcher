using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public static class PersistenceManager
{
	private string lastId;

	private List<string> otherAvailId;

	public static void SaveImage(ProcessedImage img)
	{
		FileStream file; 
		ProcessedImageList imgList = new ProcessedImageList();
		if (File.Exists (Application.persistentDataPath + "/savedData.phgm"))
		{
			imgList = LoadImageList ();
			file = File.Open (Application.persistentDataPath + "/savedData.phgm",FileMode.Create);
		}
		else
			file = File.Create (Application.persistentDataPath + "/savedData.phgm");
		imgList.Add(img);
		BinaryFormatter bf = new BinaryFormatter();
		bf.Serialize(file,imgList);
		file.Close();
	}

	public static ProcessedImageList LoadImageList()
	{
		ProcessedImageList imgList = null;
		if (File.Exists (Application.persistentDataPath + "/savedData.phgm"))
		{
			FileStream file = File.Open(Application.persistentDataPath + "/savedData.phgm",FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();
			imgList = (ProcessedImageList)bf.Deserialize(file);
			file.Close();
		}
		return imgList;
	}
}
