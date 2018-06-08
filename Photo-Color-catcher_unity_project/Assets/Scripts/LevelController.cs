﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicDataTypes;
using UnityEngine.UI;

public class LevelController : Singleton<LevelController>
{
    [SerializeField]
    private int p_maxEnemies;

    [SerializeField]
    private float p_minSpawnTime;

    [SerializeField]
    private float p_maxSpawnTime;

    private float p_remainingTime;

    [SerializeField]
    private GameObject p_enemyR;

    [SerializeField]
    private GameObject p_enemyG;

    [SerializeField]
    private GameObject p_enemyB;

    private List<GameObject> p_cells;

    private int p_remainingR;

    private int p_remainingG;

    private int p_remainingB;

    private Dictionary<GameObject,Vector3> p_occupiedPositions;

    private GameObject p_player;

    [SerializeField]
    private int p_maxLives;

    private int p_lives;

    private string p_path;

    private Vector2 p_levelPos;

    [SerializeField]
    private GameObject p_gameHud;

    private List<GameObject> p_fullLives;

    private List<GameObject> p_emptyLives;

    [SerializeField]
    private int p_maxGarbage;

    [SerializeField]
    private Sprite p_garbageSprite;

    private List<RGBContent> p_garbageCells;

    private GameObject p_garbagePrint;

    private RGBContent p_storedColor;

    private GameObject p_storedPrint;

    [SerializeField]
    private GameObject p_pauseFX;

    private GameObject p_parentGameObject;

    private List<GameObject> p_goals;

    protected LevelController()
    {
    }

