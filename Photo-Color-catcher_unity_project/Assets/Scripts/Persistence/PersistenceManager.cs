using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;
using BasicDataTypes;
using System.Linq;
using System;

/// <summary>
/// Singleton used to save and load game data.
/// </summary>
public class PersistenceManager : Singleton<PersistenceManager>
{
    public const string WORLD_EXT = ".pccw";
    public const string LEVEL_EXT = ".pccl";
    public const string LEVEL_CELL_EXT = ".pccc";

    /// <summary>
    /// Saves the a World that has been previously processed.
    /// </summary>
    /// <param name="i_world">The World to save.</param>
    public IEnumerator SaveWorld(World i_world)
    {
        
        //Creates the folder name.
        string dirName;
        do
        {
            dirName = Path.GetRandomFileName().Replace(".", string.Empty);
        }
        while (Directory.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + dirName));
        dirName = Application.persistentDataPath + Path.DirectorySeparatorChar + dirName;
        Directory.CreateDirectory(dirName);

        //Sets up the FileStream and BinaryFormatter.
        FileStream file = null; 
        BinaryFormatter bf = new BinaryFormatter();
        SurrogateSelector ss = new SurrogateSelector();

        //Used for World serialization.
        WorldSerializationSurrogate worldSs = new WorldSerializationSurrogate();

        //Used for Level serialization.
        Vector2SerializationSurrogate v2Ss = new Vector2SerializationSurrogate();
        LevelSerializationSurrogate levelSs = new LevelSerializationSurrogate();

        //Used for LevelCell serialization.
        ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate();
        LevelCellSerializationSurrogate levelCellSs = new LevelCellSerializationSurrogate();

        StreamingContext sc = new StreamingContext(StreamingContextStates.All); 
        ss.AddSurrogate(typeof(World), sc, worldSs);
        ss.AddSurrogate(typeof(Vector2), sc, v2Ss);
        ss.AddSurrogate(typeof(Level), sc, levelSs);
        ss.AddSurrogate(typeof(Color), sc, colorSs);
        ss.AddSurrogate(typeof(LevelCell), sc, levelCellSs);
        bf.SurrogateSelector = ss;
        file = File.Create(dirName + Path.DirectorySeparatorChar + i_world._name + WORLD_EXT);

        //Saves the World object.
        using (file)
        {
            bf.Serialize(file, i_world);
        }

        yield return null;

        //Saves the World image.
        File.WriteAllBytes(dirName + Path.DirectorySeparatorChar + i_world._name + ".jpg", i_world._img.EncodeToJPG());

        yield return null;

        //Creates the "Levels" directory.
        dirName = dirName + Path.DirectorySeparatorChar + "Levels";
        Directory.CreateDirectory(dirName);

