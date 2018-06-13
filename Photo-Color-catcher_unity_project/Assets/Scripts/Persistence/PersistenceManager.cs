using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BasicDataTypes;

public static class PersistenceManager
{

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
            file = File.Open (Application.persistentDataPath + ROOT_PATH + i_name + IMG_DATA_EXT, FileMode.Create);
        }
        else
        {
            file = File.Create(Application.persistentDataPath + ROOT_PATH + i_name + IMG_DATA_EXT);
        }
        using (file)
        {
            bf.Serialize(file, i_world);
        }
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

    public static void CompleteLevel(string i_path, Vector2 i_levelPos)
    {
        World aux = LoadWorld(i_path);

        aux._levels[i_levelPos]._completed = true;

        SaveWorld(aux,aux._name);
    }
}
