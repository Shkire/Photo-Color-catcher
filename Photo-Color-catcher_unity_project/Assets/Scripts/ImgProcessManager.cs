using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CIEColor;
using System;
using BasicDataTypes;
using UnityEngine.UI;
using UnityEngine.Serialization;

/// <summary>
/// Singleton object used for image processing.
/// </summary>
public class ImgProcessManager : Singleton<ImgProcessManager>
{
    /// <summary>
    /// The size of the map.
    /// </summary>
    public int _mapSize;

    /// <summary>
    /// The bar that shows the progress of the image processing.
    /// </summary>
    [SerializeField]
    private RectTransform p_progressBar;

    /// <summary>
    /// The Text label of the loading (processing) screen.
    /// </summary>
    [SerializeField]
    private Text p_creatingWorldText;

    /// <summary>
    /// The FXSequence launched when the image processing starts.
    /// </summary>
    [SerializeField]
    private FXSequence p_startFX;

    /// <summary>
    /// The FXSequence launched when the image processing ends.
    /// </summary>
    [SerializeField]
    private FXSequence p_finishFX;

    protected ImgProcessManager()
    {
    }

    /// <summary>
    /// Starts the world generation coroutine.
    /// </summary>
    /// <param name="i_img">The image to process.</param>
    /// <param name="i_imageDivisionConfig">The image division configuration ([columns,rows]).</param>
    /// <param name="i_name">The image name.</param>
    public void StartProcessImageAndGenerateWorld(Texture2D i_img, int[] i_imageDivisionConfig, string i_name)
    {
        StartCoroutine(ProcessImageAndGenerateWorld(i_img, i_imageDivisionConfig, i_name));
    }

