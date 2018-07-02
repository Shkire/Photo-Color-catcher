using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using BasicDataTypes;
using UnityEngine.UI;

/// <summary>
/// Singleton used to manage the loading previously generated Worlds for showing them in a world selection screen.
/// </summary>
public class WorldLoadingManager : Singleton<WorldLoadingManager>
{
    /// <summary>
    /// The GameObject used as a model for the world selection screen instantiation.
    /// </summary>
    [SerializeField]
    private GameObject p_worldSelectionModel;

    /// <summary>
    /// The parent canvas of the world selection screen.
    /// </summary>
    [SerializeField]
    private Transform p_parentCanvas;

    /// <summary>
    /// The GameObject that contains the FXSequence launched when the Back button is pressed.
    /// </summary>
    [SerializeField]
    private GameObject p_backButtonFX;

    /// <summary>
    /// The GameObject that contains the FXSequence launched when the Accept button is pressed.
    /// </summary>
    [SerializeField]
    private GameObject p_acceptButtonFX;

    /// <summary>
    /// The bar that shows the progress of the level selection screen loading.
    /// </summary>
    [SerializeField]
    private RectTransform p_progressBar;

    /// <summary>
    /// The Text label of the loading screen.
    /// </summary>
    [SerializeField]
    private Text p_loadingWorldsText;

    /// <summary>
    /// The FXSequence launched when the world selection screen loading starts.
    /// </summary>
    [SerializeField]
    private FXSequence p_startFX;

    /// <summary>
    /// The FXSequence launched when the level selection screen loading ends.
    /// </summary>
    [SerializeField]
    private FXSequence p_finishFX;

    /// <summary>
    /// The world selection page list.
    /// </summary>
    private List<GameObject> p_pageList;

    /// <summary>
    /// The input menu controller list.
    /// </summary>
    private List<GameObject> p_inputMenuControllerList;

    /// <summary>
    /// The InputMenuController that was active and has been disabled.
    /// </summary>
    private GameObject p_activeMenuController;

    protected WorldLoadingManager()
    {
    }

    /// <summary>
    /// Coroutine that starts the loading of the world selector screen.
    /// </summary>
    public void StartLoadWorlds()
    {
        StartCoroutine(LoadWorlds());
    }

