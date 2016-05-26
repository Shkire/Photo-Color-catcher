using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Color = UnityEngine.Color;

public static class PersistenceManager
{
	private static GameData currentData;

	//Carga inicial se cargan las imagenes padre que hay indexadas y las configuraciones
	//Carga de nivel se cargan los datos de la imagen principal y de las hijas
	//Guardado de imagen al indexarla, se guardan los datos de color de ella y sus hijas
	//Guardado de imagen completada, se guarda en playerprefs si está completada o no

	/// <summary>
	/// The starting main load. Loads all information needed to show the processed
	/// images and process new images (list of parent images)
	/// </summary>
	public static void MainLoad()
	{
		//Inicializo los datos
		currentData = new GameData ();
		//Si hay datos guardados los cargo
		if (File.Exists (Application.persistentDataPath + "/savedData.phgm"))
		{
			FileStream file = File.Open(Application.persistentDataPath + "/savedData.phgm",FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();
			SurrogateSelector ss = new SurrogateSelector();
			ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate ();
			Vector2SerializationSurrogate v2ss = new Vector2SerializationSurrogate ();
			ss.AddSurrogate (typeof(Color), new StreamingContext (StreamingContextStates.All), colorSs);
			ss.AddSurrogate (typeof(Vector2), new StreamingContext (StreamingContextStates.All), v2ss);
			bf.SurrogateSelector = ss;
			GameData loadedData = ((GameData)(bf.Deserialize (file)));
			currentData.SetParents(loadedData.GetParents());
			currentData.SetAvailableIds (loadedData.GetLastId(), loadedData.GetOtherAvail());
			Dictionary<int,ProcessedImage> images = new Dictionary<int, ProcessedImage> ();
			foreach (int imgIndex in currentData.GetParents())
				images.Add (imgIndex,loadedData.GetImage(imgIndex));
			currentData.SetImages(images);
			file.Close();
		}
	}

	public static void LevelDataLoad(int i_index)
	{
		currentData = new GameData ();
		if (File.Exists (Application.persistentDataPath + "/savedData.phgm"))
		{
			FileStream file = File.Open(Application.persistentDataPath + "/savedData.phgm",FileMode.Open);
			BinaryFormatter bf = new BinaryFormatter();
			SurrogateSelector ss = new SurrogateSelector();
			ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate ();
			Vector2SerializationSurrogate v2ss = new Vector2SerializationSurrogate ();
			ss.AddSurrogate (typeof(Color), new StreamingContext (StreamingContextStates.All), colorSs);
			ss.AddSurrogate (typeof(Vector2), new StreamingContext (StreamingContextStates.All), v2ss);
			bf.SurrogateSelector = ss;
			GameData loadedData = ((GameData)(bf.Deserialize (file)));
			List<int> parent = new List<int> ();
			parent.Add (i_index);
			currentData.SetParents(parent);
			Dictionary<int,ProcessedImage> images = new Dictionary<int, ProcessedImage> ();
			foreach (int index in loadedData.GetImage(i_index).GetChildrenId())
				images.Add (index, loadedData.GetImage (index));
			images.Add (i_index, loadedData.GetImage (i_index));
			currentData.SetImages(images);
			file.Close();
		}
	}

	/// <summary>
	/// Saves the data of the level and his children
	/// </summary>
	/// <param name="i_img">Level image.</param>
	/// <param name="i_children">List of children images.</param>
	public static void ProcessedLevelSave(ProcessedImage i_img, List<ProcessedImage> i_children)
	{
		GameData loadedData = new GameData ();
		FileStream file; 
		BinaryFormatter bf = new BinaryFormatter();
		SurrogateSelector ss = new SurrogateSelector();
		ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate ();
		Vector2SerializationSurrogate v2ss = new Vector2SerializationSurrogate ();
		ss.AddSurrogate (typeof(Color), new StreamingContext (StreamingContextStates.All), colorSs);
		ss.AddSurrogate (typeof(Vector2), new StreamingContext (StreamingContextStates.All), v2ss);
		bf.SurrogateSelector = ss;
		if (File.Exists (Application.persistentDataPath + "/savedData.phgm"))
		{
			file = File.Open(Application.persistentDataPath + "/savedData.phgm",FileMode.Open);
			loadedData = ((GameData)(bf.Deserialize (file)));
			file.Close();
		}
		loadedData.AddParent (i_img);
		loadedData.AddImages (i_children);
		loadedData.SetAvailableIds (currentData.GetLastId(),currentData.GetOtherAvail());
		if (File.Exists (Application.persistentDataPath + "/savedData.phgm"))
		{
			file = File.Open (Application.persistentDataPath + "/savedData.phgm",FileMode.Create);
			bf.Serialize(file,loadedData);
		}
		else
			file = File.Create (Application.persistentDataPath + "/savedData.phgm");
		bf.Serialize(file,loadedData);
		file.Close();
	}

	/// <summary>
	/// Saves the data info of the group of levels on given dictionary
	/// </summary>
	/// <param name="i_imgDict">Dictionary with level ids and image data.</param>
	public static void LevelDataSave(Dictionary<int,ProcessedImageData> i_imgDict)
	{
		GameData loadedData = new GameData ();
		FileStream file; 
		BinaryFormatter bf = new BinaryFormatter();
		SurrogateSelector ss = new SurrogateSelector();
		ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate ();
		Vector2SerializationSurrogate v2ss = new Vector2SerializationSurrogate ();
		ss.AddSurrogate (typeof(Color), new StreamingContext (StreamingContextStates.All), colorSs);
		ss.AddSurrogate (typeof(Vector2), new StreamingContext (StreamingContextStates.All), v2ss);
		bf.SurrogateSelector = ss;
		if (File.Exists (Application.persistentDataPath + "/savedData.phgm"))
		{
			file = File.Open(Application.persistentDataPath + "/savedData.phgm",FileMode.Open);
			loadedData = ((GameData)(bf.Deserialize (file)));
			file.Close();
		}
		loadedData.AddDataInfo (i_imgDict);
		if (File.Exists (Application.persistentDataPath + "/savedData.phgm"))
		{
			file = File.Open (Application.persistentDataPath + "/savedData.phgm",FileMode.Create);
			bf.Serialize(file,loadedData);
		}
		else
			file = File.Create (Application.persistentDataPath + "/savedData.phgm");
		bf.Serialize(file,loadedData);
		file.Close();
	}

	public static void LevelCompletedSave(int i_index, bool i_completed)
	{
		GameData loadedData = new GameData ();
		FileStream file; 
		BinaryFormatter bf = new BinaryFormatter();
		SurrogateSelector ss = new SurrogateSelector();
		ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate ();
		Vector2SerializationSurrogate v2ss = new Vector2SerializationSurrogate ();
		ss.AddSurrogate (typeof(Color), new StreamingContext (StreamingContextStates.All), colorSs);
		ss.AddSurrogate (typeof(Vector2), new StreamingContext (StreamingContextStates.All), v2ss);
		bf.SurrogateSelector = ss;
		if (File.Exists (Application.persistentDataPath + "/savedData.phgm"))
		{
			file = File.Open(Application.persistentDataPath + "/savedData.phgm",FileMode.Open);
			loadedData = ((GameData)(bf.Deserialize (file)));
			file.Close();
		}
		loadedData.SetCompleted (i_index, i_completed);
		if (File.Exists (Application.persistentDataPath + "/savedData.phgm"))
		{
			file = File.Open (Application.persistentDataPath + "/savedData.phgm",FileMode.Create);
		}
		else
			file = File.Create (Application.persistentDataPath + "/savedData.phgm");
		bf.Serialize(file,loadedData);
		file.Close();
	}

	public static int GetNewId()
	{
		int id;
		if (currentData.otherAvail) {
			id = currentData.GetOtherAvail () [0];
			currentData.GetOtherAvail ().RemoveAt (0);
		} else 
		{
			id = currentData.GetLastId ();
			currentData.NextLastId ();
		}
		return id;
	}

	public static int[] GetNewIdList(int i_idNumber)
	{
		int[] idList = new int[i_idNumber];
		for (int i = 0; i < i_idNumber; i++) 
		{
			idList [i] = GetNewId ();
		}
		return idList;
	}

	public static ProcessedImage GetImage(int id)
	{
		ProcessedImage img=null;
		if (currentData != null) 
		{
			for (int i=0; i<currentData.GetParents().Count; i++)
			{
				PersistenceManager.MainLoad ();
				PersistenceManager.LevelDataLoad (currentData.GetParents () [i]);
				if (currentData.HasImage (id))
					break;
			}
		}
		img=currentData.GetImage (id);
		return img;
	}

	/*
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
	*/
}
