using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using BasicDataTypes;
using UnityEngine.UI;

public class WorldLoadingManager : Singleton<WorldLoadingManager> 
{
    [SerializeField]
    private GameObject p_worldSelectionModel;

    [SerializeField]
    private GameObject p_parentCanvas;

    protected WorldLoadingManager()
    {
    }

    public void LoadWorlds()
    {
        List<World> auxWorlds = new List<World>();
        string[] worldPaths = GetWorlds();
        foreach (string worldPath in worldPaths)
        {
            auxWorlds.Add(PersistenceManager.LoadWorld(worldPath));
        }

        List<GameObject> pageList = new List<GameObject>();
        List<GameObject> inputMenuControllerList = new List<GameObject>();
        GameObject auxGameObject;
        Sprite spr = null;
        GUIObjectEnable objectEnable;

        //If there aren't any playable world.
        if (auxWorlds.Count == 0)
        {
            //Creates the first page of the world selector.
            pageList.Add((GameObject)Instantiate(p_worldSelectionModel));
            pageList[pageList.Count - 1].transform.SetParent(p_parentCanvas.transform, false);

            //Destroys all the useless elements of the page.
            Destroy(pageList[pageList.Count - 1].GetChild("Prev page"));
            Destroy(pageList[pageList.Count - 1].GetChild("Next page"));
            Destroy(pageList[pageList.Count - 1].GetChild("Accept button"));
            Destroy(pageList[pageList.Count - 1].GetChild("Image name"));
            Destroy(pageList[pageList.Count - 1].GetChild("Background image"));

            //Creates the first InputMenuController.
            inputMenuControllerList.Add(new GameObject("InputMenuController"));

            //Gets the "Back" button of the page.
            auxGameObject = pageList[pageList.Count - 1].GetChild("Back button");

            //Sets up the "Back" button.
            //auxGameObject.GetComponent<GUIChangeDirectory>().path = Directory.GetParent(i_path).FullName;

            //Adds the button to the InputMenuController.
            inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().startingElem = auxGameObject;
            inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements = new List<GameObject>();
            inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);
        }

        //If there are some worlds to play.
        else
        {
            //Creates the first page of the configuration selector.
            pageList.Add((GameObject)Instantiate(p_worldSelectionModel));
            pageList[pageList.Count - 1].transform.SetParent(p_parentCanvas.transform, false);

            //Destroys the "Previous page" button.
            Destroy(pageList[pageList.Count - 1].GetChild("Prev page"));

            //Creates the first InputMenuController.
            inputMenuControllerList.Add(new GameObject("InputMenuController"));
            inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();

            //For each image configuration.
            for (int i = 0; i < auxWorlds.Count; i++)
            {
                //Destroys the error text label.
                Destroy(pageList[pageList.Count - 1].GetChild("Error text"));

                //Shows the image name.
                pageList[pageList.Count - 1].GetChild("Image name").GetComponent<Text>().text = auxWorlds[i]._name + " - "+auxWorlds[i]._imageConfig[0]+"x"+auxWorlds[i]._imageConfig[1];

                //Creates the sprite using the world image.
                spr = Sprite.Create(auxWorlds[i]._img.ToTexture2D(), new Rect(0, 0, auxWorlds[i]._img._width, auxWorlds[i]._img._height), new Vector2(0, 0));

                //Shows the image.
                auxGameObject = pageList[pageList.Count - 1].GetChild("Background image");
                auxGameObject.GetComponent<Image>().sprite = spr;

                /*
                //If horizontal image.
                if (auxWorlds[i]._imageConfig[0] > auxWorlds[i]._imageConfig[1])
                {
                    //Resizes the image.
                    (auxGameObject.transform as RectTransform).anchorMax = new Vector2(0.5f + auxWorlds[i]._imageConfig[0] / (float)(auxWorlds[i]._imageConfig[0] + 2) / 2f, 0.5f + auxWorlds[i]._imageConfig[1] / (float)(auxWorlds[i]._imageConfig[0] + 2) / 2f);
                    (auxGameObject.transform as RectTransform).anchorMin = new Vector2(0.5f - auxWorlds[i]._imageConfig[0] / (float)(auxWorlds[i]._imageConfig[0] + 2) / 2f, 0.5f - auxWorlds[i]._imageConfig[1] / (float)(auxWorlds[i]._imageConfig[0] + 2) / 2f);
                }

                //If vertical image.
                else
                {
                    //Resizes the image.
                    (auxGameObject.transform as RectTransform).anchorMax = new Vector2(0.5f + auxWorlds[i]._imageConfig[0] / (float)(auxWorlds[i]._imageConfig[1] + 2) / 2f, 0.5f + auxWorlds[i]._imageConfig[1] / (float)(auxWorlds[i]._imageConfig[1] + 2) / 2f);
                    (auxGameObject.transform as RectTransform).anchorMin = new Vector2(0.5f - auxWorlds[i]._imageConfig[0] / (float)(auxWorlds[i]._imageConfig[1] + 2) / 2f, 0.5f - auxWorlds[i]._imageConfig[1] / (float)(auxWorlds[i]._imageConfig[1] + 2) / 2f);
                }
                */

                //Gets the "Back" button of the page.
                auxGameObject = pageList[pageList.Count - 1].GetChild("Back button");

                //Sets up the "Back" button.
                //auxGameObject.GetComponent<GUIChangeDirectory>().path = Directory.GetParent(i_path).FullName;

                //Adds the button to the InputMenuController.
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().startingElem = auxGameObject;
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);

                //Gets the "Accept" button of the page.
                auxGameObject = pageList[pageList.Count - 1].GetChild("Accept button");

                //Sets up the "Accept" button.
                //auxGameObject.GetComponent<GUIGenerateWorld>()._img = auxText;
                //auxGameObject.GetComponent<GUIGenerateWorld>()._imageConfig = imageConfigs[i];
                //auxGameObject.GetComponent<GUIGenerateWorld>()._name = pageList[pageList.Count - 1].GetChild("Image name").GetComponent<Text>().text;

                //Adds the button to the InputMenuController.
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);

