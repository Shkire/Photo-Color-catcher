using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BasicDataTypes;
using UnityEngine.UI;

/// <summary>
/// Singleton used to manage the loading of Levels from a previously generated World for showing them in a level selection screen.
/// </summary>
public class WorldLevelsLoadingManager : Singleton<WorldLevelsLoadingManager>
{
    /// <summary>
    /// The GameObject used as a model for level selection screen instantiation.
    /// </summary>
    [SerializeField]
    private GameObject p_levelSelectionModel;

    /// <summary>
    /// The parent canvas of the level selection screen.
    /// </summary>
    [SerializeField]
    private Transform p_parentCanvas;

    /// <summary>
    /// The Prefab used to show the level limits.
    /// </summary>
    [SerializeField]
    private GameObject p_divisionCell;

    /// <summary>
    /// The GameObject that contains the FXSequence launched when the Back button is pressed.
    /// </summary>
    [SerializeField]
    private GameObject p_backButtonFX;

    /// <summary>
    /// The bar that shows the progress of the level selection screen loading.
    /// </summary>
    [SerializeField]
    private RectTransform p_progressBar;

    /// <summary>
    /// The Text label of the loading screen.
    /// </summary>
    [SerializeField]
    private Text p_loadingText;

    /// <summary>
    /// The FXSequence launched when the level selection screen loading starts.
    /// </summary>
    [SerializeField]
    private FXSequence p_startFX;

    /// <summary>
    /// The FXSequence launched when the level selection screen loading ends.
    /// </summary>
    [SerializeField]
    private FXSequence p_finishFX;

    /// <summary>
    /// The level selection screen as GameObject.
    /// </summary>
    private GameObject levelSelectionScreen;

    /// <summary>
    /// The InputMenuController of the level selection menu as GameObject.
    /// </summary>
    private GameObject inputMenuController;

    protected WorldLevelsLoadingManager()
    {
    }

    /// <summary>
    /// Coroutine that starts the loading of the level selector screen.
    /// </summary>
    /// <param name="i_path">The World path.</param>
    public void StartLoadLevels(string i_path)
    {
        StartCoroutine(LoadLevels(i_path));
    }

