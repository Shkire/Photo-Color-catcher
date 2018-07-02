using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicDataTypes;
using UnityEngine.UI;

/// <summary>
/// Singleton used to manage the level during the game.
/// </summary>
public class LevelController : Singleton<LevelController>
{
    /// <summary>
    /// The maximum number of enemies at the same time during the game.
    /// </summary>
    [SerializeField]
    private int p_maxEnemies;

    /// <summary>
    /// The minimum time that an enemy needs to be spawned when it tis possible.
    /// </summary>
    [SerializeField]
    private float p_minSpawnTime;

    /// <summary>
    /// The maximum time that an enemy needs to be spawned when it tis possible.
    /// </summary>
    [SerializeField]
    private float p_maxSpawnTime;

    /// <summary>
    /// The remaining time to the enext enemy spawning.
    /// </summary>
    private float p_remainingTime;

    /// <summary>
    /// The R enemy Prefab.
    /// </summary>
    [SerializeField]
    private GameObject p_enemyR;

    /// <summary>
    /// The G enmy Prefab.
    /// </summary>
    [SerializeField]
    private GameObject p_enemyG;

    /// <summary>
    /// The B enemy Prefab.
    /// </summary>
    [SerializeField]
    private GameObject p_enemyB;

    /// <summary>
    /// The list of the cells of the level.
    /// </summary>
    private List<GameObject> p_cells;

    /// <summary>
    /// The remaining R enemies needed to complete the level.
    /// </summary>
    private int p_remainingR;

    /// <summary>
    /// The remaining G enemies needed to complete the level.
    /// </summary>
    private int p_remainingG;

    /// <summary>
    /// The remaining R enemies needed to complete the level.
    /// </summary>
    private int p_remainingB;

    /// <summary>
    /// Dictionary that contains the GameObject (enemy or player) as key and the position associated with it as a Vector3.
    /// </summary>
    private Dictionary<GameObject,Vector3> p_occupiedPositions;

    /// <summary>
    /// The GameObject of the player.
    /// </summary>
    private GameObject p_player;

    /// <summary>
    /// The maximum number of lives that the player can have.
    /// </summary>
    [SerializeField]
    private int p_maxLives;

    /// <summary>
    /// The current numbre of lives that the player have.
    /// </summary>
    private int p_lives;

    /// <summary>
    /// The path of the World that contains the generated Level.
    /// </summary>
    private string p_path;

    /// <summary>
    /// The position of the Level in the World.
    /// </summary>
    private Vector2 p_levelPos;

    /// <summary>
    /// The GamaeObject of the game HUD.
    /// </summary>
    [SerializeField]
    private GameObject p_gameHud;

    /// <summary>
    /// The list of GameObjects of the live counters on the HUD.
    /// </summary>
    private List<GameObject> p_fullLives;

    /// <summary>
    /// The list of GameObjects of the empty live counters on the HUD.
    /// </summary>
    private List<GameObject> p_emptyLives;

    /// <summary>
    /// The maximum number of color cells that can be accumulated as garbage.
    /// </summary>
    [SerializeField]
    private int p_maxGarbage;

    /// <summary>
    /// The Sprite used to instantiate the color cells accumulated as garbage.
    /// </summary>
    [SerializeField]
    private Sprite p_garbageSprite;

    /// <summary>
    /// The current color cells (as RGB combinations) accumulated as garbage.
    /// </summary>
    private List<RGBContent> p_garbageCells;

    /// <summary>
    /// The container GameObject where the garbage cells are shown.
    /// </summary>
    private GameObject p_garbagePrint;

    /// <summary>
    /// The color cell (as RGB combination) that has been stored and can be released to color a cell.
    /// </summary>
    private RGBContent p_storedColor;

    /// <summary>
    /// The GameObject used to show the stored color.
    /// </summary>
    private GameObject p_storedPrint;

    /// <summary>
    /// The FXSequence that is launched when the game is paused.
    /// </summary>
    [SerializeField]
    private FXSequence p_pauseFX;

