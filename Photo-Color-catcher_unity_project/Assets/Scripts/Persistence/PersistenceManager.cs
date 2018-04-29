using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BasicDataTypes;

public static class PersistenceManager
{
    private static MainGameData currentData;
    private static PersistentImageData currentImg;

    const string ROOT_PATH = "/";
    const string IMG_DATA_EXT = ".pccw";
    const string MAIN_DATA_EXT = "";

    public static void SaveWorld(World i_world, string i_name)
    {
        FileStream file = null; 
        BinaryFormatter bf = new BinaryFormatter();
        SurrogateSelector ss = new SurrogateSelector();
        ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate();
        Vector2SerializationSurrogate v2ss = new Vector2SerializationSurrogate();
        DictionarySerializationSurrogate<object,object> dictSs = new DictionarySerializationSurrogate<object, object>();
        //DictionarySerializationSurrogate<Vector2,Level> levelDictSs = new DictionarySerializationSurrogate<Vector2, Level>();
        //DictionarySerializationSurrogate<Vector2,LevelCell> levelCellDictSs = new DictionarySerializationSurrogate<Vector2, LevelCell>();
        StreamingContext sc = new StreamingContext(StreamingContextStates.All); 
        ss.AddSurrogate(typeof(Color), sc, colorSs);
        ss.AddSurrogate(typeof(Vector2), sc, v2ss);
        ss.AddSurrogate(typeof(Dictionary<object,object>), sc, dictSs);
        //ss.AddSurrogate(typeof(Dictionary<Vector2,Level>), sc, levelDictSs);
        //ss.AddSurrogate(typeof(Dictionary<Vector2,LevelCell>), sc, levelCellDictSs);
        bf.SurrogateSelector = ss;
        if (File.Exists(Application.persistentDataPath + ROOT_PATH + i_name + IMG_DATA_EXT))
        {
            //EXISTS
        }
        else
        {
            file = File.Create(Application.persistentDataPath + ROOT_PATH + i_name + IMG_DATA_EXT);
        }
        bf.Serialize(file, i_world);
        file.Close();
    }

