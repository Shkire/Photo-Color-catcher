using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCreator : MonoBehaviour
{

    public GameObject cell;

    public GameObject barrier;

    public int rows;

    public int columns;

    // Use this for initialization
    void Start()
    {
		
        GameObject auxCell;

        Vector3 pos;

        GameObject firstGo = null;

        GameObject lastGo = null;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {

                auxCell = (GameObject)Instantiate(cell);

                pos = new Vector3((x - (columns - 1) / 2f) * cell.GetSize().x, (y - (rows - 1) / 2f) * cell.GetSize().y, 0);

                auxCell.transform.position = pos;


                if (x == 0 && y == 0)
                    firstGo = auxCell;

                if (x == columns - 1 && y == rows - 1)
                    lastGo = auxCell;
            }
        }
        /*
        bool fit = false;
        bool inside = false;
        bool outside = false;
        Vector3 firstScreenPos;
        Vector3 lastScreenPos;

        do
        {
            firstScreenPos = new Vector3(firstGo.transform.position.x - firstGo.GetSize().x/2f,firstGo.transform.position.y - firstGo.GetSize().y/2f,firstGo.transform.position.z);
            lastScreenPos = new Vector3(lastGo.transform.position.x + lastGo.GetSize().x/2f,firstGo.transform.position.y + lastGo.GetSize().y/2f,lastGo.transform.position.z);
            firstScreenPos = Camera.main.WorldToScreenPoint(firstScreenPos);
            lastScreenPos = Camera.main.WorldToScreenPoint(lastScreenPos);
            if (firstScreenPos.x < 0 || firstScreenPos.y<0 || lastScreenPos.x>Screen.width || lastScreenPos.y >Screen.width)
            {
                if (inside)
                {
                    fit = true;
                }
                else
                    outside = true;
                Camera.main.orthographicSize += 0.1f;
            }
            else
            {
                if (outside)
                {
                    fit = true;
                }
                else
                {
                    inside = true;
                    Camera.main.orthographicSize -= 0.1f;
                }
            }
        }
        while (!fit);
        */

        float ySize = cell.GetSize().y * rows;
        float xSize = cell.GetSize().x * columns;

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
	
    // Update is called once per frame
    void Update()
    {
		
    }
}