                //If it is the last world.
                if (i == auxWorlds.Count - 1)
                {
                    //Destroys the "Next page" button.
                    Destroy(pageList[pageList.Count - 1].GetChild("Next page"));
                }

                //Otherwise.
                else
                {
                    //Creates the next page of the configuration selector.
                    pageList.Add((GameObject)Instantiate(p_worldSelectionModel));
                    pageList[pageList.Count - 1].transform.SetParent(p_parentCanvas.transform, false);

                    //Creates the next InputMenuController.
                    inputMenuControllerList.Add(new GameObject("InputMenuController"));
                    inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();

                    //Deactivates the page and the InputMenuController.
                    pageList[pageList.Count - 1].SetActive(false);
                    inputMenuControllerList[pageList.Count - 1].SetActive(false);

                    //Gets the "Next page" button of this page.
                    auxGameObject = pageList[pageList.Count - 2].GetChild("Next page");

                    //Sets up the "Next page" button (activate the next page and the next input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = pageList[pageList.Count - 1];

                    //Sets up the "Next page" button (deactivate this page and this input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = pageList[pageList.Count - 2];

                    //Adds the "Next page" button to the input menu controller.
                    inputMenuControllerList[inputMenuControllerList.Count - 2].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);

                    //Gets the "Previous page" button of the next page.
                    auxGameObject = pageList[pageList.Count - 1].GetChild("Prev page");

                    //Sets up the "Previous page" button (activate this page and this input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 2];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = true;
                    objectEnable.target = pageList[pageList.Count - 2];

                    //Sets up the "Previous page" button (deactivate the next page and the next input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = inputMenuControllerList[inputMenuControllerList.Count - 1];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable.enable = false;
                    objectEnable.target = pageList[pageList.Count - 1];

                    //Adds the "Previous page" button to the next input menu controller.
                    inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);
                }
            }
        }

        //For each page.
        foreach (GameObject page in pageList)
        {
            //Gets the "Back" button.
            auxGameObject = page.GetChild("Back button");

            //For each page.
            foreach (GameObject page2 in pageList)
            {
                //Adds an GUIDestroy component to the image button.
                auxGameObject.AddComponent<GUIDestroy>().target = page2;
            }

            //For each InputMenuController.
            foreach (GameObject inputMenuController in inputMenuControllerList)
            {
                //Adds an GUIDestroy component to the image button.
                auxGameObject.AddComponent<GUIDestroy>().target = inputMenuController;
            }

            //Gets the "Accept" button.
            auxGameObject = page.GetChild("Accept button");

            //For each page.
            foreach (GameObject page2 in pageList)
            {
                //Adds an GUIDestroy component to the image button.
                auxGameObject.AddComponent<GUIDestroy>().target = page2;
            }

            //For each InputMenuController.
            foreach (GameObject inputMenuController in inputMenuControllerList)
            {
                //Adds an GUIDestroy component to the image button.
                auxGameObject.AddComponent<GUIDestroy>().target = inputMenuController;
            }
        }
    }

    public string[] GetWorlds()
    {
        if (!Directory.Exists(Application.persistentDataPath))
            return null;

        string[] worlds = Directory.GetFiles(Application.persistentDataPath).Where(x => Path.GetExtension(x).Equals(".pccw")).ToArray();
        return worlds;
    }
}
