using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CIEColor;
using System;
using BasicDataTypes;

/// <summary>
/// Singleton object for image processing.
/// </summary>
public class ImgProcessManager : Singleton<ImgProcessManager>
{
    /// <summary>
    /// The max pixels processed per frame.
    /// </summary>
    public int _maxPixelsProcessedPerFrame;

    /// <summary>
    /// The size of the map.
    /// </summary>
    public int _mapSize;

    protected ImgProcessManager()
    {
    }

    /*
    void Start()
    {
        PersistenceManager.MainLoad();
    }
    */

    /// <summary>
    /// Starts the world generation coroutine.
    /// </summary>
    /// <param name="i_img">Image to process.</param>
    /// <param name="i_imageConfig">Image configuration.</param>
    /// <param name="i_name">Image name.</param>
    public void StartProcessImageAndGenerateWorld(Texture2D i_img, int[] i_imageConfig, string i_name)
    {
        StartCoroutine(ProcessImageAndGenerateWorld(i_img, i_imageConfig, i_name));
    }

    /// <summary>
    /// Processes the image and generates a world.
    /// </summary>
    /// <param name="i_img">Image to process.</param>
    /// <param name="i_imageConfig">Image configuration.</param>
    /// <param name="i_name">Image name.</param>
    public IEnumerator ProcessImageAndGenerateWorld(Texture2D i_img, int[] i_imageConfig, string i_name)
    {
        World world = new World();
        Texture2D auxText = i_img;
        int cellSize;

        //If image is horizontal.
        if (auxText.width > auxText.height)
        {
            //Calculates cell size (width/columns)
            cellSize = Mathf.CeilToInt(auxText.width / (float)i_imageConfig[0]);
        }

        //If image is vertical.
        else
        {
            //Calculates cell size (height/rows)
            cellSize = Mathf.CeilToInt(auxText.height / (float)i_imageConfig[1]);
        }

        //Resizes the texture.
        auxText = auxText.ResizeBilinear(cellSize * i_imageConfig[0], cellSize * i_imageConfig[1]);

        //Assings the world image.
        world._img = new OnArrayImage(auxText);

        //Assigns the world name.
        world._name = i_name;

        //Assings the image configuration.
        world._imageConfig = i_imageConfig;

        yield return null;

        //Creates levels map and initializes it.
        world._levels = new Dictionary<Vector2,Level>();

        //For each column.
        for (int x = 0; x < i_imageConfig[0]; x++)
        {
            //For each row.
            for (int y = 0; y < i_imageConfig[1]; y++)
            {
                //Creates a new level and adds it to the map.
                world._levels.Add(new Vector2(x, y), new Level());

                //Initializes the level image.
                world._levels[new Vector2(x, y)]._img = new OnArrayImage(cellSize, cellSize);

                //Assings the level image.
                world._levels[new Vector2(x, y)]._img._pixels = auxText.GetPixels(x * cellSize, y * cellSize, cellSize, cellSize);

                yield return null;
            }
        }

        Dictionary<Vector3,RGBContent> sampleColors;
        CIELabColor auxCIELab;

        //Stores the sample colors: all posible combinations of RGB components.
        sampleColors = new Dictionary<Vector3, RGBContent>();
        auxCIELab = Color.red.ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab.l, auxCIELab.a, auxCIELab.b), new RGBContent(true, false, false));
        auxCIELab = Color.green.ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab.l, auxCIELab.a, auxCIELab.b), new RGBContent(false, true, false));
        auxCIELab = Color.blue.ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab.l, auxCIELab.a, auxCIELab.b), new RGBContent(false, false, true));
        auxCIELab = (new Color(1, 1, 0)).ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab.l, auxCIELab.a, auxCIELab.b), new RGBContent(true, true, false));
        auxCIELab = (new Color(1, 0, 1)).ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab.l, auxCIELab.a, auxCIELab.b), new RGBContent(true, false, true));
        auxCIELab = (new Color(0, 1, 1)).ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab.l, auxCIELab.a, auxCIELab.b), new RGBContent(false, true, true));
        auxCIELab = (new Color(1, 1, 1)).ToCIELab();
        sampleColors.Add(new Vector3(auxCIELab.l, auxCIELab.a, auxCIELab.b), new RGBContent(true, true, true));

        yield return null;

        Vector3 goal = Vector3.zero;
        Vector3 average;
        float distance;
        RGBContent rgbSample;
        List<List<Vector2>> connectedVertices;
        float minGrayDif;
        Vector2 vertex = Vector2.zero;
        Vector2 adjacent = Vector2.zero;
        bool notConnected;
        int foundVertices;

        //For each level.
        foreach (Vector2 pos in world._levels.Keys)
        {
            //Gets the level image as Texture2D.
            auxText = new Texture2D(world._levels[pos]._img._width, world._levels[pos]._img._height);
            auxText.SetPixels(world._levels[pos]._img._pixels);
            auxText.Apply();

            //Calculates cell size (width/columns)
            cellSize = Mathf.CeilToInt(auxText.width / (float)_mapSize);

            //Resizes the texture.
            auxText = auxText.ResizeBilinear(cellSize * _mapSize, cellSize * _mapSize);

            //Creates level cells map and initializes it.
            world._levels[pos]._cells = new Dictionary<Vector2,LevelCell>();

            //NECESSARY?????
            //For each column.
            for (int x = 0; x < _mapSize; x++)
            {
                //For each row.
                for (int y = 0; y < _mapSize; y++)
                {
                    //Creates a new level cell and adds it to the map.
                    world._levels[pos]._cells.Add(new Vector2(x, y), new LevelCell());

                    //Initializes the cell image.
                    world._levels[pos]._cells[new Vector2(x, y)]._img = new OnArrayImage(cellSize, cellSize);

                    //Assings the cell image.
                    world._levels[pos]._cells[new Vector2(x, y)]._img._pixels = auxText.GetPixels(x * cellSize, y * cellSize, cellSize, cellSize);

                    yield return null;
                }
            }

            LevelCell cell;

            //For each cell.
            foreach (Vector2 pos2 in world._levels[pos]._cells.Keys)
            {
                cell = world._levels[pos]._cells[pos2];

                //Sets average color as black.
                cell._average = Color.black;

                //For each pixel.
                for (int i = 0; i < cell._img._pixels.Length; i++)
                {
                    //Adds pixel colors to cell average color.
                    cell._average.r += cell._img._pixels[i].r / cell._img._pixels.Length;
                    cell._average.g += cell._img._pixels[i].g / cell._img._pixels.Length;
                    cell._average.b += cell._img._pixels[i].b / cell._img._pixels.Length;

                    //Adds pixel grayscale value to cell average grayscale value.
                    cell._grayscale += cell._img._pixels[i].grayscale / cell._img._pixels.Length;
                }

                yield return null;
            }

            //Creates connected level cells graph and initializes it.
            world._levels[pos]._graph = new GraphType<Vector2>();

            //For every cell.
            foreach (Vector2 pos2 in world._levels[pos]._cells.Keys)
            {
                //Set distance as -1.
                distance = -1;

                //Gets cell average color as CIELab.
                auxCIELab = world._levels[pos]._cells[pos2]._average.ToCIELab();

                //Creates a Vector3 using the the cell average color.
                average = new Vector3(auxCIELab.l, auxCIELab.a, auxCIELab.b);

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

                //Gets RGB combination of the sample.
                rgbSample = sampleColors[goal];

                //Assigns the RGB combination of the most similar sample.
                world._levels[pos]._cells[pos2]._rgbComponents = new RGBContent(rgbSample._r, rgbSample._g, rgbSample._b);

                //Adds the vertex to the graph.
                world._levels[pos]._graph.AddVertex(pos2);

                yield return null;
            }

            //For each row.
            for (int y = 0; y < _mapSize; y++)
            {
                //For each column.
                for (int x = 0; x < _mapSize; x++)
                {
                    //Checks if grayscale difference between the cell and adjacent < 0.5.
                    if (x < _mapSize - 1 && Mathf.Abs(world._levels[pos]._cells[new Vector2(x, y)]._grayscale - world._levels[pos]._cells[new Vector2(x + 1, y)]._grayscale) < 0.5f)

                        //Makes the vertices adjacent: no barrier between cells.
                        world._levels[pos]._graph.AddAdjacent(new Vector2(x, y), new Vector2(x + 1, y));

                    //Checks if grayscale difference between the cell and adjacent < 0.5.
                    if (y < _mapSize - 1 && Mathf.Abs(world._levels[pos]._cells[new Vector2(x, y)]._grayscale - world._levels[pos]._cells[new Vector2(x, y + 1)]._grayscale) < 0.5f)

                        //Makes the vertices adjacent: no barrier between cells.
                        world._levels[pos]._graph.AddAdjacent(new Vector2(x, y), new Vector2(x, y + 1));
                }
            }

            yield return null;

            //Get connected vertices list.
            connectedVertices = world._levels[pos]._graph.GetConnectedVertices();

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

                            //If the cells are not connected and the grayscale difference between both is lower.
                            if (notConnected && Mathf.Abs(world._levels[pos]._cells[new Vector2(x, y)]._grayscale - world._levels[pos]._cells[new Vector2(x + 1, y)]._grayscale) < minGrayDif)
                            {
                                //Sets new min grayscale difference. 
                                minGrayDif = Mathf.Abs(world._levels[pos]._cells[new Vector2(x, y)]._grayscale - world._levels[pos]._cells[new Vector2(x + 1, y)]._grayscale);

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

                            //If the cells are not connected and the grayscale difference between both is lower.
                            if (notConnected && Mathf.Abs(world._levels[pos]._cells[new Vector2(x, y)]._grayscale - world._levels[pos]._cells[new Vector2(x, y + 1)]._grayscale) < minGrayDif)
                            {
                                //Sets new min grayscale difference.
                                minGrayDif = Mathf.Abs(world._levels[pos]._cells[new Vector2(x, y)]._grayscale - world._levels[pos]._cells[new Vector2(x, y + 1)]._grayscale);

                                //Gets vertex.
                                vertex = new Vector2(x, y);

                                //Gets adjacent vertex.
                                adjacent = new Vector2(x, y + 1);
                            }
                        }

                    }
                }

                //Makes the vertices adjacent
                world._levels[pos]._graph.AddAdjacent(vertex, adjacent);

                yield return null;

                //Gets connected vertices list.
                connectedVertices = world._levels[pos]._graph.GetConnectedVertices();
            }
        }

        //Stores the world on disk.
        PersistenceManager.SaveWorld(world, i_name);
    }
}