    /// <summary>
    /// Creates a world selector screen with the images of the previously generated Worlds.
    /// </summary>
    public IEnumerator LoadWorlds()
    {
        //Launches the starting FXSequence.
        p_startFX.Launch();

        //Sets up the Loading screen.
        p_loadingWorldsText.text = "Loading created worlds...";
        p_progressBar.anchorMin = (new Vector2(0, 0));
        float progress = 0;

        List<World> auxWorlds = new List<World>();

        //Gets the path of all the previously generated Worlds.
        string[] worldPaths = GetWorlds();

        //For each world path.
        foreach (string worldPath in worldPaths)
        {
            World auxWorld = new World();

            //Loads the information of the World needed for creating the world selection screen.
            yield return StartCoroutine(PersistenceManager.Instance.LoadWorld(worldPath, x => auxWorld = x));
            auxWorlds.Add(auxWorld);

            //Updates the progress bar.
            progress += 1 / (float)worldPaths.Length;
            p_progressBar.anchorMin = (new Vector2(progress, 0));

            yield return null;
        }

        //Destroys the previous page list.
        if (p_pageList != null)
            for (int i = 0; i < p_pageList.Count; i++)
                Destroy(p_pageList[i]);

        p_pageList = new List<GameObject>();

        //Destroys the previous input menu controller list.
        if (p_inputMenuControllerList != null)
            for (int i = 0; i < p_inputMenuControllerList.Count; i++)
                Destroy(p_inputMenuControllerList[i]);
        p_inputMenuControllerList = new List<GameObject>();

        GameObject auxGameObject;

        //If there aren't any playable world.
        if (auxWorlds.Count == 0)
        {
            //Creates the first page of the world selector.
            p_pageList.Add((GameObject)Instantiate(p_worldSelectionModel));
            p_pageList[p_pageList.Count - 1].transform.SetParent(p_parentCanvas.transform, false);

            //Destroys all the useless elements of the page.
            Destroy(p_pageList[p_pageList.Count - 1].GetChild("Prev page"));
            Destroy(p_pageList[p_pageList.Count - 1].GetChild("Next page"));
            Destroy(p_pageList[p_pageList.Count - 1].GetChild("Accept button"));
            Destroy(p_pageList[p_pageList.Count - 1].GetChild("Image name"));
            Destroy(p_pageList[p_pageList.Count - 1].GetChild("Background image"));

            //Creates the first InputMenuController.
            p_inputMenuControllerList.Add(new GameObject("InputMenuController"));
            p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].transform.SetParent(p_parentCanvas.transform);

            //Gets the "Back" button of the page.
            auxGameObject = p_pageList[p_pageList.Count - 1].GetChild("Back button");

            //Sets up the "Back" button.
            auxGameObject.GetComponent<GUILaunchFX>()._target = p_backButtonFX;

            //Adds the button to the InputMenuController.
            p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].AddComponent<InputMenuController>()._startingElem = auxGameObject;
            p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements = new List<GameObject>();
            p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(auxGameObject);
        }

        //If there are some worlds to play.
        else
        {
            //Creates the first page of the world selector.
            p_pageList.Add((GameObject)Instantiate(p_worldSelectionModel));
            p_pageList[p_pageList.Count - 1].transform.SetParent(p_parentCanvas.transform, false);

            //Destroys the "Previous page" button.
            Destroy(p_pageList[p_pageList.Count - 1].GetChild("Prev page"));

            //Creates the first InputMenuController.
            p_inputMenuControllerList.Add(new GameObject("InputMenuController"));
            p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].transform.SetParent(p_parentCanvas.transform);
            p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].AddComponent<InputMenuController>()._uiElements = new List<GameObject>();

            //For each world.
            for (int i = 0; i < auxWorlds.Count; i++)
            {
                //Destroys the error text label.
                Destroy(p_pageList[p_pageList.Count - 1].GetChild("Error text"));

                //Shows the image name.
                p_pageList[p_pageList.Count - 1].GetChild("Image name").GetComponent<Text>().text = auxWorlds[i]._name + " - " + auxWorlds[i]._imageDivisionConfig[0] + "x" + auxWorlds[i]._imageDivisionConfig[1];

                //Creates the sprite using the world image.
                Sprite spr = Sprite.Create(auxWorlds[i]._img, new Rect(0, 0, auxWorlds[i]._img.width, auxWorlds[i]._img.height), new Vector2(0, 0));

                //Shows the image.
                auxGameObject = p_pageList[p_pageList.Count - 1].GetChild("Background image");
                auxGameObject.GetComponent<Image>().sprite = spr;

                //Gets the "Back" button of the page.
                auxGameObject = p_pageList[p_pageList.Count - 1].GetChild("Back button");
                auxGameObject.GetComponent<GUILaunchFX>()._target = p_backButtonFX;

                //Adds the button to the InputMenuController.
                p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._startingElem = auxGameObject;
                p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(auxGameObject);

                //Gets the "Accept" button of the page.
                auxGameObject = p_pageList[p_pageList.Count - 1].GetChild("Accept button");

                //Sets up the "Accept" button.
                auxGameObject.GetComponent<GUILaunchFX>()._target = p_acceptButtonFX;
                auxGameObject.GetComponent<GUILoadLevels>()._path = worldPaths[i];

                //Adds the button to the InputMenuController.
                p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(auxGameObject);

                //If it is the last world.
                if (i == auxWorlds.Count - 1)
                {
                    //Destroys the "Next page" button.
                    Destroy(p_pageList[p_pageList.Count - 1].GetChild("Next page"));
                }

                //Otherwise.
                else
                {
                    //Creates the next page of the world selector.
                    p_pageList.Add((GameObject)Instantiate(p_worldSelectionModel));
                    p_pageList[p_pageList.Count - 1].transform.SetParent(p_parentCanvas.transform, false);

                    //Creates the next InputMenuController.
                    p_inputMenuControllerList.Add(new GameObject("InputMenuController"));
                    p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].transform.SetParent(p_parentCanvas.transform);
                    p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].AddComponent<InputMenuController>()._uiElements = new List<GameObject>();

                    //Deactivates the page and the InputMenuController.
                    p_pageList[p_pageList.Count - 1].SetActive(false);
                    p_inputMenuControllerList[p_pageList.Count - 1].SetActive(false);

                    //Gets the "Next page" button of this page.
                    auxGameObject = p_pageList[p_pageList.Count - 2].GetChild("Next page");

                    //Sets up the "Next page" button (activate the next page and the next input menu controller).
                    GUIObjectEnable objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = true;
                    objectEnable._target = p_inputMenuControllerList[p_inputMenuControllerList.Count - 1];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = true;
                    objectEnable._target = p_pageList[p_pageList.Count - 1];

                    //Sets up the "Next page" button (deactivate this page and this input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = false;
                    objectEnable._target = p_inputMenuControllerList[p_inputMenuControllerList.Count - 2];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = false;
                    objectEnable._target = p_pageList[p_pageList.Count - 2];

                    //Adds the "Next page" button to the input menu controller.
                    p_inputMenuControllerList[p_inputMenuControllerList.Count - 2].GetComponent<InputMenuController>()._uiElements.Add(auxGameObject);

                    //Gets the "Previous page" button of the next page.
                    auxGameObject = p_pageList[p_pageList.Count - 1].GetChild("Prev page");

                    //Sets up the "Previous page" button (activate this page and this input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = true;
                    objectEnable._target = p_inputMenuControllerList[p_inputMenuControllerList.Count - 2];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = true;
                    objectEnable._target = p_pageList[p_pageList.Count - 2];

                    //Sets up the "Previous page" button (deactivate the next page and the next input menu controller).
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = false;
                    objectEnable._target = p_inputMenuControllerList[p_inputMenuControllerList.Count - 1];
                    objectEnable = auxGameObject.AddComponent<GUIObjectEnable>();
                    objectEnable._enable = false;
                    objectEnable._target = p_pageList[p_pageList.Count - 1];

                    //Adds the "Previous page" button to the next input menu controller.
                    p_inputMenuControllerList[p_inputMenuControllerList.Count - 1].GetComponent<InputMenuController>()._uiElements.Add(auxGameObject);
                }
            }
        }
            
        //Launches the finishing FXSequence.
        p_finishFX.Launch();
    }

    /// <summary>
    /// Gets the path of all the previously generated Worlds.
    /// </summary>
    /// <returns>All the available World paths.</returns>
    public string[] GetWorlds()
    {
        if (!Directory.Exists(Application.persistentDataPath))
            return null;

        string[] worlds = Directory.GetDirectories(Application.persistentDataPath).Where(x => Directory.GetFiles(x).Length == 2).ToArray();
        worlds = worlds.Where(x => (Path.GetExtension(Directory.GetFiles(x)[0]).ToLower().Equals(".jpg") && Path.GetExtension(Directory.GetFiles(x)[1]).ToLower().Equals(PersistenceManager.WORLD_EXT)) || (Path.GetExtension(Directory.GetFiles(x)[0]).ToLower().Equals(PersistenceManager.WORLD_EXT) && Path.GetExtension(Directory.GetFiles(x)[1]).ToLower().Equals(".jpg"))).ToArray();
        worlds = worlds.Where(x => Directory.GetFiles(x)[0].Remove(Directory.GetFiles(x)[0].Length - Path.GetExtension(Directory.GetFiles(x)[0]).Length, Path.GetExtension(Directory.GetFiles(x)[0]).Length).Equals(Directory.GetFiles(x)[1].Remove(Directory.GetFiles(x)[1].Length - Path.GetExtension(Directory.GetFiles(x)[1]).Length, Path.GetExtension(Directory.GetFiles(x)[1]).Length))).ToArray();
        return worlds;
    }

    /// <summary>
    /// Disables the InputMenuController of the world selector.
    /// </summary>
    public void DisableMenuController()
    {
        foreach (GameObject inputMenuController in p_inputMenuControllerList)
            if (inputMenuController.activeSelf)
            {
                p_activeMenuController = inputMenuController;
                inputMenuController.SetActive(false);
                break;
            }
    }

    /// <summary>
    /// Enables the InputMenuController that was active.
    /// </summary>
    public void EnableMenuController()
    {
        p_activeMenuController.SetActive(true);
    }
}
