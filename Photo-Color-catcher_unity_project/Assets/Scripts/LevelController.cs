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

    protected LevelController()
    {
    }

    void Start()
    {
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
        int aux = Random.Range(1, p_remainingR + 1 + p_remainingG + 1 + p_remainingB + 1);

        GameObject enemy = null;

        if (aux <= p_remainingR + 1)
            enemy = (GameObject)Instantiate(p_enemyR);
        else if (aux <= p_remainingR + 1 + p_remainingG + 1)
            enemy = (GameObject)Instantiate(p_enemyG);
        else if (aux <= p_remainingR + 1 + p_remainingG + 1 + p_remainingB + 1)
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
    }

    public void FirstEnemiesGeneration()
    {
        int aux = Random.Range(0, p_maxEnemies);

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
    }

    public void AddLevelInfo(string i_name, string i_path, Vector2 i_levelPos)
    {
        p_path = i_path;
        p_levelPos = i_levelPos;

        p_gameHud.GetChild("WorldName").GetComponentInChildren<Text>().text = i_name;

        p_gameHud.GetChild("LevelName").GetComponentInChildren<Text>().text = "("+(int)i_levelPos.x+","+(int)i_levelPos.y+")";
    }
}
