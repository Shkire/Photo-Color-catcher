using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BasicDataTypes;
using System.Linq;

public class PersistenceManager : Singleton<PersistenceManager>
{
    public const string WORLD_EXT = ".pccw";
    public const string LEVEL_EXT = ".pccl";
    public const string LEVEL_CELL_EXT = ".pccc";

    public IEnumerator SaveWorld(World i_world, string i_name)
    {
        //Crear directorio
        string dirName;
        do
        {
            dirName = Path.GetRandomFileName();
        }
        while (Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName));
        Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName);

        //Almacenar world
        FileStream file = null; 
        BinaryFormatter bf = new BinaryFormatter();
        SurrogateSelector ss = new SurrogateSelector();
        WorldSerializationSurrogate worldSs = new WorldSerializationSurrogate();
        StreamingContext sc = new StreamingContext(StreamingContextStates.All); 
        ss.AddSurrogate(typeof(World), sc, worldSs);
        bf.SurrogateSelector = ss;
        file = File.Create(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName + Path.DirectorySeparatorChar + i_world._name+ WORLD_EXT);
        using (file)
        {
            bf.Serialize(file, i_world);
        }

        //Crear imagen
        File.WriteAllBytes(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName + Path.DirectorySeparatorChar + i_world._name+".jpg",i_world._img.EncodeToJPG());

        //Crear directorio
        Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName+ Path.DirectorySeparatorChar + "Levels");

        Vector2SerializationSurrogate v2Ss = new Vector2SerializationSurrogate();
        LevelSerializationSurrogate levelSs = new LevelSerializationSurrogate();

        ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate();
        LevelCellSerializationSurrogate levelCellSs = new LevelCellSerializationSurrogate();

        //Niveles
        foreach (KeyValuePair<Vector2,Level> levelDictEntry in i_world._levels)
        {
            //Crear directorio
            Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName+ Path.DirectorySeparatorChar + "Levels"+ Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y);

            //Almacenar Level
            ss = new SurrogateSelector();
            ss.AddSurrogate(typeof(Vector2), sc, v2Ss);
            ss.AddSurrogate(typeof(Level), sc, levelSs);
            bf.SurrogateSelector = ss;
            file = File.Create(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName+ Path.DirectorySeparatorChar + "Levels"+ Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y+ LEVEL_EXT);
            using (file)
            {
                bf.Serialize(file, levelDictEntry.Value);
            }

            //Crear imagen
            File.WriteAllBytes(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName+ Path.DirectorySeparatorChar + "Levels"+ Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y+".jpg",levelDictEntry.Value._img.EncodeToJPG());

            //Crear directorio
            Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName+ Path.DirectorySeparatorChar + "Levels"+ Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + Path.DirectorySeparatorChar + "Cells");

            //Celdas
            foreach (KeyValuePair<Vector2,LevelCell> levelCellDictEntry in levelDictEntry.Value._cells)
            {
                //Crear directorio
                Directory.CreateDirectory(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName+ Path.DirectorySeparatorChar + "Levels"+ Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + Path.DirectorySeparatorChar + "Cells" + Path.DirectorySeparatorChar + (int)levelCellDictEntry.Key.x +"x"+(int)levelCellDictEntry.Key.y);

                //Almacenar Celda
                ss = new SurrogateSelector();
                ss.AddSurrogate(typeof(Color), sc, colorSs);
                ss.AddSurrogate(typeof(LevelCell), sc, levelCellSs);
                bf.SurrogateSelector = ss;
                file = File.Create(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName+ Path.DirectorySeparatorChar + "Levels"+ Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + Path.DirectorySeparatorChar + "Cells" + Path.DirectorySeparatorChar + (int)levelCellDictEntry.Key.x +"x"+(int)levelCellDictEntry.Key.y + Path.DirectorySeparatorChar + (int)levelCellDictEntry.Key.x +"x"+(int)levelCellDictEntry.Key.y + LEVEL_CELL_EXT);
                using (file)
                {
                    bf.Serialize(file, levelCellDictEntry.Value);
                }

                //Crear imagen
                File.WriteAllBytes(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName+ Path.DirectorySeparatorChar + "Levels"+ Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + Path.DirectorySeparatorChar + "Cells" + Path.DirectorySeparatorChar + (int)levelCellDictEntry.Key.x +"x"+(int)levelCellDictEntry.Key.y + Path.DirectorySeparatorChar + (int)levelCellDictEntry.Key.x +"x"+(int)levelCellDictEntry.Key.y + ".jpg", levelCellDictEntry.Value._img.EncodeToJPG());
            }
        }
    }

    public IEnumerator LoadWorld(string i_path,World o_world)
    {
        Texture2D auxText;

        if (!Directory.Exists(i_path))
            return null;
        string[] files = Directory.GetFiles(i_path);
        if (files.Length != 2 || !(Path.GetExtension(files[0]).ToLower().Equals(".jpg") || Path.GetExtension(files[0]).ToLower().Equals(WORLD_EXT)) || !(Path.GetExtension(files[1]).ToLower().Equals(".jpg") || Path.GetExtension(files[1]).ToLower().Equals(WORLD_EXT)) || Path.GetExtension(files[0]).ToLower().Equals(Path.GetExtension(files[1]).ToLower()))
            return null;
        string path1 = files[0].Remove(files[0].Length - Path.GetExtension(files[0]).Length, Path.GetExtension(files[0]).Length);
        string path2 = files[1].Remove(files[1].Length - Path.GetExtension(files[1]).Length, Path.GetExtension(files[1]).Length);   
        if (!path1.Equals(path2))
            return null;

        FileStream file = File.Open(path1+WORLD_EXT, FileMode.Open); 
        BinaryFormatter bf = new BinaryFormatter();
        SurrogateSelector ss = new SurrogateSelector();
        WorldSerializationSurrogate worldSs = new WorldSerializationSurrogate();
        StreamingContext sc = new StreamingContext(StreamingContextStates.All); 
        ss.AddSurrogate(typeof(World), sc, worldSs);
        bf.SurrogateSelector = ss;
        using (file)
        {
            o_world = ((World)(bf.Deserialize(file)));
        }

        auxText = new Texture2D(4, 4);
        auxText.LoadImage(File.ReadAllBytes(path1+".jpg"));

        o_world._img = auxText;

        o_world._levels = new Dictionary<Vector2, Level>();

        Vector2SerializationSurrogate v2Ss = new Vector2SerializationSurrogate();
        LevelSerializationSurrogate levelSs = new LevelSerializationSurrogate();

        ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate();
        LevelCellSerializationSurrogate levelCellSs = new LevelCellSerializationSurrogate();

        Level auxLevel;

        LevelCell auxCell;

        //ERRORS
        for (int x = 0; x < o_world._imageConfig[0]; x++)
        {
            for (int y = 0; y < o_world._imageConfig[1]; y++)
            {
                file = File.Open(i_path+Path.DirectorySeparatorChar+"Levels"+Path.DirectorySeparatorChar+x+"x"+y+Path.DirectorySeparatorChar+x+"x"+y+LEVEL_EXT, FileMode.Open);
                ss = new SurrogateSelector();
                ss.AddSurrogate(typeof(Vector2), sc, v2Ss);
                ss.AddSurrogate(typeof(Level), sc, levelSs);
                bf.SurrogateSelector = ss;
                using (file)
                {
                    auxLevel = ((Level)(bf.Deserialize(file)));
                }

                o_world._levels.Add(new Vector2(x,y),auxLevel);

                auxText = new Texture2D(4, 4);
                auxText.LoadImage(File.ReadAllBytes(i_path+Path.DirectorySeparatorChar+"Levels"+Path.DirectorySeparatorChar+x+"x"+y+Path.DirectorySeparatorChar+x+"x"+y+".jpg"));

                auxLevel._img = auxText;

                auxLevel._cells = new Dictionary<Vector2, LevelCell>();

                //ERRORS
                for (int i = 0; i < ImgProcessManager.Instance._mapSize; i++)
                {
                    for (int j = 0; j < ImgProcessManager.Instance._mapSize; j++)
                    {
                        file = File.Open(i_path+Path.DirectorySeparatorChar+"Levels"+Path.DirectorySeparatorChar+x+"x"+y+Path.DirectorySeparatorChar+"Cells"+Path.DirectorySeparatorChar+i+"x"+j+Path.DirectorySeparatorChar+i+"x"+j+LEVEL_CELL_EXT, FileMode.Open);
                        ss = new SurrogateSelector();
                        ss.AddSurrogate(typeof(Color), sc, colorSs);
                        ss.AddSurrogate(typeof(LevelCell), sc, levelCellSs);
                        bf.SurrogateSelector = ss;
                        using (file)
                        {
                            auxCell = ((LevelCell)(bf.Deserialize(file)));
                        }

                        auxLevel._cells.Add(new Vector2(i,j),auxCell);

                        auxText = new Texture2D(4, 4);
                        auxText.LoadImage(File.ReadAllBytes(i_path+Path.DirectorySeparatorChar+"Levels"+Path.DirectorySeparatorChar+x+"x"+y+Path.DirectorySeparatorChar+"Cells"+Path.DirectorySeparatorChar+i+"x"+j+Path.DirectorySeparatorChar+i+"x"+j+".jpg"));

                        auxCell._img = auxText;
                    }
                }
            }
        }
        return o_world;
    }

    public static void CompleteLevel(string i_path, Vector2 i_levelPos)
    {
        World aux = LoadWorld(i_path);

        aux._levels[i_levelPos]._completed = true;

        SaveWorld(aux,aux._name);
    }
}
