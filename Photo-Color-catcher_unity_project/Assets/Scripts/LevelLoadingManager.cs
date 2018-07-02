using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicDataTypes;
using UnityEngine.UI;
using UnityEngine.Serialization;

/// <summary>
/// Singleton used to manage level generation from a Level in a previously generated World.
/// </summary>
public class LevelLoadingManager: Singleton<LevelLoadingManager>
{
    /// <summary>
    /// The Prefab used as uncolored level cell for level generation.
    /// </summary>
    [SerializeField]
    private GameObject p_floorTile;

    /// <summary>
    /// Gets the Prefab used as uncolored level cell for level generation..
    /// </summary>
    /// <value>The Prefab of the floor tile.</value>
    public GameObject _floorTile
    {
        get
        {
            return p_floorTile;
        }
    }

    /// <summary>
    /// The Prefab used as container for the image  tho show when a cell is completed.
    /// </summary>
    [SerializeField]
    private GameObject p_cellBackground;

    /// <summary>
    /// The Prefab used as barriers for level generation.
    /// </summary>
    [SerializeField]
    private GameObject p_barrier;

    /// <summary>
    /// The GameObject used as container of all the color goal color cells in the game HUD.
    /// </summary>
    [SerializeField]
    private GameObject p_mapGoal;

    /// <summary>
    /// The player Prefab.
    /// </summary>
    [SerializeField]
    private GameObject p_player;

    /// <summary>
    /// The bar that shows the progress of the level loading.
    /// </summary>
    [SerializeField]
    private RectTransform p_progressBar;

    /// The Text label of the loading screen.
    [SerializeField]
    private Text p_loadingText;

    /// The FXSequence launched when the level generation starts.
    [SerializeField]
    private FXSequence p_startFX;

    /// The FXSequence launched when the level generation ends.
    [SerializeField]
    private FXSequence p_finishFX;

    protected LevelLoadingManager()
    {
    }

    /// <summary>
    /// Starts the level loading and generation coroutine.
    /// </summary>
    /// <param name="i_path">The World path.</param>
    /// <param name="i_levelPos">The Level position in the World.</param>
    public void StartLoadLevel(string i_path, Vector2 i_levelPos)
    {
        StartCoroutine(LoadLevel(i_path, i_levelPos));
    }

