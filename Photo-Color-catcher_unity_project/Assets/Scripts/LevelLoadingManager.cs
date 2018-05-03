﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicDataTypes;
using UnityEngine.UI;

public class LevelLoadingManager: Singleton<LevelLoadingManager>
{
    [SerializeField]
    private GameObject p_floorTile;

    [SerializeField]
    private GameObject p_barrier;

    [SerializeField]
    private GameObject p_mapGoal;

    [SerializeField]
    private GameObject p_player;

    protected LevelLoadingManager()
    {
    }

    public void LoadLevel(string i_path, Vector2 i_levelPos)
    {
        Level level = PersistenceManager.LoadWorld(i_path)._levels[i_levelPos];

        int max = (int)Mathf.Pow(level._cells.Count,0.5f);

        GameObject auxCell;

        GameObject auxBarrier;

        GameObject auxGoalCell;

        Color goalColor;

        for (int x = 0; x < max; x++)
        {
            for (int y = 0; y < max; y++)
            {

                auxCell = (GameObject)Instantiate(p_floorTile);

                auxCell.transform.position = new Vector3((x - (max - 1) / 2f) * p_floorTile.GetSize().x, (y - (max - 1) / 2f) * p_floorTile.GetSize().y, 0);

                auxCell.GetComponent<CellColorGoal>()._RGBGoal = level._cells[new Vector2(x, y)]._rgbComponents;

                if (x == 0 && y == 0)
                    ((GameObject)Instantiate(p_player)).transform.position = auxCell.transform.position;

                if (x < max - 1 && !level._graph.IsAdjacent(new Vector2(x,y),new Vector2(x+1,y)))
                {
                    auxBarrier = (GameObject)Instantiate(p_barrier);

                    auxBarrier.transform.position = auxCell.transform.position + new Vector3(p_floorTile.GetSize().x/2f,0,0);
                }

                if (y < max - 1 && !level._graph.IsAdjacent(new Vector2(x,y),new Vector2(x,y+1)))
                {
                    auxBarrier = (GameObject)Instantiate(p_barrier);

                    auxBarrier.transform.position = auxCell.transform.position + new Vector3(0,p_floorTile.GetSize().y/2f,0);

                    auxBarrier.transform.Rotate(new Vector3(0,0,90));
                }

                //Creates and sets up the cell.
                auxGoalCell = new GameObject("Goal(" + x + "," + y + ")", typeof(RectTransform));
                auxGoalCell.transform.SetParent(p_mapGoal.transform);
                (auxGoalCell.transform as RectTransform).offsetMin = Vector2.zero;
                (auxGoalCell.transform as RectTransform).offsetMax = Vector2.zero;

                //Set ups the cell size.
                (auxGoalCell.transform as RectTransform).anchorMin = new Vector2((x+1+x*10)/(float)(max+1+max*10),(y+1+y*10)/(float)(max+1+max*10));
                (auxGoalCell.transform as RectTransform).anchorMax = new Vector2((x+1+(x+1)*10)/(float)(max+1+max*10),(y+1+(y+1)*10)/(float)(max+1+max*10));

                //Shows the goal image.

                goalColor = Color.black;

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
            }
        }

        float ySize = p_floorTile.GetSize().y * max;
        float xSize = p_floorTile.GetSize().x * max;

        float camXsize;
        float camYsize;

        camYsize = ySize;
        camXsize = ySize * (Screen.width /(float) Screen.height);

        if (camXsize > xSize)
            Camera.main.orthographicSize = ySize / 2f;
        else
        {
            camXsize = xSize;
            camYsize = xSize * (Screen.height /(float) Screen.width);
            Camera.main.orthographicSize = camYsize / 2f;
        }
    }
}