    /// <summary>
    /// The GameObject used as container of all the game elements (cells, barriers, etc).
    /// </summary>
    private GameObject p_parentGameObject;

    /// <summary>
    /// The list of GameObjects that are the goal color of every level cell on the game HUD.
    /// </summary>
    private List<GameObject> p_goals;

    protected LevelController()
    {
    }

    void Start()
    {
        //Sets up the game HUD and the spawn remaining time.
        p_parentGameObject = new GameObject("Game");
        p_remainingTime = Random.Range(p_minSpawnTime, p_maxSpawnTime);
        p_fullLives = new List<GameObject>();
        //Param args ,,,, 1,Full
        p_fullLives.Add(p_gameHud.GetChild("1").GetChild("Full"));
        p_fullLives.Add(p_gameHud.GetChild("2").GetChild("Full"));
        p_fullLives.Add(p_gameHud.GetChild("3").GetChild("Full"));
        p_emptyLives = new List<GameObject>();
        p_emptyLives.Add(p_gameHud.GetChild("1").GetChild("Empty"));
        p_emptyLives.Add(p_gameHud.GetChild("2").GetChild("Empty"));
        p_emptyLives.Add(p_gameHud.GetChild("3").GetChild("Empty"));
        p_storedPrint = p_gameHud.GetChild("Color");
        p_storedPrint.SetActive(false);
    }