    /// <summary>
    /// Creates a level selector screen with the images of the levels in a previously generated World.
    /// </summary>
    /// <param name="i_path">The World path.</param>
    public IEnumerator LoadLevels(string i_path)
    {
        //Launches the starting FXSequence.
        p_startFX.Launch();

        //Disables the InputMenuController of the world selection screen.
        WorldLoadingManager.Instance.DisableMenuController();

        //Sets up the Loading screen.
        p_loadingText.text = "Loading levels...";
        p_progressBar.anchorMin = (new Vector2(0, 0));

        //Loads the information of the levels needed for creating the level selection screen.
        World world = null;
        yield return StartCoroutine(PersistenceManager.Instance.LoadLevels(i_path, x => world = x, x => p_progressBar.anchorMin = (new Vector2(x, 0))));

        //Creates the level selection screen.
        if (levelSelectionScreen != null)
            Destroy(levelSelectionScreen);
        levelSelectionScreen = ((GameObject)Instantiate(p_levelSelectionModel));
        levelSelectionScreen.transform.SetParent(p_parentCanvas, false);

        //Creates the InputMenuController.
        if (inputMenuController != null)
            Destroy(inputMenuController);
        inputMenuController = new GameObject("InputMenuController");
        inputMenuController.transform.SetParent(p_parentCanvas);
        inputMenuController.AddComponent<InputMenuController>()._uiElements = new List<GameObject>();

        //Shows the image name.
        levelSelectionScreen.GetChild("Image name").GetComponent<Text>().text = world._name;

        GameObject auxGameObject;

        //Gets the "Back" button of the page.
        auxGameObject = levelSelectionScreen.GetChild("Back button");

        //Sets up the "Back" button.
        auxGameObject.GetComponent<GUILaunchFX>()._target = p_backButtonFX;

        //Adds the button to the InputMenuController.
        inputMenuController.GetComponent<InputMenuController>()._startingElem = auxGameObject;
        inputMenuController.GetComponent<InputMenuController>()._uiElements.Add(auxGameObject);

        //From 0 to columns - 1.
        for (int x = 0; x < world._imageDivisionConfig[0]; x++)
        {
            //From 0 to rows - 1.
            for (int y = 0; y < world._imageDivisionConfig[1]; y++)
            {
                //Creates and sets up the cell.
                auxGameObject = new GameObject("Level(" + x + "," + y + ")", typeof(RectTransform));
                auxGameObject.transform.SetParent(levelSelectionScreen.GetChild("Margin").transform);
                (auxGameObject.transform as RectTransform).offsetMin = Vector2.zero;
                (auxGameObject.transform as RectTransform).offsetMax = Vector2.zero;

                //If horizontal image.
                if (world._imageDivisionConfig[0] > world._imageDivisionConfig[1])
                {
                    //Set ups the cell size.
                    (auxGameObject.transform as RectTransform).anchorMax = new Vector2((x + 1) / (float)(world._imageDivisionConfig[0]), ((world._imageDivisionConfig[0] - world._imageDivisionConfig[1]) / 2f + y + 1) / (float)(world._imageDivisionConfig[0]));
                    (auxGameObject.transform as RectTransform).anchorMin = new Vector2(x / (float)(world._imageDivisionConfig[0]), ((world._imageDivisionConfig[0] - world._imageDivisionConfig[1]) / 2f + y) / (float)(world._imageDivisionConfig[0]));

                }

                //If vertical image.
                else
                {
                    //Set ups the cell size.
                    (auxGameObject.transform as RectTransform).anchorMax = new Vector2(((world._imageDivisionConfig[1] - world._imageDivisionConfig[0]) / 2f + x + 1) / (float)(world._imageDivisionConfig[1]), (y + 1) / (float)(world._imageDivisionConfig[1]));
                    (auxGameObject.transform as RectTransform).anchorMin = new Vector2(((world._imageDivisionConfig[1] - world._imageDivisionConfig[0]) / 2f + x) / (float)(world._imageDivisionConfig[1]), y / (float)(world._imageDivisionConfig[1]));
                }
                 
                Sprite spr = null;

                if (!world._levels[new Vector2(x, y)]._completed)
                    //Creates the level sprite.
                    spr = Sprite.Create(world._levels[new Vector2(x, y)]._img.ToGrayscale(), new Rect(0, 0, world._levels[new Vector2(x, y)]._img.width, world._levels[new Vector2(x, y)]._img.height), Vector2.zero);
                else
                    //Creates the level sprite.
                    spr = Sprite.Create(world._levels[new Vector2(x, y)]._img, new Rect(0, 0, world._levels[new Vector2(x, y)]._img.width, world._levels[new Vector2(x, y)]._img.height), Vector2.zero);
                
                //Shows the level image.
                auxGameObject.AddComponent<Image>().sprite = spr;

                //Instantiates the level limits.
                ((GameObject)Instantiate(p_divisionCell)).transform.SetParent(auxGameObject.transform, false);

                //Makes the GameObject selectable.
                auxGameObject.AddComponent<GUISelectableElement>();

                //Sets up the level button.
                auxGameObject.AddComponent<GUILoadLevel>()._path = i_path;
                auxGameObject.GetComponent<GUILoadLevel>()._levelPos = new Vector2(x, y);

                //Adds the level image to the InputMenuController.
                inputMenuController.GetComponent<InputMenuController>()._uiElements.Add(auxGameObject);

                //Creates the focused decoration GameObject.
                auxGameObject = new GameObject("Focused", typeof(RectTransform));

                //Sets the parent transform.
                auxGameObject.transform.SetParent(inputMenuController.GetComponent<InputMenuController>()._uiElements[inputMenuController.GetComponent<InputMenuController>()._uiElements.Count - 1].transform);

                //Sets up the decoration size.
                (auxGameObject.transform as RectTransform).offsetMin = Vector2.zero;
                (auxGameObject.transform as RectTransform).offsetMax = Vector2.one;
                (auxGameObject.transform as RectTransform).anchorMin = Vector2.zero;
                (auxGameObject.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);

                //Adds the GUIDecoration.
                auxGameObject.AddComponent<GUIDecoration>()._behaviour = GUIDecoration.DecorationContext.WhenFocused;

                //Adds the disabled image.
                auxGameObject.AddComponent<Image>().enabled = false;
            }
        }

        //Enables the InputMenuController of the world selection screen.
        WorldLoadingManager.Instance.EnableMenuController();

        //Launches the finishing FXSequence.
        p_finishFX.Launch();
    }

    public void DisableMenuController()
    {
        inputMenuController.SetActive(false);
    }

    public void EnableMenuController()
    {
        inputMenuController.SetActive(true);
    }
}