    /// <summary>
    /// Loads and generates the Level.
    /// </summary>
    /// <param name="i_path">The World path.</param>
    /// <param name="i_levelPos">The Level position in the World.</param>
    public IEnumerator LoadLevel(string i_path, Vector2 i_levelPos)
    {
        //Launches the starting FXSequence.
        p_startFX.Launch();

        //Disables the InputMenuController of the Level selection screen.
        WorldLevelsLoadingManager.Instance.DisableMenuController();

        //Sets up the loading screen.
        p_loadingText.text = "Loading level...";
        p_progressBar.anchorMin = (new Vector2(0, 0));

        //Loads the Level.
        World world = null;
        yield return StartCoroutine(PersistenceManager.Instance.LoadLevel(i_path, i_levelPos, x => world = x, x => p_progressBar.anchorMin = (new Vector2(x, 0))));

        //Adds the Level info to the LevelController.
        LevelController.Instance.AddLevelInfo(world._name, i_path, i_levelPos);

        Level level = world._levels[i_levelPos];

        int max = (int)Mathf.Pow(level._cells.Count, 0.5f);

        //For each column.
        for (int x = 0; x < max; x++)
        {
            //For each row.
            for (int y = 0; y < max; y++)
            {
                GameObject auxBarrier;
                GameObject auxGoalCell;
                GameObject auxPlayer;

                //Creates the cell.
                GameObject auxCell = (GameObject)Instantiate(p_floorTile);
                auxCell.transform.position = new Vector3((x - (max - 1) / 2f) * p_floorTile.GetSize().x, (y - (max - 1) / 2f) * p_floorTile.GetSize().y, 0);
                auxCell.GetComponent<CellColorGoal>()._RGBGoal = level._cells[new Vector2(x, y)]._rgbComponents;

                //Adds the cell to the LevelController.
                LevelController.Instance.AddCell(auxCell);

                //Creates the cell background.
                auxCell = (GameObject)Instantiate(p_cellBackground, auxCell.transform.position, auxCell.transform.localRotation);
                Sprite spr = Sprite.Create(level._cells[new Vector2(x, y)]._img, new Rect(0, 0, level._cells[new Vector2(x, y)]._img.width, level._cells[new Vector2(x, y)]._img.height), new Vector2(0.5f, 0.5f));
                auxCell.GetChild("Background").GetComponent<SpriteRenderer>().sprite = spr;
                auxCell.GetChild("Background").SetSize(auxCell.GetChild("Border").GetSize());

                //Adds the cell background to the LevelController.
                LevelController.Instance.AddCellBackground(auxCell);

                //If the cell is the first cell at the bottom left of the level.
                if (x == 0 && y == 0)
                {
                    //Creates the player.
                    auxPlayer = ((GameObject)Instantiate(p_player));
                    auxPlayer.transform.position = auxCell.transform.position;

                    //Adds the player to the LevelController.
                    LevelController.Instance.AddPlayer(auxPlayer);

                    //Adds the player position to the LevelController.
                    LevelController.Instance.NextPosition(auxPlayer, auxPlayer.transform.position);
                }

                //If the cell is not the last cell of the row and it is not adjacent to the cell at the rigt of this cell.
                if (x < max - 1 && !level._graph.IsAdjacent(new Vector2(x, y), new Vector2(x + 1, y)))
                {
                    //Creates a barrier between both cells.
                    auxBarrier = (GameObject)Instantiate(p_barrier);
                    auxBarrier.transform.position = auxCell.transform.position + new Vector3(p_floorTile.GetSize().x / 2f, 0, 0);

                    //Adds the barrier to the LevelController.
                    LevelController.Instance.AddBarrier(auxBarrier);
                }

                //If the cell is not the last cell of the column and it is not adjacent to its upper cell.
                if (y < max - 1 && !level._graph.IsAdjacent(new Vector2(x, y), new Vector2(x, y + 1)))
                {
                    //Creates a barrier between both cells.
                    auxBarrier = (GameObject)Instantiate(p_barrier);
                    auxBarrier.transform.position = auxCell.transform.position + new Vector3(0, p_floorTile.GetSize().y / 2f, 0);
                    auxBarrier.transform.Rotate(new Vector3(0, 0, 90));

                    //Adds the barrier to the LevelController.
                    LevelController.Instance.AddBarrier(auxBarrier);
                }

                //If it is the first cell of the row.
                if (x == 0)
                {
                    //Creates an invisible barrier at the left of the cell.
                    auxBarrier = (GameObject)Instantiate(p_barrier);
                    auxBarrier.transform.position = auxCell.transform.position - new Vector3(p_floorTile.GetSize().x / 2f, 0, 0);
                    auxBarrier.GetComponent<SpriteRenderer>().enabled = false;

                    //Adds the barrier to the LevelController.
                    LevelController.Instance.AddBarrier(auxBarrier);
                }

                //If it is the last cell of the row.
                if (x == max - 1)
                {
                    //Creates an invisible barrier at the right of the cell.
                    auxBarrier = (GameObject)Instantiate(p_barrier);
                    auxBarrier.transform.position = auxCell.transform.position + new Vector3(p_floorTile.GetSize().x / 2f, 0, 0);
                    auxBarrier.GetComponent<SpriteRenderer>().enabled = false;

                    //Adds the barrier to the LevelController.
                    LevelController.Instance.AddBarrier(auxBarrier);
                }

                //If it is the first cell of the column.
                if (y == 0)
                {
                    //Creates an invisible barrier at the lower side of this cell.
                    auxBarrier = (GameObject)Instantiate(p_barrier);
                    auxBarrier.transform.position = auxCell.transform.position - new Vector3(0, p_floorTile.GetSize().y / 2f, 0);
                    auxBarrier.transform.Rotate(new Vector3(0, 0, 90));
                    auxBarrier.GetComponent<SpriteRenderer>().enabled = false;

                    //Adds the barrier to the LevelController.
                    LevelController.Instance.AddBarrier(auxBarrier);
                }

                //If it is the last cell of the column.
                if (y == max - 1)
                {
                    //Creates an invisible barrier at the upper side of this cell.
                    auxBarrier = (GameObject)Instantiate(p_barrier);
                    auxBarrier.transform.position = auxCell.transform.position + new Vector3(0, p_floorTile.GetSize().y / 2f, 0);
                    auxBarrier.transform.Rotate(new Vector3(0, 0, 90));
                    auxBarrier.GetComponent<SpriteRenderer>().enabled = false;

                    //Adds the barrier to the LevelController.
                    LevelController.Instance.AddBarrier(auxBarrier);
                }

                //Creates and sets up the goal cell color.
                auxGoalCell = new GameObject("Goal(" + x + "," + y + ")", typeof(RectTransform));
                auxGoalCell.transform.SetParent(p_mapGoal.transform);
                (auxGoalCell.transform as RectTransform).offsetMin = Vector2.zero;
                (auxGoalCell.transform as RectTransform).offsetMax = Vector2.zero;
                (auxGoalCell.transform as RectTransform).anchorMin = new Vector2((x + 1 + x * 10) / (float)(max + 1 + max * 10), (y + 1 + y * 10) / (float)(max + 1 + max * 10));
                (auxGoalCell.transform as RectTransform).anchorMax = new Vector2((x + 1 + (x + 1) * 10) / (float)(max + 1 + max * 10), (y + 1 + (y + 1) * 10) / (float)(max + 1 + max * 10));
                Color goalColor = Color.black;
                if (level._cells[new Vector2(x, y)]._rgbComponents._r == true)
                {
                    goalColor.r = 1;
                }
                if (level._cells[new Vector2(x, y)]._rgbComponents._g == true)
                {
                    goalColor.g = 1;
                }
                if (level._cells[new Vector2(x, y)]._rgbComponents._b == true)
                {
                    goalColor.b = 1;
                }
                auxGoalCell.AddComponent<Image>().color = goalColor;

                //Adds the goal cell color to the LevelController.
                LevelController.Instance.AddGoal(auxGoalCell);
            }
        }

        //Sets the camera size.
        float ySize = p_floorTile.GetSize().y * max;
        float xSize = p_floorTile.GetSize().x * max;
        float camXsize;
        float camYsize;
        camYsize = ySize;
        camXsize = ySize * (Screen.width / (float)Screen.height);
        if (camXsize > xSize)
            Camera.main.orthographicSize = ySize / 2f;
        else
        {
            camXsize = xSize;
            camYsize = xSize * (Screen.height / (float)Screen.width);
            Camera.main.orthographicSize = camYsize / 2f;
        }

        //Generates the first enemies.
        LevelController.Instance.FirstEnemiesGeneration();

        //Enables the InputMenuController that was active on the Level selection screen.
        WorldLevelsLoadingManager.Instance.EnableMenuController();

        //Launches the finishing FXSequence.
        p_finishFX.Launch();
    }
}