    /// <summary>
    /// Processes the image and generates a world.
    /// </summary>
    /// <param name="i_img">The image to process.</param>
    /// <param name="i_imageDivisionConfig">The image division configuration.</param>
    /// <param name="i_name">The image name.</param>
    public IEnumerator ProcessImageAndGenerateWorld(Texture2D i_img, int[] i_imageDivisionConfig, string i_name)
    {
        //Launches the start FXSequence.
        p_startFX.Launch();

        //Disables the InputMenuController of the image configuration Menu.
        ImageConfigurationManager.Instance.DisableMenuController();

        //Sets up the loading screen label.
        p_creatingWorldText.text = "Creating world...";

        float progress = 0;
        p_progressBar.anchorMin = (new Vector2(0, 0));

        World world = new World();
        Texture2D auxText = i_img;
        int cellSize;

        //If image is horizontal.
        if (auxText.width > auxText.height)
        {
            //Calculates cell (Level) size (width/columns)
            cellSize = Mathf.CeilToInt(auxText.width / (float)i_imageDivisionConfig[0]);
        }

        //If image is vertical.
        else
        {
            //Calculates cell (Level) size (height/rows)
            cellSize = Mathf.CeilToInt(auxText.height / (float)i_imageDivisionConfig[1]);
        }

        //Resizes the texture.
        auxText = auxText.ResizeBilinear(cellSize * i_imageDivisionConfig[0], cellSize * i_imageDivisionConfig[1]);

        //Assings the World image.
        world._img = auxText;

        //Assigns the World name.
        world._name = i_name;

        //Assings the image configuration.
        world._imageDivisionConfig = i_imageDivisionConfig;

        //Creates World Level map and initializes it.
        world._levels = new Dictionary<Vector2,Level>();

        //For each column.
        for (int x = 0; x < i_imageDivisionConfig[0]; x++)
        {
            //For each row.
            for (int y = 0; y < i_imageDivisionConfig[1]; y++)
            {
                //Creates a new Level and adds it to the map.
                world._levels.Add(new Vector2(x, y), new Level());

                //Initializes the Level image.
                world._levels[new Vector2(x, y)]._img = new Texture2D(cellSize, cellSize);

                //Assings the Level image.
                world._levels[new Vector2(x, y)]._img.SetPixels(auxText.GetPixels(x * cellSize, y * cellSize, cellSize, cellSize));
                world._levels[new Vector2(x, y)]._img.Apply();

                //Updates progress bar.
                progress += 0.25f / (float)(i_imageDivisionConfig[0] * i_imageDivisionConfig[1]);
                p_progressBar.anchorMin = (new Vector2(progress, 0));

                yield return null;
            }
        }

        Dictionary<Vector3,RGBContent> sampleColors;
        CIELabColor auxCIELab;

        //Stores the sample colors: all posible combinations of RGB components.
        sampleColors = new Dictionary<Vector3, RGBContent>();
        auxCIELab = Color.red.ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab._l, auxCIELab._a, auxCIELab._b), new RGBContent(true, false, false));
        auxCIELab = Color.green.ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab._l, auxCIELab._a, auxCIELab._b), new RGBContent(false, true, false));
        auxCIELab = Color.blue.ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab._l, auxCIELab._a, auxCIELab._b), new RGBContent(false, false, true));
        auxCIELab = (new Color(1, 1, 0)).ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab._l, auxCIELab._a, auxCIELab._b), new RGBContent(true, true, false));
        auxCIELab = (new Color(1, 0, 1)).ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab._l, auxCIELab._a, auxCIELab._b), new RGBContent(true, false, true));
        auxCIELab = (new Color(0, 1, 1)).ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab._l, auxCIELab._a, auxCIELab._b), new RGBContent(false, true, true));
        auxCIELab = (new Color(1, 1, 1)).ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab._l, auxCIELab._a, auxCIELab._b), new RGBContent(true, true, true));

        //For each Level.
        foreach (KeyValuePair<Vector2,Level> levelDictEntry in world._levels)
        {
            yield return null;

            //Gets the Level image.
            auxText = levelDictEntry.Value._img;

            //Calculates LevelCell size (width/columns)
            cellSize = Mathf.CeilToInt(auxText.width / (float)_mapSize);

            //Resizes the texture.
            auxText = auxText.ResizeBilinear(cellSize * _mapSize, cellSize * _mapSize);

            //Creates Level LevelCell map and initializes it.
            levelDictEntry.Value._cells = new Dictionary<Vector2,LevelCell>();

            //For each column.
            for (int x = 0; x < _mapSize; x++)
            {
                //For each row.
                for (int y = 0; y < _mapSize; y++)
                {
                    //Creates a new LevelCell and adds it to the map.
                    levelDictEntry.Value._cells.Add(new Vector2(x, y), new LevelCell());

                    //Initializes the LevelCell image.
                    levelDictEntry.Value._cells[new Vector2(x, y)]._img = new Texture2D(cellSize, cellSize);

                    //Assings the LevelCell image.
                    levelDictEntry.Value._cells[new Vector2(x, y)]._img.SetPixels(auxText.GetPixels(x * cellSize, y * cellSize, cellSize, cellSize));

                    //Updates the progress bar.
                    progress += (1 - 0.25f) / (float)(world._levels.Count + 1) / 5f / (float)(_mapSize * _mapSize);
                    p_progressBar.anchorMin = (new Vector2(progress, 0));

                    yield return null;
                }
            }
                
            //Creates the connected LevelCell graph and initializes it.
            levelDictEntry.Value._graph = new GraphType<Vector2>();

            //For each cell.
            foreach (KeyValuePair<Vector2,LevelCell> cellDictEntry in levelDictEntry.Value._cells)
            {
                LevelCell cell = cellDictEntry.Value;

                //Sets average color as black.
                cell._average = Color.black;

                Color[] pixels = cell._img.GetPixels();

                //For each pixel.
                for (int i = 0; i < pixels.Length; i++)
                {
                    //Adds pixel colors to LevelCell average color.
                    cell._average.r += pixels[i].r / pixels.Length;
                    cell._average.g += pixels[i].g / pixels.Length;
                    cell._average.b += pixels[i].b / pixels.Length;

                    //Adds pixel grayscale value to LevelCell average grayscale value.
                    cell._grayscale += pixels[i].grayscale / pixels.Length;
                }

                //Updates progress bar.
                progress += (1 - 0.25f) / (float)(world._levels.Count + 1) / 5f / (float)levelDictEntry.Value._cells.Count;
                p_progressBar.anchorMin = (new Vector2(progress, 0));

                yield return null;

                //Sets distance as -1.
                float distance = -1;

                //Gets LevelCell average color as CIELab.
                auxCIELab = cellDictEntry.Value._average.ToCIELab();

                //Creates a Vector3 using the the cell average color.
                Vector3 average = new Vector3(auxCIELab._l, auxCIELab._a, auxCIELab._b);

                Vector3 goal = Vector3.zero;

                //For each sample.
                foreach (Vector3 sample in sampleColors.Keys)
                {
                    //If distance is -1 or Delta E (color difference) is lower than distance.
                    if (distance == -1 || (Vector3.Distance(average, sample) < distance))
                    {
                        //Sample is the new goal.
                        goal = sample;

                        //Distance is the new Delta E value.
                        distance = Vector3.Distance(average, goal);
                    }
                }

                //Gets the RGB combination of the sample.
                RGBContent rgbSample = sampleColors[goal];

                //Assigns the RGB combination of the most similar sample.
                cellDictEntry.Value._rgbComponents = new RGBContent(rgbSample._r, rgbSample._g, rgbSample._b);

                //Adds the vertex to the graph.
                levelDictEntry.Value._graph.AddVertex(cellDictEntry.Key);

                //Updates progress bar.
                progress += (1 - 0.25f) / (float)(world._levels.Count + 1) / 5f / (float)levelDictEntry.Value._cells.Count;
                p_progressBar.anchorMin = (new Vector2(progress, 0));

                yield return null;
            }

            //For each row.
            for (int y = 0; y < _mapSize; y++)
            {
                //For each column.
                for (int x = 0; x < _mapSize; x++)
                {
                    //Checks if grayscale difference between the cell and adjacent < 0.5.
                    if (x < _mapSize - 1 && Mathf.Abs(levelDictEntry.Value._cells[new Vector2(x, y)]._grayscale - levelDictEntry.Value._cells[new Vector2(x + 1, y)]._grayscale) < 0.5f)

                        //Makes the vertices adjacent: no barrier between cells.
                        levelDictEntry.Value._graph.AddAdjacent(new Vector2(x, y), new Vector2(x + 1, y));

                    //Checks if grayscale difference between the cell and adjacent < 0.5.
                    if (y < _mapSize - 1 && Mathf.Abs(levelDictEntry.Value._cells[new Vector2(x, y)]._grayscale - levelDictEntry.Value._cells[new Vector2(x, y + 1)]._grayscale) < 0.5f)

                        //Makes the vertices adjacent: no barrier between cells.
                        levelDictEntry.Value._graph.AddAdjacent(new Vector2(x, y), new Vector2(x, y + 1));
                }
            }

            //Updates the progress bar.
            progress += (1 - 0.25f) / (float)(world._levels.Count + 1) / 5f;
            p_progressBar.anchorMin = (new Vector2(progress, 0));

            yield return null;

            //Get connected vertices list.
            List<List<Vector2>> connectedVertices = levelDictEntry.Value._graph.GetConnectedVertices();

            float minGrayDif;
            bool notConnected;
            int foundVertices;
            Vector2 vertex = Vector2.zero;
            Vector2 adjacent = Vector2.zero;

            //While the graph is not fully connected.
            while (connectedVertices.Count > 1)
            {
                //Sets min grayscale difference as 2.
                minGrayDif = 2f;

                //For each row.
                for (int y = 0; y < _mapSize; y++)
                {
                    //For each column.
                    for (int x = 0; x < _mapSize; x++)
                    {
                        if (x < _mapSize - 1)
                        {
                            notConnected = true;
                            foundVertices = 0;

                            //For each list of connected vertices.
                            for (int i = 0; i < connectedVertices.Count; i++)
                            {
                                //If the vertices are connected (on the same list).
                                if (foundVertices == 0 && connectedVertices[i].Contains(new Vector2(x, y)) && connectedVertices[i].Contains(new Vector2(x + 1, y)))
                                {
                                    notConnected = false;
                                    break;
                                }

                                //If one of the vertex is on the list.
                                if (connectedVertices[i].Contains(new Vector2(x, y)) || connectedVertices[i].Contains(new Vector2(x + 1, y)))
                                {
                                    foundVertices++;

                                    //If both vertices have been found on lists.
                                    if (foundVertices == 2)
                                        break;
                                }
                            }

                            //If the LevelCells are not connected and the grayscale difference between both is lower.
                            if (notConnected && Mathf.Abs(levelDictEntry.Value._cells[new Vector2(x, y)]._grayscale - levelDictEntry.Value._cells[new Vector2(x + 1, y)]._grayscale) < minGrayDif)
                            {
                                //Sets new min grayscale difference. 
                                minGrayDif = Mathf.Abs(levelDictEntry.Value._cells[new Vector2(x, y)]._grayscale - levelDictEntry.Value._cells[new Vector2(x + 1, y)]._grayscale);

                                //Gets vertex.
                                vertex = new Vector2(x, y);

                                //Gets adjacent vertex.
                                adjacent = new Vector2(x + 1, y);
                            }
                        }
                        if (y < _mapSize - 1)
                        {
                            notConnected = true;
                            foundVertices = 0;

                            //For each list of connected vertices.
                            for (int i = 0; i < connectedVertices.Count; i++)
                            {
                                //If the vertices are connected (on the same list).
                                if (foundVertices == 0 && connectedVertices[i].Contains(new Vector2(x, y)) && connectedVertices[i].Contains(new Vector2(x, y + 1)))
                                {
                                    notConnected = false;
                                    break;
                                }

                                //If one of the vertex is on the list.
                                if (connectedVertices[i].Contains(new Vector2(x, y)) || connectedVertices[i].Contains(new Vector2(x, y + 1)))
                                {
                                    foundVertices++;

                                    //If both vertices have been found on lists.
                                    if (foundVertices == 2)
                                        break;
                                }
                            }

                            //If the LevelCells are not connected and the grayscale difference between both is lower.
                            if (notConnected && Mathf.Abs(levelDictEntry.Value._cells[new Vector2(x, y)]._grayscale - levelDictEntry.Value._cells[new Vector2(x, y + 1)]._grayscale) < minGrayDif)
                            {
                                //Sets new min grayscale difference.
                                minGrayDif = Mathf.Abs(levelDictEntry.Value._cells[new Vector2(x, y)]._grayscale - levelDictEntry.Value._cells[new Vector2(x, y + 1)]._grayscale);

                                //Gets vertex.
                                vertex = new Vector2(x, y);

                                //Gets adjacent vertex.
                                adjacent = new Vector2(x, y + 1);
                            }
                        }

                    }
                }

                //Makes the vertices adjacent
                levelDictEntry.Value._graph.AddAdjacent(vertex, adjacent);

                yield return null;

                //Gets connected vertices list.
                connectedVertices = levelDictEntry.Value._graph.GetConnectedVertices();
            }

            //Updates the progress bar.
            progress += (1 - 0.25f) / (float)(world._levels.Count + 1) / 5f;
            p_progressBar.anchorMin = (new Vector2(progress, 0));
        }

        //Stores the world on disk.
        yield return StartCoroutine(PersistenceManager.Instance.SaveWorld(world));

        //Updates the progress bar.
        progress = 1;
        p_progressBar.anchorMin = (new Vector2(progress, 0));

        yield return null;

        //Enables the InputMenuController of the image configuration screen.
        ImageConfigurationManager.Instance.EnableMenuController();

        //Launches the finishing FXSequence.
        p_finishFX.Launch();
    }
}