    void Start()
    {
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
        if (p_occupiedPositions != null && p_occupiedPositions.Count - 1 < p_maxEnemies)
        {
            if (p_remainingTime <= 0)
            {
                p_remainingTime = Random.Range(p_minSpawnTime, p_maxSpawnTime);
                GenerateEnemy();                
            }
            else
                p_remainingTime -= Time.deltaTime;
        }
    }

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
            PersistenceManager.CompleteLevel(p_path, p_levelPos);
    }

    public void NextPosition(GameObject i_gameObject, Vector3 i_nextPos)
    {
        if (p_occupiedPositions == null)
            p_occupiedPositions = new Dictionary<GameObject, Vector3>();
        else if (p_occupiedPositions.ContainsKey(i_gameObject))
            p_occupiedPositions.Remove(i_gameObject);

        p_occupiedPositions.Add(i_gameObject, i_nextPos);
    }

    public void GenerateEnemy()
    {
        int aux = Random.Range(1, p_remainingR*2 + 1 + p_remainingG*2 + 1 + p_remainingB*2 + 1);

        GameObject enemy = null;

        if (aux <= p_remainingR*2 + 1)
            enemy = (GameObject)Instantiate(p_enemyR);
        else if (aux <= p_remainingR*2 + 1 + p_remainingG*2 + 1)
            enemy = (GameObject)Instantiate(p_enemyG);
        else
            enemy = (GameObject)Instantiate(p_enemyB);

        bool validPos;
        Vector3 pos;

        do
        {
            pos = p_cells[Random.Range(0, p_cells.Count - 1)].transform.position;

            validPos = true;

            foreach (Vector3 auxPos in p_occupiedPositions.Values)
            {
                if (pos == auxPos)
                {
                    validPos = false;
                    break;
                }

                if (pos == auxPos + new Vector3(p_cells[0].GetSize().x, 0, 0))
                {
                    validPos = false;
                    break;
                }

                if (pos == auxPos - new Vector3(p_cells[0].GetSize().x, 0, 0))
                {
                    validPos = false;
                    break;
                }

                if (pos == auxPos + new Vector3(0, p_cells[0].GetSize().y, 0))
                {
                    validPos = false;
                    break;
                }

                if (pos == auxPos - new Vector3(0, p_cells[0].GetSize().y, 0))
                {
                    validPos = false;
                    break;
                }
            }
        }
        while (!validPos);

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

    public void EnemyKilled(GameObject i_enemy)
    {
        p_occupiedPositions.Remove(i_enemy);
    }

    public void PlayerHit()
    {
        p_lives--;

        for (int i = 0; i < p_maxLives; i++)
            if (i <= p_lives-1)
            {
                p_emptyLives[i].SetActive(false);
                p_fullLives[i].SetActive(true);
            }
            else
            {
                p_emptyLives[i].SetActive(true);
                p_fullLives[i].SetActive(false);
            }

        if (p_lives == 0)
            Destroy(p_player);
        else
        {
            bool validPos;
            Vector3 pos;

            do
            {
                pos = p_cells[Random.Range(0, p_cells.Count - 1)].transform.position;

                validPos = true;

                foreach (Vector3 auxPos in p_occupiedPositions.Values)
                {
                    if (pos == auxPos)
                    {
                        validPos = false;
                        break;
                    }

                    if (pos == auxPos + new Vector3(p_cells[0].GetSize().x, 0, 0))
                    {
                        validPos = false;
                        break;
                    }

                    if (pos == auxPos - new Vector3(p_cells[0].GetSize().x, 0, 0))
                    {
                        validPos = false;
                        break;
                    }

                    if (pos == auxPos + new Vector3(0, p_cells[0].GetSize().y, 0))
                    {
                        validPos = false;
                        break;
                    }

                    if (pos == auxPos - new Vector3(0, p_cells[0].GetSize().y, 0))
                    {
                        validPos = false;
                        break;
                    }
                }
            }
            while (!validPos);

            p_player.transform.position = pos;

            NextPosition(p_player, pos);

            p_player.GetComponent<PlayerController>().ResetPlayer();
        }
    }

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

    public void AddLevelInfo(string i_name, string i_path, Vector2 i_levelPos)
    {
        p_path = i_path;
        p_levelPos = i_levelPos;

        p_gameHud.GetChild("WorldName").GetComponentInChildren<Text>().text = i_name;

        p_gameHud.GetChild("LevelName").GetComponentInChildren<Text>().text = "("+(int)i_levelPos.x+","+(int)i_levelPos.y+")";
    }

    public void AddGarbage(RGBContent i_garbage)
    {
        p_garbageCells.Add(i_garbage);

        if (p_garbageCells.Count > p_maxGarbage)
            PlayerHit();

        PrintGarbage();
    }

    private void PrintGarbage()
    {
        if (p_garbagePrint != null)
            Destroy(p_garbagePrint);
        p_garbagePrint = new GameObject("GarbagePrint",typeof(RectTransform));

        p_garbagePrint.transform.SetParent(p_gameHud.GetChild("GarbageCells").GetChild("Margin").transform);

        (p_garbagePrint.transform as RectTransform).offsetMin = Vector2.zero;
        (p_garbagePrint.transform as RectTransform).offsetMax = Vector2.zero;
        (p_garbagePrint.transform as RectTransform).anchorMin = Vector2.zero;
        (p_garbagePrint.transform as RectTransform).anchorMax = Vector2.one;

        GameObject aux;
        Color auxCol;

        for (int i = 0; i < p_garbageCells.Count; i++)
        {
            aux = new GameObject("Garbage"+i,typeof(RectTransform));

            aux.transform.SetParent(p_garbagePrint.transform);
            (aux.transform as RectTransform).offsetMin = Vector2.zero;
            (aux.transform as RectTransform).offsetMax = Vector2.zero;
            (aux.transform as RectTransform).anchorMin = new Vector2(0,i/(float)p_maxGarbage);
            (aux.transform as RectTransform).anchorMax = new Vector2(1,(i+1)/(float)p_maxGarbage);

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

    public bool EmptySpace(Vector3 i_pos)
    {
        foreach (KeyValuePair<GameObject,Vector3> entry in p_occupiedPositions)
        {
            if (entry.Value == i_pos && entry.Key != p_player)
                return false;
        }
        return true;
    }

    public void StoreColor(RGBContent i_color)
    {
        p_storedColor = i_color;
        p_player.GetComponent<PlayerController>()._colorStored = true;
        PrintStoredColor();
    }

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

    public void Pause()
    {
        p_pauseFX.SendMessage("Launch", SendMessageOptions.DontRequireReceiver);
    }

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

    public void AddBarrier(GameObject i_barrier)
    {
        i_barrier.transform.SetParent(p_parentGameObject.transform);
    }

    public void AddCellBackground(GameObject i_cell)
    {
        i_cell.transform.SetParent(p_parentGameObject.transform);
    }

    public void AddGoal(GameObject i_goal)
    {
        if (p_goals == null)
            p_goals = new List<GameObject>();
        p_goals.Add(i_goal);
    }
}
