﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ImageConfigurationManager : Singleton<ImageConfigurationManager>
{

    public float deformationMargin;

    [SerializeField]
    private GameObject imageConfigurationPageModel;

    [SerializeField]
    private GameObject pageCanvas;

    protected ImageConfigurationManager()
    {
    }

    public void GetImageConfigurations(string i_path)
    {
        byte[] loadingBytes = File.ReadAllBytes(i_path);

        Texture2D auxText = new Texture2D(1, 1);

        auxText.LoadImage(loadingBytes);

        List<int[]> imageConfigs = new List<int[]>();

        int cellSize;

        int aux;

        List<GameObject> pageList = new List<GameObject>();

        List<GameObject> inputMenuControllerList = new List<GameObject>();

        GameObject auxGameObject;

        string[] pathSplit;

        GUIObjectEnable objectEnable;

        Sprite spr = null;

        Vector2 anchorMax;

        Vector2 anchorMin;

        for (int i = 4; i > 0; i--)
        {
            if (auxText.width > auxText.height)
            {
                cellSize = auxText.width / i;
                aux = Mathf.RoundToInt(auxText.height / (float)cellSize);
                if (aux > 0 && aux <= 4 && Mathf.Abs(1 - auxText.height / (float)(cellSize * aux)) <= deformationMargin)
                    imageConfigs.Add(new int[]{i,aux});
            }
            else
            {
                cellSize = auxText.height / i;
                aux = Mathf.RoundToInt(auxText.width / (float)cellSize);
                if (aux > 0 && aux <= 4 && Mathf.Abs(1 - auxText.width / (float)(cellSize * aux)) <= deformationMargin)
                    imageConfigs.Add(new int[]{i,aux});
            }
        }

        if (imageConfigs.Count == 0)
        {
            pageList.Add((GameObject)Instantiate(imageConfigurationPageModel));
            pageList[pageList.Count-1].transform.SetParent(pageCanvas.transform, false);
            Destroy(pageList[pageList.Count-1].GetChild("Prev page"));
            Destroy(pageList[pageList.Count-1].GetChild("Next page"));
            Destroy(pageList[pageList.Count-1].GetChild("Accept button"));
            Destroy(pageList[pageList.Count-1].GetChild("Cells config"));
            pathSplit = i_path.Split(Path.DirectorySeparatorChar);
            aux = Path.GetExtension(pathSplit[pathSplit.Length - 1]).Length;
            pathSplit[pathSplit.Length - 1].Remove(pathSplit[pathSplit.Length - 1].Length - aux, aux);
            pageList[pageList.Count-1].GetChild("Image name").GetComponent<Text>().text = pathSplit[pathSplit.Length - 1];
            inputMenuControllerList.Add(new GameObject("InputMenuController"));
            auxGameObject = pageList[pageList.Count - 1].GetChild("Back button");
            inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().startingElem = auxGameObject;
            inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements = new List<GameObject>();
            inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);
        }
        else
        {
            pageList.Add((GameObject)Instantiate(imageConfigurationPageModel));
            pageList[pageList.Count-1].transform.SetParent(pageCanvas.transform, false);
            Destroy(pageList[pageList.Count-1].GetChild("Prev page"));
            inputMenuControllerList.Add(new GameObject("InputMenuController"));
            inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();
            for (int i = 0; i < imageConfigs.Count; i++)
            {
                Destroy(pageList[pageList.Count-1].GetChild("Error text"));
                pathSplit = i_path.Split(Path.DirectorySeparatorChar);
                aux = Path.GetExtension(pathSplit[pathSplit.Length - 1]).Length;
                pathSplit[pathSplit.Length - 1].Remove(pathSplit[pathSplit.Length - 1].Length - aux, aux);
                pageList[pageList.Count-1].GetChild("Image name").GetComponent<Text>().text = pathSplit[pathSplit.Length - 1];
                pageList[pageList.Count-1].GetChild("Cells config").GetComponent<Text>().text = imageConfigs[i][0]+"x"+imageConfigs[i][1];

                //SPRITE DE FONDO
                if (spr==null)
                    spr = Sprite.Create(auxText, new Rect(0, 0, auxText.width, auxText.height), new Vector2(0, 0));
                auxGameObject = pageList[pageList.Count - 1].GetChild("Background image");
                auxGameObject.GetComponent<Image>().sprite = spr;
                if (imageConfigs[i][0] > imageConfigs[i][1])
                {
                    (auxGameObject.transform as RectTransform).anchorMax = new Vector2(0.5f + imageConfigs[i][0] / (float)(imageConfigs[i][0] + 2)/2f, 0.5f + imageConfigs[i][1] / (float)(imageConfigs[i][0] + 2)/2f);
                    (auxGameObject.transform as RectTransform).anchorMin = new Vector2(0.5f - imageConfigs[i][0] / (float)(imageConfigs[i][0] + 2)/2f, 0.5f - imageConfigs[i][1] / (float)(imageConfigs[i][0] + 2)/2f);
                }
                else
                {
                    (auxGameObject.transform as RectTransform).anchorMax = new Vector2(0.5f + imageConfigs[i][0] / (float)(imageConfigs[i][1] + 2)/2f, 0.5f + imageConfigs[i][1] / (float)(imageConfigs[i][1] + 2)/2f);
                    (auxGameObject.transform as RectTransform).anchorMin = new Vector2(0.5f - imageConfigs[i][0] / (float)(imageConfigs[i][1] + 2)/2f, 0.5f - imageConfigs[i][1] / (float)(imageConfigs[i][1] + 2)/2f);
                }

                //for (int j)

                auxGameObject = pageList[pageList.Count - 1].GetChild("Back button");
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().startingElem = auxGameObject;
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);
                auxGameObject = pageList[pageList.Count - 1].GetChild("Accept button");
                //ADD FUNC
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);
                if (i == imageConfigs.Count - 1)
                {
                    Destroy(pageList[pageList.Count - 1].GetChild("Next page"));
                }
                else
                {
                    pageList.Add((GameObject)Instantiate(imageConfigurationPageModel));
                    pageList[pageList.Count-1].transform.SetParent(pageCanvas.transform, false);
                    inputMenuControllerList.Add(new GameObject("InputMenuController"));
                    inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();

                    //Deactivates the page and the input menu controller.
                    pageList[pageList.Count - 1].SetActive(false);
                    inputMenuControllerList[pageList.Count - 1].SetActive(false);

                    //Gets the "next page" button of this page.
                    auxGameObject = pageList[pageList.Count - 2].GetChild("Next page");

                    //Sets up the "next page" button (activate the next page and the next input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = pageList[pageList.Count - 1];

                    //Sets up the "next page" button (deactivate this page and this input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = pageList[pageList.Count - 2];

                    //Adds the "next page" button to the input menu controller.
                    inputMenuControllerList[inputMenuControllerList.Count - 2].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);

                    //Maps the elements of the input menu controller.
                    //inputMenuControllerList[inputMenuControllerList.Count - 2].GetComponent<InputMenuController>().MapElements();

                    //Gets the "previous page" button of the next page.
                    auxGameObject = pageList[pageList.Count - 1].GetChild("Prev page");

                    //Sets up the "previous page" button (activate this page and this input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = pageList[pageList.Count - 2];

                    //Sets up the "previous page" button (deactivate the next page and the next input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = pageList[pageList.Count - 1];

                    //Adds the "previous page" button to the next input menu controller.
                    inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);
                }
            }
        }

    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