        //For each Level in the World.
        foreach (KeyValuePair<Vector2,Level> levelDictEntry in i_world._levels)
        {
            //Creates the "columnxrow" directory for the Level.
            Directory.CreateDirectory(dirName + Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y);

            //Saves the Level object.
            file = File.Create(dirName + Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + LEVEL_EXT);
            using (file)
            {
                bf.Serialize(file, levelDictEntry.Value);
            }

            yield return null;

            //Saves the Level image.
            File.WriteAllBytes(dirName + Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + ".jpg", levelDictEntry.Value._img.EncodeToJPG());

            yield return null;

            //Creates the "Cells" directory.
            Directory.CreateDirectory(dirName + Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + Path.DirectorySeparatorChar + "Cells");

            //For each LevelCell.
            foreach (KeyValuePair<Vector2,LevelCell> levelCellDictEntry in levelDictEntry.Value._cells)
            {
                //Creates the "columnxrow" directory for the LevelCell.
                Directory.CreateDirectory(dirName + Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + Path.DirectorySeparatorChar + "Cells" + Path.DirectorySeparatorChar + (int)levelCellDictEntry.Key.x + "x" + (int)levelCellDictEntry.Key.y);

                //Saves the LevelCell object.
                file = File.Create(dirName + Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + Path.DirectorySeparatorChar + "Cells" + Path.DirectorySeparatorChar + (int)levelCellDictEntry.Key.x + "x" + (int)levelCellDictEntry.Key.y + Path.DirectorySeparatorChar + (int)levelCellDictEntry.Key.x + "x" + (int)levelCellDictEntry.Key.y + LEVEL_CELL_EXT);
                using (file)
                {
                    bf.Serialize(file, levelCellDictEntry.Value);
                }

                yield return null;

                //Saves the LevelCell image.
                File.WriteAllBytes(dirName + Path.DirectorySeparatorChar + (int)levelDictEntry.Key.x + "x" + (int)levelDictEntry.Key.y + Path.DirectorySeparatorChar + "Cells" + Path.DirectorySeparatorChar + (int)levelCellDictEntry.Key.x + "x" + (int)levelCellDictEntry.Key.y + Path.DirectorySeparatorChar + (int)levelCellDictEntry.Key.x + "x" + (int)levelCellDictEntry.Key.y + ".jpg", levelCellDictEntry.Value._img.EncodeToJPG());

                yield return null;
            }
        }
    }

    /// <summary>
    /// Loads a previously generated World.
    /// </summary>
    /// <param name="i_path">The World path.</param>
    /// <param name="i_worldCallback">The callback used to catch the loaded world.</param>
    public IEnumerator LoadWorld(string i_path, Action<World> i_worldCallback)
    {
        if (!Directory.Exists(i_path))
            yield break;
        string[] files = Directory.GetFiles(i_path);
        if (files.Length != 2 || !(Path.GetExtension(files[0]).ToLower().Equals(".jpg") || Path.GetExtension(files[0]).ToLower().Equals(WORLD_EXT)) || !(Path.GetExtension(files[1]).ToLower().Equals(".jpg") || Path.GetExtension(files[1]).ToLower().Equals(WORLD_EXT)) || Path.GetExtension(files[0]).ToLower().Equals(Path.GetExtension(files[1]).ToLower()))
            yield break;
        string path1 = files[0].Remove(files[0].Length - Path.GetExtension(files[0]).Length, Path.GetExtension(files[0]).Length);
        string path2 = files[1].Remove(files[1].Length - Path.GetExtension(files[1]).Length, Path.GetExtension(files[1]).Length);   
        if (!path1.Equals(path2))
            yield break;

        //Sets up the FileStream and BinaryFormatter.
        FileStream file = File.Open(path1 + WORLD_EXT, FileMode.Open); 
        BinaryFormatter bf = new BinaryFormatter();
        SurrogateSelector ss = new SurrogateSelector();
        WorldSerializationSurrogate worldSs = new WorldSerializationSurrogate();
        StreamingContext sc = new StreamingContext(StreamingContextStates.All); 
        ss.AddSurrogate(typeof(World), sc, worldSs);
        bf.SurrogateSelector = ss;
        World aux;

        //Loads the World.
        using (file)
        {
            aux = ((World)(bf.Deserialize(file)));
        }

        yield return null;

        //Loads the World image.
        Texture2D auxText = new Texture2D(4, 4);
        auxText.LoadImage(File.ReadAllBytes(path1 + ".jpg"));
        aux._img = auxText;

        //Passes the World to the callback.
        i_worldCallback(aux);
    }

    /// <summary>
    /// Loads the Levels in a previously generated World.
    /// </summary>
    /// <param name="i_path">The World path.</param>
    /// <param name="i_worldCallback">The callback used to catch the loaded world.</param>
    /// <param name="i_progressCallback">The callback used to catch the progress value.</param>
    public IEnumerator LoadLevels(string i_path, Action<World> i_worldCallback, Action<float> i_progressCallback)
    {
        if (!Directory.Exists(i_path))
            yield break;
        string[] files = Directory.GetFiles(i_path);
        if (files.Length != 2 || !(Path.GetExtension(files[0]).ToLower().Equals(".jpg") || Path.GetExtension(files[0]).ToLower().Equals(WORLD_EXT)) || !(Path.GetExtension(files[1]).ToLower().Equals(".jpg") || Path.GetExtension(files[1]).ToLower().Equals(WORLD_EXT)) || Path.GetExtension(files[0]).ToLower().Equals(Path.GetExtension(files[1]).ToLower()))
            yield break;
        string path1 = files[0].Remove(files[0].Length - Path.GetExtension(files[0]).Length, Path.GetExtension(files[0]).Length);
        string path2 = files[1].Remove(files[1].Length - Path.GetExtension(files[1]).Length, Path.GetExtension(files[1]).Length);   
        if (!path1.Equals(path2))
            yield break;

        //Sets up the FileStream and BinaryFormatter.
        FileStream file = File.Open(path1 + WORLD_EXT, FileMode.Open); 
        BinaryFormatter bf = new BinaryFormatter();
        SurrogateSelector ss = new SurrogateSelector();

        //Used for World serialization.
        WorldSerializationSurrogate worldSs = new WorldSerializationSurrogate();

        //Used for Level serialization.
        Vector2SerializationSurrogate v2Ss = new Vector2SerializationSurrogate();
        LevelSerializationSurrogate levelSs = new LevelSerializationSurrogate();

        StreamingContext sc = new StreamingContext(StreamingContextStates.All); 
        ss.AddSurrogate(typeof(World), sc, worldSs);
        ss.AddSurrogate(typeof(Vector2), sc, v2Ss);
        ss.AddSurrogate(typeof(Level), sc, levelSs);
        World aux;
        bf.SurrogateSelector = ss;

        //Loads the World.
        using (file)
        {
            aux = ((World)(bf.Deserialize(file)));
        }

        yield return null;

        //Creates the Level dictionary.
        aux._levels = new Dictionary<Vector2, Level>();

        float progress = 0;

        //For each column.
        for (int x = 0; x < aux._imageDivisionConfig[0]; x++)
        {

            //For each row.
            for (int y = 0; y < aux._imageDivisionConfig[1]; y++)
            {
                Level auxLevel;

                //Loads the Level.
                file = File.Open(i_path + Path.DirectorySeparatorChar + "Levels" + Path.DirectorySeparatorChar + x + "x" + y + Path.DirectorySeparatorChar + x + "x" + y + LEVEL_EXT, FileMode.Open);
                using (file)
                {
                    auxLevel = ((Level)(bf.Deserialize(file)));
                }

                //Passes the progress value to the callback.
                progress += 1 / (float)(aux._imageDivisionConfig[0] * aux._imageDivisionConfig[1]);
                i_progressCallback(progress);

                yield return null;

                //Adds the Level to the dictionary.
                aux._levels.Add(new Vector2(x, y), auxLevel);

                //Loads the Level image.
                Texture2D auxText = new Texture2D(4, 4);
                auxText.LoadImage(File.ReadAllBytes(i_path + Path.DirectorySeparatorChar + "Levels" + Path.DirectorySeparatorChar + x + "x" + y + Path.DirectorySeparatorChar + x + "x" + y + ".jpg"));
                auxLevel._img = auxText;
            }
        }

        //Passes the world to the callback.
        i_worldCallback(aux);
    }


    /// <summary>
    /// Loads a Level in a previously generated World.
    /// </summary>
    /// <param name="i_path">The World path.</param>
    /// <param name="i_pos">>The Level position.</param>
    /// <param name="i_worldCallback">The callback used to catch the loaded world.</param>
    /// <param name="i_progressCallback">The callback used to catch the progress value.</param>
    public IEnumerator LoadLevel(string i_path, Vector2 i_pos, Action<World> i_worldCallback, Action<float> i_progressCallback)
    {
        if (!Directory.Exists(i_path))
            yield break;
        string[] files = Directory.GetFiles(i_path);
        if (files.Length != 2 || !(Path.GetExtension(files[0]).ToLower().Equals(".jpg") || Path.GetExtension(files[0]).ToLower().Equals(WORLD_EXT)) || !(Path.GetExtension(files[1]).ToLower().Equals(".jpg") || Path.GetExtension(files[1]).ToLower().Equals(WORLD_EXT)) || Path.GetExtension(files[0]).ToLower().Equals(Path.GetExtension(files[1]).ToLower()))
            yield break;
        string path1 = files[0].Remove(files[0].Length - Path.GetExtension(files[0]).Length, Path.GetExtension(files[0]).Length);
        string path2 = files[1].Remove(files[1].Length - Path.GetExtension(files[1]).Length, Path.GetExtension(files[1]).Length);   
        if (!path1.Equals(path2))
            yield break;

        //Sets up the FileStream and BinaryFormatter.
        FileStream file = File.Open(path1 + WORLD_EXT, FileMode.Open); 
        BinaryFormatter bf = new BinaryFormatter();
        SurrogateSelector ss = new SurrogateSelector();

        //Used for World serialization.
        WorldSerializationSurrogate worldSs = new WorldSerializationSurrogate();

        //Used for Level serialization.
        Vector2SerializationSurrogate v2Ss = new Vector2SerializationSurrogate();
        LevelSerializationSurrogate levelSs = new LevelSerializationSurrogate();

        //Used for LevelCell serialization.
        ColorSerializationSurrogate colorSs = new ColorSerializationSurrogate();
        LevelCellSerializationSurrogate levelCellSs = new LevelCellSerializationSurrogate();

        StreamingContext sc = new StreamingContext(StreamingContextStates.All); 
        ss.AddSurrogate(typeof(World), sc, worldSs);
        ss.AddSurrogate(typeof(Vector2), sc, v2Ss);
        ss.AddSurrogate(typeof(Level), sc, levelSs);
        ss.AddSurrogate(typeof(Color), sc, colorSs);
        ss.AddSurrogate(typeof(LevelCell), sc, levelCellSs);
        bf.SurrogateSelector = ss;
        World aux;

        //Loads the World.
        using (file)
        {
            aux = ((World)(bf.Deserialize(file)));
        }

        yield return null;

        //Creates the Level dictionary.
        aux._levels = new Dictionary<Vector2, Level>();

        Level auxLevel;

        //Loads the Level.
        file = File.Open(i_path + Path.DirectorySeparatorChar + "Levels" + Path.DirectorySeparatorChar + (int)i_pos.x + "x" + (int)i_pos.y + Path.DirectorySeparatorChar + (int)i_pos.x + "x" + (int)i_pos.y + LEVEL_EXT, FileMode.Open);
        using (file)
        {
            auxLevel = ((Level)(bf.Deserialize(file)));
        }
        aux._levels.Add(i_pos, auxLevel);

        yield return null;

        //Creates the LevelCell dictionary.
        auxLevel._cells = new Dictionary<Vector2, LevelCell>();

        float progress = 0;

        //For each column
        for (int i = 0; i < ImgProcessManager.Instance._mapSize; i++)
        {

            //For each row.
            for (int j = 0; j < ImgProcessManager.Instance._mapSize; j++)
            {
                LevelCell auxCell;
                Texture2D auxText;

                //Loads the LevelCell.
                file = File.Open(i_path + Path.DirectorySeparatorChar + "Levels" + Path.DirectorySeparatorChar + (int)i_pos.x + "x" + (int)i_pos.y + Path.DirectorySeparatorChar + "Cells" + Path.DirectorySeparatorChar + i + "x" + j + Path.DirectorySeparatorChar + i + "x" + j + LEVEL_CELL_EXT, FileMode.Open);
                using (file)
                {
                    auxCell = ((LevelCell)(bf.Deserialize(file)));
                }
                auxLevel._cells.Add(new Vector2(i, j), auxCell);

                //Passes the progress value to the callback.
                progress += 1 / (float)(ImgProcessManager.Instance._mapSize * ImgProcessManager.Instance._mapSize);
                i_progressCallback(progress);

                yield return null;

                //Loads the LevelCell image.
                auxText = new Texture2D(4, 4);
                auxText.LoadImage(File.ReadAllBytes(i_path + Path.DirectorySeparatorChar + "Levels" + Path.DirectorySeparatorChar + (int)i_pos.x + "x" + (int)i_pos.y + Path.DirectorySeparatorChar + "Cells" + Path.DirectorySeparatorChar + i + "x" + j + Path.DirectorySeparatorChar + i + "x" + j + ".jpg"));
                auxCell._img = auxText;

                yield return null;
            }
        }

        //Passes the World to the callback.
        i_worldCallback(aux);
    }

    /// <summary>
    /// Saves the Level state as completed.
    /// </summary>
    /// <param name="i_path">The World path.</param>
    /// <param name="i_levelPos">The Level position.</param>
    public void CompleteLevel(string i_path, Vector2 i_levelPos)
    {

        //Sets up the FileStream and BinaryFormatter
        FileStream file = File.Open(i_path + Path.DirectorySeparatorChar + "Levels" + Path.DirectorySeparatorChar + (int)i_levelPos.x + "x" + (int)i_levelPos.y + Path.DirectorySeparatorChar + (int)i_levelPos.x + "x" + (int)i_levelPos.y + LEVEL_EXT, FileMode.Open);
        BinaryFormatter bf = new BinaryFormatter();
        SurrogateSelector ss = new SurrogateSelector();
        Vector2SerializationSurrogate v2Ss = new Vector2SerializationSurrogate();
        LevelSerializationSurrogate levelSs = new LevelSerializationSurrogate();
        StreamingContext sc = new StreamingContext(StreamingContextStates.All); 
        ss.AddSurrogate(typeof(Vector2), sc, v2Ss);
        ss.AddSurrogate(typeof(Level), sc, levelSs);
        bf.SurrogateSelector = ss;

        //Loads the Level, updates completed value, and saves the Level.
        Level auxLevel;
        using (file)
        {
            auxLevel = ((Level)(bf.Deserialize(file)));
            auxLevel._completed = true;
            file.Position = 0;
            bf.Serialize(file, auxLevel);
        }
    }
}