    void Update()
    {
        //If there are less enemies than the maximum number of enemies.
        if (p_occupiedPositions != null && p_occupiedPositions.Count - 1 < p_maxEnemies)
        {
            //If the remaining spawn time is 0 or less.
            if (p_remainingTime <= 0)
            {
                //Generates an enemy and sets a new remaining spawn time between minimum and maximum.
                p_remainingTime = Random.Range(p_minSpawnTime, p_maxSpawnTime);
                GenerateEnemy();                
            }
            else
                p_remainingTime -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Adds the LevelCell to the generated level.
    /// </summary>
    /// <param name="i_cell">The cell to add.</param>
    public void AddCell(GameObject i_cell)
    {
        if (p_cells == null)
            p_cells = new List<GameObject>();

        p_cells.Add(i_cell);

        RGBContent auxRGB = i_cell.GetComponent<CellColorGoal>()._RGBGoal;

        if (auxRGB._r)
            p_remainingR++;
        
        if (auxRGB._g)
            p_remainingG++;
        
        if (auxRGB._b)
            p_remainingB++;

        i_cell.transform.SetParent(p_parentGameObject.transform);
    }

    /// <summary>
    /// Completes the cell in the level.
    /// </summary>
    /// <param name="i_cell">I cell.</param>
    public void CellCompleted(GameObject i_cell)
    {
        RGBContent auxRGB = i_cell.GetComponent<CellColorGoal>()._RGBGoal;

        if (auxRGB._r)
            p_remainingR--;

        if (auxRGB._g)
            p_remainingG--;

        if (auxRGB._b)
            p_remainingB--;

        p_garbageCells.RemoveAll(garbage => garbage.Equals(auxRGB));

        PrintGarbage();

        if (p_remainingR == 0 && p_remainingG == 0 && p_remainingB == 0)
            PersistenceManager.Instance.CompleteLevel(p_path, p_levelPos);
    }

    /// <summary>
    /// Sets a new occupied position for the enemy or player.
    /// </summary>
    /// <param name="i_gameObject">The GameObject which position will be changed.</param>
    /// <param name="i_nextPos">The new position for the GameObject.</param>
    public void NextPosition(GameObject i_gameObject, Vector3 i_nextPos)
    {
        if (p_occupiedPositions == null)
            p_occupiedPositions = new Dictionary<GameObject, Vector3>();
        else if (p_occupiedPositions.ContainsKey(i_gameObject))
            p_occupiedPositions.Remove(i_gameObject);

        p_occupiedPositions.Add(i_gameObject, i_nextPos);
    }

    /// <summary>
    /// Generates a new enemy and adds it to the game.
    /// </summary>
    public void GenerateEnemy()
    {
        //Gets the random value to select the type of enemy to create.
        int aux = Random.Range(1, p_remainingR * 2 + 1 + p_remainingG * 2 + 1 + p_remainingB * 2 + 1);

        GameObject enemy = null;

        //Creates the enemy depending on the random value.
        if (aux <= p_remainingR * 2 + 1)
            enemy = (GameObject)Instantiate(p_enemyR);
        else if (aux <= p_remainingR * 2 + 1 + p_remainingG * 2 + 1)
            enemy = (GameObject)Instantiate(p_enemyG);
        else
            enemy = (GameObject)Instantiate(p_enemyB);

        bool validPos;
        Vector3 pos;

        do
        {
            //Gets a random cell from the game cell list.
            pos = p_cells[Random.Range(0, p_cells.Count - 1)].transform.position;

            //Sets the position as valid.
            validPos = true;

            //For each position in the occupied position dictionary.
            foreach (Vector3 auxPos in p_occupiedPositions.Values)
            {
                //If the position is the same (position occupied).
                if (pos == auxPos)
                {
                    //Is not valid.
                    validPos = false;
                    break;
                }

                //If the chosen position is the cell at the right of the position which is comparing to.
                if (pos == auxPos + new Vector3(p_cells[0].GetSize().x, 0, 0))
                {
                    //It is not valid.
                    validPos = false;
                    break;
                }

                //If the chosen position is the cell at the left of the position which is comparing to.
                if (pos == auxPos - new Vector3(p_cells[0].GetSize().x, 0, 0))
                {
                    //It is not valid.
                    validPos = false;
                    break;
                }

                //If the chosen position is the upper cell to the position which is comparing to.
                if (pos == auxPos + new Vector3(0, p_cells[0].GetSize().y, 0))
                {
                    //It is not valid.
                    validPos = false;
                    break;
                }

                //If the chosen position is the lower cell to the position which is comparing to.
                if (pos == auxPos - new Vector3(0, p_cells[0].GetSize().y, 0))
                {
                    //It is not valid.
                    validPos = false;
                    break;
                }
            }
        }
        //Until the position is valid.
        while (!validPos);

        //Sets the enemy position.
        enemy.transform.position = pos;
        NextPosition(enemy, pos);
        enemy.transform.SetParent(p_parentGameObject.transform);
    }

    public void FirstEnemiesGeneration()
    {
        int aux = Random.Range(1, p_maxEnemies);

        for (int i = 0; i < aux; i++)
            GenerateEnemy();
    }

    /// <summary>
    /// Removes a killed enemy from the occupied positions.
    /// </summary>
    /// <param name="i_enemy">The enemy that has been killed.</param>
    public void EnemyKilled(GameObject i_enemy)
    {
        p_occupiedPositions.Remove(i_enemy);
    }

    /// <summary>
    /// Manages what happen when a player is hit.
    /// </summary>
    public void PlayerHit()
    {
        //The player loses a live.
        p_lives--;

        //For each posible live.
        for (int i = 0; i < p_maxLives; i++)
        //If the live is full.
            if (i <= p_lives - 1)
            {
                p_emptyLives[i].SetActive(false);
                p_fullLives[i].SetActive(true);
            }
            //If the live is empty.
            else
            {
                p_emptyLives[i].SetActive(true);
                p_fullLives[i].SetActive(false);
            }

        //If the lives are empty.
        if (p_lives == 0)
            Destroy(p_player);
        else
        {
            bool validPos;
            Vector3 pos;

            do
            {
                //Gets a random cell from the game cell list.
                pos = p_cells[Random.Range(0, p_cells.Count - 1)].transform.position;

                //Sets the position as valid.
                validPos = true;

                //For each position in the occupied position dictionary.
                foreach (Vector3 auxPos in p_occupiedPositions.Values)
                {
                    //If the position is the same (position occupied).
                    if (pos == auxPos)
                    {
                        //It is not valid.
                        validPos = false;
                        break;
                    }

                    //If the chosen position is the cell at the right of the position which is comparing to.
                    if (pos == auxPos + new Vector3(p_cells[0].GetSize().x, 0, 0))
                    {
                        //It is not valid.
                        validPos = false;
                        break;
                    }

                    //If the chosen position is the cell at the left of the position which is comparing to.
                    if (pos == auxPos - new Vector3(p_cells[0].GetSize().x, 0, 0))
                    {
                        //It is not valid.
                        validPos = false;
                        break;
                    }

                    //If the chosen position is the upper cell to the position which is comparing to.
                    if (pos == auxPos + new Vector3(0, p_cells[0].GetSize().y, 0))
                    {
                        //It is not valid.
                        validPos = false;
                        break;
                    }

                    //If the chosen position is the lower cell to the position which is comparing to.
                    if (pos == auxPos - new Vector3(0, p_cells[0].GetSize().y, 0))
                    {
                        //It is not valid.
                        validPos = false;
                        break;
                    }
                }
            }
            //Until the position is valid.
            while (!validPos);

            //Sets the enemy position.
            p_player.transform.position = pos;
            NextPosition(p_player, pos);
            p_player.GetComponent<PlayerController>().ResetPlayer();
        }
    }

    /// <summary>
    /// Adds the player to the game.
    /// </summary>
    /// <param name="i_player">The player to add.</param>
    public void AddPlayer(GameObject i_player)
    {
        p_lives = p_maxLives;
        p_player = i_player;

        foreach (GameObject aux in p_fullLives)
            aux.SetActive(true);

        foreach (GameObject aux in p_emptyLives)
            aux.SetActive(false);

        p_garbageCells = new List<RGBContent>();

        i_player.transform.SetParent(p_parentGameObject.transform);
    }

    /// <summary>
    /// Adds the level info to the game HUD.
    /// </summary>
    /// <param name="i_name">The image name.</param>
    /// <param name="i_path">The World path.</param>
    /// <param name="i_levelPos">The Level position.</param>
    public void AddLevelInfo(string i_name, string i_path, Vector2 i_levelPos)
    {
        p_path = i_path;
        p_levelPos = i_levelPos;

        p_gameHud.GetChild("WorldName").GetComponentInChildren<Text>().text = i_name;

        p_gameHud.GetChild("LevelName").GetComponentInChildren<Text>().text = "(" + (int)i_levelPos.x + "," + (int)i_levelPos.y + ")";
    }

    /// <summary>
    /// Adds color cell to the garbage.
    /// </summary>
    /// <param name="i_garbage">The color cell as a RGB combination.</param>
    public void AddGarbage(RGBContent i_garbage)
    {
        p_garbageCells.Add(i_garbage);

        if (p_garbageCells.Count > p_maxGarbage)
            PlayerHit();

        PrintGarbage();
    }

    /// <summary>
    /// Shows the color cells accumulated as garbage.
    /// </summary>
    private void PrintGarbage()
    {
        if (p_garbagePrint != null)
            Destroy(p_garbagePrint);
        p_garbagePrint = new GameObject("GarbagePrint", typeof(RectTransform));

        p_garbagePrint.transform.SetParent(p_gameHud.GetChild("GarbageCells").GetChild("Margin").transform);

        (p_garbagePrint.transform as RectTransform).offsetMin = Vector2.zero;
        (p_garbagePrint.transform as RectTransform).offsetMax = Vector2.zero;
        (p_garbagePrint.transform as RectTransform).anchorMin = Vector2.zero;
        (p_garbagePrint.transform as RectTransform).anchorMax = Vector2.one;

        GameObject aux;
        Color auxCol;

        for (int i = 0; i < p_garbageCells.Count; i++)
        {
            aux = new GameObject("Garbage" + i, typeof(RectTransform));

            aux.transform.SetParent(p_garbagePrint.transform);
            (aux.transform as RectTransform).offsetMin = Vector2.zero;
            (aux.transform as RectTransform).offsetMax = Vector2.zero;
            (aux.transform as RectTransform).anchorMin = new Vector2(0, i / (float)p_maxGarbage);
            (aux.transform as RectTransform).anchorMax = new Vector2(1, (i + 1) / (float)p_maxGarbage);

            auxCol = Color.black;

            if (p_garbageCells[i]._r)
                auxCol.r = 1;

            if (p_garbageCells[i]._g)
                auxCol.g = 1;

            if (p_garbageCells[i]._b)
                auxCol.b = 1;

            aux.AddComponent<Image>().sprite = p_garbageSprite;
            aux.GetComponent<Image>().color = auxCol;
            aux.GetComponent<Image>().preserveAspect = true;
        }
    }

    /// <summary>
    /// Checks if the position is occupied or not.
    /// </summary>
    /// <returns><c>true</c>, if the position is not occupied by an enemy, <c>false</c> otherwise.</returns>
    /// <param name="i_pos">The position to check.</param>
    public bool IsEmptySpace(Vector3 i_pos)
    {
        foreach (KeyValuePair<GameObject,Vector3> entry in p_occupiedPositions)
        {
            if (entry.Value == i_pos && entry.Key != p_player)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Stores the color cell so that the player can use it to color a cell in the level.
    /// </summary>
    /// <param name="i_color">The color cell to store as RGB combination.</param>
    public void StoreColor(RGBContent i_color)
    {
        p_storedColor = i_color;
        p_player.GetComponent<PlayerController>()._colorStored = true;
        PrintStoredColor();
    }

    /// <summary>
    /// Shows the stored color cell.
    /// </summary>
    private void PrintStoredColor()
    {
        if (p_storedColor == null)
        {
            p_storedPrint.SetActive(false);
        }
        else
        {
            p_storedPrint.SetActive(true);
            if (p_storedColor._r)
                p_storedPrint.GetComponent<Image>().color = Color.red;
            else if (p_storedColor._g)
                p_storedPrint.GetComponent<Image>().color = Color.green;
            else if (p_storedColor._b)
                p_storedPrint.GetComponent<Image>().color = Color.blue;
        }
    }

    /// <summary>
    /// Releases the stored color cell.
    /// </summary>
    public void ReleaseColor()
    {
        foreach (GameObject cell in p_cells)
            if (p_player.transform.position == cell.transform.position)
            {
                cell.GetComponent<CellColorGoal>().AddRGBComponent(p_storedColor);
                break;
            }
        p_storedColor = null;

        p_player.GetComponent<PlayerController>()._colorStored = false;

        PrintStoredColor();
    }

    /// <summary>
    /// Pauses the game.
    /// </summary>
    public void Pause()
    {
        p_pauseFX.Launch();
    }

    /// <summary>
    /// Quits the game.
    /// </summary>
    public void Quit()
    {
        Destroy(p_parentGameObject);

        p_parentGameObject = new GameObject("Game");

        //Reset game
        p_remainingTime = Random.Range(p_minSpawnTime, p_maxSpawnTime);
        p_occupiedPositions = null;
        p_cells = null;
        p_remainingR = 0;
        p_remainingG = 0;
        p_remainingB = 0;
        p_player = null;
        Destroy(p_garbagePrint);
        p_storedColor = null;
        PrintStoredColor();
        for (int i = 0; i < p_goals.Count; i++)
            Destroy(p_goals[i]);
        p_goals = null;
    }

    /// <summary>
    /// Adds the barrier to the game container GameObject.
    /// </summary>
    /// <param name="i_barrier">The barrier to add.</param>
    public void AddBarrier(GameObject i_barrier)
    {
        i_barrier.transform.SetParent(p_parentGameObject.transform);
    }

    /// <summary>
    /// Adds the cell background to the game container GameObject.
    /// </summary>
    /// <param name="i_cell">The cell background to add.</param>
    public void AddCellBackground(GameObject i_cell)
    {
        i_cell.transform.SetParent(p_parentGameObject.transform);
    }

    /// <summary>
    /// Adds the goal color cell (in game HUD) to the list of goal color cells.
    /// </summary>
    /// <param name="i_goal">The goal color cell to add.</param>
    public void AddGoal(GameObject i_goal)
    {
        if (p_goals == null)
            p_goals = new List<GameObject>();
        p_goals.Add(i_goal);
    }
}