    public static World LoadWorld(string i_path)
    {
        World aux;
        if (!File.Exists(i_path))
            return null;
        using (FileStream file = File.Open(i_path, FileMode.Open))
        {
            BinaryFormatter bf = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();
            ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate();
            Vector2SerializationSurrogate v2ss = new Vector2SerializationSurrogate();
            DictionarySerializationSurrogate<object,object> dictSs = new DictionarySerializationSurrogate<object, object>();
            //DictionarySerializationSurrogate<Vector2,Level> levelDictSs = new DictionarySerializationSurrogate<Vector2, Level>();
            //DictionarySerializationSurrogate<Vector2,LevelCell> levelCellDictSs = new DictionarySerializationSurrogate<Vector2, LevelCell>();
            StreamingContext sc = new StreamingContext(StreamingContextStates.All); 
            ss.AddSurrogate(typeof(Color), sc, colorSs);
            ss.AddSurrogate(typeof(Vector2), sc, v2ss);
            ss.AddSurrogate(typeof(Dictionary<object,object>), sc, dictSs);
            //ss.AddSurrogate(typeof(Dictionary<Vector2,Level>), sc, levelDictSs);
            //ss.AddSurrogate(typeof(Dictionary<Vector2,LevelCell>), sc, levelCellDictSs);
            bf.SurrogateSelector = ss;
            aux = ((World)(bf.Deserialize(file)));
        }
        return aux;

    }

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
        currentData = new MainGameData();
        //Si hay datos guardados los cargo
        if (File.Exists(Application.persistentDataPath + ROOT_PATH + "/savedData" + MAIN_DATA_EXT))
        {
            FileStream file = File.Open(Application.persistentDataPath + ROOT_PATH + "/savedData" + MAIN_DATA_EXT, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();
            ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate();
            ss.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), colorSs);
            bf.SurrogateSelector = ss;
            currentData = ((MainGameData)(bf.Deserialize(file)));
            file.Close();
        }
    }

    public static void LevelDataLoad(int i_index)
    {
        string path = currentData.GetPath(i_index);
        if (File.Exists(Application.persistentDataPath + ROOT_PATH + path + IMG_DATA_EXT))
        {
            FileStream file = File.Open(Application.persistentDataPath + ROOT_PATH + path + IMG_DATA_EXT, FileMode.Open);
            BinaryFormatter bf = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();
            StreamingContext sc = new StreamingContext(StreamingContextStates.All);
            ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate();
            Vector2SerializationSurrogate v2ss = new Vector2SerializationSurrogate();
            ss.AddSurrogate(typeof(Color), sc, colorSs);
            ss.AddSurrogate(typeof(Vector2), sc, v2ss);
            bf.SurrogateSelector = ss;
            currentImg = ((PersistentImageData)(bf.Deserialize(file)));
            file.Close();
        }
        else
            Debug.LogError("The file you're looking for doesn't exist or has been moved");
    }

    public static void MainDataFlush()
    {
        FileStream file; 
        BinaryFormatter bf = new BinaryFormatter();
        SurrogateSelector ss = new SurrogateSelector();
        ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate();
        ss.AddSurrogate(typeof(Color), new StreamingContext(StreamingContextStates.All), colorSs);
        bf.SurrogateSelector = ss;
        if (File.Exists(Application.persistentDataPath + ROOT_PATH + "/savedData" + MAIN_DATA_EXT))
        {
            file = File.Open(Application.persistentDataPath + ROOT_PATH + "/savedData" + MAIN_DATA_EXT, FileMode.Create);
        }
        else
        {
            file = File.Create(Application.persistentDataPath + ROOT_PATH + "/savedData" + MAIN_DATA_EXT);
        }
        bf.Serialize(file, currentData);
        file.Close();
    }

    public static void ImgDataFlush()
    {
        FileStream file; 
        BinaryFormatter bf = new BinaryFormatter();
        SurrogateSelector ss = new SurrogateSelector();
        ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate();
        Vector2SerializationSurrogate v2ss = new Vector2SerializationSurrogate();
        StreamingContext sc = new StreamingContext(StreamingContextStates.All); 
        ss.AddSurrogate(typeof(Color), sc, colorSs);
        ss.AddSurrogate(typeof(Vector2), sc, v2ss);
        bf.SurrogateSelector = ss;
        if (File.Exists(Application.persistentDataPath + ROOT_PATH + currentImg.path + IMG_DATA_EXT))
        {
            file = File.Open(Application.persistentDataPath + ROOT_PATH + currentImg.path + IMG_DATA_EXT, FileMode.Create);
        }
        else
        {
            file = File.Create(Application.persistentDataPath + ROOT_PATH + currentImg.path + IMG_DATA_EXT);
        }
        bf.Serialize(file, currentImg);
        file.Close();
    }




    public static void PushImgAndChildren(ProcessedImage i_img, List<ProcessedImage> i_children)
    {
        string path = GenerateNewPath();
        currentImg = new PersistentImageData(i_img, i_children, path);
        ImgPersistanceInfo info = new ImgPersistanceInfo(path, i_img.GetChildrenId(), i_img.GetPixels());
        currentData.AddParent(i_img.GetId(), info);
    }

    public static void PushImgData(Dictionary<int,ProcessedImageData> i_imgData)
    {
        currentImg.AddData(i_imgData);
    }

    public static int GetNewId()
    {
        int id;
        if (currentData.otherAvail)
        {
            id = currentData.GetOtherAvail()[0];
            currentData.GetOtherAvail().RemoveAt(0);
        }
        else
        {
            id = currentData.GetLastId();
            currentData.NextLastId();
        }
        return id;
    }

    public static int[] GetNewIdList(int i_idNumber)
    {
        int[] idList = new int[i_idNumber];
        for (int i = 0; i < i_idNumber; i++)
        {
            idList[i] = GetNewId();
        }
        return idList;
    }

    public static void LoadLevelPack(int i_id)
    {
        if (currentData.HasParent(i_id))
        {
            FileStream file; 
            BinaryFormatter bf = new BinaryFormatter();
            SurrogateSelector ss = new SurrogateSelector();
            ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate();
            Vector2SerializationSurrogate v2ss = new Vector2SerializationSurrogate();
            StreamingContext sc = new StreamingContext(StreamingContextStates.All); 
            ss.AddSurrogate(typeof(Color), sc, colorSs);
            ss.AddSurrogate(typeof(Vector2), sc, v2ss);
            bf.SurrogateSelector = ss;
            string path = currentData.GetParentPath(i_id);
            if (File.Exists(Application.persistentDataPath + ROOT_PATH + path + IMG_DATA_EXT))
            {
                file = File.Open(Application.persistentDataPath + ROOT_PATH + path + IMG_DATA_EXT, FileMode.Open);
                currentImg = (PersistentImageData)bf.Deserialize(file);
                file.Close();
            }
        }
    }

    public static void LoadLevelParent(int i_id)
    {
        LoadLevelPack(currentData.GetParent(i_id));
    }

    /*
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
	*/

    public static string GenerateNewPath()
    {
        string path;
        do
        {
            path = Path.GetRandomFileName();
            path = path.Split(new char[]{ '.' }, 2)[0];
        }
        while (!UnusedPath(path));
        return path;
    }

    public static bool UnusedPath(string i_path)
    {
        if (currentData.GetPaths().Contains(i_path))
            return false;
        return true;
    }

    public static ProcessedImage GetImage(int i_id)
    {
        if (currentImg != null)
            return currentImg.GetChild(i_id);
        return null;
    }

    public static List<ChildImgInfo> GetChildrenInfo()
    {
        return currentImg.GetChildrenInfo();
    }

    public static ProcessedImage GetImage(int x, int y)
    {
        foreach (ChildImgInfo info in GetChildrenInfo())
        {
            if (info.pos.Equals(new Vector2(x, y)))
                return info.img;
        }
        return null;
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
