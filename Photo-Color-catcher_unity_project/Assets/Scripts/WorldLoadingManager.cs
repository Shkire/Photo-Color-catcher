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

    [SerializeField]
    private GameObject p_backButtonFX;

    [SerializeField]
    private GameObject p_acceptButtonFX;

    [SerializeField]
    private GameObject p_progress;

    [SerializeField]
    private GameObject p_loadingWorldsText;

    [SerializeField]
    private GameObject p_startFX;

    [SerializeField]
    private GameObject p_finishFX;


    /// <summary>
    /// The file system explorer page list.
    /// </summary>
    private List<GameObject> pageList;

    /// <summary>
    /// The input menu controller list.
    /// </summary>
    private List<GameObject> inputMenuControllerList;

    private GameObject activeMenuController;

    protected WorldLoadingManager()
    {
    }

    public void StartLoadWorlds()
    {
        StartCoroutine(LoadWorlds());
    }

    public IEnumerator LoadWorlds()
    {
        p_startFX.SendMessage("Launch", SendMessageOptions.DontRequireReceiver);

        p_loadingWorldsText.GetComponent<Text>().text = "Loading created worlds...";

        (p_progress.transform as RectTransform).anchorMin = (new Vector2(0,0));

        List<World> auxWorlds = new List<World>();
        World auxWorld;
        string[] worldPaths = GetWorlds();

        float progress = 0;

        foreach (string worldPath in worldPaths)
        {
            auxWorld = new World();

            yield return StartCoroutine(PersistenceManager.Instance.LoadWorld(worldPath,x=> auxWorld=x));

            auxWorlds.Add(auxWorld);

            progress += 1/(float)worldPaths.Length;

            (p_progress.transform as RectTransform).anchorMin = (new Vector2(progress,0));

            yield return null;
        }

        //Destroys the previous page list.
        if (pageList != null)
            for (int i = 0; i < pageList.Count; i++)
                Destroy(pageList[i]);

        pageList = new List<GameObject>();

        //Destroys the previous input menu controller list.
        if (inputMenuControllerList != null)
            for (int i = 0; i < inputMenuControllerList.Count; i++)
                Destroy(inputMenuControllerList[i]);
        inputMenuControllerList = new List<GameObject>();
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

            inputMenuControllerList[inputMenuControllerList.Count - 1].transform.SetParent(p_parentCanvas.transform);

            //Gets the "Back" button of the page.
            auxGameObject = pageList[pageList.Count - 1].GetChild("Back button");

            auxGameObject.GetComponent<GUILaunchFX>()._target = p_backButtonFX;

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

            inputMenuControllerList[inputMenuControllerList.Count - 1].transform.SetParent(p_parentCanvas.transform);

            inputMenuControllerList[inputMenuControllerList.Count - 1].AddComponent<InputMenuController>().uiElements = new List<GameObject>();

            //For each world.
            for (int i = 0; i < auxWorlds.Count; i++)
            {
                //Destroys the error text label.
                Destroy(pageList[pageList.Count - 1].GetChild("Error text"));

                //Shows the image name.
                pageList[pageList.Count - 1].GetChild("Image name").GetComponent<Text>().text = auxWorlds[i]._name + " - " + auxWorlds[i]._imageConfig[0] + "x" + auxWorlds[i]._imageConfig[1];

                //Creates the sprite using the world image.
                spr = Sprite.Create(auxWorlds[i]._img, new Rect(0, 0, auxWorlds[i]._img.width, auxWorlds[i]._img.height), new Vector2(0, 0));

                //Shows the image.
                auxGameObject = pageList[pageList.Count - 1].GetChild("Background image");
                auxGameObject.GetComponent<Image>().sprite = spr;

                //Gets the "Back" button of the page.
                auxGameObject = pageList[pageList.Count - 1].GetChild("Back button");

                auxGameObject.GetComponent<GUILaunchFX>()._target = p_backButtonFX;

                //Sets up the "Back" button.
                //auxGameObject.GetComponent<GUIChangeDirectory>().path = Directory.GetParent(i_path).FullName;

                //Adds the button to the InputMenuController.
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().startingElem = auxGameObject;
                inputMenuControllerList[inputMenuControllerList.Count - 1].GetComponent<InputMenuController>().uiElements.Add(auxGameObject);

                //Gets the "Accept" button of the page.
                auxGameObject = pageList[pageList.Count - 1].GetChild("Accept button");

                auxGameObject.GetComponent<GUILaunchFX>()._target = p_acceptButtonFX;

                //Sets up the "Accept" button.
                auxGameObject.GetComponent<GUILoadLevels>()._path = worldPaths[i];

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

                    inputMenuControllerList[inputMenuControllerList.Count - 1].transform.SetParent(p_parentCanvas.transform);

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

        /*

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
        */

        p_finishFX.SendMessage("Launch", SendMessageOptions.DontRequireReceiver);
    }

    public string[] GetWorlds()
    {
        if (!Directory.Exists(Application.persistentDataPath))
            return null;

        string[] worlds = Directory.GetDirectories(Application.persistentDataPath).Where(x => Directory.GetFiles(x).Length == 2).ToArray();
        worlds = worlds.Where(x => (Path.GetExtension(Directory.GetFiles(x)[0]).ToLower().Equals(".jpg") && Path.GetExtension(Directory.GetFiles(x)[1]).ToLower().Equals(PersistenceManager.WORLD_EXT)) || (Path.GetExtension(Directory.GetFiles(x)[0]).ToLower().Equals(PersistenceManager.WORLD_EXT) && Path.GetExtension(Directory.GetFiles(x)[1]).ToLower().Equals(".jpg"))).ToArray();
        worlds = worlds.Where(x => Directory.GetFiles(x)[0].Remove(Directory.GetFiles(x)[0].Length - Path.GetExtension(Directory.GetFiles(x)[0]).Length, Path.GetExtension(Directory.GetFiles(x)[0]).Length).Equals(Directory.GetFiles(x)[1].Remove(Directory.GetFiles(x)[1].Length - Path.GetExtension(Directory.GetFiles(x)[1]).Length, Path.GetExtension(Directory.GetFiles(x)[1]).Length))).ToArray();
        return worlds;
    }

    public void DisableMenuController()
    {
        foreach (GameObject inputMenuController in inputMenuControllerList)
            if (inputMenuController.activeSelf)
            {
                activeMenuController = inputMenuController;
                inputMenuController.SetActive(false);
                break;
            }
    }

    public void EnableMenuController()
    {
        activeMenuController.SetActive(true);
    }
}
